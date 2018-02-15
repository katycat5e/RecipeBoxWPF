using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>An Ingredient joined with Unit data</summary>
    public class DetailIngredient : Ingredient
    {
        /// <summary>Unit ID</summary>
        public int U_ID
        {
            get { return (int)GetValue(U_IDProperty); }
            set { SetValue(U_IDProperty, value); }
        }

        /// <summary>Unit ID</summary>
        public static readonly DependencyProperty U_IDProperty =
            DependencyProperty.Register("U_ID", typeof(int), typeof(DetailIngredient), new PropertyMetadata(1, OnRowChanged));


        /// <summary>Unit Name</summary>
        public string U_Name
        {
            get { return (string)GetValue(U_NameProperty); }
            set { SetValue(U_NameProperty, value); }
        }

        /// <summary>Unit Name</summary>
        public static readonly DependencyProperty U_NameProperty =
            DependencyProperty.Register("U_Name", typeof(string), typeof(DetailIngredient), new PropertyMetadata("New Unit", OnRowChanged));


        /// <summary>Unit Plural Name</summary>
        public string U_Plural
        {
            get { return (string)GetValue(U_PluralProperty); }
            set { SetValue(U_PluralProperty, value); }
        }

        /// <summary>Unit Plural Name</summary>
        public static readonly DependencyProperty U_PluralProperty =
            DependencyProperty.Register("U_Plural", typeof(string), typeof(DetailIngredient), new PropertyMetadata("New Units", OnRowChanged));


        /// <summary>Unit Abbreviation</summary>
        public string U_Abbreviation
        {
            get { return (string)GetValue(U_AbbreviationProperty); }
            set { SetValue(U_AbbreviationProperty, value); }
        }

        /// <summary>Unit Abbreviation</summary>
        public static readonly DependencyProperty U_AbbreviationProperty =
            DependencyProperty.Register("U_Abbreviation", typeof(string), typeof(DetailIngredient), new PropertyMetadata("", OnRowChanged));


        /// <summary>Unit type code, see <see cref="Unit.UnitType"/> for values</summary>
        public Unit.UnitType U_TypeCode
        {
            get { return (Unit.UnitType)GetValue(U_TypeCodeProperty); }
            set { SetValue(U_TypeCodeProperty, value); }
        }

        /// <summary>Unit type code, see <see cref="Unit.UnitType"/> for values</summary>
        public static readonly DependencyProperty U_TypeCodeProperty =
            DependencyProperty.Register("U_TypeCode", typeof(Unit.UnitType), typeof(DetailIngredient), new PropertyMetadata(Unit.UnitType.Mass, OnRowChanged));


        /// <summary>Unit Ratio</summary>
        public float U_Ratio
        {
            get { return (float)GetValue(U_RatioProperty); }
            set { SetValue(U_RatioProperty, value); }
        }

        /// <summary>Unit Ratio</summary>
        public static readonly DependencyProperty U_RatioProperty =
            DependencyProperty.Register("U_Ratio", typeof(float), typeof(DetailIngredient), new PropertyMetadata(1.0F, OnRowChanged));


        /// <summary>Unit system, see <see cref="Unit.System"/> for values</summary>
        public Unit.System U_System
        {
            get { return (Unit.System)GetValue(U_SystemProperty); }
            set { SetValue(U_SystemProperty, value); }
        }

        /// <summary>Unit system, see <see cref="Unit.System"/> for values</summary>
        public static readonly DependencyProperty U_SystemProperty =
            DependencyProperty.Register("U_System", typeof(Unit.System), typeof(DetailIngredient), new PropertyMetadata(Unit.System.Metric, OnRowChanged));


        /// <summary>Create a new Ingredient (does not create unit data)</summary>
        public DetailIngredient() : base()
        {
            Status = RowStatus.New;
        }

        /// <summary>Create a copy of a DetailIngredient entry</summary>
        /// <param name="source"></param>
        public DetailIngredient(DetailIngredient source) : base(source)
        {
            U_Name = source.U_Name;
            U_Plural = source.U_Plural;
            U_Abbreviation = source.U_Abbreviation;
            U_TypeCode = source.U_TypeCode;
            U_Ratio = source.U_Ratio;
            U_System = source.U_System;
        }
    }
}
