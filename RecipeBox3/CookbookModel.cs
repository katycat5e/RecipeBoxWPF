using RecipeBox3.CookbookDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3
{
    public class CookbookModel
    {
        public string ConnectionString;

        public RecipesTableAdapter RecipesTableAdapter
        {
            get
            {
                var adapter = new RecipesTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public ImagesTableAdapter ImagesTableAdapter
        {
            get
            {
                var adapter = new ImagesTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public IngredientsTableAdapter IngredientsTableAdapter
        {
            get
            {
                var adapter = new IngredientsTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public UnitsTableAdapter UnitsTableAdapter
        {
            get
            {
                var adapter = new UnitsTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public CategoriesTableAdapter CategoriesTableAdapter
        {
            get
            {
                var adapter = new CategoriesTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public SimpleRecipeViewTableAdapter SimpleRecipeViewTableAdapter
        {
            get
            {
                var adapter = new SimpleRecipeViewTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public SimpleIngredientsViewTableAdapter SimpleIngredientsViewTableAdapter
        {
            get
            {
                var adapter = new SimpleIngredientsViewTableAdapter();
                adapter.Connection.ConnectionString = ConnectionString;
                return adapter;
            }
        }

        public QueriesTableAdapter QueriesTableAdapter
        {
            get
            {
                var adapter = new QueriesTableAdapter();
                adapter.SetConnectionString(ConnectionString);
                return adapter;
            }
        }

        public CookbookModel(string connectionString)
        {
            ConnectionString = connectionString;
        }


        public delegate void AssignRecipeDelegate(CookbookDataSet.RecipesRow row, string category);

        public delegate void AssignBitmapDelegate(Bitmap image);

        public void GetDetailedRecipeData(object args)
        {
            // int RecipeID, AssignRecipeDelegate assignRecipeDelegate, AssignImageDelegate assignImageDelegate = null
            if (!(args is object[] argsArray) || argsArray.Length < 2) return;
            if (!((argsArray[0] is int recipeID) && (argsArray[1] is AssignRecipeDelegate assignRecipeDelegate))) return;

            var results = RecipesTableAdapter.GetDataByID(recipeID);
            if (results.Count < 1) return;

            var row = results[0];

            // pull category name from db
            string category = "";
            if (!row.IsR_CategoryNull())
            {
                try
                {
                    var categories = CategoriesTableAdapter.GetDataByID(row.R_Category);
                    if (categories.Count > 0)
                    {
                        category = categories[0].C_Name;
                    }
                }
                catch (Exception) { }
            }
            
            Application.Current.Dispatcher.BeginInvoke(assignRecipeDelegate, row, category);

            if ((argsArray.Length < 3) || !(argsArray[2] is AssignBitmapDelegate assignImageDelegate)) return;

            // pull image preview from db
            Image image = null;
            try
            {
                var images = ImagesTableAdapter.GetDataByRecipe(row.R_ID);
                if (images.Count > 0)
                {
                    var imageRow = images[0];
                    if (!imageRow.IsIMG_DataNull())
                    {
                        var data = imageRow.IMG_Data;
                        using (var imgStream = new System.IO.MemoryStream(data))
                        {
                            image = new Bitmap(Image.FromStream(imgStream));
                        }
                    }
                }
            }
            catch { }

            if (image != null)
            {
                Application.Current.Dispatcher.BeginInvoke(assignImageDelegate, image);
            }
        }
    }
}
