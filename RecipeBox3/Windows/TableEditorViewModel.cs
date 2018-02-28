using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using RecipeBox3.SQLiteModel.Data;
using RecipeBox3.SQLiteModel.Adapters;
using System.Collections.Specialized;

namespace RecipeBox3.Windows
{
    /// <summary>Generic view model for direct table editing</summary>
    /// <typeparam name="T">Type of rows in the table</typeparam>
    /// <typeparam name="U">Type of adapter that manages rows for the table</typeparam>
    public class TableEditorViewModel<T, U> : DependencyObject
        where T : CookbookRow
        where U : SQLiteAdapter<T>
    {
        /// <summary>Table adapter for this view model</summary>
        protected U Adapter;

        /// <summary>Collection of rows acquired from the database</summary>
        public ObservableCollection<T> Items
        {
            get { return (ObservableCollection<T>)GetValue(ItemsProperty); }
            set
            {
                SetValue(ItemsProperty, value);
                Items.CollectionChanged += Items_CollectionChanged;
            }
        }

        /// <summary>Collection of rows acquired from the database</summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<T>), typeof(TableEditorViewModel<T, U>), new PropertyMetadata(null));

        /// <summary>
        /// The currently selected item from the table as this table's row type,
        /// or null if no row is selected or the selected item does not match this table's row type
        /// </summary>
        protected T SelectedItem
        {
            get => SelectedItemObject as T;
            set => SelectedItemObject = value;
        }

        /// <summary>The currently selected object from the table</summary>
        public object SelectedItemObject
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>The currently selected object from the table</summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TableEditorViewModel<T, U>), new PropertyMetadata(null));


        /// <summary>Collection of items marked for deletion from the database</summary>
        public List<T> DeletedItems = new List<T>();


        /// <summary>Create a new instance of the view model and fetch the table contents from the database</summary>
        public TableEditorViewModel()
        {
            Adapter = Activator.CreateInstance(typeof(U)) as U;
            Items = new ObservableCollection<T>(Adapter?.SelectAll());
        }

        /// <summary>Add a new row to the table</summary>
        public void AddItem()
        {
            if (Activator.CreateInstance(typeof(T)) is T newObject) Items.Add(newObject);
        }

        /// <summary>Mark a row for deletion from the table</summary>
        public void DeleteSelectedItem()
        {
            if (SelectedItem?.IsUserEditable == true) Items.Remove(SelectedItem);
        }

        /// <summary>Commit all staged changes to the database</summary>
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
