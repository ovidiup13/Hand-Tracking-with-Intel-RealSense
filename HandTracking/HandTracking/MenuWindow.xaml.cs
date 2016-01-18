using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace HandTracking
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private Window _markerWindow;

        public MenuWindow()
        {
            InitializeComponent();
            _markerWindow = new MarkerTrackingWindow(this);
        }

        private void StartExperimentClick(object sender, RoutedEventArgs e)
        {
            _markerWindow.Show();
            Hide();
        }

        /// <summary>
        /// Method called when the user clicks on the X button on the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApplication(object sender, CancelEventArgs e)
        {
            //ask for confirmation
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit?", "Exit Application",
                MessageBoxButton.YesNo);

            //close or cancel according to result
            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
            else
            {
                //cancel closing
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Method called when user clicks on Quit button. References the CloseApplication method. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit_button_OnClick(object sender, RoutedEventArgs e)
        {      
            Application.Current.Shutdown();         
        }
    }
}
