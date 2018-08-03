using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace TheGameEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Attribute> Items { get; }

        public UserProperty Property { get; set; }
        public string Value
        {
            get
            {
                return Property.SourceCode;
            }
        }
        public ObservableCollection<float> Dictionary
        {
            get
            {
                return DictionaryStatic;
            }
            private set
            {
                DictionaryStatic = value;
            }
        }

        DataTable dataTable = new DataTable();

        private RelayCommand changeCommand;
        public RelayCommand ChangeCommand
        {
            get
            {
                return changeCommand ?? (changeCommand = new RelayCommand(ChangeAction));
            }
        }

        [AlsoNotifyFor(nameof(Dictionary))]
        public ObservableCollection<float> DictionaryStatic { get; set; } = new ObservableCollection<float>() { 5 };

        public MainViewModel()
        {
            //if (IsInDesignMode)
            //{
            //    // Code runs in Blend --> create design time data.
            //}
            //else
            //{
            //    // Code runs "for real"
            //}

            // testy
            Property = new UserProperty();
        }

        private void ChangeAction()
        {
            DictionaryStatic = new ObservableCollection<float>() { 10 };
        }
    }
}