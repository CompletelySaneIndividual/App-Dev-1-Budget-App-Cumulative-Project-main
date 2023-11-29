using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EnterpriseBudget.DeptBudgets;
using EnterpriseBudget.DeptBudgets.Windows;
using Budget;

namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Interaction logic for ReadWriteDataView.xaml
    /// </summary>
    public partial class ReadWriteView : Window, InterfaceView
    {
        /// <summary>
        /// presenter for the DeptBudgets.ReadWriteView
        /// </summary>
        private Presenter _presenter;
        /// <summary>
        /// 
        /// </summary>
        public Presenter presenter
        {
            get
            {
                return _presenter;
            }
            set
            {
                _presenter = value;
                //_presenter.LoadData();
            }
        }

        /// <summary>
        /// view for the mainControl (starting point for the app)
        /// </summary>
        public MainControl.InterfaceView mainControl { get; set; }

        /// <summary>
        /// Standard Windows constructor
        /// </summary>
        public ReadWriteView()
        {

            InitializeComponent();
            
        }

        /// <summary>
        /// The window is about to close, cleans up anything
        /// that needs to be done before closing
        /// </summary>
        public void TidyUpAndClose()
        {
            this.Close();
        }

        // Call this when the window is closing,
        // put main control back to the forefront
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            presenter.onClose();
            if (presenter.allowedToClose)
            {
                if (mainControl != null)
                {
                    mainControl.ComeBackToForeground();
                }
                else
                {
                    MessageBox.Show("You did not set mainControl - You need to fix this bug", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Cancel = true;
            }
            
        }

        // window is being displayed via the "ShowDialog" method
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (presenter != null)
            {
                if (presenter.LoadData())
                {
                    txtWait.Visibility = Visibility.Hidden;
                    //txtWait.Text = "Successfully created home budget... now YOU have to do the rest :)";
                    UpdateCatCmbBox();

                    UpdateGrid();

                    chartDisplay.Visibility = Visibility.Hidden;
                    gridDisplay.Visibility = Visibility.Visible;

                    gridDisplay.SetPresenter(presenter);
                    chartDisplay.presenter = presenter;

                    //presenter.PropertyChanged += PropertyChangedInPresenter;
                    //presenter.Path = presenter.Path;
                }
                else
                {
                    startDate.IsEnabled = endDate.IsEnabled = filterByCatFlag.IsEnabled =  cmbBoxCategories.IsEnabled = false;
                    byMonthFlag.IsEnabled = byCategoryFlag.IsEnabled = byChart.IsEnabled = false;
                    txtWait.Text = "something went wrong, unable to load home budget";
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateCatCmbBox()
        {
            cmbBoxCategories.ItemsSource = presenter.GetCategories();
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateGrid()
        {
            //this event will trigger whenever any of the filter or summary options' values change
            //and is meant to grap those values and display to the ListBox accordingly

            //grabbing all of the filter and summary options' values
            List<Category> cats;
            DateTime start = startDate.SelectedDate ?? new DateTime(1900, 1, 1);
            DateTime end = endDate.SelectedDate ?? new DateTime(2500, 1, 1);

            bool filter = filterByCatFlag.IsChecked ?? false;
            int idOfCatSelected = 0;
            cats = presenter.GetCategories();
            if (filter)
            {
                if (cmbBoxCategories.SelectedItem != null)
                {
                    string catDesc = cmbBoxCategories.SelectedItem.ToString(); //if the filter option is selected we need to get a reference to the cat using the desc
                    foreach (Category cat in cats)
                    {
                        if (cat.Description == catDesc)
                            idOfCatSelected = cat.Id;
                    }
                }

            }

            bool sumerizeByMonth = byMonthFlag.IsChecked ?? false;
            bool sumerizeByCat = byCategoryFlag.IsChecked ?? false;
            bool summerizeByChart = byChart.IsChecked ?? false;

            if (summerizeByChart)
            {
                chartDisplay.Visibility = Visibility.Visible;
                gridDisplay.Visibility = Visibility.Hidden;
                presenter.DisplayData(start, end, filter, idOfCatSelected, sumerizeByMonth, sumerizeByCat, chartDisplay);
            }
            else
            {
                chartDisplay.Visibility = Visibility.Hidden;
                gridDisplay.Visibility = Visibility.Visible;
                presenter.DisplayData(start, end, filter, idOfCatSelected, sumerizeByMonth, sumerizeByCat, gridDisplay);

            }
        }
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void OptionChanged(object sender, RoutedEventArgs e)
        {
            //if (presenter.Path != "" && presenter.Path != null)
                UpdateGrid();
        }
        private void addExpeseBtn_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow(presenter);
            addExpenseWindow.ShowDialog(); // Make sure user closes first the add exp window before using the main window
        }

        public void ShowMessagebox(string messageBoxFix, string caption, object btn, object img)
        {
            MessageBox.Show(messageBoxFix, caption, (MessageBoxButton)btn, (MessageBoxImage)img);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.SaveFile();
        }

        public void ShowNotSavedMessageBox(string messageBoxFix, string caption)
        {
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxFix, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes){
                presenter.allowedToClose = true;
            }
            else{
                presenter.allowedToClose = false;
            }
           
        }


    }
}
