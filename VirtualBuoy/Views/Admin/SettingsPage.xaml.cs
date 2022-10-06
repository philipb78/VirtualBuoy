using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.AdminVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            SettingsVM settingsVM = new SettingsVM();
            this.BindingContext = settingsVM;
        }
    }
}