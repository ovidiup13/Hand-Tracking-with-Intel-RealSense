using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AudioModule.Implementation.AudioController;
using AudioModule.Implementation.AudioDesigns.Constant;
using AudioModule.Interfaces;
using CoreModule.Implementation;
using CoreModule.Interfaces;
using Condition = CoreModule.Implementation.Condition;

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
        }       
    }
}