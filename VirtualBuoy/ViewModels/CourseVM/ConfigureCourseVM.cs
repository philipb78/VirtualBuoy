using BoatDataService;
using Interfaces;
using Models.CourseItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ViewModels.CourseVM
{
    public class ConfigureCourseVM : BaseViewModel
    {
        private IDataStore m_dateStore;

        private ObservableCollection<CourseMarkVM> m_allCourseMarks;

        public ObservableCollection<CourseMarkVM> AllCourseMarks
        {
            get { return m_allCourseMarks; }
            set
            {
                m_allCourseMarks = value;
                SetProperty();
            }
        }

        private ObservableCollection<ActiveCourseMarkVM> m_activeCourseMarks;

        public ObservableCollection<ActiveCourseMarkVM> ActiveCourseMarks
        {
            get
            {
                return m_activeCourseMarks;
            }

            set
            {
                m_activeCourseMarks = value;
                SetProperty();
            }
        }

        public ICommand AddMarkToCourseCommand { private set; get; }

        public ICommand DeleteMarkCommand { private set; get; }

        public ICommand UpCommand { private set; get; }
        public ICommand DownCommand { private set; get; }

        private IDataController m_dataController;

        public ConfigureCourseVM()
        {
            AddMarkToCourseCommand = new Command(AddMarkToCourse, CanExecuteAddMarkToCourse);
            DeleteMarkCommand = new Command(DeleteMarkFromCourse, CanExecuteDeleteMarkFromCourse);
            UpCommand = new Command(UpMark, CanExecuteUpMark);
            DownCommand = new Command(DownMark, CanExecuteDownMark);

            m_dateStore = DependencyService.Get<IDataStore>();
            m_dataController = DependencyService.Get<IDataController>();
            AllCourseMarks = new ObservableCollection<CourseMarkVM>();

            ActiveCourseMarks = new ObservableCollection<ActiveCourseMarkVM>();

            SetupCourse();
        }

        private void CourseUpdated(object sender, EventArgs e)
        {
            try
            {
                if (sender != this)
                {
                    UpdateFromActiveCourse();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " " + ex.StackTrace);
            }
        }

        private bool CanExceuteUpdateCourse(object arg)
        {
            return true;
        }

        private bool CanExecuteDownMark(object arg)
        {
            ActiveCourseMarkVM activeRaceMarkVM = arg as ActiveCourseMarkVM;
            int index = ActiveCourseMarks.IndexOf(activeRaceMarkVM);

            // Zero based
            if (index < ActiveCourseMarks.Count - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DownMark(object obj)
        {
            ActiveCourseMarkVM activeRaceMarkVM = obj as ActiveCourseMarkVM;
            int index = ActiveCourseMarks.IndexOf(activeRaceMarkVM);
            ActiveCourseMarks.Move(index, index + 1);
            ResortCourseOrder();
        }

        private bool CanExecuteUpMark(object arg)
        {
            ActiveCourseMarkVM activeRaceMarkVM = arg as ActiveCourseMarkVM;
            int index = ActiveCourseMarks.IndexOf(activeRaceMarkVM);

            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpMark(object obj)
        {
            ActiveCourseMarkVM activeRaceMarkVM = obj as ActiveCourseMarkVM;
            int index = ActiveCourseMarks.IndexOf(activeRaceMarkVM);
            ActiveCourseMarks.Move(index, index - 1);
            ResortCourseOrder();
        }

        private bool CanExecuteDeleteMarkFromCourse(object arg)
        {
            return true;
        }

        private void DeleteMarkFromCourse(object obj)
        {
            ActiveCourseMarkVM activeCourseMarkVM = obj as ActiveCourseMarkVM;
            ActiveCourseMarks.Remove(activeCourseMarkVM);
            ResortCourseOrder();
        }

        protected override void UpdateCommands()
        {
            (DownCommand as Command).ChangeCanExecute();
            (UpCommand as Command).ChangeCanExecute();
        }

        private bool CanExecuteAddMarkToCourse(object arg)
        {
            return true;
        }

        private void ResortCourseOrder()
        {
            int order = 1;
            foreach (ActiveCourseMarkVM nextVM in ActiveCourseMarks)
            {
                nextVM.Order = order;

                order++;
            }
            UpdateCommands();
            UpdateActiveCourse();
        }

        private void AddMarkToCourse(object obj)
        {
            ActiveCourseMarkVM activeCourseMark = new ActiveCourseMarkVM();

            activeCourseMark.PropertyChanged += ActiveCourseMark_PropertyChanged;
            ActiveCourseMarks.Add(activeCourseMark);

            ResortCourseOrder();
        }

        private void ActiveCourseMark_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ActiveCourseMarkVM courseVM = sender as ActiveCourseMarkVM;
            if ((e.PropertyName == "CourseMark" || e.PropertyName == "IsStarboardPass" || e.PropertyName == "Order") && courseVM.CourseMark != null)
            {
                UpdateActiveCourse();
                m_dataController.NotifyCourseUpdated(this);
            }
        }

        /// <summary>
        /// Creates an Active Race
        /// </summary>
        /// <returns></returns>
        private void UpdateActiveCourse()
        {
            m_dataController.ActiveCourse.CourseMarks.Clear();
            foreach (ActiveCourseMarkVM nextCourseMark in ActiveCourseMarks)
            {
                m_dataController.ActiveCourse.CourseMarks.Add(nextCourseMark.ActiveCourseMark.Clone());
            }
            m_dataController.NotifyCourseUpdated(this);
        }

        private void UpdateFromActiveCourse()
        {
            foreach (ActiveCourseMarkVM nextCourseMark in ActiveCourseMarks)
            {
                nextCourseMark.PropertyChanged -= ActiveCourseMark_PropertyChanged;
            }
            ActiveCourseMarks.Clear();

            foreach (ActiveCourseMark nextCourseMark in m_dataController.ActiveCourse.CourseMarks)
            {
                ActiveCourseMarkVM nextMark = new ActiveCourseMarkVM(nextCourseMark);

                ActiveCourseMarks.Add(nextMark);
                nextMark.CourseMark = m_allCourseMarks.First(m => m.Id == nextCourseMark.Mark.Id);
                nextMark.PropertyChanged += ActiveCourseMark_PropertyChanged;
            }
        }

        private async void SetupCourse()
        {
            IsBusy = true;
            List<Task> tasks = new List<Task>();
            tasks.Add(SetupAllMarks());

            await Task.WhenAll(tasks);
            UpdateFromActiveCourse();
            m_dataController.CourseUpdated += CourseUpdated;
            IsBusy = false;
        }

        private async Task SetupAllMarks()
        {
            List<CourseMark> allMarks = await m_dateStore.GetCourseMarksAsync();
            allMarks = allMarks.OrderBy(m => m.Name).ToList();

            foreach (CourseMark nextMark in allMarks)
            {
                CourseMarkVM courseMarkVM = new CourseMarkVM(nextMark);
                AllCourseMarks.Add(courseMarkVM);
            }
        }
    }
}