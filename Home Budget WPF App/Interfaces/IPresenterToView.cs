using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home_Budget_WPF_App.Interfaces
{
    public interface IPresenterToView
    {
        void ShowSuccessMessage(string mssg, string caption);
        void ShowErrorMessage(string mssg, string errors, string caption);
        void ResetFields();
        void SetDefaultViewValues();

    }
}
