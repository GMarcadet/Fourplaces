using Fourplaces.Views;
using Storm.Mvvm;

namespace Fourplaces
{
    public partial class App : MvvmApplication
    {
        public App():base(() => new LoginPage())
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
