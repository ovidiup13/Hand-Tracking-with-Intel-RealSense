using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using CameraModule.Annotations;
using CoreModule.Interfaces;

namespace CoreModule.Implementation
{
    public class Condition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Condition()
        {
            Conditions = new ObservableCollection<ConditionDesign>();
        }

        /// <summary>
        ///     Method that adds a new ConditionDesign at the specified position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="conditionDesign"></param>
        public void AddConditionAtIndex(int index, ConditionDesign conditionDesign)
        {
            if (index < 0 || index > Conditions.Count)
            {
                throw new ExperimentException("Remove ConditionDesign index out of range.");
            }

            if (conditionDesign == null)
            {
                throw new ExperimentException("ConditionDesign cannot be null.");
            }

            Conditions.Insert(index, conditionDesign);
        }

        /// <summary>
        /// Method which removes the specified ConditionDesign from the group.
        /// </summary>
        /// <param name="cond">ConditionDesign object</param>
        public void RemoveCondition(ConditionDesign cond)
        {
            if (cond == null)
            {
                throw new ExperimentException("Remove ConditionDesign index out of range.");
            }

            Conditions.Remove(cond);
        }

        /// <summary>
        ///     Method that removes a ConditionDesign from the ConditionDesign group.
        /// </summary>
        /// <param name="index">Index at which the ConditionDesign will be removed</param>
        public void RemoveConditionAt(int index)
        {
            if (index < 0 || index > Conditions.Count)
            {
                throw new ExperimentException("Remove ConditionDesign index out of range.");
            }

            Conditions.RemoveAt(index);
        }

        /// <summary>
        ///     Method that inserts a new ConditionDesign at the end of the ConditionDesign group.
        /// </summary>
        /// <param name="conditionDesign"></param>
        public void AddNewCondition(ConditionDesign conditionDesign)
        {
            if (conditionDesign == null)
            {
                throw new ExperimentException("ConditionDesign cannot be null.");
            }

            Conditions.Add(conditionDesign);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region vars

        private string _description = "Condition";
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        ///     List of conditions.
        /// </summary>
        private ObservableCollection<ConditionDesign> _conditions;
        public ObservableCollection<ConditionDesign> Conditions
        {
            get { return _conditions; }
            set
            {
                if (value == null) return;
                _conditions = value;
                ConditionsCollectionView = CollectionViewSource.GetDefaultView(_conditions);
                OnPropertyChanged(nameof(Conditions));
            }
        }

        public ICollectionView ConditionsCollectionView { get; private set; }


        #endregion
    }
}