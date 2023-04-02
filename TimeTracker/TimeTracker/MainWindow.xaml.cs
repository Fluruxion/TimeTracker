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
            Window.GetWindow(this).KeyDown += HotKeyDown;
        }

        private void HotKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                var context = (MainWindowViewModel)Window.GetWindow(this).DataContext;
                switch (e.Key)
                {
                    case Key.N:
                        txtName.Focus();
                        break;
                    case Key.Enter:
                        if (txtComments.IsEnabled)
                            txtComments.Focus();
                        break;
                    case Key.S:
                        btnEnd.Focus();
                        if (btnEnd.IsEnabled)
                            context.CommandFinish.Execute(null);
                        break;
                }
            }
        }
    }
}
