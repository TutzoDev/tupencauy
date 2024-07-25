

using tupencauymobile.Services;
using tupencauymobile.ViewModels;

namespace tupencauymobile.Views;

public partial class PencaDisplay : ContentPage
{
	public PencaDisplay(PencaDisplayVM pencaDisplayVM)
	{
		InitializeComponent();
       BindingContext = pencaDisplayVM;

    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await GetUsuario();
    }
    private async Task GetUsuario()
    {
        try
        {
            ServicioUsuario servicioUser = new ServicioUsuario(Preferences.Get("jwt", string.Empty));
            UsuarioVM userVM = new UsuarioVM(servicioUser);
            await Shell.Current.Navigation.PushAsync(new PerfilUsuario(userVM));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No hemos podido cargar tu perfil: {ex.Message}", "Cerrar");
        }
    }
}