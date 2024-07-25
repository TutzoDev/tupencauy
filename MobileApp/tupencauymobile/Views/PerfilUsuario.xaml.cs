using tupencauymobile.Models;
using tupencauymobile.ViewModels;

namespace tupencauymobile.Views;

public partial class PerfilUsuario : ContentPage
{
	public PerfilUsuario(UsuarioVM usuario)
	{
		InitializeComponent();
        BindingContext = usuario;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetUsuarioCommand();
    }
    private void GetUsuarioCommand()
    {
        if (BindingContext is UsuarioVM usuario)
        {
            if (usuario.GetUserCommand.CanExecute(null))
            {
                usuario.GetUserCommand.Execute(null);
            }
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // L�gica para cerrar sesi�n
        await DisplayAlert("Cerrar Sesi�n", "Sesi�n cerrada correctamente", "OK");
        // Redirigir a la p�gina de inicio o login
        await Navigation.PopToRootAsync();
    }
}