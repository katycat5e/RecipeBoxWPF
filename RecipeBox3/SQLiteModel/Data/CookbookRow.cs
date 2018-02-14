using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>
    /// Enumeration for possible row states
    /// </summary>
    public enum RowStatus
    {
        Unchanged,
        New,
        Modified,
        Deleted
    }

    public abstract class CookbookRow : DependencyObject
    {
        public abstract int ID { get; set; }

        public RowStatus Status = RowStatus.Unchanged;

        public virtual bool IsUserEditable { get => true; set { } }
        
        protected static void OnRowChanged()
        {
            OnRowChanged(null, new DependencyPropertyChangedEventArgs());
        }

        protected static void OnRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CookbookRow row)
            {
                if (row.Status == RowStatus.Unchanged) row.Status = RowStatus.Modified;
            }
        }
    }
}
