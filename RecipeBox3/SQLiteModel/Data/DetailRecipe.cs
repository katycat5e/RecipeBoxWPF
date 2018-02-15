using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>Recipe joined with a Category and Image</summary>
    public class DetailRecipe : Recipe
    {
        /// <summary>Category Name</summary>
        public string C_Name
        {
            get { return (string)GetValue(C_NameProperty); }
            set { SetValue(C_NameProperty, value); }
        }

        /// <summary>Category Name</summary>
        public static readonly DependencyProperty C_NameProperty =
            DependencyProperty.Register("C_Name", typeof(string), typeof(DetailRecipe),
                new PropertyMetadata("", OnRowChanged));

        
        /// <summary>Binary Image data</summary>
        public byte[] IMG_Data
        {
            get { return (byte[])GetValue(IMG_DataProperty); }
            set { SetValue(IMG_DataProperty, value); }
        }

        /// <summary>Binary Image data</summary>
        public static readonly DependencyProperty IMG_DataProperty =
            DependencyProperty.Register("IMG_Data", typeof(byte[]), typeof(DetailRecipe),
                new PropertyMetadata(null, OnRowChanged));


        /// <summary>Create a new Recipe (Category and Image data is not set)</summary>
        public DetailRecipe() : base() { }

        /// <summary>Create a new detailed recipe using data from a <see cref="Recipe"/> entry</summary>
        /// <param name="source"></param>
        public DetailRecipe(Recipe source) : base(source)
        {
            C_Name = null;
            IMG_Data = null;
        }

        /// <summary>Create a copy of a detailed recipe</summary>
        /// <param name="source"></param>
        public DetailRecipe(DetailRecipe source) : base(source)
        {
            C_Name = source.C_Name;
            IMG_Data = source.IMG_Data;
        }
    }
}
