using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public abstract class CookbookRow<T> : DependencyObject where T : CookbookRow<T>
    {
        public RowStatus Status = RowStatus.Unchanged;
        
        protected static void OnRowChanged()
        {
            OnRowChanged(null, new DependencyPropertyChangedEventArgs());
        }

        protected static void OnRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CookbookRow<T> row)
            {
                if (row.Status == RowStatus.Unchanged) row.Status = RowStatus.Modified;
            }
        }

        public static T Clone(T source)
        {
            var constructor = source.GetType().GetConstructor(new Type[] { source.GetType() });

            if (constructor != null)
            {
                try
                {
                    return constructor.Invoke(new object[] { source }) as T;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else return null;
        }
    }
}
