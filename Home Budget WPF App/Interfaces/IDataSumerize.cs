using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace Home_Budget_WPF_App.Interfaces
{
    public interface IDataSumerize
    {
        void SetPresenter(Presenter presenter);
        void DisplayForMonthAndCategory(List<Dictionary<string, object>> data);
        void DisplayForCategory(List<BudgetItemsByCategory> data);
        void DisplayForMonth(List<BudgetItemsByMonth> data);
        void DisplayForStandard(List<BudgetItem> data);
    }
}
