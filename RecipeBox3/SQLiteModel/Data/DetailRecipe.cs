using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    public class DetailRecipe : RecipeBase<DetailRecipe>
    {
        public string C_Name
        {
            get { return (string)GetValue(C_NameProperty); }
            set { SetValue(C_NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for C_Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty C_NameProperty =
            DependencyProperty.Register("C_Name", typeof(string), typeof(DetailRecipe), new PropertyMetadata(""));

        
        public byte[] IMG_Data
        {
            get { return (byte[])GetValue(IMG_DataProperty); }
            set { SetValue(IMG_DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IMG_Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IMG_DataProperty =
            DependencyProperty.Register("IMG_Data", typeof(byte[]), typeof(DetailRecipe), new PropertyMetadata(null));

        

        public DetailRecipe() : base() { }

        public DetailRecipe(Recipe source)
        {
            R_ID = source.R_ID;
            R_Name = source.R_Name;
            R_Description = source.R_Description;
            R_Modified = source.R_Modified;
            R_PrepTime = source.R_PrepTime;
            R_CookTime = source.R_CookTime;
            R_Steps = source.R_Steps;
            R_Category = source.R_Category;
            
            C_Name = null;
            IMG_Data = null;

            Status = source.Status;
        }

        public DetailRecipe(DetailRecipe source) : base(source)
        {
            C_Name = source.C_Name;
            IMG_Data = source.IMG_Data;
        }
    }
}
