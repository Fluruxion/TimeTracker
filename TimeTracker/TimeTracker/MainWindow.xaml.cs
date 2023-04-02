using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Window.GetWindow(this).KeyDown += Grid_KeyDown;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                var context = (MainWindowViewModel)Window.GetWindow(this).DataContext;

                switch (e.Key)
                {
                    case Key.Enter:
                        txtComments.Focus();
                        if (btnStart.IsEnabled && txtName.IsEnabled)
                            context.CommandStart.Execute(null);
                        break;

                    case Key.S:
                        btnEnd.Focus();
                        if (btnEnd.IsEnabled)
                            context.CommandFinish.Execute(null);
                        break;

                    case Key.N:
                        txtName.Focus();
                        break;

                    case Key.P:
                        context.CommandPause.Execute(null);
                        break;

                    case Key.O:
                        context.CommandImportCSV.Execute(null);
                        break;

                    case Key.C:
                        MessageBoxResult result = MessageBox.Show(
                            "This will delete all logged tasks, are you sure?",
                            "Clear Tasks?",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning,
                            MessageBoxResult.No
                            );

                        if (result == MessageBoxResult.Yes)
                        {
                            context.loggedTasks = new List<TimeItem>();
                        }

                        break;
                }
            }
            else if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                var context = (MainWindowViewModel)Window.GetWindow(this).DataContext;

                switch (e.Key)
                {
                    case Key.S:
                        if (context.currentTask == null)
                            context.CommandSave.Execute(null);
                        break;
                }
            }
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                txtName.Focus();
            }
        }
    }
}
