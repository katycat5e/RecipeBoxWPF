using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    public class Unit : CookbookRow<Unit>
    {
        public override int ID
        {
            get => U_ID;
            set => U_ID = value;
        }

        public int U_ID
        {
            get { return (int)GetValue(U_IDProperty); }
            set { SetValue(U_IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for U_ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_IDProperty =
            DependencyProperty.Register("U_ID", typeof(int), typeof(Unit), new PropertyMetadata(-1, OnRowChanged));



        public string U_Name
        {
            get { return (string)GetValue(U_NameProperty); }
            set { SetValue(U_NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for U_Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_NameProperty =
            DependencyProperty.Register("U_Name", typeof(string), typeof(Unit), new PropertyMetadata("New Unit", OnRowChanged));



        public string U_Plural
        {
            get { return (string)GetValue(U_PluralProperty); }
            set { SetValue(U_PluralProperty, value); }
        }

        // Using a DependencyProperty as the backing store for U_Plural.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_PluralProperty =
            DependencyProperty.Register("U_Plural", typeof(string), typeof(Unit), new PropertyMetadata("New Units", OnRowChanged));



        public string U_Abbreviation
        {
            get { return (string)GetValue(U_AbbreviationProperty); }
            set { SetValue(U_AbbreviationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Abbreviation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_AbbreviationProperty =
            DependencyProperty.Register("U_Abbreviation", typeof(string), typeof(Unit), new PropertyMetadata("", OnRowChanged));



        public UnitType U_TypeCode
        {
            get { return (UnitType)GetValue(U_TypeCodeProperty); }
            set { SetValue(U_TypeCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for U_TypeCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_TypeCodeProperty =
            DependencyProperty.Register("U_TypeCode", typeof(UnitType), typeof(Unit), new PropertyMetadata(UnitType.Mass, OnRowChanged));



        public float U_Ratio
        {
            get { return (float)GetValue(U_RatioProperty); }
            set { SetValue(U_RatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for U_Ratio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_RatioProperty =
            DependencyProperty.Register("U_Ratio", typeof(float), typeof(Unit), new PropertyMetadata(1.0, OnRowChanged));



        public System U_System
        {
            get { return (System)GetValue(U_SystemProperty); }
            set { SetValue(U_SystemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for U_System.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty U_SystemProperty =
            DependencyProperty.Register("U_System", typeof(System), typeof(Unit), new PropertyMetadata(System.Metric, OnRowChanged));


        public Unit()
        {
            Status = RowStatus.New;
        }

        public Unit(Unit source)
        {
            U_ID = source.U_ID;
            U_Name = source.U_Name;
            U_Plural = source.U_Plural;
            U_Abbreviation = source.U_Abbreviation;
            U_TypeCode = source.U_TypeCode;
            U_Ratio = source.U_Ratio;
            U_System = source.U_System;
            Status = source.Status;
        }

        public enum UnitType : int
        {
            Mass = 1, Volume = 2, Amount = 3
        }

        public enum System : int
        {
            Metric = 0, Customary = 1
        }
    }
}
