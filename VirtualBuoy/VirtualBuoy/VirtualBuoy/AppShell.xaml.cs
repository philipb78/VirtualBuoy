using System;
using System.Collections.Generic;
using VirtualBuoy.ViewModels;
using VirtualBuoy.Views;
using Xamarin.Forms;

namespace VirtualBuoy
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
