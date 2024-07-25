using tupencauymobile.Services;
using tupencauymobile.ViewModels;

namespace tupencauymobile.Views;

public partial class PrediccionDisplay : ContentPage
{
	public PrediccionDisplay(PrediccionVM prediccionVM)
	{
		InitializeComponent();
		BindingContext = prediccionVM;
	}

    private void GuardarPrediccion(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string idEvento)
        {
            GuardarPrediccion(idEvento);
        }
    }

    private void EditarPrediccion(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string idEvento)
        {
            EditarPrediccion(idEvento);
        }
    }

    private async void GuardarPrediccion(string idEvento)
    {
        if(GolesEquipoUno!= null && GolesEquipoDos!= null)
        {
            string aux1 = GolesEquipoUno.Text;
            string aux2 = GolesEquipoDos.Text;
            int golesEquipoUno = int.Parse(aux1);
            int golesEquipoDos = int.Parse(aux2);
            bool yaPredijoAntes = false;
            ServicioPrediccion servicioPrediccion = new ServicioPrediccion(Preferences.Get("jwt", string.Empty));
            PrediccionVM prediccionVM = new PrediccionVM(servicioPrediccion);
            prediccionVM.EnviarPrediccion(idEvento, golesEquipoUno, golesEquipoDos, yaPredijoAntes);
        }
        else
        {
            await DisplayAlert("Error", "Debes ingresar un resultado", "Cerrar");
        }
    }

    private async void EditarPrediccion(string idEvento)
    {
        if (GolesEquipoUno != null && GolesEquipoDos != null)
        {
            string aux1 = GolesEquipoUno.Text;
            string aux2 = GolesEquipoDos.Text;
            int golesEquipoUno = int.Parse(aux1);
            int golesEquipoDos = int.Parse(aux2);
            bool yaPredijoAntes = true;
            ServicioPrediccion servicioPrediccion = new ServicioPrediccion(Preferences.Get("jwt", string.Empty));
            PrediccionVM prediccionVM = new PrediccionVM(servicioPrediccion);
            prediccionVM.EnviarPrediccion(idEvento, golesEquipoUno, golesEquipoDos, yaPredijoAntes);
        }
        else
        {
            await DisplayAlert("Error", "Debes ingresar un resultado", "Cerrar");
        }
    }

    private async void CloseModal(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}