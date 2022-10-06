using System;
using VirtualBuoy.Services;
using VirtualBuoy.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VirtualBuoy
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
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
