using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RecipeBox3
{
    class RecipeViewModel : INotifyPropertyChanged
    {
        private CookbookModel CookbookAdapter { get { return App.GlobalCookbookModel; } }
        private CookbookDataSet.RecipesRow RecipesRow;


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public void AssignRecipe(CookbookDataSet.RecipesRow row, string category)
        {
            RecipesRow = row;
            RecipeName = row.R_Name;

            RecipeCategory = category;
            
            RecipeModified = (row.IsR_ModifiedNull()) ? null : row.R_Modified.ToShortTimeString() + " " + row.R_Modified.ToShortDateString();

            PrepTime = (row.IsR_PrepTimeNull()) ? 0 : row.R_PrepTime;
            CookTime = (row.IsR_CookTimeNull()) ? 0 : row.R_CookTime;

            RecipeSteps = (row.IsR_StepsNull()) ? "" : row.R_Steps;
        }

        public void AssignPreview(Bitmap preview)
        {
            RecipePreview = Imaging.CreateBitmapSourceFromHBitmap(preview.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public int? RecipeID
        {
            get { return RecipesRow?.R_ID; }
            set
            {
                RecipesRow = null;
                RecipePreview = null;

                if (value == null) return;
                var getDataThread = new System.Threading.Thread(CookbookAdapter.GetDetailedRecipeData);

                var recipeDelegate = new CookbookModel.AssignRecipeDelegate(AssignRecipe);
                var imageDelegate = new CookbookModel.AssignBitmapDelegate(AssignPreview);
                object[] args = { value, recipeDelegate, imageDelegate };

                getDataThread.Start(args);
            }
        }

        private string _RecipeName = "Recipe Name";
        public string RecipeName
        {
            get { return _RecipeName; }
            set
            {
                if (value != _RecipeName)
                {
                    _RecipeName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _RecipeCategory = "Category";
        public string RecipeCategory
        {
            get { return _RecipeCategory; }
            set
            {
                if (value != _RecipeCategory)
                {
                    _RecipeCategory = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _RecipeModified = null;
        public string RecipeModified
        {
            get { return _RecipeModified; }
            set
            {
                if (value != _RecipeModified)
                {
                    _RecipeModified = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _PrepTime = 0;
        public string PrepTimeString
        {
            get { return String.Format("{0} min", _PrepTime); }
        }

        public int PrepTime
        {
            get { return _PrepTime; }
            set
            {
                if (value != _PrepTime)
                {
                    _PrepTime = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("PrepTimeString");
                }
            }
        }

        private int _CookTime = 0;
        public string CookTimeString
        {
            get { return String.Format("{0} min", _CookTime); }
        }

        public int CookTime
        {
            get { return _CookTime; }
            set
            {
                if (value != _CookTime)
                {
                    _CookTime = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("CookTimeString");
                }
            }
        }

        private string _RecipeSteps = null;
        public string RecipeSteps
        {
            get { return _RecipeSteps; }
            set
            {
                if (value != _RecipeSteps)
                {
                    _RecipeSteps = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ImageSource _RecipePreview = null;
        public ImageSource RecipePreview
        {
            get { return _RecipePreview; }
            set
            {
                if (value != _RecipePreview)
                {
                    _RecipePreview = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
