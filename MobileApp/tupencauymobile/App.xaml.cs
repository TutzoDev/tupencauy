
using tupencauymobile.Services;
using tupencauymobile.ViewModels;
using tupencauymobile.Views;

namespace tupencauymobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}