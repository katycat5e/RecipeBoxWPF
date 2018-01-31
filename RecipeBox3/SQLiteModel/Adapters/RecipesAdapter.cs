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
    class RecipesAdapter : SQLiteAdapter<Recipe>
    {
        private SQLiteParameter idParameter;
        private SQLiteParameter nameParameter;
        private SQLiteParameter descParameter;
        private SQLiteParameter modParameter;
        private SQLiteParameter prepParameter;
        private SQLiteParameter cookParameter;
        private SQLiteParameter stepsParameter;
        private SQLiteParameter categoryParameter;

        public RecipesAdapter() : base() { }

        public RecipesAdapter(string connectionString) : base(connectionString) { }

        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            idParameter = new SQLiteParameter("@id", DbType.Int32);
            nameParameter = new SQLiteParameter("@name", DbType.String, 50);
            descParameter = new SQLiteParameter("@desc", DbType.String, 254);
            modParameter = new SQLiteParameter("@modified", DbType.Int64);
            prepParameter = new SQLiteParameter("@prep", DbType.Int32);
            cookParameter = new SQLiteParameter("@cook", DbType.Int32);
            stepsParameter = new SQLiteParameter("@steps", DbType.String);
            categoryParameter = new SQLiteParameter("@category", DbType.Int32);

            var allFields = new SQLiteParameter[]
            {
                nameParameter, descParameter, modParameter,
                prepParameter, cookParameter, stepsParameter, categoryParameter
            };

            SelectCommand.CommandText = "SELECT R_ID, R_Name, R_Description, R_Modified, " +
                "R_PrepTime, R_CookTime, R_Steps, R_Category FROM Recipes WHERE (@id IS NULL) OR (R_ID=@id)";
            SelectCommand.Parameters.Add(idParameter);

            UpdateCommand.CommandText = "UPDATE Recipes SET R_Name=@name, R_Description=@desc, R_Modified=@modified " +
                "R_PrepTime=@prep, R_CookTime=@cook, R_Steps=@steps, R_Category=@category WHERE R_ID=@id";
            UpdateCommand.Parameters.AddRange(allFields);
            UpdateCommand.Parameters.Add(idParameter);

            InsertCommand.CommandText = "INSERT INTO Recipes (R_Name, R_Description, R_Modified, " +
                "R_PrepTime, R_CookTime, R_Steps, R_Category) " +
                "VALUES (@name, @desc, @modified, @prep, @cook, @steps, @category)";
            InsertCommand.Parameters.AddRange(allFields);

            DeleteCommand.CommandText = "DELETE FROM Recipes WHERE R_ID=@id";
            DeleteCommand.Parameters.Add(idParameter);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(Recipe row)
        {
            throw new NotImplementedException();
        }

        public override bool Insert(Recipe row)
        {
            throw new NotImplementedException();
        }

        public override bool Modify(Recipe row)
        {
            throw new NotImplementedException();
        }

        public override Recipe Select(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Recipe> SelectAll()
        {
            throw new NotImplementedException();
        }
    }
}
