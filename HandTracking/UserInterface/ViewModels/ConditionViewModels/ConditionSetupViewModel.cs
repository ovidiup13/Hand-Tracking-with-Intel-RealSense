using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces;
using CoreModule.Implementation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GongSolutions.Wpf.DragDrop;
using Condition = CoreModule.Implementation.Condition;

namespace UserInterface.ViewModels.ConditionViewModels
{
    public class ConditionSetupViewModel : ViewModelBase, IDropTarget
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ConditionSetupViewModel()
        {
            ConditionsGroups = new ObservableCollection<ConditionGroup>();

            SpeakerController = SimpleIoc.Default.GetInstance<SpeakerControllerImpl>();

            //retrieve default audio designs
            InitializeDefaultDesigns();

            //retrieve default conditions
            InitializeDefaultGroups();
        }

        /// <summary>
        ///     Method called when an UIElement is dragged
        /// </summary>
        /// <param name="dropInfo"></param>
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is ConditionSetupViewModel && dropInfo.TargetItem is ConditionSetupViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }

            if (dropInfo.Data is ConditionGroup && dropInfo.TargetItem is ConditionGroup)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        /// <summary>
        ///     No idea what this does
        /// </summary>
        /// <param name="dropInfo"></param>
        public void Drop(IDropInfo dropInfo)
        {
        }

        /// <summary>
        ///     Initializes the default audio designs available
        /// </summary>
        private void InitializeDefaultDesigns()
        {
            DesignTypes = SpeakerController.AudioSettings.GetDesignTypes();
            FeedbackTypes = SpeakerController.AudioSettings.GetFeedbackTypes();

            //TODO: create class to hold the design and feedback type as properties
        }

        /// <summary>
        ///     Method that initializes the default condition groups.
        /// </summary>
        private void InitializeDefaultGroups()
        {
            //TODO: think about how to simplify this initialization
            //constant
            ConditionsGroups.Add(new ConditionGroup
            {
                Description = "Constant Group",
                Conditions = new ObservableCollection<Condition>
                {
                    new Condition
                    {
                        DesignType = DesignType.Constant,
                        FeedbackType = FeedbackType.Individual
                    },
                    new Condition
                    {
                        DesignType = DesignType.Constant,
                        FeedbackType = FeedbackType.Coalescent
                    },
                    new Condition
                    {
                        DesignType = DesignType.Constant,
                        FeedbackType = FeedbackType.Wrist
                    }
                }
            });

            /*//Geiger
            ConditionsGroups.Add(new ConditionGroup
            {
                Description = "Geiger Group",
                Conditions = new ObservableCollection<Condition>
                {
                    new Condition
                    {
                        DesignType = DesignType.Geiger,
                        FeedbackType = FeedbackType.Individual
                    },
                    new Condition
                    {
                        DesignType = DesignType.Geiger,
                        FeedbackType = FeedbackType.Coalescent
                    },
                    new Condition
                    {
                        DesignType = DesignType.Geiger,
                        FeedbackType = FeedbackType.Wrist
                    }
                }
            });

            //pitch
            ConditionsGroups.Add(new ConditionGroup
            {
                Description = "Pitch Group",
                Conditions = new ObservableCollection<Condition>
                {
                    new Condition
                    {
                        DesignType = DesignType.Pitch,
                        FeedbackType = FeedbackType.Individual
                    },
                    new Condition
                    {
                        DesignType = DesignType.Pitch,
                        FeedbackType = FeedbackType.Coalescent
                    },
                    new Condition
                    {
                        DesignType = DesignType.Pitch,
                        FeedbackType = FeedbackType.Wrist
                    }
                }
            });

            //control
            ConditionsGroups.Add(new ConditionGroup
            {
                Description = "Control Group",
                Conditions = new ObservableCollection<Condition>
                {
                    new Condition
                    {
                        DesignType = DesignType.Control,
                        FeedbackType = FeedbackType.Individual
                    }
                }
            });*/

            ConditionsGroupCollectionView = CollectionViewSource.GetDefaultView(ConditionsGroups);
        }

        #region vars

        /// <summary>
        ///     Holds condition groups
        /// </summary>
        private ObservableCollection<ConditionGroup> _conditionGroups;

        public ObservableCollection<ConditionGroup> ConditionsGroups
        {
            get { return _conditionGroups; }
            set
            {
                if (value == null) return;
                _conditionGroups = value;
                RaisePropertyChanged("ConditionsGroup");
            }
        }

        //holds collection for UI view
        public ICollectionView ConditionsGroupCollectionView { get; set; }
        private SpeakerControllerImpl SpeakerController { get; }

        //design and feedback types
        public List<FeedbackType> FeedbackTypes { get; private set; }
        public List<DesignType> DesignTypes { get; private set; }

        #endregion
    }
}