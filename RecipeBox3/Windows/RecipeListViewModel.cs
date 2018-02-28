using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace RecipeBox3.Windows
{
    /// <summary>View Model for the <see cref="RecipeListWindow"/></summary>
    public class RecipeListViewModel : DependencyObject
    {
        private DetailRecipesAdapter recipesAdapter;
        private ImagesAdapter imagesAdapter;

        private BackgroundWorker getImageWorker;

        /// <summary>Container for search parameters</summary>
        public SearchItems SearchItems { get; set; }

        /// <summary></summary>
        public ObservableCollection<DetailRecipe> RecipeCollection
        {
            get { return (ObservableCollection<DetailRecipe>)GetValue(RecipeCollectionProperty); }
            set { SetValue(RecipeCollectionProperty, value); }
        }

        /// <summary>Property store for <see cref='RecipeCollection'/></summary>
        public static readonly DependencyProperty RecipeCollectionProperty =
            DependencyProperty.Register("RecipeCollection", typeof(ObservableCollection<DetailRecipe>), typeof(RecipeListViewModel), new PropertyMetadata(null));



        /// <summary>List of recipes to show in the grid</summary>
        public List<DetailRecipe> Recipes
        {
            get { return (List<DetailRecipe>)GetValue(RecipesProperty); }
            set { SetValue(RecipesProperty, value); }
        }

        /// <summary>List of recipes to show in the grid</summary>
        public static readonly DependencyProperty RecipesProperty =
            DependencyProperty.Register("Recipes", typeof(List<DetailRecipe>), typeof(RecipeListViewModel), new PropertyMetadata(null));


        /// <summary>Currently selected item in the recipe list</summary>
        public object SelectedGridItem
        {
            get { return (object)GetValue(SelectedGridItemProperty); }
            set { SetValue(SelectedGridItemProperty, value); }
        }

        /// <summary>Currently selected item in the recipe list</summary>
        public static readonly DependencyProperty SelectedGridItemProperty =
            DependencyProperty.Register("SelectedGridItem", typeof(object), typeof(RecipeListViewModel), new PropertyMetadata(null));

        
        /// <summary>Whether preview images are shown in the recipe list</summary>
        public bool ShowImages
        {
            get { return (bool)GetValue(ShowImagesProperty); }
            set { SetValue(ShowImagesProperty, value); }
        }

        /// <summary>Whether preview images are shown in the recipe list</summary>
        public static readonly DependencyProperty ShowImagesProperty =
            DependencyProperty.Register("ShowImages", typeof(bool), typeof(RecipeListViewModel), new PropertyMetadata(true));

        
        /// <summary>Create a new instance of the window, loading all recipes</summary>
        public RecipeListViewModel()
        {
            recipesAdapter = new DetailRecipesAdapter();
            imagesAdapter = new ImagesAdapter();

            ImageDataDelegate = new SetImageDataDelegate(SetImageData);

            getImageWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            getImageWorker.DoWork += GetImageWorker_DoWork;

            GetAllRecipes();
        }

        /// <summary>Fetch complete list of recipes from the database</summary>
        public void GetAllRecipes()
        {
            RecipeCollection = new ObservableCollection<DetailRecipe>(recipesAdapter.SelectAll());
            Recipes = RecipeCollection.ToList();
            UpdateImages();
        }

        /// <summary>Apply search parameters to recipe list</summary>
        public void FilterRecipes()
        {
            Recipes = SearchItems.FilterRecipes(RecipeCollection);
        }

        /// <summary>Reset search parameters</summary>
        public void ClearSearch()
        {
            SearchItems.ClearParameters();
            GetAllRecipes();
        }

        /// <summary>Delete a recipe from the database by id</summary>
        /// <param name="id"></param>
        public void DeleteRecipe(int id)
        {
            recipesAdapter.Delete(id);
        }

        /// <summary>Start to fetch preview images asynchronously</summary>
        public void UpdateImages()
        {
            if (!getImageWorker.IsBusy)
                getImageWorker.RunWorkerAsync(Recipes.Select(p => p.ID).ToList());
        }

        private void GetImageWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is IEnumerable<int> recipeList)
            {
                foreach (int recipeID in recipeList)
                {
                    if (getImageWorker.CancellationPending) return;
                    else
                    {
                        var imgData = imagesAdapter.SelectByRecipe(recipeID)?.IMG_Data;
                        Dispatcher.BeginInvoke(ImageDataDelegate, recipeID, imgData);
                    }
                }
            }
        }

        private delegate void SetImageDataDelegate(int recipeID, byte[] data);
        private SetImageDataDelegate ImageDataDelegate;

        private void SetImageData(int recipeID, byte[] data)
        {
            try
            {
                var recipe = Recipes.ToList().Find(p => p.ID == recipeID);
                recipe.IMG_Data = data;
            }
            catch (Exception e)
            {
                App.LogException(e);
            }
        }

        private void Recipes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (getImageWorker.IsBusy) getImageWorker.CancelAsync();
            if (ShowImages) UpdateImages();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.Property.Name)
            {
                case "RecipeCollection":
                    if (e.OldValue is ObservableCollection<DetailRecipe> oldval)
                        oldval.CollectionChanged -= Recipes_CollectionChanged;
                    if (e.NewValue is ObservableCollection<DetailRecipe> newval)
                        newval.CollectionChanged += Recipes_CollectionChanged;
                    break;

                case "ShowImages":
                    if (e.NewValue is bool showImages)
                    {
                        if (showImages && !getImageWorker.IsBusy) UpdateImages();
                        else if (!showImages && getImageWorker.IsBusy) getImageWorker.CancelAsync();
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
