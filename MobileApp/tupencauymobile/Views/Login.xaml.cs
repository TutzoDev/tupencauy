using tupencauymobile.Models;
using tupencauymobile.Services;
using tupencauymobile.ViewModels;
using Plugin.Firebase.CloudMessaging;
using Firebase.Auth;


namespace tupencauymobile.Views;

public partial class Login : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly IServicioGoogleAuth _googleAuthService = new ServicioGoogleAuth();
    private SitioVM sitio;

    public Login()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        ServicioSitio servicioSitio = new ServicioSitio();
        SitioVM sitioVM = new SitioVM(servicioSitio);
        sitio = sitioVM;
        BindingContext = sitioVM;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SitiosAutoCommand();
    }

    private void SitiosAutoCommand()
    {
        if (BindingContext is SitioVM sitios)
        {
            if (sitios.GetSitiosCommand.CanExecute(null))
            {
                sitios.GetSitiosCommand.Execute(null);
            }
        }
    }

    private async void LoginClicked(object sender, EventArgs e)
    {
        if (sitio.SitioSeleccionado != null)
        {

            var request = new LoginReq
            {
                EmailUsername = EmailEntry.Text,
                Password = PasswordEntry.Text,
                tenantId = sitio.SitioSeleccionado.TenantId
            };

            var servicioLogin = new ServicioLogin();
            var authResult = await servicioLogin.LoginAsync(request);

            if (authResult.Success)
            {
                Preferences.Set("jwt", authResult.Token);
                Preferences.Set("tenantId", authResult.TenantId);
                Preferences.Set("userId", authResult.IdUser);
                ServicioPenca servicioPenca = new ServicioPenca(Preferences.Get("jwt", string.Empty));
                PencaVM pencaVM = new PencaVM(servicioPenca);
                await DisplayAlert("Login exitoso", "¡Bienvenido!", "Ok");

                await ConfigureFCMToken(Preferences.Get("userId", string.Empty));

                await Navigation.PushAsync(new Pencas(pencaVM));
            }
            else
            {
                await DisplayAlert("Login error", "Usuario y/o contraseña incorrecto!", "Cerrar");
            }
        }
        else
        {
            await DisplayAlert("Error", "Debes seleccionar un Sitio", "Cerrar");
        }
    }

    private async void GoogleLoginClicked(object sender, EventArgs e)
    {
        if (sitio.SitioSeleccionado != null)
        {
            //var loggedUser = await _googleAuthService.GetCurrentUserAsync();

            Preferences.Set("tenantId", sitio.SitioSeleccionado.TenantId);

            //if (loggedUser == null)
            //{
            var loggedUser = await _googleAuthService.AuthenticateAsync();
            //}

            await DisplayAlert("Login exitoso", "¡Bienvenido " + loggedUser.Nombre!, "Ok");

            ServicioPenca servicioPenca = new ServicioPenca(Preferences.Get("jwt", string.Empty));
            PencaVM pencaVM = new PencaVM(servicioPenca);

            await Navigation.PushAsync(new Pencas(pencaVM));
        }
        else
        {
            await DisplayAlert("Error", "Debes seleccionar un Sitio", "Cerrar");
        }
    }

    private async void logoutBtn_Clicked(object sender, EventArgs e)
    {

        await _googleAuthService?.LogoutAsync();

        await Application.Current.MainPage.DisplayAlert("Logout message", "Goodbye", "Ok");

    }

    private async Task ConfigureFCMToken(string userId)
    {
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        var fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        FirebaseToken tokenUser = new FirebaseToken
        {
            fcmToken = fcmToken,
            IdUser = userId
        };
        ServicioFcm servicioFcmToken = new ServicioFcm(Preferences.Get("jwt", string.Empty));
        await servicioFcmToken.SendToken(tokenUser);
    }

    public class JwtResponse
    {
        public string Token { get; set; }
        public string TenantId { get; set; }
        public string IdUser { get; set; }
    }
}