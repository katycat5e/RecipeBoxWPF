using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>A row from the Properties table</summary>
    public class PropertyRow : CookbookRow
    {
        /// <inheritdoc/>
        public override int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        /// <summary>Property store for <see cref='ID'/></summary>
        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register("ID", typeof(int), typeof(PropertyRow), new PropertyMetadata(-1, OnRowChanged));


        /// <summary>Name of the property</summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>Property store for <see cref='Name'/></summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(PropertyRow), new PropertyMetadata(null, OnRowChanged));



        /// <summary>Property value as a string</summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>Property store for <see cref='Value'/></summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(PropertyRow), new PropertyMetadata(null, OnRowChanged));


        /// <summary>Create a new instance of the class</summary>
        public PropertyRow()
        {
            Status = RowStatus.New;
        }

        /// <summary>Create a new instance of the class with the specified values</summary>
        public PropertyRow(string name, string value) : base()
        {
            Name = name;
            Value = value;
            Status = RowStatus.New;
        }
    }
}
