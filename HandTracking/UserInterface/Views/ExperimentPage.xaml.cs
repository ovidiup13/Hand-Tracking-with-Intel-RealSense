using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AudioModule.Implementation.AudioController;
using AudioModule.Implementation.AudioDesigns.Constant;
using AudioModule.Interfaces;
using CoreModule.Implementation;
using CoreModule.Interfaces;

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

        /// <summary>
        ///     Method that initializes a new Experiment.
        /// </summary>
        private void InitializeExperiment(Dictionary<int, PXCMPoint3DF32> markerLocation)
        {
            //TODO: conditions and audio designs must be initialized in another page
            AudioDesign audioDesign = new ControlDesign();
            var conditions = new List<ConditionImpl>();
            for (var i = 0; i < 2; i++)
            {
                var condition = new ConditionImpl(_numberOfTrials) {AudioDesign = audioDesign};
                conditions.Add(condition);
            }

            //pass these to main experiment
            //TODO: check marker data for null
//            _mainExperiment = new MainExperiment(conditions.ToArray(), new SpeakerControllerImpl(markerLocation),
//                new Participant(1));
        }

        /// <summary>
        ///     Method that starts the experiment.
        /// </summary>
        private void StartExperiment()
        {
            //start experiment
            if (_mainExperiment != null && !_mainExperiment.IsStarted())
            {
                _mainExperiment.StartExperiment();
            }

            else
            {
                MessageBox.Show(
                    "Experiment is already started or has not been initialized.",
                    "Experiment started", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        ///     Method that is called when a key is pressed on the keyboard during the experiment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            //if we pressed space, move to next trial
            if (e.Key == Key.Space && _mainExperiment.IsStarted())
                _mainExperiment.NextTrial();
        }

        #region experiment vars

        private IExperiment _mainExperiment;
        private static readonly int _numberOfTrials = 1;

        #endregion
    }
}