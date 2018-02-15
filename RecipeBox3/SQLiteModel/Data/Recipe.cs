using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace RecipeBox3.SQLiteModel.Data
{
    /// <summary>Row for the Recipes Table</summary>
    public class Recipe : CookbookRow
    {
        /// <inheritdoc/>
        public override int ID
        {
            get => R_ID;
            set => R_ID = value;
        }

        /// <summary>Recipe ID</summary>
        public int R_ID
        {
            get { return (int)GetValue(R_IDProperty); }
            set { SetValue(R_IDProperty, value); }
        }

        /// <summary>Recipe ID</summary>
        public static readonly DependencyProperty R_IDProperty =
            DependencyProperty.Register("R_ID", typeof(int), typeof(Recipe), new PropertyMetadata(-1, OnRowChanged));


        /// <summary>Recipe Name</summary>
        public string R_Name
        {
            get { return (string)GetValue(R_NameProperty); }
            set { SetValue(R_NameProperty, value); }
        }

        /// <summary>Recipe Name</summary>
        public static readonly DependencyProperty R_NameProperty =
            DependencyProperty.Register("R_Name", typeof(string), typeof(Recipe), new PropertyMetadata("NewRecipe", OnRowChanged));


        /// <summary>Recipe Description</summary>
        public string R_Description
        {
            get { return (string)GetValue(R_DescriptionProperty); }
            set { SetValue(R_DescriptionProperty, value); }
        }

        /// <summary>Recipe Description</summary>
        public static readonly DependencyProperty R_DescriptionProperty =
            DependencyProperty.Register("R_Description", typeof(string), typeof(Recipe), new PropertyMetadata("", OnRowChanged));


        /// <summary>Get/set the last modified time as a date string</summary>
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

        /// <summary>Last modified timestamp in 64 bit file time, or null if unset</summary>
        public long? R_Modified
        {
            get { return (long?)GetValue(R_ModifiedProperty); }
            set { SetValue(R_ModifiedProperty, value); }
        }

        /// <summary>Backing for <see cref="R_Modified"/></summary>
        public static readonly DependencyProperty R_ModifiedProperty =
            DependencyProperty.Register("R_Modified", typeof(long?), typeof(Recipe), new PropertyMetadata(null, OnRowChanged));


        /// <summary>Prep time in minutes</summary>
        public int R_PrepTime
        {
            get { return (int)GetValue(R_PrepTimeProperty); }
            set { SetValue(R_PrepTimeProperty, value); }
        }

        /// <summary>Prep time in minutes</summary>
        public static readonly DependencyProperty R_PrepTimeProperty =
            DependencyProperty.Register("R_PrepTime", typeof(int), typeof(Recipe), new PropertyMetadata(0, OnRowChanged));


        /// <summary>Cook time in minutes</summary>
        public int R_CookTime
        {
            get { return (int)GetValue(R_CookTimeProperty); }
            set { SetValue(R_CookTimeProperty, value); }
        }

        /// <summary>Cook time in minutes</summary>
        public static readonly DependencyProperty R_CookTimeProperty =
            DependencyProperty.Register("R_CookTime", typeof(int), typeof(Recipe), new PropertyMetadata(0, OnRowChanged));


        /// <summary>Serialized recipe instructions document</summary>
        public string R_Steps
        {
            get { return (string)GetValue(R_StepsProperty); }
            set { SetValue(R_StepsProperty, value); }
        }

        /// <summary>Serialized recipe instructions document</summary>
        public static readonly DependencyProperty R_StepsProperty =
            DependencyProperty.Register("R_Steps", typeof(string), typeof(Recipe), new PropertyMetadata("", OnRowChanged));


        /// <summary>Recipe Category</summary>
        public int R_Category
        {
            get { return (int)GetValue(R_CategoryProperty); }
            set { SetValue(R_CategoryProperty, value); }
        }

        /// <summary>Recipe Category</summary>
        public static readonly DependencyProperty R_CategoryProperty =
            DependencyProperty.Register("R_Category", typeof(int), typeof(Recipe), new PropertyMetadata(1, OnRowChanged));


        /// <summary>Create a new Recipe with default values</summary>
        public Recipe()
        {
            R_Modified = DateTime.Now.ToFileTime();
            Status = RowStatus.New;
        }

        /// <summary>Create a copy of a Recipe</summary>
        /// <param name="source"></param>
        public Recipe(Recipe source)
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

        /// <summary>Convert a serialized document from the database into a viewable/editable <see cref="FlowDocument"/></summary>
        /// <param name="stepsString">string containing XAML representation of a <see cref="FlowDocument"/></param>
        /// <returns><see cref="FlowDocument"/> constructed from the input</returns>
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
                return new FlowDocument();
            }
        }

        /// <summary>Convert a <see cref="FlowDocument"/> into a database-storable string</summary>
        /// <param name="document">Document to be serialized</param>
        /// <returns>A string that can be saved and converted back into a document</returns>
        public static string SerializeSteps(FlowDocument document)
        {
            if (document == null) return null;
            return XamlWriter.Save(document);
        }
    }
}
