using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>Row from the Ingredients table</summary>
    public class Ingredient : CookbookRow
    {
        /// <inheritdoc/>
        public override int ID
        {
            get => IE_ID;
            set => IE_ID = value;
        }
        
        /// <summary>Ingredient ID</summary>
        public int IE_ID
        {
            get { return (int)GetValue(IE_IDProperty); }
            set { SetValue(IE_IDProperty, value); }
        }

        /// <summary>Ingredient ID</summary>
        public static readonly DependencyProperty IE_IDProperty =
            DependencyProperty.Register("IE_ID", typeof(int), typeof(Ingredient), new PropertyMetadata(-1, OnRowChanged));


        /// <summary>Ingredient Name</summary>
        public string IE_Name
        {
            get { return (string)GetValue(IE_NameProperty); }
            set { SetValue(IE_NameProperty, value); }
        }

        /// <summary>Ingredient Name</summary>
        public static readonly DependencyProperty IE_NameProperty =
            DependencyProperty.Register("IE_Name", typeof(string), typeof(Ingredient), new PropertyMetadata("New Ingredient", OnRowChanged));


        /// <summary>Ingredient Amount</summary>
        public Fraction IE_Amount
        {
            get { return (Fraction)GetValue(IE_AmountProperty); }
            set { SetValue(IE_AmountProperty, value); }
        }

        /// <summary>Ingredient Amount</summary>
        public static readonly DependencyProperty IE_AmountProperty =
            DependencyProperty.Register("IE_Amount", typeof(Fraction), typeof(Ingredient), new PropertyMetadata(new Fraction(0F), OnRowChanged));


        /// <summary>Ingredient Unit ID</summary>
        public int IE_Unit
        {
            get { return (int)GetValue(IE_UnitProperty); }
            set { SetValue(IE_UnitProperty, value); }
        }

        /// <summary>Ingredient Unit ID</summary>
        public static readonly DependencyProperty IE_UnitProperty =
            DependencyProperty.Register("IE_Unit", typeof(int), typeof(Ingredient), new PropertyMetadata(1, OnRowChanged));


        /// <summary>Ingredient Recipe ID</summary>
        public int? IE_RecipeID
        {
            get { return (int?)GetValue(IE_RecipeIDProperty); }
            set { SetValue(IE_RecipeIDProperty, value); }
        }

        /// <summary>Ingredient Recipe ID</summary>
        public static readonly DependencyProperty IE_RecipeIDProperty =
            DependencyProperty.Register("IE_RecipeID", typeof(int?), typeof(Ingredient), new PropertyMetadata(null, OnRowChanged));


        /// <summary>Create a new Ingredient entry</summary>
        public Ingredient()
        {
            Status = RowStatus.New;
        }

        /// <summary>Create a copy of an Ingredient</summary>
        /// <param name="source"></param>
        public Ingredient(Ingredient source)
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
