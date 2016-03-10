using System.ComponentModel;
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

        //TODO: release all resources when closing the main window
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
        }


        private void CloseAll()
        {
            
        }
    }
}