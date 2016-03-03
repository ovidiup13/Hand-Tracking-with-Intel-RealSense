using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserInterface.Views
{
    /// <summary>
    /// Interaction logic for ConditionPage.xaml
    /// </summary>
    public partial class ConditionPage : UserControl
    {
        public ConditionPage()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
