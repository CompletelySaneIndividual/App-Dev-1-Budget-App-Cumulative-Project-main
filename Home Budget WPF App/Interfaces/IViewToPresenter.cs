using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;


namespace Home_Budget_WPF_App.Interfaces
{
    public interface IViewToPresenter
    {
        void AskForFile(string filter, string title, string initialDir);

        //MessageBoxButton
        //MessageBoxImage
        void ShowMessagebox(string messageBoxFix, string caption, object btn, object img );

        void UpdateGrid();

        void UpdateCatCmbBox();

        bool askForDefault(string messageBoxFix, string caption, object btn);

        bool AskForReset(string messageBoxFix, string caption, object btn);
    }
}
