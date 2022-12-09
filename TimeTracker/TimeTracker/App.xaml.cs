using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TimeTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainWindow window = new MainWindow();
            MainWindowViewModel windowViewModel = new MainWindowViewModel();
            window.DataContext = windowViewModel;
            window.Show();
        }
    }
}
