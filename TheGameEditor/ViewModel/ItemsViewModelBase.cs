using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;
using TheGameEditor.Model;

namespace TheGameEditor.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public partial class ItemsViewModelBase<TItem> : ViewModelBase where TItem : GameData
    {
        public ObservableCollection<TItem> Items
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Filter))
                {
                    return new ObservableCollection<TItem>(items.Where(i => i.Name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) >= 0));
                }
                else
                {
                    return items;
                }
            }
        }
        public TItem SelectedItem { get; set; }

        public static Dictionary<string, UserProperty> UserProperties { get; private set; } = new Dictionary<string, UserProperty>();

        public string Filter { get; set; }

        public RelayCommand AddItemCommand => addItemCommand ?? (addItemCommand = new RelayCommand(AddItem));
        public RelayCommand RemoveItemsCommand => removeItemsCommand ?? (removeItemsCommand = new RelayCommand(RemoveItems));
        public RelayCommand CopyItemsCommand => copyItemsCommand ?? (copyItemsCommand = new RelayCommand(CopyItems));
        public RelayCommand CutItemsCommand => cutItemsCommand ?? (cutItemsCommand = new RelayCommand(CutItems));
        public RelayCommand PasteItemsCommand => pasteItemsCommand ?? (pasteItemsCommand = new RelayCommand(PasteItems));
        public RelayCommand DuplicateItemsCommand => duplicateItemsCommand ?? (duplicateItemsCommand = new RelayCommand(DuplicateItems));
        public RelayCommand<ICollection> SelectionChangedCommand => selectionChangedCommand ?? (selectionChangedCommand = new RelayCommand<ICollection>(i => SelectionChangedAction(i)));


        private RelayCommand addItemCommand;
        private RelayCommand removeItemsCommand;
        private RelayCommand copyItemsCommand;
        private RelayCommand cutItemsCommand;
        private RelayCommand pasteItemsCommand;
        private RelayCommand duplicateItemsCommand;
        private RelayCommand<ICollection> selectionChangedCommand;

        private List<TItem> selectedItems;
        private TItem defaultItem;
        private ObservableCollection<TItem> items;


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="items"></param>
        /// <param name="defaultItem"></param>
        public ItemsViewModelBase(ICollection<TItem> items, TItem defaultItem)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.defaultItem = defaultItem ?? throw new ArgumentNullException(nameof(defaultItem));
            this.items = new ObservableCollection<TItem>(items);
        }


        public void SetItems(ICollection<TItem> items)
        {
            this.items = new ObservableCollection<TItem>(items);
        }


        private void AddItem()
        {
            AddItem((TItem)defaultItem.GetCopy());
        }

        private void AddItem(TItem item)
        {
            items.Add(item);
        }

        private void AddItems(List<TItem> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        private void RemoveItem(TItem item)
        {
            items.Remove(item);
        }

        private void RemoveItems()
        {
            foreach (var item in selectedItems)
            {
                RemoveItem(item);
            }
        }

        private void CopyItems()
        {
            Clipboard.SetData($"{typeof(TItem).Name}List", SerializeToString(selectedItems));
        }

        private void CutItems()
        {
            CopyItems();
            RemoveItems();
        }

        private void PasteItems()
        {
            string xml = Clipboard.GetData($"{typeof(TItem).Name}List").ToString();

            AddItems(DeserializeFromString<List<TItem>>(xml));
        }

        private void DuplicateItems()
        {
            CopyItems();
            PasteItems();
        }

        private void SelectionChangedAction(ICollection items)
        {
            selectedItems = items.Cast<TItem>().ToList();
        }
        
        #region Serialization
        
        private string SerializeToString<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            string xml;

            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream, obj);
                xml = stream.ToString();
            }

            return xml;
        }

        private T DeserializeFromString<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            T obj;

            using (var stream = new StringReader(xml))
            {
                obj = (T)serializer.Deserialize(stream);
            }

            return (T)obj;
        }

        #endregion
    }
}