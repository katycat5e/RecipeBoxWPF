using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace RecipeBox3.Windows
{
    /// <summary>
    /// Interaction logic for EditRecipeDialog.xaml
    /// </summary>
    public partial class EditRecipeWindow : Window
    {
        private EditRecipeViewModel ViewModel
        {
            get => DataContext as EditRecipeViewModel;
            set => DataContext = value;
        }

        /// <summary>Whether the editor inputs hold valid entries</summary>
        public bool InputsAreValid => PrepTimeInput.ValidInput && CookTimeInput.ValidInput;

        private FontSizeConverter fontSizeConverter = new FontSizeConverter();

        /// <summary>Create a new instance of the class</summary>
        public EditRecipeWindow()
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

        /// <summary>
        /// Create a new instance of the recipe editor for the specified recipe.
        /// A null argument creates a new recipe
        /// </summary>
        public EditRecipeWindow(int? recipeID)
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
            var imagePicker = new ChooseImageWindow();
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
            if (InputsAreValid)
            {
                if (ViewModel != null)
                {
                    DialogResult = ViewModel?.SaveRecipe();
                }

                Close();
            }
            else
            {
                MessageBox.Show(
                    "Not all fields contain valid entries, please verify and try again",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
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
        

        private void ToggleOrderedListButton_Click(object sender, RoutedEventArgs e)
        {
            List<Block> selectedBlocks = GetBlocksBetween(StepsEditor.Document.Blocks, StepsEditor.Selection.Start, StepsEditor.Selection.End);
            if (selectedBlocks.Count < 1) return;

            if (selectedBlocks[0] is Paragraph startParagraph)
            {
                var newList = new List()
                {
                    MarkerStyle = TextMarkerStyle.Decimal,
                    StartIndex = 1
                };

                Block nextBlock = selectedBlocks.Last().NextBlock;

                if (selectedBlocks.Count(block => block is List) > 0)
                {
                    // part of the selection is a list, so combine into a single list
                    selectedBlocks.RemoveAt(0);
                    StepsEditor.Document.Blocks.Remove(startParagraph);
                    newList.ListItems.Add(new ListItem(startParagraph));
                    CombineItemsIntoList(newList, selectedBlocks);
                }
                else
                {
                    List<object> selectedContentItems = GetAllContentItems(selectedBlocks);
                    newList.ListItems.AddRange(selectedContentItems.Select(
                        content =>
                        {
                            if (content is Paragraph p) return new ListItem(p);
                            else if (content is List<Block> blocks)
                            {
                                var newItem = new ListItem();
                                newItem.Blocks.AddRange(blocks);
                                return newItem;
                            }
                            else return null;
                        }
                        ).Where(item => item != null));

                    foreach (Block selectedBlock in selectedBlocks)
                    {
                        StepsEditor.Document.Blocks.Remove(selectedBlock);
                    }
                }

                if (nextBlock != null)
                    StepsEditor.Document.Blocks.InsertBefore(nextBlock, newList);
                else
                    StepsEditor.Document.Blocks.Add(newList);

                StepsEditor.Selection.Select(newList.ContentStart, newList.ContentEnd);
            }
            else if (selectedBlocks[0] is List masterList)
            {
                if (selectedBlocks.Count == 1)
                {
                    // Only 1 list selected, just pop selected items out of it
                    List<ListItem> selectedItems = GetListItemsBetween(masterList, StepsEditor.Selection.Start, StepsEditor.Selection.End);

                    if (selectedItems.Count == masterList.ListItems.Count)
                    {
                        // remove whole list
                        var listContents = GetAllListItemContents(masterList.ListItems.ToList(), true).OfType<List<Block>>();

                        Block nextBlock = masterList.NextBlock;
                        StepsEditor.Document.Blocks.Remove(masterList);

                        Action<Block> addMethod;
                        if (nextBlock != null)
                            addMethod = newItem => StepsEditor.Document.Blocks.InsertBefore(nextBlock, newItem);
                        else
                            addMethod = StepsEditor.Document.Blocks.Add;

                        foreach (List<Block> contentItem in listContents)
                        {
                            if (contentItem != null)
                                foreach (Block block in contentItem)
                                    addMethod(block);
                        }

                        if (listContents.Count() > 0)
                        {
                            var contentStart = listContents.First().First().ContentStart;
                            var contentEnd = listContents.Last().Last().ContentEnd;
                            StepsEditor.Selection.Select(contentStart, contentEnd);
                        }
                    }
                    else
                    {
                        // not the whole list is selected
                        ListItem prevItem = selectedItems.First().PreviousListItem;
                        ListItem nextItem = selectedItems.Last().NextListItem;

                        Action<Block> addMethod;
                        if (prevItem == null && nextItem != null)
                        {
                            // we're taking out the start of the list
                            addMethod = newBlock => StepsEditor.Document.Blocks.InsertBefore(masterList, newBlock);
                        }
                        else
                        {
                            // we're taking out the end or middle of the list
                            addMethod = newBlock => StepsEditor.Document.Blocks.InsertAfter(masterList, newBlock);

                            if (nextItem != null)
                            {
                                // not going to the end of the list, need to split it
                                List<ListItem> newListItems = masterList.ListItems.Where(
                                    item =>
                                        item.ContentStart.CompareTo(nextItem.ContentStart) >= 0)
                                    .ToList();

                                foreach (ListItem item in newListItems)
                                {
                                    masterList.ListItems.Remove(item);
                                }

                                List newList = new List()
                                {
                                    MarkerStyle = TextMarkerStyle.Decimal,
                                    StartIndex = 1
                                };

                                newList.ListItems.AddRange(newListItems);
                                addMethod(newList);
                            }
                        }

                        foreach (ListItem item in selectedItems)
                        {
                            masterList.ListItems.Remove(item);
                        }

                        var selectedContents = GetAllListItemContents(selectedItems, true).OfType<List<Block>>();
                        foreach (List<Block> content in selectedContents)
                        {
                            foreach (Block block in content)
                                addMethod(block);
                        }

                        if (selectedContents.Count() > 0)
                        {
                            var contentStart = selectedContents.First().First().ContentStart;
                            var contentEnd = selectedContents.Last().Last().ContentEnd;
                            StepsEditor.Selection.Select(contentStart, contentEnd);
                        }
                    }
                }
                else
                {
                    // multiple blocks selected, combine them into the master list
                    selectedBlocks.Remove(masterList);
                    CombineItemsIntoList(masterList, selectedBlocks);
                    StepsEditor.Selection.Select(masterList.ContentStart, masterList.ContentEnd);
                }
            }
            
            StepsEditor.Focus();
        }

        private static Block GetBlockAroundPosition(IEnumerable<Block> blocks, TextPointer position)
        {
            return blocks.Where(
                block =>
                    block.ContentStart.CompareTo(position) == -1 &&
                    block.ContentEnd.CompareTo(position) == 1)
                .FirstOrDefault();
        }

        private static List<Block> GetBlocksBetween(IEnumerable<Block> blocks, TextPointer start, TextPointer end)
        {
            return blocks.Where(
                block =>
                    (block.ContentStart.CompareTo(start) == -1 && block.ContentEnd.CompareTo(start) == 1) || // selection starts inside
                    (block.ContentStart.CompareTo(end) == -1 && block.ContentEnd.CompareTo(end) == 1) || // selection ends inside
                    (block.ContentStart.CompareTo(start) == 1 && block.ContentEnd.CompareTo(end) == -1) // block is inside selection
                ).ToList();
        }

        private static List<ListItem> GetListItemsBetween(List list, TextPointer start, TextPointer end)
        {
            return list.ListItems.Where(
                item =>
                    (item.ContentStart.CompareTo(start) == -1 && item.ContentEnd.CompareTo(start) == 1) || // selection starts inside
                    (item.ContentStart.CompareTo(end) == -1 && item.ContentEnd.CompareTo(end) == 1) || // selection ends inside
                    (item.ContentStart.CompareTo(start) == 1 && item.ContentEnd.CompareTo(end) == -1) // item is inside selection
                ).ToList();
        }

        private static List<object> GetAllContentItems(IEnumerable<Block> blocks)
        {
            var results = new List<object>();

            foreach (Block block in blocks)
            {
                if (block is Paragraph paragraph)
                    results.Add(paragraph);
                else if (block is List list)
                    results.Add(GetAllListItemContents(list.ListItems));
            }

            return results;
        }

        private static List<object> GetAllListItemContents(IEnumerable<ListItem> items, bool popItems = false)
        {
            var results = new List<object>();

            foreach (ListItem item in items)
            {
                results.Add(new List<Block>(item.Blocks));
                if (popItems) item.Blocks.Clear();
            }

            return results;
        }

        private void CombineItemsIntoList(List masterList, IEnumerable<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                if (block is Paragraph paragraph)
                {
                    masterList.ListItems.Add(new ListItem(paragraph));
                }
                else if (block is List list)
                {
                    List<ListItem> items = list.ListItems.ToList();
                    foreach (ListItem item in items)
                    {
                        list.ListItems.Remove(item);
                        masterList.ListItems.Add(item);
                    }
                }

                StepsEditor.Document.Blocks.Remove(block);
            }
        }
    }
}
