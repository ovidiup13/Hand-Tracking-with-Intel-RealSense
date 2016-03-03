using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using CameraModule.Annotations;
using CoreModule.Interfaces;

namespace CoreModule.Implementation
{
    public class ConditionGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Method that adds a new condition at the specified position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="condition"></param>
        public void AddConditionAtIndex(int index, Condition condition)
        {
            if (index < 0 || index > Conditions.Count)
            {
                throw new ExperimentException("Remove condition index out of range.");
            }

            if (condition == null)
            {
                throw new ExperimentException("Condition cannot be null.");
            }

            Conditions.Insert(index, condition);
        }

        /// <summary>
        ///     Method that removes a condition from the condition group.
        /// </summary>
        /// <param name="index">Index at which the condition will be removed</param>
        public void RemoveCondition(int index)
        {
            if (index < 0 || index > Conditions.Count)
            {
                throw new ExperimentException("Remove condition index out of range.");
            }

            Conditions.RemoveAt(index);
        }

        /// <summary>
        ///     Method that inserts a new condition at the end of the condition group.
        /// </summary>
        /// <param name="condition"></param>
        public void AddNewCondition(Condition condition)
        {
            if (condition == null)
            {
                throw new ExperimentException("Condition cannot be null.");
            }

            Conditions.Add(condition);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region vars

        private string _description;

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
        private ObservableCollection<Condition> _conditions;
        public ObservableCollection<Condition> Conditions
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