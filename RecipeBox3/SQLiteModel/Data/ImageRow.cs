using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>Row for the Images table</summary>
    public class ImageRow : CookbookRow
    {
        /// <inheritdoc/>
        public override int ID
        {
            get => IMG_ID;
            set => IMG_ID = value;
        }

        /// <summary>Image ID</summary>
        public int IMG_ID
        {
            get { return (int)GetValue(IMG_IDProperty); }
            set { SetValue(IMG_IDProperty, value); }
        }

        /// <summary>Image ID</summary>
        public static readonly DependencyProperty IMG_IDProperty =
            DependencyProperty.Register("IMG_ID", typeof(int), typeof(ImageRow), new PropertyMetadata(-1));

        
        /// <summary>Image Recipe ID</summary>
        public int? IMG_RecipeID
        {
            get { return (int?)GetValue(IMG_RecipeIDProperty); }
            set { SetValue(IMG_RecipeIDProperty, value); }
        }

        /// <summary>Image Recipe ID</summary>
        public static readonly DependencyProperty IMG_RecipeIDProperty =
            DependencyProperty.Register("IMG_RecipeID", typeof(int?), typeof(ImageRow), new PropertyMetadata(null));


        /// <summary>Binary Image data</summary>
        public byte[] IMG_Data
        {
            get { return (byte[])GetValue(IMG_DataProperty); }
            set { SetValue(IMG_DataProperty, value); }
        }

        /// <summary>Binary Image data</summary>
        public static readonly DependencyProperty IMG_DataProperty =
            DependencyProperty.Register("IMG_Data", typeof(byte[]), typeof(ImageRow), new PropertyMetadata(null));


        /// <summary>Create a new Image row</summary>
        public ImageRow()
        {
            Status = RowStatus.New;
        }

        /// <summary>Create a copy of an Image row</summary>
        /// <param name="source"></param>
        public ImageRow(ImageRow source)
        {
            IMG_ID = source.IMG_ID;
            IMG_RecipeID = source.IMG_RecipeID;
            IMG_Data = source.IMG_Data;
            Status = source.Status;
        }
    }
}
