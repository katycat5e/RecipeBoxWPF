using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace RecipeBox3
{
    class ViewRecipeModel : DependencyObject
    {


        public string RecipeName
        {
            get { return (string)GetValue(RecipeNameProperty); }
            set { SetValue(RecipeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecipeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipeNameProperty =
            DependencyProperty.Register("RecipeName", typeof(string), typeof(ViewRecipeModel), new PropertyMetadata("Recipe"));



        public string RecipeCategory
        {
            get { return (string)GetValue(RecipeCategoryProperty); }
            set { SetValue(RecipeCategoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecipeCategory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipeCategoryProperty =
            DependencyProperty.Register("RecipeCategory", typeof(string), typeof(ViewRecipeModel), new PropertyMetadata("Category"));



        public string RecipeModified
        {
            get { return (string)GetValue(RecipeModifiedProperty); }
            set { SetValue(RecipeModifiedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecipeModified.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipeModifiedProperty =
            DependencyProperty.Register("RecipeModified", typeof(string), typeof(ViewRecipeModel), new PropertyMetadata(string.Empty));



        public int PrepTime
        {
            get { return (int)GetValue(PrepTimeProperty); }
            set { SetValue(PrepTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrepTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrepTimeProperty =
            DependencyProperty.Register("PrepTime", typeof(int), typeof(ViewRecipeModel), new PropertyMetadata(0));



        public int CookTime
        {
            get { return (int)GetValue(CookTimeProperty); }
            set { SetValue(CookTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CookTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CookTimeProperty =
            DependencyProperty.Register("CookTime", typeof(int), typeof(ViewRecipeModel), new PropertyMetadata(0));



        public string RecipeSteps
        {
            get { return (string)GetValue(RecipeStepsProperty); }
            set { SetValue(RecipeStepsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecipeSteps.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipeStepsProperty =
            DependencyProperty.Register("RecipeSteps", typeof(string), typeof(ViewRecipeModel), new PropertyMetadata(string.Empty));



        public Image RecipePreview
        {
            get { return (Image)GetValue(RecipePreviewProperty); }
            set { SetValue(RecipePreviewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecipePreview.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipePreviewProperty =
            DependencyProperty.Register("RecipePreview", typeof(Image), typeof(ViewRecipeModel), new PropertyMetadata(null));


    }
}
