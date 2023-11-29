using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace EnterpriseBudget.DeptBudgets.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataSumerize
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="presenter"></param>
        void SetPresenter(Presenter presenter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void DisplayForMonthAndCategory(List<Dictionary<string, object>> data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void DisplayForCategory(List<BudgetItemsByCategory> data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void DisplayForMonth(List<BudgetItemsByMonth> data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void DisplayForStandard(List<BudgetItem> data);
    }
}
