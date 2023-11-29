using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Home_Budget_WPF_App.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;
using Home_Budget_WPF_App;
using Home_Budget_WPF_App.Utils;
using System.Runtime;
using Budget;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1Net
{
    public class TestView : IPresenterToView
    {
        public bool calledResetFields;
        public bool calledSetDefaultView;
        public bool calledErrorMessage;
        public bool calledSuccessMessage;
        public void ResetFields()
        {
            calledResetFields = true;
        }

        public void SetDefaultViewValues()
        {
            calledSetDefaultView = true;
        }

        public void ShowErrorMessage(string mssg, string errors, string caption)
        {
            calledErrorMessage = true;
        }

        public void ShowSuccessMessage(string mssg, string caption)
        {
            calledSuccessMessage = true;
        }
    }
    public class TestViewFile : IViewToPresenter
    {
        public bool calledShowMessageBox;
        public bool calledUpdateCatCmbBox;
        public bool calledUpdateGrid;
        public bool calledAskForDef;
        public bool calledAskForFile;

        public bool askForDefault(string messageBoxFix, string caption, object btn)
        {
            calledAskForDef = true;
            return true;
        }

        public bool AskForReset(string messageBoxFix, string caption, object btn)
        {
            throw new NotImplementedException();
        }

        public void ShowMessagebox(string messageBoxFix, string caption, object btn, object img)
        {
            calledShowMessageBox = true;
        }

        public void UpdateCatCmbBox()
        {
            calledUpdateCatCmbBox = true;
        }

        public void UpdateGrid()
        {
            calledUpdateGrid = true;
        }

        public void AskForFile(string filter, string title, string initialDir)
        {
            calledAskForFile=true;
        }
    }

    [TestClass]
    public class UnitTest1
    {
        private static Random random = new Random();

        Presenter presenter;
        TestViewFile testViewFile;
        TestView testCatExpView;

        string defBudget = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Budgets\\DefaultBudget.db";
        string anotherDb = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Budgets\\newDb.db";
        [TestMethod]
        public void TestAskForFile()
        {
            InitializePresenterAndFileView();
            Assert.IsTrue(testViewFile.calledAskForDef);
            presenter.CloseDB();
        }

        [TestMethod]
        public void TestValidDbFile()
        {
            InitializePresenterAndFileView();
            presenter.pickDatabaseFile(defBudget);
            Assert.AreEqual(presenter.Path, defBudget);
            presenter.CloseDB();
        }

        [TestMethod]
        public void TestConnectionClosedMainWindowShowsError()
        {
            InitializePresenterAndFileView();
            presenter.pickDatabaseFile(defBudget);
            presenter.CloseDB();
            presenter.AddCategory("Connection was closed", "So it should call the message box displaying error");
            Assert.IsTrue(testViewFile.calledShowMessageBox);
            presenter.CloseDB();

        }
        [TestMethod]
        public void TestAskForFilePresenter()
        {
            InitializePresenterAndFileView();
            presenter.AskForFile();
            Assert.IsTrue(testViewFile.calledAskForFile);
            presenter.CloseDB();

        }
        [TestMethod]
        public void TestAddCategorySuccessfully()
        {
            int randomLengthOfString = RandomInt();

            InitializeTwoViewsAndPresenter();
            // Random, since the app does not allow to add duplicate cats
            string randomOne = RandomString(randomLengthOfString);
            string randomTwo = RandomString(randomLengthOfString + 2);
            presenter.AddCategory(randomOne, randomTwo);
            Assert.IsTrue(testViewFile.calledUpdateCatCmbBox);  //Updates the main window categories combo box
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);
            presenter.CloseDB();

        }
        [TestMethod]
        public void TestAddCategoryDuplicate()
        {
            InitializeTwoViewsAndPresenter();

            presenter.AddCategory("Hello", "World");

            testViewFile.calledUpdateCatCmbBox = false;
            testCatExpView.calledSuccessMessage = false;

            presenter.AddCategory("Hello", "World");
            Assert.IsFalse(testViewFile.calledUpdateCatCmbBox); // Not updated since adding duplicates is not valid
            Assert.IsFalse(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);
            presenter.CloseDB();

        }
        [TestMethod]
        public void TestAddCategoryInvalidString()
        {
            InitializeTwoViewsAndPresenter();

            presenter.AddCategory("", "");
            Assert.IsFalse(testViewFile.calledUpdateCatCmbBox);
            Assert.IsFalse(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);
            presenter.CloseDB();

        }

        [TestMethod]
        public void TestAddExpense()
        {
            InitializeTwoViewsAndPresenter();
            int randomLengthOfString = RandomInt();

            string randomTwo = RandomString(randomLengthOfString + 5);
            presenter.AddExpense(DateTime.Now, "Income", "90.8", randomTwo);

            Assert.IsTrue(testViewFile.calledUpdateGrid);
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);

            presenter.CloseDB();

        }
        [TestMethod]
        public void TestAddDuplicateExpense()
        {
            InitializeTwoViewsAndPresenter();
            int randomLengthOfString = RandomInt();
            DateTime now = new DateTime(2015, 12, 31);
            string randomTwo = RandomString(randomLengthOfString + 5);
            presenter.AddExpense(now, "Income", "90.8", randomTwo);
            Assert.IsTrue(testViewFile.calledUpdateGrid);
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);

            testViewFile.calledUpdateGrid = false;
            testCatExpView.calledSuccessMessage= false;
            testCatExpView.calledResetFields= false;

            presenter.AddExpense(now, "Income", "90.8", randomTwo);

            Assert.IsFalse(testViewFile.calledUpdateGrid);
            Assert.IsFalse(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledErrorMessage);     //Duplicates are invalid
            Assert.IsTrue(testCatExpView.calledResetFields);
            presenter.CloseDB();

        }
        [TestMethod]
        public void TestAddInvalidExpense()
        {
            InitializeTwoViewsAndPresenter();
            int randomLengthOfString = RandomInt();

            string randomTwo = RandomString(randomLengthOfString + 5);
            presenter.AddExpense(DateTime.Now, "Income", "true", randomTwo);

            Assert.IsTrue(testCatExpView.calledErrorMessage);
            presenter.CloseDB();

        }

        [TestMethod]
        public void TestDeleteValidExpense()
        {
            InitializeTwoViewsAndPresenter();
            int randomInt = RandomInt();
            string randomTwo = RandomString(randomInt + 5);
            presenter.AddExpense(DateTime.Now, "Savings", "1.8", randomTwo);

            Assert.IsTrue(testViewFile.calledUpdateGrid);
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);

            testViewFile.calledUpdateGrid = false;
            testCatExpView.calledSuccessMessage = false;
            testCatExpView.calledResetFields= false;

            presenter.DeleteExpense(1);

            Assert.IsTrue(testViewFile.calledUpdateGrid);
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);
            presenter.CloseDB();

        }

        [TestMethod]
        public void TestCloseDBDeleteExpense()
        {
            InitializeTwoViewsAndPresenter();
            int randomInt = RandomInt();
            string randomTwo = RandomString(randomInt + 5);
            presenter.AddExpense(DateTime.Now, "Savings", "500.8", randomTwo);

            Assert.IsTrue(testViewFile.calledUpdateGrid);
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);

            presenter.CloseDB();
            presenter.DeleteExpense(1);

            Assert.IsTrue(testCatExpView.calledErrorMessage);
            presenter.CloseDB();

        }

        [TestMethod]
        public void TestUpdateExpense()
        {
            InitializeTwoViewsAndPresenter();
            presenter.UpdateExpense(1, DateTime.Now, "Updated", "15.99", "Description");

            Assert.IsTrue(testViewFile.calledUpdateGrid);
            Assert.IsTrue(testCatExpView.calledSuccessMessage);
            Assert.IsTrue(testCatExpView.calledResetFields);
            presenter.CloseDB();

        }

        public class TestChart : IDataSumerize
        {
            public bool calledDisplayForCategory = false;
            public bool calledDisplayForMonth = false;
            public bool calledDisplayForMonthAndCategory = false;
            public bool calledDisplayForStandard = false;
            public bool calledSetPresenter = false;
            public void DisplayForCategory(List<BudgetItemsByCategory> data)
            {
                calledDisplayForCategory = true;
            }

            public void DisplayForMonth(List<BudgetItemsByMonth> data)
            {
                calledDisplayForMonth = true;
            }

            public void DisplayForMonthAndCategory(List<Dictionary<string, object>> data)
            {
                calledDisplayForMonthAndCategory = true;
            }

            public void DisplayForStandard(List<BudgetItem> data)
            {
                calledDisplayForStandard = true;
            }

            public void SetPresenter(Presenter presenter)
            {
                calledSetPresenter = true;
            }
        }
        [TestMethod]
        public void CallsDisplayForCategory()
        {
            InitializeTwoViewsAndPresenter();
            TestChart testChart = new TestChart();
            presenter.DisplayData(DateTime.Now, DateTime.Now, true, 1, false, true, testChart);
            Assert.IsTrue(testChart.calledDisplayForCategory);
            presenter.CloseDB();

        }
        [TestMethod]
        public void CallsDisplayForMonth()
        {
            InitializeTwoViewsAndPresenter();
            TestChart testChart = new TestChart();
            presenter.DisplayData(DateTime.Now, DateTime.Now, true, 1, true, false, testChart);
            Assert.IsTrue(testChart.calledDisplayForMonth);
            presenter.CloseDB();

        }
        [TestMethod]
        public void CallsDisplayForStandard()
        {
            InitializeTwoViewsAndPresenter();
            TestChart testChart = new TestChart();
            presenter.DisplayData(DateTime.Now, DateTime.Now, true, 1, false, false, testChart);
            Assert.IsTrue(testChart.calledDisplayForStandard);
            presenter.CloseDB();

        }
        [TestMethod]
        public void CallsDisplayForCatAndMonth()
        {
            InitializeTwoViewsAndPresenter();
            TestChart testChart = new TestChart();
            presenter.DisplayData(DateTime.Now, DateTime.Now, true, 1, true, true, testChart);
            Assert.IsTrue(testChart.calledDisplayForMonthAndCategory);
            presenter.CloseDB();

        }
        // Helper methods
        public void InitializeTwoViewsAndPresenter()
        {
            testViewFile = new TestViewFile();
            presenter = new Presenter(testViewFile);
            testCatExpView = new TestView();
            presenter.InitializeCatExpView(testCatExpView);
        }
        public void InitializePresenterAndFileView()
        {
            testViewFile = new TestViewFile();
            presenter = new Presenter(testViewFile);
        }
        public int RandomInt()
        {
            Random rnd = new Random();
            int randomLengthOfString = rnd.Next(1, 35);
            return randomLengthOfString;
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
