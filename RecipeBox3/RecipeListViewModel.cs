using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3
{
    public class RecipeListViewModel : DependencyObject
    {
        private DetailRecipesAdapter recipesAdapter;
        private ImagesAdapter imagesAdapter;

        private BackgroundWorker getImageWorker;

        public ObservableCollection<DetailRecipe> Recipes
        {
            get { return (ObservableCollection<DetailRecipe>)GetValue(RecipesProperty); }
            set { SetValue(RecipesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Recipes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipesProperty =
            DependencyProperty.Register("Recipes", typeof(ObservableCollection<DetailRecipe>), typeof(RecipeListViewModel), new PropertyMetadata(null));


        public object SelectedGridItem
        {
            get { return (object)GetValue(SelectedGridItemProperty); }
            set { SetValue(SelectedGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedGridItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedGridItemProperty =
            DependencyProperty.Register("SelectedGridItem", typeof(object), typeof(RecipeListViewModel), new PropertyMetadata(null));

        
        public bool ShowImages
        {
            get { return (bool)GetValue(ShowImagesProperty); }
            set { SetValue(ShowImagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowImages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowImagesProperty =
            DependencyProperty.Register("ShowImages", typeof(bool), typeof(RecipeListViewModel), new PropertyMetadata(true));



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

        public void GetAllRecipes()
        {
            Recipes = new ObservableCollection<DetailRecipe>(recipesAdapter.SelectAll());
        }

        public void DeleteRecipe(int id)
        {
            recipesAdapter.Delete(id);
        }

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

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.Property.Name)
            {
                case "Recipes":
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
