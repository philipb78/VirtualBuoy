using System;
using System.Collections.Generic;
using System.ComponentModel;
using VirtualBuoy.Models;
using VirtualBuoy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VirtualBuoy.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}