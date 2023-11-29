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
using Budget;
using EnterpriseBudget.DeptBudgets.Interfaces;

namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Interaction logic for DataTable.xaml
    /// </summary>
    public partial class DataTable : UserControl, IDataSumerize
    {
        /// <summary>
        /// 
        /// </summary>
        private Presenter pres;
        private List<BudgetItem> budgetItems;
        static int current = 0;
        /// <summary>
        /// 
        /// </summary>
        public DataTable()
        {
            InitializeComponent();

            datagridDisplay.AutoGenerateColumns = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void DisplayForCategory(List<BudgetItemsByCategory> data)
        {
            searchBar.Visibility = Visibility.Hidden;
            datagridDisplay.ItemsSource = data;

            datagridDisplay.Columns.Clear();

            var column = new DataGridTextColumn();
            column.Header = " Category ";
            column.Binding = new Binding("Category");
            datagridDisplay.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = " Total ";
            column.Binding = new Binding("Total");
            datagridDisplay.Columns.Add(column);
            //cat, total
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void DisplayForMonth(List<BudgetItemsByMonth> data)
        {
            searchBar.Visibility = Visibility.Hidden;
            datagridDisplay.ItemsSource = data;

            datagridDisplay.Columns.Clear();

            var column = new DataGridTextColumn();
            column.Header = " Month ";
            column.Binding = new Binding("Month");
            datagridDisplay.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = " Total ";
            column.Binding = new Binding("Total");
            datagridDisplay.Columns.Add(column);
            //month, total
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void DisplayForMonthAndCategory(List<Dictionary<string, object>> data)
        {
            searchBar.Visibility = Visibility.Hidden;
            List<Category> cats;
            datagridDisplay.ItemsSource = data;

            datagridDisplay.Columns.Clear();

            var column = new DataGridTextColumn();
            column.Header = " Month ";
            column.Binding = new Binding("[Month]");
            datagridDisplay.Columns.Add(column);

            cats = pres.GetCategories();
            foreach (Category cat in cats)
            {
                column = new DataGridTextColumn();
                column.Header = " " + cat.Description + " ";
                column.Binding = new Binding("[" + cat.Description + "]");
                datagridDisplay.Columns.Add(column);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void DisplayForStandard(List<BudgetItem> data)
        {
            if(data != null && data.Count != 0)
            {
                datagridDisplay.ItemsSource = data;
                datagridDisplay.SelectedItem = datagridDisplay.Items[current];
                searchBar.Visibility = Visibility.Visible;
                budgetItems = data;
                datagridDisplay.Columns.Clear();

                var column = new DataGridTextColumn();
                column.Header = " Date ";
                column.Binding = new Binding("Date");
                datagridDisplay.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Header = " Category ";
                column.Binding = new Binding("Category");
                datagridDisplay.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Header = " Description ";
                column.Binding = new Binding("ShortDescription");
                datagridDisplay.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Header = " Amount ";
                column.Binding = new Binding("Amount");
                datagridDisplay.Columns.Add(column);

                column = new DataGridTextColumn();
                column.Header = " Balance ";
                column.Binding = new Binding("Balance");
                datagridDisplay.Columns.Add(column);
            }


            //date, category, desc, amount, balance
        }
        private void datagridDisplay_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            menu_Update_Click(sender, e);
        }

        private void menu_Update_Click(object sender, RoutedEventArgs e)
        {
            var selected = datagridDisplay.SelectedItem as BudgetItem;

            if (selected != null)
            {
                //UpdateExpense updateExpense = new UpdateExpense(pres, selected);
                //
                //updateExpense.ShowDialog();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_Delete_Click(object sender, RoutedEventArgs e)
        {


            //BudgetItem data = datagridDisplay.SelectedItem as BudgetItem;
            //if (data != null)
            //{
            //    pres.DeleteExpense(data.ExpenseID);
            //}



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="presenter"></param>
        public void SetPresenter(Presenter presenter)
        {
            this.pres = presenter;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {
                var xf = budgetItems.Where(x => x.ShortDescription.ToLower().Contains(tbx.Text.ToLower()) || x.Amount.ToString().Contains(tbx.Text));
                if (xf.Count() == 0)
                {
                    MessageBox.Show("No matches with the description or amount", "No results were found", MessageBoxButton.OK);
                    datagridDisplay.RowBackground = null;
                    datagridDisplay.SelectedItem = null;
                    datagridDisplay.ItemsSource = budgetItems;
                }
                else
                {

                    datagridDisplay.ItemsSource = null;
                    datagridDisplay.ItemsSource = xf;
                    current = 0;
                    datagridDisplay.SelectedItem = datagridDisplay.Items[current];
                }
            }
            else
            {
                datagridDisplay.SelectedItem = null;
                datagridDisplay.RowBackground = null;
                datagridDisplay.ItemsSource = budgetItems;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ++current;
            datagridDisplay.SelectedItem = datagridDisplay.Items[current % datagridDisplay.Items.Count];
        }
    }
}
