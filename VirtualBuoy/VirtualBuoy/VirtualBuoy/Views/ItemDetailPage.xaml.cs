using System.ComponentModel;
using VirtualBuoy.ViewModels;
using Xamarin.Forms;

namespace VirtualBuoy.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}