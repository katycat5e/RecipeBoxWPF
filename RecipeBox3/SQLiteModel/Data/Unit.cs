using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>Row for the Units table</summary>
    public class Unit : CookbookRow
    {
        /// <inheritdoc/>
        public override int ID
        {
            get => U_ID;
            set => U_ID = value;
        }

        /// <summary>Unit ID</summary>
        public int U_ID
        {
            get { return (int)GetValue(U_IDProperty); }
            set { SetValue(U_IDProperty, value); }
        }

        /// <summary>Unit ID</summary>
        public static readonly DependencyProperty U_IDProperty =
            DependencyProperty.Register("U_ID", typeof(int), typeof(Unit), new PropertyMetadata(-1, OnRowChanged));


        /// <summary>Unit Name</summary>
        public string U_Name
        {
            get { return (string)GetValue(U_NameProperty); }
            set { SetValue(U_NameProperty, value); }
        }

        /// <summary>Unit Name</summary>
        public static readonly DependencyProperty U_NameProperty =
            DependencyProperty.Register("U_Name", typeof(string), typeof(Unit), new PropertyMetadata("New Unit", OnRowChanged));


        /// <summary>Unit Plural Name</summary>
        public string U_Plural
        {
            get { return (string)GetValue(U_PluralProperty); }
            set { SetValue(U_PluralProperty, value); }
        }

        /// <summary>Unit Plural Name</summary>
        public static readonly DependencyProperty U_PluralProperty =
            DependencyProperty.Register("U_Plural", typeof(string), typeof(Unit), new PropertyMetadata("New Units", OnRowChanged));


        /// <summary>Unit Abbreviation</summary>
        public string U_Abbreviation
        {
            get { return (string)GetValue(U_AbbreviationProperty); }
            set { SetValue(U_AbbreviationProperty, value); }
        }

        /// <summary>Unit Abbreviation</summary>
        public static readonly DependencyProperty U_AbbreviationProperty =
            DependencyProperty.Register("U_Abbreviation", typeof(string), typeof(Unit), new PropertyMetadata("", OnRowChanged));


        /// <summary>Unit type code, see <see cref="UnitType"/> for values</summary>
        public UnitType U_TypeCode
        {
            get { return (UnitType)GetValue(U_TypeCodeProperty); }
            set { SetValue(U_TypeCodeProperty, value); }
        }

        /// <summary>Backing for <see cref="U_TypeCode"/></summary>
        public static readonly DependencyProperty U_TypeCodeProperty =
            DependencyProperty.Register("U_TypeCode", typeof(UnitType), typeof(Unit), new PropertyMetadata(UnitType.Mass, OnRowChanged));


        /// <summary>Unit Ratio</summary>
        public float U_Ratio
        {
            get { return (float)GetValue(U_RatioProperty); }
            set { SetValue(U_RatioProperty, value); }
        }

        /// <summary>Unit Ratio</summary>
        public static readonly DependencyProperty U_RatioProperty =
            DependencyProperty.Register("U_Ratio", typeof(float), typeof(Unit), new PropertyMetadata(1.0F, OnRowChanged));


        /// <summary>Unit system, see <see cref="System"/> for values</summary>
        public System U_System
        {
            get { return (System)GetValue(U_SystemProperty); }
            set { SetValue(U_SystemProperty, value); }
        }

        /// <summary>Backing for <see cref="U_System"/></summary>
        public static readonly DependencyProperty U_SystemProperty =
            DependencyProperty.Register("U_System", typeof(System), typeof(Unit), new PropertyMetadata(System.Metric, OnRowChanged));


        /// <inheritdoc/>
        public override bool IsUserEditable
        {
            get { return (bool)GetValue(IsUserEditableProperty); }
            set { SetValue(IsUserEditableProperty, value); }
        }

        /// <summary>Backing for <see cref="IsUserEditable"/></summary>
        public static readonly DependencyProperty IsUserEditableProperty =
            DependencyProperty.Register("IsUserEditable", typeof(bool), typeof(Unit), new PropertyMetadata(true, OnRowChanged));


        /// <summary>Create a new Unit with default values</summary>
        public Unit()
        {
            Status = RowStatus.New;
        }

        /// <summary>Create a copy of a Unit</summary>
        /// <param name="source"></param>
        public Unit(Unit source)
        {
            U_ID = source.U_ID;
            U_Name = source.U_Name;
            U_Plural = source.U_Plural;
            U_Abbreviation = source.U_Abbreviation;
            U_TypeCode = source.U_TypeCode;
            U_Ratio = source.U_Ratio;
            U_System = source.U_System;
            IsUserEditable = source.IsUserEditable;
            Status = source.Status;
        }

        /// <summary>Enum of unit classes, units sharing a type can be converted to each other</summary>
        public enum UnitType : int
        {
            /// <summary>Unit of mass, based on kilogram</summary>
            Mass = 1,

            /// <summary>Unit of volume, based on liter</summary>
            Volume = 2,

            /// <summary>Unit of count/amount</summary>
            Amount = 3
        }

        /// <summary>Systems of units</summary>
        public enum System : int
        {
            /// <summary>Any/all systems</summary>
            Any = 0,

            /// <summary>Metric/SI units</summary>
            Metric = 1,

            /// <summary>United States Customary units</summary>
            Customary = 2
        }
    }

    /// <summary>Extension methods for Unit enums</summary>
    public static class UnitEnumExtensions
    {
        /// <summary>Get a string representation of a <see cref="Unit.UnitType"/></summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string GetString(this Unit.UnitType me)
        {
            switch (me)
            {
                case Unit.UnitType.Mass:
                    return "Mass";
                case Unit.UnitType.Volume:
                    return "Volume";
                case Unit.UnitType.Amount:
                    return "Amount";
                default:
                    return me.ToString();
            }
        }

        /// <summary>Get a string representation of a <see cref="Unit.System"/></summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string GetString(this Unit.System me)
        {
            switch (me)
            {
                case Unit.System.Metric:
                    return "Metric";
                case Unit.System.Customary:
                    return "Customary";
                case Unit.System.Any:
                    return "Any";
                default:
                    return me.ToString();
            }
        }
    }
}
