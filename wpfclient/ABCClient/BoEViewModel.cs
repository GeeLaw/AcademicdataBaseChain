using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ABCClient
{
    public class BoEViewModel : INotifyPropertyChanged
    {
        public BoEViewModel()
        {
            mark = new MarkAsSchoolCommand(this);
            unmark = new UnmarkAsSchoolCommand(this);
        }

        public IEnumerable<Entity> Entities
        {
            get
            {
                return Entity.Entities;
            }
        }

        string selectedEntity;
        public string SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (value != selectedEntity)
                {
                    selectedEntity = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectedEntity"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        bool isUIEnabled = true;
        public bool IsUIEnabled
        {
            get { return isUIEnabled; }
            private set
            {
                if (value != isUIEnabled)
                {
                    isUIEnabled = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsUIEnabled"));
                }
            }
        }

        class MarkAsSchoolCommand : ICommand
        {
            public MarkAsSchoolCommand(BoEViewModel vm)
            {
                owner = vm;
            }

            BoEViewModel owner;

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async void Execute(object parameter)
            {
                try
                {
                    owner.IsUIEnabled = false;
                    bool result = await Client.MarkAsSchoolAsync(owner.SelectedEntity, Client.BoE.AccountAddress);
                    MessageBox.Show(result ? "Marked this entity as a school." : "Could not mark this entity as a school.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "An exception occurred", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                finally
                {
                    owner.IsUIEnabled = true;
                }
            }
        }

        class UnmarkAsSchoolCommand : ICommand
        {
            public UnmarkAsSchoolCommand(BoEViewModel vm)
            {
                owner = vm;
            }

            BoEViewModel owner;

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async void Execute(object parameter)
            {
                try
                {
                    owner.IsUIEnabled = false;
                    bool result = await Client.UnmarkAsSchoolAsync(owner.SelectedEntity, Client.BoE.AccountAddress);
                    MessageBox.Show(result ? "Unmarked this entity as a school." : "Could not unmark this entity as a school.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "An exception occurred", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                finally
                {
                    owner.IsUIEnabled = true;
                }
            }
        }

        MarkAsSchoolCommand mark;
        public ICommand MarkAsSchool
        {
            get
            {
                return mark;
            }
        }

        UnmarkAsSchoolCommand unmark;
        public ICommand UnmarkAsSchool
        {
            get
            {
                return unmark;
            }
        }
    }
}
