using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.DeptBudgets.Interfaces
{
    public interface IPresenterToView
    {
        void ShowSuccessMessage(string mssg, string caption);
        void ShowErrorMessage(string mssg, string errors, string caption);
        void ResetFields();
        void SetDefaultViewValues();

        void UpdateCatLimit();

        void UpdateCatTotal();
    }
}
