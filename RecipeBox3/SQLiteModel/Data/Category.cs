using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    public class Category : CookbookRow
    {
        public override int ID
        {
            get => C_ID;
            set => C_ID = value;
        }

        public int C_ID
        {
            get { return (int)GetValue(C_IDProperty); }
            set { SetValue(C_IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for C_ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty C_IDProperty =
            DependencyProperty.Register("C_ID", typeof(int), typeof(Category),
                new PropertyMetadata(-1, OnRowChanged));


        
        public string C_Name
        {
            get { return (string)GetValue(C_NameProperty); }
            set { SetValue(C_NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for C_Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty C_NameProperty =
            DependencyProperty.Register("C_Name", typeof(string), typeof(Category),
                new PropertyMetadata("NewCategory", OnRowChanged));



        public override bool IsUserEditable
        {
            get { return (bool)GetValue(IsUserEditableProperty); }
            set { SetValue(IsUserEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUserEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUserEditableProperty =
            DependencyProperty.Register("IsUserEditable", typeof(bool), typeof(Category), new PropertyMetadata(true, OnRowChanged));


        public Category()
        {
            Status = RowStatus.New;
        }

        public Category(Category source)
        {
            C_ID = source.C_ID;
            C_Name = source.C_Name;
            IsUserEditable = source.IsUserEditable;
            Status = source.Status;
        }
    }
}
