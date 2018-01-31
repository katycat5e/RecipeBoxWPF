using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    public class ImagesAdapter : SQLiteAdapter<ImageRow>
    {
        private SQLiteParameter recipeParameter = new SQLiteParameter("@recipe", DbType.Int32);
        private SQLiteParameter dataParameter   = new SQLiteParameter("@data", DbType.Binary);

        private SQLiteCommand SelectByRecipeCommand;

        protected override SQLiteConnection Connection
        {
            get { return base.Connection; }
            set
            {
                if (value == _connection) return;
                else
                {
                    if (SelectByRecipeCommand != null) SelectByRecipeCommand.Connection = value;
                    base.Connection = value;
                }
            }
        }

        public ImagesAdapter() : base() { }

        public ImagesAdapter(string connectionString) : base(connectionString) { }

        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            SelectCommand.CommandText = "SELECT IMG_ID, IMG_RecipeID, IMG_Data FROM Images " +
                "WHERE (@id IS NULL) OR (IMG_ID = @id)";
            SelectCommand.Parameters.Add(idParameter);
            SelectCommand.Parameters.Add(dataParameter);

            SelectByRecipeCommand = new SQLiteCommand(
                "SELECT IMG_ID, IMG_RecipeID, IMG_Data FROM IMAGES WHERE IMG_RecipeID=@recipe",
                Connection);
            SelectByRecipeCommand.Parameters.Add(recipeParameter);

            UpdateCommand.CommandText = "UPDATE Images SET IMG_RecipeID=@recipe, IMG_Data=@data " +
                "WHERE IMG_ID=@id";
            UpdateCommand.Parameters.Add(idParameter);
            UpdateCommand.Parameters.Add(recipeParameter);
            UpdateCommand.Parameters.Add(dataParameter);

            InsertCommand.CommandText = "INSERT INTO Images (IMG_RecipeID, IMG_Data) VALUES (@recipe, @data)";
            InsertCommand.Parameters.Add(recipeParameter);
            InsertCommand.Parameters.Add(dataParameter);

            DeleteCommand.CommandText = "DELETE FROM Images WHERE IMG_ID=@id";
            DeleteCommand.Parameters.Add(idParameter);
        }

        public ImageRow SelectByRecipe(int recipeID)
        {
            if (SelectByRecipeCommand.Connection == null) return null;
            else
            {
                recipeParameter.Value = recipeID;
                ImageRow row = null;

                using (var reader = ExecuteCommandReader(SelectByRecipeCommand))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        row = GetRowFromReader(reader);
                    }
                }

                return row;
            }
        }

        protected override ImageRow GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                ImageRow row = new ImageRow()
                {
                    IMG_ID = reader.GetInt32(0),
                    IMG_RecipeID = reader.GetValue(1) as int?
                };

                //TODO: get image stream

                return row;
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine(e.Message + " at " + e.TargetSite);
                return null;
            }
        }

        protected override void SetDataParametersFromRow(ImageRow row)
        {
            recipeParameter.Value = row.IMG_RecipeID;
            dataParameter.Value = row.IMG_Data;
        }
    }
}
