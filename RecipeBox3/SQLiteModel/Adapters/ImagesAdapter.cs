﻿using System;
using System.Data;
using System.Data.SQLite;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Images table</summary>
    public class ImagesAdapter : SQLiteAdapter<ImageRow>
    {
        private SQLiteParameter recipeParameter = new SQLiteParameter("@recipe", DbType.Int32, "IMG_RecipeID");
        private SQLiteParameter dataParameter   = new SQLiteParameter("@data", DbType.Binary, "IMG_Data");
        
        /// <inheritdoc/>
        protected override SQLiteParameter[] DataParameters =>
            new SQLiteParameter[] { recipeParameter, dataParameter };

        /// <inheritdoc/>
        protected override string TableName => "Images";
        /// <inheritdoc/>
        protected override string IDColumn => "IMG_ID";

        private SQLiteCommand SelectByRecipeCommand;

        /// <inheritdoc/>
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

        /// <summary>Create a new instance of the class</summary>
        public ImagesAdapter() : base() { }

        /// <summary>Create a new instance of the class with the specified connection</summary>
        public ImagesAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);
            
            SelectByRecipeCommand = new SQLiteCommand(
                "SELECT IMG_ID, IMG_RecipeID, IMG_Data FROM IMAGES WHERE IMG_RecipeID=@recipe",
                Connection);

            SelectByRecipeCommand.Parameters.Add(recipeParameter);
        }

        /// <summary>Fetch the Image for the specified Recipe</summary>
        /// <param name="recipeID">ID of the Recipe to search for</param>
        /// <returns>The Image for the desired Recipe, or null if no image was found</returns>
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

        /// <inheritdoc/>
        protected override ImageRow GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                ImageRow row = new ImageRow()
                {
                    IMG_ID = reader.GetInt32(0),
                    IMG_RecipeID = reader.GetValue(1) as int?
                };

                row.IMG_Data = GetIMG_DataFromReader(reader, 2);

                return row;
            }
            catch (InvalidCastException e)
            {
                App.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Pull binary image data from the datareader
        /// </summary>
        /// <param name="reader">
        /// <see cref="SQLiteDataReader"/> to read data from
        /// </param>
        /// <param name="columnIndex">
        /// index of column in the <paramref name="reader"/> that holds the image data
        /// </param>
        /// <returns>Array of bytes containing the image data</returns>
        public static byte[] GetIMG_DataFromReader(SQLiteDataReader reader, int columnIndex)
        {
            const int BUFFER_SIZE = 2048;
            byte[] buffer = new byte[BUFFER_SIZE];
            long bytesRead;
            long currentOffset = 0;

            try
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    while ((bytesRead = reader.GetBytes(columnIndex, currentOffset, buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, (int)bytesRead);
                        currentOffset += bytesRead;
                    }

                    return stream.ToArray();
                }
            }
            catch (Exception e)
            {
                App.LogException(e);
                return null;
            }
        }
        
        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(ImageRow row)
        {
            recipeParameter.Value = row.IMG_RecipeID;
            dataParameter.Value = row.IMG_Data;
        }
    }
}
