using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecipeBox3
{
    public partial class SplashDialog : Window
    {
        /// <summary>Status message to be shown</summary>
        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        /// <summary>Property store for <see cref='StatusText'/></summary>
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(SplashDialog), new PropertyMetadata("Initializing..."));


        /// <summary>Whether to show the progress bar</summary>
        public bool ShowProgressBar
        {
            get { return (bool)GetValue(ShowProgressBarProperty); }
            set { SetValue(ShowProgressBarProperty, value); }
        }

        /// <summary>Property store for <see cref='ShowProgressBar'/></summary>
        public static readonly DependencyProperty ShowProgressBarProperty =
            DependencyProperty.Register("ShowProgressBar", typeof(bool), typeof(SplashDialog), new PropertyMetadata(false));


        /// <summary>Percent complete to show on progress bar</summary>
        public int ProgressPercent
        {
            get { return (int)GetValue(ProgressPercentProperty); }
            set { SetValue(ProgressPercentProperty, value); }
        }

        /// <summary>Property store for <see cref='ProgressPercent'/></summary>
        public static readonly DependencyProperty ProgressPercentProperty =
            DependencyProperty.Register("ProgressPercent", typeof(int), typeof(SplashDialog), new PropertyMetadata(0));


        /// <summary>Create a new instance of the class</summary>
        public SplashDialog()
        {
            InitializeComponent();
            VersionLabel.Content = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
