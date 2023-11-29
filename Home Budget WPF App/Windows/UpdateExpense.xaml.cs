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
using Home_Budget_WPF_App.Interfaces;
using Budget;

namespace Home_Budget_WPF_App.Windows
{
    /// <summary>
    /// Interaction logic for UpdateExpense.xaml
    /// </summary>
    public partial class UpdateExpense : Window, IPresenterToView
    {
        private Presenter presenter;
        private BudgetItem budgetItem;
        public UpdateExpense(Presenter _presenter, BudgetItem selectedExpense)
        {
            InitializeComponent();
            presenter = _presenter;
            presenter.InitializeCatExpView(this);
            budgetItem = selectedExpense;
            SetDefaultViewValues();
        }

        public void ResetFields()
        {
            this.Close();           // If expense was updated close the window
        }

        public void SetDefaultViewValues()
        {
            dateInput.SelectedDate = budgetItem.Date;
            Amount.Text = budgetItem.Amount.ToString();
            Description.Text = budgetItem.ShortDescription;
            UpdateCatsComboBox();
            cmbBoxCategories.SelectedItem = budgetItem.Category;
        }

        public void ShowErrorMessage(string mssg, string errors, string caption)
        {
            MessageBox.Show(mssg + errors, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowSuccessMessage(string mssg, string caption)
        {
            MessageBox.Show(mssg, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.UpdateExpense(budgetItem.ExpenseID, (DateTime)dateInput.SelectedDate, cmbBoxCategories.SelectedItem.ToString(), Amount.Text, Description.Text);
        }
        public void UpdateCatsComboBox()
        {
            cmbBoxCategories.ItemsSource = presenter.GetAllCatsDescription();
        }
        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow categoryWindow = new AddCategoryWindow(presenter);
            categoryWindow.ShowDialog();

            UpdateCatsComboBox();
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dltBtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.DeleteExpense(budgetItem.ExpenseID);
        }
    }
}
