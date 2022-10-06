using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.CourseVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Views.Course
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigureCourse : ContentPage
    {
        public ConfigureCourse()
        {
            InitializeComponent();
            ConfigureCourseVM configureRaceCourse = new ConfigureCourseVM();

            this.BindingContext = configureRaceCourse;
        }
    }
}