using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.DeptBudgets.Interfaces
{
    interface IViewToPresenter
    {
        void AskForFile(string filter, string title, string initialDir);

        //MessageBoxButton
        //MessageBoxImage
        void ShowMessagebox(string messageBoxFix, string caption, object btn, object img);

        

        void UpdateGrid();

        void UpdateCatCmbBox();

        bool askForDefault(string messageBoxFix, string caption, object btn);

        bool AskForReset(string messageBoxFix, string caption, object btn);
    }
}
