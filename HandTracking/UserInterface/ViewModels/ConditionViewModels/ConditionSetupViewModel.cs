using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces.Designs.Types;
using CoreModule.Implementation;
using FirstFloor.ModernUI.Presentation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GongSolutions.Wpf.DragDrop;
using UserInterface.Commands;
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

            new PassThroughConverter();

            //initialize commands
            AddNewConditionCommand = new RelayCommand(o => AddNewCondition(o));
            AddNewConditionGroupCommand = new RelayCommand(AddNewConditionGroup);
            RemoveConditionCommand = new RemoveConditionCommand(o => RemoveCondition(o), o => CanRemoveCondition(o));
            RemoveConditionGroupCommand = new RelayCommand(RemoveConditionGroup, CanRemoveConditionGroup);
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

        private void RemoveCondition(object o)
        {
            var obj = (object[]) o;
            var group = obj[0] as ConditionGroup;

            group?.RemoveCondition((Condition) obj[1]);
        }

        private bool CanRemoveCondition(object o)
        {
            if (o == null) return true;
            var obj = (object[]) o;
            var group = obj[0] as ConditionGroup;
            return group?.Conditions.Count > 0;
        }

        /// <summary>
        ///     Method that returns a boolean indicating whether a condition group can be removed.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanRemoveConditionGroup(object arg)
        {
            return ConditionsGroupCollectionView.CanRemove;
        }

        /// <summary>
        ///     Method that removesd a condition group from the Condition Collection View.
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveConditionGroup(object obj)
        {
            var group = obj as ConditionGroup;
            ConditionsGroupCollectionView.Remove(group);
        }

        /// <summary>
        ///     Method that is called by an UI command and adds a new condition group to the model.
        /// </summary>
        /// <param name="obj"></param>
        private void AddNewConditionGroup(object obj)
        {
            ConditionsGroupCollectionView.AddNew();
        }

        /// <summary>
        ///     /// Method that is called by an UI command and adds a new condition to the current item
        ///     selected in condition group collection.
        /// </summary>
        /// <param name="o"></param>
        private void AddNewCondition(object o)
        {
            //cast to condition group
            var group = o as ConditionGroup;
            if (o == null) return;

            group?.AddNewCondition(new Condition());
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
            /*//constant
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
                        FeedbackType = FeedbackType.Individual
                    }
                }
            });

            //Geiger
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
            });*/

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
            });

            ConditionsGroupCollectionView = new ListCollectionView(ConditionsGroups);
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
        public IEditableCollectionView ConditionsGroupCollectionView { get; set; }
        private SpeakerControllerImpl SpeakerController { get; }

        //design and feedback types
        public List<FeedbackType> FeedbackTypes { get; private set; }
        public List<DesignType> DesignTypes { get; private set; }

        #endregion

        #region commands

        public ICommand AddNewConditionGroupCommand { get; protected set; }
        public ICommand AddNewConditionCommand { get; protected set; }
        public ICommand RemoveConditionCommand { get; protected set; }
        public ICommand RemoveConditionGroupCommand { get; protected set; }

        #endregion
    }

    public class PassThroughConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}