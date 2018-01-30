using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3.SQLiteModel.Data
{
    public class Category : CookbookRow
    {
        private int _C_ID;
        public int C_ID
        {
            get { return _C_ID; }
            set
            {
                _C_ID = value;
                OnRowChanged();
            }
        }

        public string _C_Name;
        public string C_Name
        {
            get { return _C_Name; }
            set
            {
                _C_Name = value;
                OnRowChanged();
            }
        }

        public Category()
        {
            C_ID = -1;
            C_Name = null;
            Status = RowStatus.New;
        }

        public Category(int id, string name, RowStatus status = RowStatus.Unchanged)
        {
            C_ID = id;
            C_Name = name;
            Status = status;
        }

        public override bool Equals(CookbookRow row)
        {
            if (row is Category c)
            {
                return (c.C_ID == C_ID && c.C_Name == C_Name);
            }
            else return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CookbookRow);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + C_ID;
                hash = (hash * 7) + ((C_Name == null) ? 0 : C_Name.GetHashCode());
                return hash;
            }
        }
    }
}
