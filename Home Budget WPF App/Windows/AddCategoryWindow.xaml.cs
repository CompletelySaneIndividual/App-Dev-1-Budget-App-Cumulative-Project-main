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
using Budget;
using Home_Budget_WPF_App.Interfaces;


namespace Home_Budget_WPF_App.Windows
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window, IPresenterToView
    {
        private Presenter presenter;

        public AddCategoryWindow(Presenter _presenter)
        {
            InitializeComponent();
            presenter = _presenter;
            presenter.InitializeCatExpView(this);           
            SetDefaultViewValues();
        }

        public void ShowSuccessMessage(string mssg, string caption)
        {
            MessageBox.Show(mssg, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowErrorMessage(string mssg, string errors, string caption)
        {
            MessageBox.Show(mssg + errors, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            presenter.AddCategory(Description.Text, categoryTypesCmbBox.SelectedItem.ToString());
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            string mssgBoxText = "Are you sure, do you want to exit?";
            string caption = "Add category";

            MessageBoxResult result = MessageBox.Show(mssgBoxText, caption, MessageBoxButton.YesNo);
            if (result.ToString() == "Yes")
                Close();
        }

        public void ResetFields()
        {
            Description.Text = "";
        }

        public void SetDefaultViewValues()
        {
            categoryTypesCmbBox.ItemsSource = presenter.GetCategoryTypes();
        }
    }
}
