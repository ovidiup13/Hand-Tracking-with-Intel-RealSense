using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserInterface.Views
{
    /// <summary>
    ///     Interaction logic for ExperimentPage.xaml
    /// </summary>
    public partial class ExperimentPage : UserControl
    {
        public ExperimentPage()
        {
            InitializeComponent();
            /*InputManager.Current.PreProcessInput += (sender, e) =>
            {
                Console.WriteLine("Window: " + IsKeyboardFocused);
                Console.WriteLine("Grid: " + Grid.IsKeyboardFocused);
                Console.WriteLine("ScrollViewer: " + ScrollViewer.IsKeyboardFocused);
                Console.WriteLine("Panel: " + ButtonPanel.IsKeyboardFocusWithin);
            };*/
        }

        private void Focus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(ScrollViewer);
        }
    }
}