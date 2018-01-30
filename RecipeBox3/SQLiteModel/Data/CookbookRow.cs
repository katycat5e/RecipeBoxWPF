using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public abstract class CookbookRow : IEquatable<CookbookRow>
    {
        public RowStatus Status = RowStatus.Unchanged;

        public delegate void RowChangedHandler(string property);

        protected virtual void OnRowChanged()
        {
            if (Status == RowStatus.Unchanged) Status = RowStatus.Modified;
        }

        public abstract bool Equals(CookbookRow row);

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();
    }
}
