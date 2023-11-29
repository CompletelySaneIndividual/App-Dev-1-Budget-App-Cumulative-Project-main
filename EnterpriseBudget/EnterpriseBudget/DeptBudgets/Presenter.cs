using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnterpriseBudget.Model;
using Budget;
using EnterpriseBudget.DeptBudgets.Interfaces;

using System.Data.SqlClient;



namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Presenter logic for the DeptBudgets.View
    /// </summary>
    public class Presenter
    {
        Model.DepartmentBudgets budget;
        InterfaceView view;
        int deptId;
        bool fileSaved = true;
        public bool allowedToClose = false;
        //public IViewToPresenter mainView;
        private IPresenterToView catExpenseView;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_view">View associated with this presenter</param>
        /// <param name="deptId">This presenter is for a specific department</param>
        public Presenter(InterfaceView _view,int deptId)
        {
            view = _view;
            this.deptId = deptId;
        }
        
        /// <summary>
        /// Get the data from the database, etc
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public bool LoadData()
        {
            budget = new Model.DepartmentBudgets();
            return budget.DownLoadAndOpenDepartmentBudgetFile(deptId);
        }

        /// <summary>
        /// The view is closing, and needs to tidy-up by calling
        /// this routine.
        /// </summary>
        public void onClose()
        {
            if(fileSaved == false)
            {
                view.ShowNotSavedMessageBox("File is not save are you sure you wish to conitnue?", "File Not Saved");
            }
            else
            {
                allowedToClose = true;
            }

            if (budget != null && allowedToClose)
            {
                budget.Close();
            }
            else
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="filter"></param>
        /// <param name="idOfCatSelected"></param>
        /// <param name="sumerizeByMonth"></param>
        /// <param name="sumerizeByCat"></param>
        /// <param name="sumerize"></param>
        public void DisplayData(DateTime start, DateTime end, bool filter, int idOfCatSelected, bool sumerizeByMonth, bool sumerizeByCat, IDataSumerize sumerize)
        {
            sumerize.SetPresenter(this);
            if (sumerizeByMonth && sumerizeByCat)
            {
                sumerize.DisplayForMonthAndCategory(budget.GetBudgetDictionaryByCategoryAndMonth(start, end, filter, idOfCatSelected));
            }
            else if (sumerizeByCat)
            {
                sumerize.DisplayForCategory(budget.GetBudgetItemsByCategory(start, end, filter, idOfCatSelected));
            }
            else if (sumerizeByMonth)
            {
                sumerize.DisplayForMonth(budget.GetBudgetItemsByMonth(start, end, filter, idOfCatSelected));
            }
            else
            {
                sumerize.DisplayForStandard(budget.GetBudgetItems(start, end, filter, idOfCatSelected));
            }
        }
        public List<Category> GetCategories()
        {
            if (budget != null)
                return budget.GetCategories();
            return null;
        }
        public void InitializeCatExpView(IPresenterToView view)
        {
            catExpenseView = view;
        }
        public void AddExpense(DateTime date, string selectedCat, string amount_, string description, double limit, double total)
        {
            
            string mssg = "The expense was ";
            string caption;
            string errors = ValidateExpenseFields(date, amount_, description, selectedCat, limit, total);
            if (errors == string.Empty)
            {
                int catId = GetCategoryId(selectedCat);
                double amount = Double.Parse(amount_);
                if (ExpenseAlreadyExists(date, catId, amount, description))
                {
                    mssg += " not added\n\n";
                    errors = "An exact same expense already exists";
                    catExpenseView.ShowErrorMessage(mssg, errors, "Invalid expense");
                }
                else
                {

                        mssg += "successfully added";
                        caption = "Valid expense";
                        budget.homeBudget.expenses.Add(date, catId, amount, description);

                    view.UpdateGrid();
                    catExpenseView.UpdateCatLimit();
                    catExpenseView.UpdateCatTotal();
                    catExpenseView.ShowSuccessMessage(mssg, caption);
                }
                catExpenseView.ResetFields();
                fileSaved = false;
            }
            else
            {
                    mssg += "not added\n\n";
                    caption = "Invalid expense";

                catExpenseView.ShowErrorMessage(mssg, errors, caption);
            }
            
        }
        public int GetCategoryId(string catDesc)
        {
            List<Category> cats = GetCategories();
            int catId = 0;
            foreach (Category cat in cats)
            {
                if (cat.Description == catDesc)
                    catId = cat.Id;
            }
            return catId;
        }
        public bool ExpenseAlreadyExists(DateTime date, int cat, double amount, string desc)
        {
            List<Expense> explist = budget.homeBudget.expenses.List();
            foreach (Expense expense in explist)
            {
                if (date == expense.Date && cat == expense.Category && amount == expense.Amount && desc == expense.Description)
                    return true;
            }
            return false;
        }
        public string ValidateExpenseFields(DateTime date, string amount, string description, string selectedCat, double limit, double total)
        {
            string required = " is a required field\n";
            string invalid = " entered is invalid\n";
            string negativeError = " must be greater than zero\n";
            StringBuilder errors = new StringBuilder();

            DateTime _date;
            double _amount;
            string _description;

            // Amount negative?
            if (amount == string.Empty)
                errors.Append("Amount" + required);
            else
                if (!ValidNumber(amount))
                errors.Append("Amount" + invalid);
            else if (Double.Parse(amount) <= 0)
                errors.Append("Amount" + negativeError);
            else
            {
                _amount = Double.Parse(amount);
                if (_amount+total>limit)
                    errors.Append("Amount exceeds limit");
            }

            // Check Description
            if (description == string.Empty)
                errors.Append("Description" + required);
            else
                _description = description;

            // Check category
            if (selectedCat == "")
                errors.Append("Category" + required);

            // Check date
            if (date == null)
                errors.Append("Date" + required);
            else
                _date = date;

            return errors.ToString();
        }
        public bool ValidNumber(string value) => double.TryParse(value, out _);

        public Category.CategoryType GetCatType(string catChose)
        {
            for (int i = 1; i <= Enum.GetNames(typeof(Category.CategoryType)).Length; i++)
            {
                if (((Category.CategoryType)i).ToString() == catChose)
                    return (Category.CategoryType)i;
            }
            return Category.CategoryType.Credit; // This line should never be reached
        }
        public List<Category.CategoryType> GetCategoryTypes() => Enum.GetValues(typeof(Category.CategoryType)).Cast<Category.CategoryType>().ToList();


        public double Limit(int catId)
        {
            SqlCommand getCatLimit = new SqlCommand(
                "SELECT limit " +
                "FROM budgetCategoryLimits " +
                $"WHERE catId={catId} AND " +
                $"deptId= {deptId}", Model.Connection.cnn);

            SqlDataReader rdr = getCatLimit.ExecuteReader();
            rdr.Read();
            var p=rdr.GetDouble(0);
            return p;
        }
        public List<BudgetItemsByCategory> budgetsOfCategory(int catId) => budget.GetBudgetItemsByCategory(null, null, false, catId);

        /// <summary>
        /// Saves the current db file to the sql server
        /// </summary>
        public void SaveFile()
        {
            try
            {   
                
                budget.Close();
                budget.WriteBlobToSQLServer(Model.Connection.cnn, "deptBudget.db", "deptBudgets", "sqlitefile", $"deptId={deptId};");
                fileSaved = true;
                LoadData();
                
            }
            catch(Exception e) { 

            }
        }
    }

    

}
