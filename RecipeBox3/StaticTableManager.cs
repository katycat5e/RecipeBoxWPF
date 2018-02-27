using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>Class for caching table values</summary>
    /// <typeparam name="T">Adapter type</typeparam>
    /// <typeparam name="U">Row Type</typeparam>
    public abstract class StaticTableManager<T, U> : DependencyObject
        where T : SQLiteAdapter<U>
        where U : CookbookRow
    {
        /// <summary>Table adapter</summary>
        protected T Adapter { get; set; }

        /// <summary>Dictionary of rows in the table keyed by row ID</summary>
        public Dictionary<int, U> Items
        {
            get { return (Dictionary<int, U>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        /// <summary>Property store for <see cref='Items'/></summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(Dictionary<int, U>), typeof(StaticTableManager<T,U>), new PropertyMetadata(null));

        /// <summary>Create a new instance of the class</summary>
        public StaticTableManager()
        {
            try
            {
                Adapter = Activator.CreateInstance(typeof(T)) as T;
            }
            catch (Exception) { }
        }

        /// <summary>Pull the latest data from the database</summary>
        public virtual void UpdateTable()
        {
            Items = Adapter?.SelectAll()?
                .ToDictionary(
                    item => item.ID,
                    item => item);
        }
    }
}
