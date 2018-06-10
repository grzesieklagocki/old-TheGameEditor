using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TheGameEditor.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public partial class ItemsViewModelBase<TItem> : ViewModelBase
    {
        public ObservableCollection<TItem> Items { get; protected set; } = new ObservableCollection<TItem>();
        public TItem SelectedItem { get; set; }
        public ICollection<TItem> SelectedItems { get; set; }

        public static Dictionary<string, UserProperty> UserProperties { get; private set; } = new Dictionary<string, UserProperty>();

        public RelayCommand AddItemCommand => addItemCommand ?? (addItemCommand = new RelayCommand(AddItemAction));
        public RelayCommand RemoveItemCommand => removeItemCommand ?? (removeItemCommand = new RelayCommand(RemoveItemAction));


        private RelayCommand addItemCommand;
        private RelayCommand removeItemCommand;


        private void RemoveItemAction()
        {
            foreach (var item in SelectedItems)
            {
                Items.Remove(item);
            }
        }

        private void AddItemAction()
        {
            Items.Add(default(TItem));
        }
    }

}
