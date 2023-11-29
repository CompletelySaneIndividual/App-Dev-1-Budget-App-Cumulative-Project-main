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
using EnterpriseBudget.DeptBudgets.Interfaces;
using Budget;

namespace EnterpriseBudget.DeptBudgets.Windows
{
    /// <summary>
    /// Interaction logic for AddExpenseWindow.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window, IPresenterToView
    {
        Presenter presenter;
        Category currentCategory;
        public AddExpenseWindow(Presenter _presenter)
        {
            InitializeComponent();
            presenter = _presenter;
            presenter.InitializeCatExpView(this);
            SetDefaultViewValues();
        }
        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBoxCategories.SelectedItem == null)
                presenter.AddExpense((DateTime)dateInput.SelectedDate, "", Amount.Text, Description.Text, presenter.Limit(currentCategory.Id), TotalCategory(currentCategory.Id));
            else
                presenter.AddExpense((DateTime)dateInput.SelectedDate, cmbBoxCategories.SelectedItem.ToString(), Amount.Text, Description.Text, presenter.Limit(currentCategory.Id), TotalCategory(currentCategory.Id));
        }


        public void ShowSuccessMessage(string mssg, string caption)
        {
            MessageBox.Show(mssg, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowErrorMessage(string mssg, string errors, string caption)
        {
            MessageBox.Show(mssg + errors, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            string mssgBoxText = "Are you sure, do you want to exit?";
            string caption = "Add expense";

            MessageBoxResult result = MessageBox.Show(mssgBoxText, caption, MessageBoxButton.YesNo);
            if (result.ToString() == "Yes")
                Close();

        }
        public void UpdateCatsComboBox()
        {
            cmbBoxCategories.ItemsSource = presenter.GetCategories();
        }
        public void ResetFields()
        {
            Amount.Text = Description.Text = "";
        }

        public void SetDefaultViewValues()
        {
            UpdateCatsComboBox();
            dateInput.SelectedDate = DateTime.Now;
        }

        private void cmbBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentCategory = cmbBoxCategories.SelectedItem as Category;
            UpdateCatLimit();
            UpdateCatTotal();
        }
        double TotalCategory(int catId)  {
            List<BudgetItemsByCategory> tmp = presenter.budgetsOfCategory(catId);
            foreach (var item in tmp)
            {
                if (item.CategoryID==catId)
                    return item.Total;
            }
            return 0;
        }

        public void UpdateCatLimit()
        {
            limit.Text = currentCategory.Description + " limit: $" + presenter.Limit(currentCategory.Id);
        }

        public void UpdateCatTotal()
        {
            currentCat.Text = currentCategory.Description + " total: $" + TotalCategory(currentCategory.Id);
        }
    }
}
