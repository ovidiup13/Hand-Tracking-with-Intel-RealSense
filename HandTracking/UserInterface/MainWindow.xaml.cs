using System.ComponentModel;
using System.Net.Mime;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;

namespace UserInterface
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Dispatcher.InvokeShutdown();
        }
    }
}