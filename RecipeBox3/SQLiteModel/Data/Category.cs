using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>Row from the Categories table</summary>
    public class Category : CookbookRow
    {
        /// <inheritdoc/>
        public override int ID
        {
            get => C_ID;
            set => C_ID = value;
        }

        /// <summary>Category ID</summary>
        public int C_ID
        {
            get { return (int)GetValue(C_IDProperty); }
            set { SetValue(C_IDProperty, value); }
        }

        /// <summary>Category ID</summary>
        public static readonly DependencyProperty C_IDProperty =
            DependencyProperty.Register("C_ID", typeof(int), typeof(Category),
                new PropertyMetadata(-1, OnRowChanged));


        /// <summary>Category Name</summary>
        public string C_Name
        {
            get { return (string)GetValue(C_NameProperty); }
            set { SetValue(C_NameProperty, value); }
        }

        /// <summary>Category Name</summary>
        public static readonly DependencyProperty C_NameProperty =
            DependencyProperty.Register("C_Name", typeof(string), typeof(Category),
                new PropertyMetadata("NewCategory", OnRowChanged));


        /// <inheritdoc/>
        public override bool IsUserEditable
        {
            get { return (bool)GetValue(IsUserEditableProperty); }
            set { SetValue(IsUserEditableProperty, value); }
        }

        /// <summary>Backing for user editable property</summary>
        public static readonly DependencyProperty IsUserEditableProperty =
            DependencyProperty.Register("IsUserEditable", typeof(bool), typeof(Category), new PropertyMetadata(true, OnRowChanged));


        /// <summary>Create a new category with default values</summary>
        public Category()
        {
            Status = RowStatus.New;
        }

        /// <summary>Create a copy of a category</summary>
        public Category(Category source)
        {
            C_ID = source.C_ID;
            C_Name = source.C_Name;
            IsUserEditable = source.IsUserEditable;
            Status = source.Status;
        }
    }
}
