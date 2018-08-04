using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using TheGameEditor.Model;
using TheGameEditor.UndoRedo;

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
            private set
            {
                items = value;
            }
        }
        public TItem SelectedItem { get; set; }

        public static Dictionary<string, UserProperty> UserProperties { get; private set; } = new Dictionary<string, UserProperty>();

        public string Filter { get; set; }

        #region Commands


        public RelayCommand UndoCommand => undoCommand ?? (undoCommand = new RelayCommand(undoManager.Undo, () => undoManager.CanUndo));
        public RelayCommand RedoCommand => redoCommand ?? (redoCommand = new RelayCommand(undoManager.Redo, () => undoManager.CanRedo));
        public RelayCommand AddItemCommand => addItemCommand ?? (addItemCommand = new RelayCommand(AddItemReported));
        public RelayCommand RemoveItemsCommand => removeItemsCommand ?? (removeItemsCommand = new RelayCommand(RemoveItemsReported, () => selectedItems?.Count > 0));
        public RelayCommand CopyItemsCommand => copyItemsCommand ?? (copyItemsCommand = new RelayCommand(CopyItems, () => selectedItems?.Count > 0));
        public RelayCommand CutItemsCommand => cutItemsCommand ?? (cutItemsCommand = new RelayCommand(CutItems, () => selectedItems?.Count > 0));
        public RelayCommand PasteItemsCommand => pasteItemsCommand ?? (pasteItemsCommand = new RelayCommand(PasteItems));
        public RelayCommand DuplicateItemsCommand => duplicateItemsCommand ?? (duplicateItemsCommand = new RelayCommand(DuplicateItems, () => selectedItems?.Count > 0));
        public RelayCommand<ICollection> SelectionChangedCommand => selectionChangedCommand ?? (selectionChangedCommand = new RelayCommand<ICollection>(i => SelectionChangedAction(i)));
        
        private RelayCommand undoCommand;
        private RelayCommand redoCommand;
        private RelayCommand addItemCommand;
        private RelayCommand removeItemsCommand;
        private RelayCommand copyItemsCommand;
        private RelayCommand cutItemsCommand;
        private RelayCommand pasteItemsCommand;
        private RelayCommand duplicateItemsCommand;
        private RelayCommand<ICollection> selectionChangedCommand;

        #endregion

        private TItem defaultItem;
        private ObservableCollection<TItem> items;
        private List<TItem> selectedItems;

        private UndoManager undoManager = new UndoManager(50);


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="defaultItem"></param>
        public ItemsViewModelBase(TItem defaultItem)
        {
            this.defaultItem = defaultItem ?? throw new ArgumentNullException(nameof(defaultItem));
            items = new ObservableCollection<TItem>();
        }


        public void SetItems(ICollection<TItem> items)
        {
            Items = new ObservableCollection<TItem>(items);
        }

        #region Command Actions

        #region Add

        private void AddItem(TItem item)
        {
            items.Add(item);
        }

        private void AddItemReported()
        {
            var item = GetCopy(defaultItem);
            AddItem(item);
            undoManager.ReportCommand(() => AddItem(item), () => RemoveItem(item));
        }

        private void AddItems(List<TItem> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        private void AddItemsReported(List<TItem> items)
        {
            AddItems(items);

            undoManager.ReportCommand(() => AddItems(items), () => RemoveItems(items));
        }

        #endregion

        #region Remove

        private void RemoveItem(TItem item)
        {
            items.Remove(item);
        }

        private void RemoveItems(List<TItem> items)
        {
            foreach (var item in items)
            {
                RemoveItem(item);
            }
        }

        private void RemoveItemsReported()
        {
            var items = selectedItems;
            RemoveItems(items);

            undoManager.ReportCommand(() => RemoveItems(items), () => AddItems(items));
        }

        #endregion

        #region Copy / Cut / Paste / Duplicate


        private void CopyItems()
        {
            var data = new DataObject();

            data.SetData($"{typeof(TItem).Name}List", SerializeToStringXml(selectedItems));
            data.SetData(DataFormats.UnicodeText, SerializeToStringExcel(selectedItems));

            Clipboard.SetDataObject(data, true);
        }

        private void CutItems()
        {
            CopyItems();
            RemoveItemsReported();
        }

        private void PasteItems()
        {
            string xml = Clipboard.GetData($"{typeof(TItem).Name}List")?.ToString() ?? string.Empty;

            if (xml != string.Empty)
            {
                AddItemsReported(DeserializeFromStringXml<List<TItem>>(xml));
            }
        }

        private void DuplicateItems()
        {
            AddItemsReported(GetCopy(selectedItems));
        }

        #endregion

        #region Selection Changed
        
        private void SelectionChangedAction(ICollection items)
        {
            selectedItems = items.Cast<TItem>().ToList();
        }

        #endregion

        #endregion
        
        #region Serialization
        
        private string SerializeToStringXml<T>(T obj)
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

        private T DeserializeFromStringXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            T obj;

            using (var stream = new StringReader(xml))
            {
                obj = (T)serializer.Deserialize(stream);
            }

            return (T)obj;
        }

        private string SerializeToStringExcel<T>(ICollection<T> collection)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            string text = string.Empty;

            foreach (var item in collection)
            {
                foreach (PropertyDescriptor property in properties)
                {
                    if (property.PropertyType.IsPublic)
                    {
                        text += (property.GetValue(item).ToString() ?? string.Empty) + '\t';
                    }
                }

                text += Environment.NewLine;
            }

            return text;
        }

        #endregion

        
        private T GetCopy<T>(T obj)
        {
            return DeserializeFromStringXml<T>(SerializeToStringXml<T>(obj));
        }
    }
}