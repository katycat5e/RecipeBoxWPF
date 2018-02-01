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

        private SQLiteCommand SelectWithImageCommand;

        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            SelectWithImageCommand = new SQLiteCommand(Connection)
            {
                CommandText = String.Format(
                    "SELECT {0}, {1}, IMG_Data FROM {2} WHERE (@id IS NULL) OR ({0}=@id)",
                    IDColumn,
                    DataColumns,
                    TableName)
            };
        }

        /// <inheritdoc/>
        public override DetailRecipe Select(int id)
        {
            if (RetrieveImages)
            {
                if (SelectWithImageCommand.Connection == null) return null;
                else
                {
                    IDParameter.Value = id;
                    DetailRecipe row = null;

                    using (var reader = ExecuteCommandReader(SelectWithImageCommand))
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            row = GetRowFromReaderWithImage(reader);
                        }
                    }

                    return row;
                }
            }
            else
            {
                return base.Select(id);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<DetailRecipe> SelectAll()
        {
            if (RetrieveImages)
            {
                if (SelectWithImageCommand.Connection == null) return null;
                else
                {
                    var results = new List<DetailRecipe>();
                    IDParameter.Value = null;

                    using (var reader = ExecuteCommandReader(SelectWithImageCommand))
                    {
                        if (reader.HasRows)
                        {
                            DetailRecipe nextRow;

                            while (reader.Read())
                            {
                                nextRow = GetRowFromReaderWithImage(reader);
                                if (nextRow != null) results.Add(nextRow);
                            }
                        }
                    }

                    return results;
                }
            }
            else
            {
                return base.SelectAll();
            }
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

        protected DetailRecipe GetRowFromReaderWithImage(SQLiteDataReader reader)
        {
            var nextRow = GetRowFromReader(reader);
            nextRow.IMG_Data = ImagesAdapter.GetIMG_DataFromReader(reader, 9);

            return nextRow;
        }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(DetailRecipe row)
        {
            base.SetDataParametersFromRow(row);
        }
    }
}
