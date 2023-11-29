using Home_Budget_WPF_App.Interfaces;
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
using System.Windows.Controls.DataVisualization.Charting;
using Budget;

namespace Home_Budget_WPF_App
{
    /// <summary>
    /// Interaction logic for ChartTable.xaml
    /// </summary>
    public partial class ChartTable : UserControl, IDataSumerize
    {
        
        public void DisplayForMonthAndCategory(List<Dictionary<string, object>> data)
        {
            List<Category> cats = presenter.budget.categories.List();
            Categories = new List<string>();
            foreach (Category c in cats)
                Categories.Add(c.Description);
            
            InitializeByCategoryAndMonthDisplay(Categories);
            List<Object> theObjects = data.Cast<object>().ToList();
            this.DataSource = theObjects;
        }

        public void DisplayForCategory(List<BudgetItemsByCategory> data)
        {
            List<Category> cats = presenter.budget.categories.List();
            Categories = new List<string>();
            foreach (Category c in cats)
                Categories.Add(c.Description);

            InitializeByCategoryDisplay();
            List<Object> theObjects = data.Cast<object>().ToList();
            this.DataSource = theObjects;
        }

        public void DisplayForMonth(List<BudgetItemsByMonth> data)
        {
            List<Category> cats = presenter.budget.categories.List();
            Categories = new List<string>();
            foreach (Category c in cats)
                Categories.Add(c.Description);

            InitializeByMonthDisplay();
            List<Object> theObjects = data.Cast<object>().ToList();
            this.DataSource = theObjects;
        }

        public void DisplayForStandard(List<BudgetItem> data)
        {
            List<Category> cats = presenter.budget.categories.List();
            Categories = new List<string>();
            foreach (Category c in cats)
                Categories.Add(c.Description);

            InitializeStandardDisplay();
            List<Object> theObjects = data.Cast<object>().ToList();
            this.DataSource = theObjects;
        }
        
        // ----------------------------------------------------------------------------------
        // private globals
        // ----------------------------------------------------------------------------------
        private List<object> _dataSource;
        private enum ChartType
        {
            Standard,
            ByCategory,
            ByMonth,
            ByMonthAndCategory
        }
        private ChartType chartType = ChartType.Standard;
        private List<string> Categories;
        // ----------------------------------------------------------------------------------
        // public properites
        // ----------------------------------------------------------------------------------
        public Presenter presenter { get; set; }
        public List<object> DataSource
        {
            get { return _dataSource; }
            set
            {
                // if changing data source, then redraw chart
                _dataSource = value;
                if (chartType == ChartType.ByMonthAndCategory)
                    drawByMonthPieChart();
                if (chartType == ChartType.ByMonth) 
                    drawByMonthLineChart();
                if (chartType == ChartType.ByCategory)
                    drawByCategoryPieChart();
                if (chartType == ChartType.Standard)
                    drawByStandardPieChart();
            }
        }
        #region public methods
        // -----------------------------------------------------------------------------------
        // constructor
        // -----------------------------------------------------------------------------------
        public ChartTable()
        {
            InitializeComponent();
        }
        // -----------------------------------------------------------------------------------
        // clear the current data
        // -----------------------------------------------------------------------------------
        public void DataClear()
        {
            ((PieSeries)chPie.Series[0]).ItemsSource = null;
        }
        // -----------------------------------------------------------------------------------
        // Get prepared for displaying Month and Category
        // Inputs: usedCategoryList... a list of categories
        // -----------------------------------------------------------------------------------
        public void InitializeByCategoryAndMonthDisplay(List<string> CategoryList)
        {
            txtTitle.Text = "By Month";
            chartType = ChartType.ByMonthAndCategory; // set chart type appropriately

            chPie.Visibility = Visibility.Visible; // show the pie chart
            txtInvalid.Visibility = Visibility.Hidden; // hide the "invalid parameters" text

            this.Categories = CategoryList; // save the categories list
        }
        // -----------------------------------------------------------------------------------
        // prepare for 'byCategory',
        // NOTE: just show invalid text... this chart is not implemented
        // -----------------------------------------------------------------------------------
        public void InitializeByCategoryDisplay()
        {
            txtTitle.Text = "By Category";
            chartType = ChartType.ByCategory;

            chPie.Visibility = Visibility.Visible;
            txtInvalid.Visibility = Visibility.Hidden;
        }
        // -----------------------------------------------------------------------------------
        // prepare for 'byMonth',
        // NOTE: just show invalid text... this chart is not implemented
        // -----------------------------------------------------------------------------------
        public void InitializeByMonthDisplay()
        {
            txtTitle.Text = "By Month";
            chartType = ChartType.ByMonth;

            chPie.Visibility = Visibility.Visible;
            txtInvalid.Visibility = Visibility.Hidden;
        }
        // -----------------------------------------------------------------------------------
        // prepare for standard display,
        // NOTE: just show invalid text... this chart is not implemented
        // -----------------------------------------------------------------------------------
        public void InitializeStandardDisplay()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }
        #endregion

