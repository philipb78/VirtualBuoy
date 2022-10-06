using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI.Forms;
using Mapsui.UI.Forms.Extensions;
using Mapsui.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Views.Course
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseMapView : ContentPage
    {
        private MapVM m_mapViewModel;

        public CourseMapView()
        {
            InitializeComponent();

            m_mapViewModel = new MapVM();

            BindingContext = m_mapViewModel;

            mapView.MapLongClicked += MapView_MapLongClicked;

            try
            {
                mapView.Navigator.ZoomTo(ZoomLevelExtensions.ToMapsuiResolution(17));
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
            }
        }

        private async void MapView_MapLongClicked(object sender, MapLongClickedEventArgs e)
        {
            string[] buttons = { "Add New Mark" };
            string result = await DisplayActionSheet("Menu", "Cancel", null, buttons);
            if (result == "Add New Mark")
            {
                m_mapViewModel.AddNewMark(e.Point);
            }
        }
    }
}