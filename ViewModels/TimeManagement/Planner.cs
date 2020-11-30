using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class Planner : ObservableObjectWithNotifyDataErrorInfo, IMessaging
    {
        private int _userId;
        internal int UserId 
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;

                foreach(var note in Notes)
                {
                    note.UserId = value;
                }
            }
        }

        private string _senderPath;
        public string SenderPath
        {
            get
            {
                return _senderPath;
            }
            internal set
            {
                _senderPath = value;

                foreach (var note in Notes)
                {
                    note.SenderPath = string.Format(MESSAGING_FORMAT, value, nameof(Lists.Note));
                }
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                Set(ref _startDate, value);
            }
        }

        private DateTime _finishDate;
        public DateTime FinishDate
        {
            get
            {
                return _finishDate;
            }
            set
            {
                Set(ref _finishDate, value);
            }
        }

        public ObservableCollection<Lists.Note> Notes { get; set; }

        public ICommand DeleteNoteCommand { get; set; }

        public Planner()
        {
            Notes = new ObservableCollection<Lists.Note>();
            Notes.CollectionChanged += Notes_CollectionChanged;

            DeleteNoteCommand = new RelayCommand<Lists.Note>(DeleteNote, true);
        }

        ~Planner()
        {
            Notes.CollectionChanged -= Notes_CollectionChanged;
        }

        private void Notes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var element = (Lists.Note)item;
                        element.SenderPath = string.Format(MESSAGING_FORMAT, SenderPath, nameof(Lists.Note));

                        if (element.ScheduleDate == DateTime.MinValue)
                        {
                            element.ScheduleDate = StartDate;
                        }

                        element.UserId = UserId;
                        element.SaveCommand = new RelayCommand(element.SaveAsync, element.CanSave);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    //RemoveCommand?.RaiseCanExecuteChanged();
                    break;
                default: throw new NotImplementedException();
            }
        }

        private async void DeleteNote(Lists.Note note)
        {
            if (note != null && await note.RemoveAsync())
            {
                Notes.Remove(note);
            }
        }
    }
}
