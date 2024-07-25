using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Firebase.Auth;
using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public class ServicioGoogleAuth : IServicioGoogleAuth
    {
        public static Activity _activity;
        public static GoogleSignInOptions _gso;
        public static GoogleSignInClient _googleSignInClient;
        public static HttpClient _httpClient;

        private const string WebApiKey = "96724135476-pl2ftplt4s798tqqtvrbmafnn7m878l1.apps.googleusercontent.com";

        private TaskCompletionSource<GoogleUser> _taskCompletionSource = new TaskCompletionSource<GoogleUser>();
        private Task<GoogleUser> GoogleAuthentication
        {
            get => _taskCompletionSource.Task;
        }

        public ServicioGoogleAuth()
        {
            _httpClient = new HttpClient();

            _activity = Platform.CurrentActivity;

            //Google Auth Option
            _gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                            .RequestIdToken(WebApiKey)
                            .RequestEmail()
                            .RequestId()
                            .RequestProfile()
                            .Build();

            _googleSignInClient = GoogleSignIn.GetClient(_activity, _gso);


            MainActivity.ResultGoogleAuth += async (sender, e) => await ResultGoogleAuth(sender, e);
        }


        public Task<GoogleUser> AuthenticateAsync()
        {
            _taskCompletionSource = new TaskCompletionSource<GoogleUser>();

            _activity.StartActivityForResult(_googleSignInClient.SignInIntent, 9001);

            return GoogleAuthentication;

        }

        private async Task ResultGoogleAuth(object sender, (bool Success, GoogleSignInAccount Account) e)
        {
            if (e.Success)
            {
                try
                {
                    var currentAccount = e.Account;

                    var googleUser = new GoogleUser
                    {
                        Email = currentAccount.Email,
                        Nombre = currentAccount.DisplayName,
                        TokenId = currentAccount.IdToken,
                        UserName = currentAccount.GivenName,
                        GoogleId = currentAccount.Id,
                        tenantId = Preferences.Get("tenantId", string.Empty)
                    };

                    var response = await _httpClient.PostAsJsonAsync($"{NetworkConstants.localUrl}/Auth/google-login-maui", googleUser);

                    if(response.IsSuccessStatusCode)
                    {
                        var dataUser = await response.Content.ReadFromJsonAsync<AuthReturn>();
                        Preferences.Set("userId", dataUser.IdUser);
                        Preferences.Set("jwt", dataUser.Token);
                        Console.WriteLine("El usuario se guardó correctamente en base de datos");
                        _taskCompletionSource.SetResult(googleUser);
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await Shell.Current.DisplayAlert("Error", "El usuario no pertenece a este sitio", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error durante la autenticación: {ex.Message}");
                }
            }

        }

        public async Task<GoogleUser> GetCurrentUserAsync()
        {
            try
            {
                var user = await _googleSignInClient.SilentSignInAsync();
                return new GoogleUser
                {
                    Email = user.Email,
                    Nombre = $"{user.DisplayName}",
                    TokenId = user.IdToken,
                    UserName = user.GivenName
                };

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task LogoutAsync() => _googleSignInClient.SignOutAsync();

    }
}
