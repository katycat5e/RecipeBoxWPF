using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3.SQLiteModel.Adapters
{
    class DetailRecipesAdapter : RecipesBaseAdapter<DetailRecipe>
    {
        public bool RetrieveImages { get; set; }

        protected override string TableName => "Recipes LEFT JOIN Categories ON R_Category=C_ID";
        
        protected override IEnumerable<string> DataColumns =>
            base.DataColumns.Union(new string[] { "C_Name" });

        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);
        }
        
        /// <inheritdoc/>
        protected override DetailRecipe GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                var nextRow = new DetailRecipe()
                {
                    R_ID = reader.GetInt32(0),
                    R_Name = reader.GetString(1),
                    R_Description = reader.GetString(2),
                    R_Modified = reader.GetValue(3) as long?,
                    R_PrepTime = reader.GetInt32(4),
                    R_CookTime = reader.GetInt32(5),
                    R_Steps = reader.GetString(6),
                    R_Category = reader.GetInt32(7),
                    C_Name = reader.GetString(8),
                    IMG_Data = null,
                    Status = RowStatus.Unchanged,
                };

                return nextRow;
            }
            catch (InvalidCastException e)
            {
                App.LogException(e);
                return null;
            }
        }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(DetailRecipe row)
        {
            base.SetDataParametersFromRow(row);
        }
    }
}
