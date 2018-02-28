using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for ChooseImageDialog.xaml
    /// </summary>
    public partial class ChooseImageDialog : Window
    {
        private bool MouseLeftButtonDownInCanvas = false;
        private Bitmap baseBitmap = null;

        /// <summary>Final output of this image picker</summary>
        public Bitmap FinalBitmap = null;
        
        private ImageEditorViewModel ViewModel
        {
            get => DataContext as ImageEditorViewModel;
            set => DataContext = value;
        }

        /// <summary>Create a new instance of the class</summary>
        public ChooseImageDialog()
        {
            InitializeComponent();
        }

        /// <summary>Create a new instance with the specified image data</summary>
        /// <param name="imgData"></param>
        public ChooseImageDialog(byte[] imgData) : this()
        {
            ViewModel.PreviewImage = ByteImageConverter.ConvertBytesToBitmapImage(imgData);
            baseBitmap = ByteImageConverter.ConvertBytesToBitmap(imgData);
        }

        private void CroppingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDownInCanvas = true;
            if (ViewModel != null)
            {
                ViewModel.CaptureTopLeftPoint = ClipMouseCoords(e.GetPosition(CroppingCanvas));
                ViewModel.CaptureWidth = 0.0;
            }
        }

        private void CroppingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDownInCanvas = false;
        }

        private void CroppingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseLeftButtonDownInCanvas) return;
            else
            {
                if (ViewModel != null)
                {
                    var mouseLoc = ClipMouseCoords(e.GetPosition(CroppingCanvas));

                    double xDiff = mouseLoc.X - ViewModel.CaptureLeft;
                    double yDiff = mouseLoc.Y - ViewModel.CaptureTop;

                    ViewModel.CaptureWidth = Math.Min(xDiff, yDiff);
                }
            }
        }

        private System.Windows.Point ClipMouseCoords(System.Windows.Point mouseLocation)
        {
            double mouseX = mouseLocation.X;
            double mouseY = mouseLocation.Y;

            if (mouseX < 0) mouseX = 0;
            if (mouseX > CroppingCanvas.ActualWidth) mouseX = CroppingCanvas.ActualWidth;

            if (mouseY < 0) mouseY = 0;
            if (mouseY > CroppingCanvas.ActualHeight) mouseY = CroppingCanvas.ActualHeight;

            return new System.Windows.Point(mouseX, mouseY);
        }

        private void PickImageFileButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Image files|*.jpg;*.png;*.bmp",
                InitialDirectory = Environment.ExpandEnvironmentVariables("%USERPROFILE%"),
                Multiselect = false,
                Title = "Open Image File",
                ValidateNames = true
            };

            bool? result = filePicker.ShowDialog();
            if (result == true)
            {
                // got an image to try to load
                try
                {
                    baseBitmap = new Bitmap(filePicker.FileName);
                    var ms = new MemoryStream();
                    baseBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var newImage = new BitmapImage();
                    newImage.BeginInit();
                    newImage.StreamSource = ms;
                    newImage.EndInit();

                    if (ViewModel != null) ViewModel.PreviewImage = newImage;
                }
                catch (FileNotFoundException ex)
                {
                    App.LogException(ex);
                    MessageBox.Show(
                        "Could not open the requested file.", "Error loading file",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            FinalBitmap = new Bitmap(200, 200);
            Graphics graphics = Graphics.FromImage(FinalBitmap);

            double pixelToDisplayRatio = baseBitmap.Width / CroppingCanvas.ActualWidth;

            float srcX = Convert.ToSingle(Math.Round(ViewModel.CaptureLeft * pixelToDisplayRatio));
            float srcY = Convert.ToSingle(Math.Round(ViewModel.CaptureTop * pixelToDisplayRatio));
            float srcSize = Convert.ToSingle(Math.Round(ViewModel.CaptureWidth * pixelToDisplayRatio));

            var srcRect = new RectangleF(srcX, srcY, srcSize, srcSize);
            var destRect = new RectangleF(0, 0, 200, 200);

            // draw the subset to FinalBitmap
            graphics.DrawImage(baseBitmap, destRect, srcRect, GraphicsUnit.Pixel);
            graphics.Dispose();

            DialogResult = true;
            Close();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CaptureTopLeftPoint = new System.Windows.Point(0, 0);
            ViewModel.CaptureWidth = Math.Min(CroppingCanvas.ActualWidth, CroppingCanvas.ActualHeight);
        }
    }

    /// <summary>
    /// View model for the image editor dialog
    /// </summary>
    public class ImageEditorViewModel : DependencyObject
    {
        /// <summary>Create a new instance of the class</summary>
        public ImageEditorViewModel()
        {

        }

        /// <summary>Preview image to show in editor window</summary>
        public BitmapImage PreviewImage
        {
            get { return (BitmapImage)GetValue(PreviewImageProperty); }
            set { SetValue(PreviewImageProperty, value); }
        }

        /// <summary>Preview image to show in editor window</summary>
        public static readonly DependencyProperty PreviewImageProperty =
            DependencyProperty.Register("PreviewImage", typeof(BitmapImage), typeof(ImageEditorViewModel), new PropertyMetadata(null));


        /// <summary>Left bound for image cropping</summary>
        public double CaptureLeft
        {
            get { return (double)GetValue(CaptureLeftProperty); }
            set { SetValue(CaptureLeftProperty, value); }
        }

        /// <summary>Left bound for image cropping</summary>
        public static readonly DependencyProperty CaptureLeftProperty =
            DependencyProperty.Register("CaptureLeft", typeof(double), typeof(ImageEditorViewModel), new PropertyMetadata(0.0));


        /// <summary>Upper bound for image cropping</summary>
        public double CaptureTop
        {
            get { return (double)GetValue(CaptureTopProperty); }
            set { SetValue(CaptureTopProperty, value); }
        }

        /// <summary>Upper bound for image cropping</summary>
        public static readonly DependencyProperty CaptureTopProperty =
            DependencyProperty.Register("CaptureTop", typeof(double), typeof(ImageEditorViewModel), new PropertyMetadata(0.0));


        /// <summary>Top left point of the image cropping selection</summary>
        public System.Windows.Point CaptureTopLeftPoint
        {
            get => new System.Windows.Point(CaptureLeft, CaptureTop);
            set
            {
                CaptureLeft = value.X;
                CaptureTop = value.Y;
            }
        }


        /// <summary>Width/height of the cropping square</summary>
        public double CaptureWidth
        {
            get { return (double)GetValue(CaptureWidthProperty); }
            set { SetValue(CaptureWidthProperty, value); }
        }

        /// <summary>Width/height of the cropping square</summary>
        public static readonly DependencyProperty CaptureWidthProperty =
            DependencyProperty.Register("CaptureWidth", typeof(double), typeof(ImageEditorViewModel), new PropertyMetadata(0.0));
    }
}
