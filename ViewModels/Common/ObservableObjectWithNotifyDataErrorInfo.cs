using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class ObservableObjectWithNotifyDataErrorInfo : ObservableObject, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsByPropertyName;
        protected readonly IList<string> TrackedPropertyNames;

        public bool HasErrors => _errorsByPropertyName.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected ObservableObjectWithNotifyDataErrorInfo()
        {
            _errorsByPropertyName = new Dictionary<string, List<string>>();
            TrackedPropertyNames = GetType().GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(TrackChangeAttribute))).Select(p => p.Name)
                .ToList();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (_errorsByPropertyName[propertyName].Contains(error)) return;
            _errorsByPropertyName[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }

        protected void ClearErrors(string propertyName)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName)) return;
            _errorsByPropertyName.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}