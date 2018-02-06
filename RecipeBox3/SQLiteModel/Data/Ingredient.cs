using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    public class Ingredient : IngredientBase<Ingredient> { }

    public abstract class IngredientBase<U> : CookbookRow<U> where U : IngredientBase<U>
    {
        public override int ID
        {
            get => IE_ID;
            set => IE_ID = value;
        }
        
        public int IE_ID
        {
            get { return (int)GetValue(IE_IDProperty); }
            set { SetValue(IE_IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IE_ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IE_IDProperty =
            DependencyProperty.Register("IE_ID", typeof(int), typeof(Ingredient), new PropertyMetadata(-1, OnRowChanged));



        public string IE_Name
        {
            get { return (string)GetValue(IE_NameProperty); }
            set { SetValue(IE_NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IE_Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IE_NameProperty =
            DependencyProperty.Register("IE_Name", typeof(string), typeof(Ingredient), new PropertyMetadata("New Ingredient", OnRowChanged));



        public decimal IE_Amount
        {
            get { return (decimal)GetValue(IE_AmountProperty); }
            set { SetValue(IE_AmountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IE_Amount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IE_AmountProperty =
            DependencyProperty.Register("IE_Amount", typeof(decimal), typeof(Ingredient), new PropertyMetadata(0.000M, OnRowChanged));



        public int IE_Unit
        {
            get { return (int)GetValue(IE_UnitProperty); }
            set { SetValue(IE_UnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IE_Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IE_UnitProperty =
            DependencyProperty.Register("IE_Unit", typeof(int), typeof(Ingredient), new PropertyMetadata(1, OnRowChanged));



        public int? IE_RecipeID
        {
            get { return (int?)GetValue(IE_RecipeIDProperty); }
            set { SetValue(IE_RecipeIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IE_RecipeID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IE_RecipeIDProperty =
            DependencyProperty.Register("IE_RecipeID", typeof(int?), typeof(Ingredient), new PropertyMetadata(null, OnRowChanged));


        public IngredientBase()
        {
            Status = RowStatus.New;
        }

        public IngredientBase(IngredientBase<U> source)
        {
            IE_ID = source.IE_ID;
            IE_Name = source.IE_Name;
            IE_Amount = source.IE_Amount;
            IE_Unit = source.IE_Unit;
            IE_RecipeID = source.IE_RecipeID;
            Status = source.Status;
        }
    }
}