        // -----------------------------------------------------------------------------------
        // draw by Month is NOT implemented :(
        // -----------------------------------------------------------------------------------
        private void drawByMonthLineChart()
        {
            // create a list of months from the source data
            List<String> months = new List<String>();
            foreach (object obj in _dataSource)
            {
                var item = obj as BudgetItemsByMonth;
                if (item != null)
                {
                    months.Add(item.Month);
                }
            }
            // add the months to the combobox dropdown
            cbMonths.ItemsSource = months;
            // reset selected index to last 'month' in list
            cbMonths.SelectedIndex = -1;
            // set the data for the pie-chart
            set_Month_Data();
        }
        private void drawByCategoryPieChart()
        {
            // create a list of months from the source data
            List<String> months = new List<String>();
            foreach (object obj in _dataSource)
            {
                var item = obj as BudgetItemsByCategory;
                if (item != null)
                {
                    months.Add(item.Category);
                }
            }
            // add the months to the combobox dropdown
            cbMonths.ItemsSource = months;
            // reset selected index to last 'month' in list
            cbMonths.SelectedIndex = -1;
            // set the data for the pie-chart
            set_Category_Data();
        }
        private void drawByStandardPieChart()
        {

        }
        #region byMonthAndCategory
        // --------------------------------------------------------------------
        // Draw the 'ByMonth' chart
        // --------------------------------------------------------------------
        private void drawByMonthPieChart()
        {
            // create a list of months from the source data
            List<String> months = new List<String>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                if (item != null)
                {
                    months.Add(item["Month"].ToString());
                }
            }
            // add the months to the combobox dropdown
            cbMonths.ItemsSource = months;
            // reset selected index to last 'month' in list
            cbMonths.SelectedIndex = -1;
            // set the data for the pie-chart
            set_MonthCategory_Data();
        }
        // --------------------------------------------------------------------
        // define the data for the given month from the datasoure,
        // ... which in this case is a list of Dictionary<String,object>
        // defining totals for each category for a given month
        // --------------------------------------------------------------------
        private void set_MonthCategory_Data()
        {
            DataClear();
            // bail out if there are no 'month' items in the drop down
            if (cbMonths.Items.Count == 0) return;
            // set the default selection to the last in the list
            if (cbMonths.SelectedIndex < 0 || cbMonths.SelectedIndex >

            cbMonths.Items.Count - 1)

            {
                cbMonths.SelectedIndex = cbMonths.Items.Count - 1;
            }
            // what is the selected month?
            String selectedMonth = cbMonths.SelectedItem.ToString();
            // ---------------------------------------------------------------
            // define which data is to be displayed
            // ---------------------------------------------------------------
            var DisplayData = new List<KeyValuePair<String, double>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                // is the item listed in the _dataSource part of the selected month ?
        
                if (item != null && (string)item["Month"] == selectedMonth)
                {
                // go through each key/value pair in this item (item is a dictionary)
                    foreach (var pair in item)
                    {
                        String category = pair.Key;
                        String value = pair.Value.ToString();
                        // if the key is not a category, skip processing
                        if (!Categories.Contains(category)) continue;
                        // what is the amount of money for this category (item[category])

                        var amount = 0.0;
                        double.TryParse(value, out amount);
                        // only display expenses (i.e., amount < 0)
                        if (amount< 0)
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, -amount));
                        }
                        else
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, amount));
                        }

                    }
                    // we found the month we wanted, no need to loop through other months, so
                    // stop looking
                    break;
                }
            }
            // set the data for the pie-chart
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }
        private void set_Month_Data()
        {
            DataClear();
            // bail out if there are no 'month' items in the drop down
            if (cbMonths.Items.Count == 0) return;
            // set the default selection to the last in the list
            if (cbMonths.SelectedIndex < 0 || cbMonths.SelectedIndex > cbMonths.Items.Count - 1)
            {
                cbMonths.SelectedIndex = cbMonths.Items.Count - 1;
            }
            // what is the selected month?
            String selectedMonth = cbMonths.SelectedItem.ToString();
            // ---------------------------------------------------------------
            // define which data is to be displayed
            // ---------------------------------------------------------------
            var DisplayData = new List<KeyValuePair<String, double>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as BudgetItemsByMonth;
                // is the item listed in the _dataSource part of the selected month ?

                if (item != null && item.Month == selectedMonth)
                {

                    // go through each key/value pair in this item (item is a dictionary)
                    foreach (var pair in item.Details)
                    {
                        String category = pair.ShortDescription;
                        String value = pair.Amount.ToString();
                        // if the key is not a category, skip processing
                        //if (!Categories.Contains(category)) continue;
                        // what is the amount of money for this category (item[category])

                        var amount = 0.0;
                        double.TryParse(value, out amount);
                        // only display expenses (i.e., amount < 0)
                        if (amount < 0)
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, -amount));
                        }
                        else
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, amount));
                        }

                    }
                    // we found the month we wanted, no need to loop through other months, so
                    // stop looking
                    break;
                }
            }
            // set the data for the pie-chart
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }
        private void set_Category_Data()
        {
            DataClear();
            // bail out if there are no 'month' items in the drop down
            if (cbMonths.Items.Count == 0) return;
            // set the default selection to the last in the list
            if (cbMonths.SelectedIndex < 0 || cbMonths.SelectedIndex > cbMonths.Items.Count - 1)
            {
                cbMonths.SelectedIndex = cbMonths.Items.Count - 1;
            }
            // what is the selected month?
            String selectedMonth = cbMonths.SelectedItem.ToString();
            // ---------------------------------------------------------------
            // define which data is to be displayed
            // ---------------------------------------------------------------
            var DisplayData = new List<KeyValuePair<String, double>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as BudgetItemsByCategory;
                // is the item listed in the _dataSource part of the selected month ?

                if (item != null && item.Category == selectedMonth)
                {

                    // go through each key/value pair in this item (item is a dictionary)
                    foreach (var pair in item.Details)
                    {
                        String category = pair.ShortDescription;
                        String value = pair.Amount.ToString();
                        // if the key is not a category, skip processing
                        //if (!Categories.Contains(category)) continue;
                        // what is the amount of money for this category (item[category])

                        var amount = 0.0;
                        double.TryParse(value, out amount);
                        // only display expenses (i.e., amount < 0)
                        if (amount < 0)
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, -amount));
                        }
                        else
                        {
                            DisplayData.Add(new KeyValuePair<String, double>

                            (category, amount));
                        }

                    }
                    // we found the month we wanted, no need to loop through other months, so
                    // stop looking
                    break;
                }
            }
            // set the data for the pie-chart
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }
        #endregion
        private void cbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chartType == ChartType.ByMonthAndCategory)
                set_MonthCategory_Data();
            if (chartType == ChartType.ByMonth)
                set_Month_Data();
            if (chartType == ChartType.ByCategory)
                set_Category_Data();
        }

        public void SetPresenter(Presenter presenter)
        {
            this.presenter = presenter;
        }
    }
}