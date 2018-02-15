using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>
    /// Enumeration of possible row states
    /// </summary>
    public enum RowStatus
    {
        /// <summary>Row is unmodified from the database</summary>
        Unchanged,

        /// <summary>Row is newly created</summary>
        New,

        /// <summary>Row has been modified from the database</summary>
        Modified,

        /// <summary>Row has been marked for deletion from the database</summary>
        Deleted
    }

    /// <summary>Abstract object representing a row from the database</summary>
    public abstract class CookbookRow : DependencyObject
    {
        /// <summary>Primary key ID of this row</summary>
        public abstract int ID { get; set; }

        /// <summary>Current status of the row</summary>
        public RowStatus Status = RowStatus.Unchanged;

        /// <summary>Whether this row's values can be edited by the user</summary>
        public virtual bool IsUserEditable { get => true; set { } }

        /// <summary>see <see cref="OnRowChanged(DependencyObject, DependencyPropertyChangedEventArgs)"/></summary>
        protected static void OnRowChanged()
        {
            OnRowChanged(null, new DependencyPropertyChangedEventArgs());
        }

        /// <summary>Callback for changes in this row's values</summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected static void OnRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CookbookRow row)
            {
                if (row.Status == RowStatus.Unchanged) row.Status = RowStatus.Modified;
            }
        }
    }
}
