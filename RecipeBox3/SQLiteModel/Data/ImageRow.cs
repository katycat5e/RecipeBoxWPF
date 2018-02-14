using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeBox3.SQLiteModel.Data
{
    public class ImageRow : CookbookRow
    {
        public override int ID
        {
            get => IMG_ID;
            set => IMG_ID = value;
        }

        public int IMG_ID
        {
            get { return (int)GetValue(IMG_IDProperty); }
            set { SetValue(IMG_IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IMG_ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IMG_IDProperty =
            DependencyProperty.Register("IMG_ID", typeof(int), typeof(ImageRow), new PropertyMetadata(-1));

        

        public int? IMG_RecipeID
        {
            get { return (int?)GetValue(IMG_RecipeIDProperty); }
            set { SetValue(IMG_RecipeIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IMG_RecipeID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IMG_RecipeIDProperty =
            DependencyProperty.Register("IMG_RecipeID", typeof(int?), typeof(ImageRow), new PropertyMetadata(null));



        public byte[] IMG_Data
        {
            get { return (byte[])GetValue(IMG_DataProperty); }
            set { SetValue(IMG_DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IMG_Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IMG_DataProperty =
            DependencyProperty.Register("IMG_Data", typeof(byte[]), typeof(ImageRow), new PropertyMetadata(null));


        public ImageRow()
        {
            Status = RowStatus.New;
        }

        public ImageRow(ImageRow source)
        {
            IMG_ID = source.IMG_ID;
            IMG_RecipeID = source.IMG_RecipeID;
            IMG_Data = source.IMG_Data;
            Status = source.Status;
        }
    }
}
