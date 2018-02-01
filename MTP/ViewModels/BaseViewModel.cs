using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTP
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected static IDataStore<Item> DataStore =>
            ServiceLocator.Instance.Get<IDataStore<Item>>() ?? new MockDataStore();

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            protected set => SetProperty(ref _isBusy, value);
        }

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            protected set => SetProperty(ref _title, value);
        }

        private void SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}