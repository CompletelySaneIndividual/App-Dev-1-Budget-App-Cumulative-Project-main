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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Home_Budget_WPF_App.Interfaces;
using Microsoft.Win32;
using Home_Budget_WPF_App.Windows;
using Budget;
using System.ComponentModel;

namespace Home_Budget_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewToPresenter
    {
        Presenter presenter;
       
        public MainWindow()
        {
            InitializeComponent();

            presenter = new Presenter(this);

            //startDate.SelectedDate = new DateTime(1900, 1, 1);
            //endDate.SelectedDate = new DateTime(2500, 1, 1);
            if(presenter.budget != null)
            {
                UpdateCatCmbBox();

                UpdateGrid();

                chartDisplay.Visibility = Visibility.Hidden;
                gridDisplay.Visibility = Visibility.Visible;

                gridDisplay.pres = presenter;
                chartDisplay.presenter = presenter;

                presenter.PropertyChanged += PropertyChangedInPresenter;
                presenter.Path = presenter.Path;
            }
        }
        public void AskForFile(string filter, string title, string initialDir)
        {
            try
            {
                OpenFileDialog save = new OpenFileDialog();
                save.Filter = filter;
                save.Title = title;
                save.InitialDirectory = initialDir;

                if (save.ShowDialog() == true)
                    presenter.pickDatabaseFile(save.FileName);

            }
            catch (Exception ex)
            {
                ShowMessagebox("Error selecting a file: "+ex.Message, "Error with file",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowMessagebox(string messageBoxFix, string caption, object btn, object img)
        {
            MessageBox.Show(messageBoxFix, caption, (MessageBoxButton)btn, (MessageBoxImage)img);
        }

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            presenter.AskForFile();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Home_Budget_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "You might have unsaved changes, are you sure you want to exit?";
            MessageBoxResult result = MessageBox.Show(msg, "Home budget", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
            if(presenter!=null)
                presenter.CloseDB();
        }

        private void addCatBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow(presenter);
            addCategoryWindow.ShowDialog(); // Make sure user closes first the add cat window before using the main window
        }

        private void addExpeseBtn_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow(presenter);
            addExpenseWindow.ShowDialog(); // Make sure user closes first the add exp window before using the main window
        }

        public void UpdateCatCmbBox()
        {
            cmbBoxCategories.ItemsSource = presenter.GetAllCatsDescription();
        }

        private void OptionChanged(object sender, RoutedEventArgs e)
        {
            if(presenter.Path != "" && presenter.Path!=null)
                UpdateGrid();
        }
        public void RefreshPath()
        {
            filePathDisplay.Text = presenter.Path;
        }
        void PropertyChangedInPresenter(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Path":
                    RefreshPath();
                    break;
            }
        }

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

        //private void datagridDisplay_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    sender = (sender as DataGrid).CurrentItem;
        //    if (sender != null)
        //    {

        //        if (sender is BudgetItem)
        //        {
        //            UpdateExpense updateExpense = new UpdateExpense(presenter, (BudgetItem)sender);
        //            updateExpense.ShowDialog();
        //        }
             
        //    }         
        //}
        public bool AskForReset(string message, string caption, object btn)
        {
            MessageBoxResult result = MessageBox.Show(message, caption, (MessageBoxButton)btn);
            if (result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        public bool askForDefault(string message, string caption, object btn)
        {
            MessageBoxResult result = MessageBox.Show(message, caption, (MessageBoxButton)btn);
            if (result == MessageBoxResult.Yes)
            {
                EnableAll();
                return true;
            }
            else if (result == MessageBoxResult.Cancel)
                Close();
            return false;
        }
        public void EnableAll()
        {
            expBtn.IsEnabled = catBtn.IsEnabled = filterByCatFlag.IsEnabled = byMonthFlag.IsEnabled = startDate.IsEnabled = endDate.IsEnabled = byCategoryFlag.IsEnabled = true;
        }

        private void defaultBtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.pickDatabaseFile(presenter.DefBudget);
        }
    }
}


