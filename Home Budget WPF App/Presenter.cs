using Budget;
using Home_Budget_WPF_App.Interfaces;
using Home_Budget_WPF_App.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Home_Budget_WPF_App
{

    /// <summary>
    /// a presenter class for the WPF Budget
    /// </summary>
    public class Presenter : INotifyPropertyChanged

    {
        static string DEFAULT_DB_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Budgets\\";
        static string DEFAULT_DB_FILE= DEFAULT_DB_PATH + "DefaultBudget.db";
        string dbfile;
        public IViewToPresenter mainView;
        private IPresenterToView catExpenseView;
        IniFileUtils iniFileUtils;
        public HomeBudget budget;
        private bool resetflag;
        public string DefBudget  => DEFAULT_DB_FILE; 
        public bool ResetFlag { get { return resetflag; } set { resetflag = value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name = null)
        {
            //Raise PropertyChanged event
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }

            remove
            {
                PropertyChanged -= value;
            }
        }

        private string path;
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;

                NotifyPropertyChanged("Path");
            }
        }
        /// <summary>
        /// construstor that intializes with a default db file
        /// </summary>
        /// <param name="viewToPresenter"></param>

        public Presenter(IViewToPresenter viewToPresenter)  // Designed for choose file view
        {
            mainView = viewToPresenter;
            if (!File.Exists(IniFileUtils.INI_FILE + ".ini"))
            {
                File.Create(IniFileUtils.INI_FILE + ".ini");//make sure the file exists
            }
            iniFileUtils = new IniFileUtils(IniFileUtils.INI_FILE + ".ini");
            dbfile = "";
            try
            {
                bool choice = mainView.askForDefault("Load previous budget file if exists or create default budget?", "Load or create default budget", MessageBoxButton.YesNoCancel);
                if (choice)
                { 
                    loadOrCreateDefBudget();
                }
                

            }
            catch (Exception e)
            {
                mainView.ShowMessagebox("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        public void loadOrCreateDefBudget()
        {

            dbfile = "";

            if (!iniFileUtils.KeyExists("defaultPath"))
            {
                if (!Directory.Exists(DEFAULT_DB_PATH))
                    Directory.CreateDirectory(DEFAULT_DB_PATH);
                if (!File.Exists(DEFAULT_DB_FILE))
                {
                    using (FileStream file = File.Create(DEFAULT_DB_FILE))   // When file is created is left as open, so close it
                    {
                        file.Close();
                    }
                }


                dbfile = DEFAULT_DB_FILE;
                iniFileUtils.Write("defaultPath", System.IO.Path.GetDirectoryName(dbfile));
                iniFileUtils.Write("databaseFile", System.IO.Path.GetFileName(dbfile));

                budget = new HomeBudget(dbfile, true);
                Path = dbfile;
            }
            else
            {
                dbfile = iniFileUtils.Read("defaultPath") + iniFileUtils.Read("databaseFile");
                budget = new HomeBudget(dbfile);
                Path = dbfile;

            }
        }

        #region File View Business

        public string GetPath()
        {
            if (iniFileUtils.KeyExists("defaultPath"))
            {
                return iniFileUtils.Read("defaultPath");
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Budgets";
            }


            //throw new Exception("A directory has yet to be selected");
        }
        public string GetFile()
        {
            if (iniFileUtils.KeyExists("databaseFile"))
                return iniFileUtils.Read("databaseFile");
            throw new Exception("A file has yet to be selected");
        }
        /// <summary>
        /// Chooses another file to use other than the default file
        /// </summary>
        public void pickDatabaseFile(string dbfile)
        {
            HomeBudget prevHomeBudget = budget;
            try
            {
                if(mainView.AskForReset("Would you like to reset this database file, reset will remove all your budgets", "Reset", MessageBoxButton.YesNo))
                {
                   if(prevHomeBudget != null)
                       prevHomeBudget.CloseDB();
                   budget = new HomeBudget(dbfile, true);
                }
                else
                    budget = new HomeBudget(dbfile);


                Path = dbfile;

                mainView.UpdateCatCmbBox();
                mainView.UpdateGrid();
                // Sets db file to ini file, so next time it will remember it if user chooses to load previous
                iniFileUtils.Write("defaultPath", System.IO.Path.GetDirectoryName(dbfile) + "\\");  
                iniFileUtils.Write("databaseFile", System.IO.Path.GetFileName(dbfile));
                
            }
            catch (Exception e)
            {
                if (e.Message.Contains("used by another process"))
                {
                    mainView.ShowMessagebox("you are already using this database!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    mainView.ShowMessagebox("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    budget = prevHomeBudget;
                }

            }

        }
        #endregion


        #region Expense Business
        /// <summary>
        /// adding an expense to the expenses list
        /// </summary>
        /// <param name="date"></param>
        /// <param name="categoryId"></param>
        /// <param name="amount"></param>
        /// <param name="description"></param>
        public void AddExpense(DateTime date, string selectedCat, string amount, string description)
        {
            UpdateOrAdd(-1, date, selectedCat, amount, description);
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

        /// <summary>
        /// gets a list of expenses in the budget
        /// </summary>
        /// <returns> list of Expense objects</returns>
        public List<Expense> GetExpenses()
        {
            return budget.expenses.List();
        }
        public void UpdateExpense(int expId, DateTime newDate, string newCat, string amount ,string newDesc)
        {
            UpdateOrAdd(expId, newDate, newCat, amount, newDesc);
        }
        // To avoid code redundancy in update and add methods
        public void UpdateOrAdd(int expId, DateTime newDate, string newCat, string newAmount, string newDesc )
        {
            string mssg = "The expense was ";
            string caption;
            string errors = ValidateExpenseFields(newDate, newAmount, newDesc, newCat);
            if (errors == string.Empty)
            {
                int catId = GetCategoryId(newCat);
                double amount = Double.Parse(newAmount);
                if (ExpenseAlreadyExists(newDate, catId, amount, newDesc))
                {
                    mssg += " not added\n\n";
                    errors = "An exact same expense already exists";
                    catExpenseView.ShowErrorMessage(mssg, errors, "Invalid expense");
                }
                else
                {

                    if (expId != -1) // If it is not a -1 is an update
                    {
                        mssg += "successfully updated";
                        caption = "Updated expense";
                        budget.expenses.UpdateProperties(expId, newDate, catId, amount, newDesc);
                    }
                    else
                    {
                        mssg += "successfully added";
                        caption = "Valid expense";
                        budget.expenses.Add(newDate, catId, amount, newDesc);
                    }

                    mainView.UpdateGrid();
                    catExpenseView.ShowSuccessMessage(mssg, caption);
                }
                catExpenseView.ResetFields();

            }
            else
            {
                if (expId != -1)
                {
                    mssg += "not updated\n\n";
                    caption = "Invalid update";
                }
                else
                {
                    mssg += "not added\n\n";
                    caption = "Invalid expense";
                }
                
                catExpenseView.ShowErrorMessage(mssg, errors, caption);
            }
        }

        public void DeleteExpense(int id)
        {
            try
            {
                budget.expenses.Delete(id);
                mainView.UpdateGrid();
                if(catExpenseView != null)
                {
                    catExpenseView.ShowSuccessMessage("The expense was deleted successfully", "Valid deletion");
                    catExpenseView.ResetFields(); //closes window
                }
            }
            catch (Exception e)
            {
                string error = "Unexpected error: the expense was not deleted";
                catExpenseView.ShowErrorMessage(error,e.Message,"Delete failed");
            }
        }
        public bool ExpenseAlreadyExists(DateTime date, int cat, double amount, string desc)
        {
            List<Expense> explist = budget.expenses.List();
            foreach (Expense expense in explist)
            {
                if (date == expense.Date && cat == expense.Category && amount == expense.Amount && desc == expense.Description )
                    return true;
            }
            return false;
        }
        #endregion


        #region Category Business
        public void AddCategory(string description, string categoryType)
        {
            #region checking for duplicate expense beforehand (unfinished)
            //List<Expense> expenses = GetExpenses();
            //Expense oldExpense = expenses.Last();

            //if (oldExpense.Date == date && oldExpense.Category == categoryId && oldExpense.Amount == amount && oldExpense.Description == description)
            //{
            //    view.showAnswerMessagebox()
            //}
            #endregion
            try
            {
                string mssg = "The category was ";
                string caption;

                string errors = ValidateCategoryFields(description, categoryType);
                if (errors == string.Empty)
                {
                    if (CatDescAlreadyExists(description))
                    {
                        mssg += "not added\n\n";
                        errors = "A category with that description already exists";
                        caption = "Invalid category";

                        catExpenseView.ShowErrorMessage(mssg, errors, caption);
                    }
                    else
                    {
                        Category.CategoryType cat = GetCatType(categoryType);
                        budget.categories.Add(description, cat);
                        mssg += "added";
                        caption = "Valid category";

                        mainView.UpdateCatCmbBox();
                        catExpenseView.ShowSuccessMessage(mssg, caption);
                    }

                }
                else
                {
                    mssg += "not added\n\n";
                    caption = "Invalid category";

                    catExpenseView.ShowErrorMessage(mssg, errors, caption);
                }
                catExpenseView.ResetFields();
            }
            catch (Exception e)
            {
                mainView.ShowMessagebox("Category was not added - Error:\n"+e.Message,"Error with database", MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }

        public Category.CategoryType GetCatType(string catChose)
        {
            for (int i = 1; i <= Enum.GetNames(typeof(Category.CategoryType)).Length; i++)
            {
                if (((Category.CategoryType)i).ToString() == catChose)
                    return (Category.CategoryType)i;
            }
            return Category.CategoryType.Credit; // This line should never be reached
        }
        public List<Category.CategoryType> GetCategoryTypes()=> Enum.GetValues(typeof(Category.CategoryType)).Cast<Category.CategoryType>().ToList();
        
        public List<Category> GetCategories()
        {
            if(budget != null)
            return budget.categories.List();
            return null;
        }

        public List<Category> GetAllCatsDescription()
        {
            return GetCategories();
        }
        public bool CatDescAlreadyExists(string desc)
        {
            
            List<Category> cats = budget.categories.List();
            foreach(Category c in cats)
            {
                if (desc == c.Description)
                    return true;
            }
            return false;
        }
        #endregion


        #region Validation Business
        public string ValidateExpenseFields(DateTime date, string amount, string description, string selectedCat)
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
                _amount = Double.Parse(amount);

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

        public string ValidateCategoryFields(string description, string categoryType)
        {
            string required = " is a required field\n";
            StringBuilder sB = new StringBuilder();

            if (description == string.Empty)
                sB.Append("Description" + required);
            if (categoryType == string.Empty)
                sB.Append("Category" + required);

            return sB.ToString();
        }
        #endregion

        public void CloseDB()
        {
            if(budget!=null)
            budget.CloseDB();
        }

        public List<BudgetItem> GetBudgetItems(DateTime start, DateTime end, bool filterFlag, int catId)
        {
            return budget.GetBudgetItems(start, end, filterFlag, catId);
        }

        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime start, DateTime end, bool filterFlag, int catId)
        {
            return budget.GetBudgetItemsByMonth(start, end, filterFlag, catId);
        }

        public List<BudgetItemsByCategory> GetBudgetItemsByCategory(DateTime start, DateTime end, bool filterFlag, int catId)
        {

            return budget.GetBudgetItemsByCategory(start, end, filterFlag, catId);
        }

        public List<Dictionary<string, object>> GetBudgetDictionaryByCategoryAndMonth(DateTime start, DateTime end, bool filterFlag, int catId)
        {
            return budget.GetBudgetDictionaryByCategoryAndMonth(start, end, filterFlag, catId);
        }

        // Initialize view
        public void InitializeCatExpView(IPresenterToView view)
        {
            catExpenseView = view;
        }
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
        public void AskForFile()
        {
            string filter = "Database Files|*.db|Text Files|*.txt|Any Files|*.*";
            string title = "Please select a database file for your application";
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Budgets";
            mainView.AskForFile(filter,title,initialDir);
        }
    }

}
