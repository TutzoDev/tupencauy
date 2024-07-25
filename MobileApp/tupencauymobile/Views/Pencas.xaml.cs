using tupencauymobile.Models;
using tupencauymobile.Services;
using tupencauymobile.ViewModels;

namespace tupencauymobile.Views;

public partial class Pencas : ContentPage
{
	public Pencas(PencaVM pencas)
	{
		InitializeComponent();
        BindingContext = pencas;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PencasAutoCommand();
    }

    private void PencasAutoCommand()
    {
        if (BindingContext is PencaVM pencas)
        {
            if (pencas.GetPencasCommand.CanExecute(null))
            {
                pencas.GetPencasCommand.Execute(null);
            }
        }
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