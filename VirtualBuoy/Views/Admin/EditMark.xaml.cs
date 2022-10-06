using Models.CourseItems;
using System;

using ViewModels.AdminVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditMark : ContentPage
    {
        public EditMark()
        {
            InitializeComponent();
            CourseMark raceMark = new CourseMark();

            EditMarkVM raceMarkVM = new EditMarkVM(raceMark);
            try
            {
                this.BindingContext = raceMarkVM;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }
    }
}