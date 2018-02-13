using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for EditRecipeDialog.xaml
    /// </summary>
    public partial class EditRecipeDialog : Window
    {
        private EditRecipeViewModel ViewModel
        {
            get => DataContext as EditRecipeViewModel;
            set => DataContext = value;
        }

        private FontSizeConverter fontSizeConverter = new FontSizeConverter();

        public EditRecipeDialog()
        {
            InitializeComponent();
            InitEditor();

            if (ViewModel != null)
            {
                ViewModel.MyRecipe = new SQLiteModel.Data.DetailRecipe();
                StepsEditor.Document = new FlowDocument()
                {
                    FontSize = 12
                };
                ViewModel.StepsDocument = StepsEditor.Document;
            }
        }

        public EditRecipeDialog(int? recipeID)
        {
            InitializeComponent();
            InitEditor();

            if (ViewModel != null && recipeID.HasValue)
            {
                ViewModel.RecipeID = recipeID;
                if (ViewModel.StepsDocument != null)
                    // if we have a valid doc, pass to UI
                    StepsEditor.Document = ViewModel.StepsDocument;
                else
                    // otherwise send the blank doc to viewmodel
                    ViewModel.StepsDocument = StepsEditor.Document;
            }
        }

        private void InitEditor()
        {
            FontFamilySelector.ItemsSource = Fonts.SystemFontFamilies.OrderBy(font => font.Source);
            FontSizeSelector.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            FontSizeSelector.Text = "11";
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var imagePicker = new ChooseImageDialog();
            Cursor = Cursors.Wait;
            bool? result = imagePicker.ShowDialog();

            if (result == true && ViewModel != null)
            {
                ViewModel.MyRecipe.IMG_Data = ByteImageConverter.ConvertBitmapToBytes(imagePicker.FinalBitmap);
            }
            Cursor = Cursors.Arrow;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                
                DialogResult = ViewModel?.SaveRecipe();
            }
            
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void FontFamilySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilySelector.SelectedItem != null)
                StepsEditor.Selection.ApplyPropertyValue(
                    Inline.FontFamilyProperty,
                    FontFamilySelector.SelectedItem);
            StepsEditor.Focus();
        }

        private void FontSizeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                StepsEditor.Selection.ApplyPropertyValue(
                  Inline.FontSizeProperty,
                  Convert.ToDouble(e.AddedItems[0]));
            }
            StepsEditor.Focus();
        }

        private void StepsEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object propertyVal = StepsEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldButton.IsChecked = (propertyVal != DependencyProperty.UnsetValue) && (propertyVal.Equals(FontWeights.Bold));

            propertyVal = StepsEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicButton.IsChecked = (propertyVal != DependencyProperty.UnsetValue) && (propertyVal.Equals(FontStyles.Italic));

            propertyVal = StepsEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineButton.IsChecked = (propertyVal != DependencyProperty.UnsetValue) && (propertyVal.Equals(TextDecorations.Underline));

            propertyVal = StepsEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilySelector.SelectedItem = propertyVal;

            propertyVal = StepsEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (propertyVal != DependencyProperty.UnsetValue) FontSizeSelector.Text = propertyVal.ToString();
        }
        

        private void NewOrderedListButton_Click(object sender, RoutedEventArgs e)
        {
            Block nextBlock = null;
            Block selectedBlock = GetBlockAroundPosition(StepsEditor.Document.Blocks, StepsEditor.Selection.Start);

            if (selectedBlock is Paragraph)
            {
                nextBlock = selectedBlock.NextBlock;

                var newList = new List(new ListItem(selectedBlock as Paragraph))
                {
                    MarkerStyle = TextMarkerStyle.Decimal,
                    StartIndex = 1
                };

                StepsEditor.Document.Blocks.Remove(selectedBlock);

                if (nextBlock != null)
                    StepsEditor.Document.Blocks.InsertBefore(nextBlock, newList);
                else
                    StepsEditor.Document.Blocks.Add(newList);
            }
            else if (selectedBlock is List selectedList)
            {
                nextBlock = selectedBlock.NextBlock;

                StepsEditor.Document.Blocks.Remove(selectedBlock);

                var children = selectedList.ListItems.ToList();
                Action<Block> addMethod;
                if (nextBlock != null)
                    addMethod = newItem => StepsEditor.Document.Blocks.InsertBefore(nextBlock, newItem);
                else
                    addMethod = StepsEditor.Document.Blocks.Add;

                foreach (ListItem item in children)
                {
                    var itemChildren = item.Blocks.ToList();
                    foreach (Block child in itemChildren)
                    {
                        item.Blocks.Remove(child);
                        addMethod(child);
                    }
                }
            }
        }

        private static Block GetBlockAroundPosition(IEnumerable<Block> blocks, TextPointer position)
        {
            return blocks.Where(
                block =>
                    block.ContentStart.CompareTo(position) == -1 &&
                    block.ContentEnd.CompareTo(position) == 1)
                .FirstOrDefault();
        }
    }
}
