using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RecipeBox3.SQLiteModel.Data;
using RecipeBox3.SQLiteModel.Adapters;
using System.Collections.Specialized;

namespace RecipeBox3
{
    public class TableEditorViewModel<T, U> : DependencyObject
        where T : CookbookRow<T>
        where U : SQLiteAdapter<T>
    {
        protected U Adapter;

        public ObservableCollection<T> Items
        {
            get { return (ObservableCollection<T>)GetValue(ItemsProperty); }
            set
            {
                SetValue(ItemsProperty, value);
                Items.CollectionChanged += Items_CollectionChanged;
            }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<T>), typeof(TableEditorViewModel<T, U>), new PropertyMetadata(null));

        protected T SelectedItem
        {
            get => SelectedItemObject as T;
            set => SelectedItemObject = value;
        }

        public object SelectedItemObject
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TableEditorViewModel<T, U>), new PropertyMetadata(null));


        public List<T> DeletedItems = new List<T>();


        public TableEditorViewModel()
        {
            Adapter = Activator.CreateInstance(typeof(U)) as U;
            Items = new ObservableCollection<T>(Adapter?.SelectAll());
        }

        public void AddItem()
        {
            if (Activator.CreateInstance(typeof(T)) is T newObject) Items.Add(newObject);
        }

        public void DeleteSelectedItem()
        {
            if (SelectedItem?.IsUserEditable == true) Items.Remove(SelectedItem);
        }

        public void SaveItems()
        {
            Adapter?.Update(Items);
            Adapter?.Update(DeletedItems);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // items removed
                foreach (T row in e.OldItems)
                {
                    if (!DeletedItems.Contains(row))
                    {
                        row.Status = RowStatus.Deleted;
                        DeletedItems.Add(row);
                    }
                }
            }
        }
    }
}
