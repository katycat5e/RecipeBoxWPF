using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace RecipeBox3.SQLiteModel.Data
{
    public class Recipe : RecipeBase<Recipe> { }

    /// <summary>
    /// Abstract class for creating joins using the Recipes table
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public abstract class RecipeBase<U> : CookbookRow<U> where U : RecipeBase<U>
    {
        public override int ID
        {
            get => R_ID;
            set => R_ID = value;
        }

        public int R_ID
        {
            get { return (int)GetValue(R_IDProperty); }
            set { SetValue(R_IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_IDProperty =
            DependencyProperty.Register("R_ID", typeof(int), typeof(RecipeBase<U>), new PropertyMetadata(-1, OnRowChanged));



        public string R_Name
        {
            get { return (string)GetValue(R_NameProperty); }
            set { SetValue(R_NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_NameProperty =
            DependencyProperty.Register("R_Name", typeof(string), typeof(RecipeBase<U>), new PropertyMetadata("NewRecipe", OnRowChanged));



        public string R_Description
        {
            get { return (string)GetValue(R_DescriptionProperty); }
            set { SetValue(R_DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_DescriptionProperty =
            DependencyProperty.Register("R_Description", typeof(string), typeof(RecipeBase<U>), new PropertyMetadata("", OnRowChanged));



        public string ModifiedDateTime
        {
            get
            {
                if (R_Modified.HasValue)
                    return DateTime.FromFileTime(R_Modified.Value).ToString("d/M/yy h:mm tt");
                else return "N/A";
            }
            set
            {
                if (DateTime.TryParse(value, out DateTime inDate))
                {
                    R_Modified = inDate.ToFileTime();
                }
                else R_Modified = null;
            }
        }

        public long? R_Modified
        {
            get { return (long?)GetValue(R_ModifiedProperty); }
            set { SetValue(R_ModifiedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_Modified.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_ModifiedProperty =
            DependencyProperty.Register("R_Modified", typeof(long?), typeof(RecipeBase<U>), new PropertyMetadata(null, OnRowChanged));



        public int R_PrepTime
        {
            get { return (int)GetValue(R_PrepTimeProperty); }
            set { SetValue(R_PrepTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_PrepTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_PrepTimeProperty =
            DependencyProperty.Register("R_PrepTime", typeof(int), typeof(RecipeBase<U>), new PropertyMetadata(0, OnRowChanged));



        public int R_CookTime
        {
            get { return (int)GetValue(R_CookTimeProperty); }
            set { SetValue(R_CookTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_CookTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_CookTimeProperty =
            DependencyProperty.Register("R_CookTime", typeof(int), typeof(RecipeBase<U>), new PropertyMetadata(0, OnRowChanged));



        public string R_Steps
        {
            get { return (string)GetValue(R_StepsProperty); }
            set { SetValue(R_StepsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_Steps.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_StepsProperty =
            DependencyProperty.Register("R_Steps", typeof(string), typeof(RecipeBase<U>), new PropertyMetadata("", OnRowChanged));



        public int R_Category
        {
            get { return (int)GetValue(R_CategoryProperty); }
            set { SetValue(R_CategoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R_Category.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty R_CategoryProperty =
            DependencyProperty.Register("R_Category", typeof(int), typeof(RecipeBase<U>), new PropertyMetadata(1, OnRowChanged));
        
        

        public RecipeBase()
        {
            R_Modified = DateTime.Now.ToFileTime();
            Status = RowStatus.New;
        }

        public RecipeBase(RecipeBase<U> source)
        {
            R_ID = source.R_ID;
            R_Name = source.R_Name;
            R_Description = source.R_Description;
            R_Modified = source.R_Modified;
            R_PrepTime = source.R_PrepTime;
            R_CookTime = source.R_CookTime;
            R_Steps = source.R_Steps;
            R_Category = source.R_Category;
            Status = source.Status;
        }

        public static FlowDocument ParseSteps(string stepsString)
        {
            try
            {
                using (var sr = new System.IO.StringReader(stepsString))
                {
                    using (var xmlReader = new XmlTextReader(sr))
                    {
                        return XamlReader.Parse(stepsString) as FlowDocument;
                    }
                }
            }
            catch (Exception e)
            {
                App.LogException(e);
                return null;
            }
        }

        public static string SerializeSteps(FlowDocument document)
        {
            if (document == null) return null;
            return XamlWriter.Save(document);
        }
    }
}
