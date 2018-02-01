using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using RecipeBox3.SQLiteModel.Data;
using Xceed.Wpf.DataGrid;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RecipeListWindow : Window
    {
        public CookbookDataSet DataSet { get; set; }
        private static CookbookModel CookbookAdapter { get { return App.GlobalCookbookModel; } }

        private RecipeListViewModel ViewModel => DataContext as RecipeListViewModel;
        
        public RecipeListWindow()
        {
            InitializeComponent();
            DataSet = new CookbookDataSet();
            ShowImagesMenuItem.IsChecked = true;
        }

        public void ReloadTable(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            if (ViewModel != null) ViewModel.GetAllRecipes();

            Cursor = Cursors.Arrow;
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new EditSettingsDialog { Owner = this };
            settingsDialog.ShowDialog();
        }

        private void RecipeListView_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateImages()
        {
            if (!Properties.Settings.Default.ShowPreviewImages) return;

            //int visibleRowCount = RecipeGrid.vis;
            //int firstDisplayedRowNumber = RecipeGrid.FirstDisplayedCell.RowIndex;

            //DataGridViewRow row;
            //for (int i = firstDisplayedRowNumber; i < (firstDisplayedRowNumber + visibleRowCount); i++)
            for (int i = 0; i < DataSet.SimpleRecipeView.Rows.Count; i++)
            {
                //var row = DataSet.SimpleRecipeView.Rows[i];
                var getImageThread = new Thread(GetRecipeImageAsync);
                getImageThread.Start(i);
            }
        }

        private Mutex ImageConnectorLock = new Mutex();

        private void GetRecipeImageAsync(object rowIndex)
        {
            CookbookDataSet.ImagesDataTable images = null;
            if (!(rowIndex is int rowNum)) return;

            try
            {
                if (CookbookAdapter.ImagesTableAdapter == null) return;
                //if (!(dataRow.Cells["R_ID"] is DataGridViewTextBoxCell idCell)) return;
                int? id = (DataSet.SimpleRecipeView.Rows[rowNum] as CookbookDataSet.SimpleRecipeViewRow)?.R_ID;
                if (id == null) return;

                ImageConnectorLock.WaitOne();
                images = CookbookAdapter.ImagesTableAdapter.GetDataByRecipe(id);
                ImageConnectorLock.ReleaseMutex();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while downloading the preview image:\n\n" + ex.Message,
                    "Error loading image", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Bitmap previewImage;

            if (images.Count > 0)
            {
                if (!images[0].IsIMG_DataNull())
                {
                    using (var imgStream = new MemoryStream(images[0].IMG_Data))
                    {
                        previewImage = new Bitmap(System.Drawing.Image.FromStream(imgStream));
                    }
                }
                else previewImage = null;
            }
            else previewImage = null;

            //if (Dispatcher != null)
            //    Dispatcher.BeginInvoke(new SetPreviewImageDelegate(SetPreviewImage), new object[] { previewImage, rowNum });

        }

        private delegate void SetPreviewImageDelegate(Bitmap image, int rowNum);
        private void SetPreviewImage(Bitmap image, int rowNum)
        {
            //PreviewPanel.BackgroundImage = image;
            //DataGridViewImageCell previewCell = (row.Cells["IMG_Preview"] as DataGridViewImageCell);
            //if (previewCell != null)
            //{
            //    previewCell.Value = image;
            //}

            var imageCellInfo = new DataGridCellInfo(RecipeGrid.Items[rowNum], IMG_Preview);
            if (imageCellInfo == null) return;

            var imageCell = FindGridCell(RecipeGrid, imageCellInfo);
            if (imageCell != null)
            {
                var imageItem = new System.Windows.Controls.Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                };
                imageCell.Content = imageItem;
            }
        }

        static DataGridCell FindGridCell(DataGrid grid, DataGridCellInfo cellInfo)
        {
            DataGridCell result = null;
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = App.GetVisualChild<DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }
            }
            return result;
        }

        // Event Handlers
        //--------------------------------------------------------------------------------------------------------

        private void ImgReload_Click(object sender, RoutedEventArgs e)
        {
            UpdateImages();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static void ShowRecipeDetails(int recipeID)
        {
            var viewRecipeWindow = new ViewRecipeWindow();
            if (viewRecipeWindow.DataContext is RecipeViewModel viewModel)
            {
                viewModel.RecipeID = recipeID;
            }

            viewRecipeWindow.Show();
            viewRecipeWindow.Focus();
        }

        private static void ShowRecipeEditor(int recipeID)
        {
            throw new NotImplementedException();
        }
        
        private void RecipeGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.SelectedGridItem is DetailRecipe row)
            {
                ShowRecipeDetails(row.R_ID);
            }
        }

        private void RecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem item)) return;

            // Recipe Options
            if (ViewModel?.SelectedGridItem is DetailRecipe selectedRow)
            {
                switch (item.Name)
                {
                    case "ViewRecipeMenuItem":
                    case "ViewRecipeContextItem":
                        ShowRecipeDetails(selectedRow.ID);
                        return;

                    case "EditRecipeMenuItem":
                    case "EditRecipeContextItem":
                        ShowRecipeEditor(selectedRow.ID);
                        return;

                    case "DeleteRecipeMenuItem":
                    case "DeleteRecipeContextItem":
                        if (ViewModel != null) ViewModel.DeleteRecipe(selectedRow.ID);
                        return;

                    default:
                        return;
                }
            }
        }

        private void EditCategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var categoriesEditor = new CategoriesView();
            categoriesEditor.ShowDialog();
        }

        private void ShowImagesMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (IMG_Preview != null) IMG_Preview.Visibility = Visibility.Visible;
        }

        private void ShowImagesMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IMG_Preview != null) IMG_Preview.Visibility = Visibility.Collapsed;
        }
    }
}
