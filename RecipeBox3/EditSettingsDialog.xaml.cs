using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for EditSettingsDialog.xaml
    /// </summary>
    public partial class EditSettingsDialog : Window
    {
        /// <summary>Dictionary of values for unit system selector</summary>
        public static Dictionary<Unit.System, string> UnitSystemDict =
            Enum.GetValues(typeof(Unit.System))
            .Cast<Unit.System>()
            .ToDictionary(p => p, p => p.GetString());

        private EditSettingsViewModel ViewModel
        {
            get => DataContext as EditSettingsViewModel;
            set => DataContext = value;
        }

        /// <summary>Create a new instance of the class</summary>
        public EditSettingsDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.SaveSettings();
            Close();
        }
    }

    /// <summary>View model for settings editor</summary>
    public class EditSettingsViewModel : DependencyObject
    {
        /// <summary></summary>
        public bool ShowPreviewImages
        {
            get { return (bool)GetValue(ShowPreviewImagesProperty); }
            set { SetValue(ShowPreviewImagesProperty, value); }
        }

        /// <summary>Property store for <see cref='ShowPreviewImages'/></summary>
        public static readonly DependencyProperty ShowPreviewImagesProperty =
            DependencyProperty.Register("ShowPreviewImages", typeof(bool), typeof(EditSettingsViewModel), new PropertyMetadata(true));


        /// <summary></summary>
        public Unit.System SelectedUnitSystem
        {
            get { return (Unit.System)GetValue(SelectedUnitSystemProperty); }
            set { SetValue(SelectedUnitSystemProperty, value); }
        }

        /// <summary>Property store for <see cref='SelectedUnitSystem'/></summary>
        public static readonly DependencyProperty SelectedUnitSystemProperty =
            DependencyProperty.Register("SelectedUnitSystem", typeof(Unit.System), typeof(EditSettingsViewModel), new PropertyMetadata(Unit.System.Any));


        /// <summary>Create a new instance of the view model</summary>
        public EditSettingsViewModel()
        {
            ShowPreviewImages = ((App)Application.Current).ShowPreviewImages;
            SelectedUnitSystem = ((App)Application.Current).UnitSystem;
        }

        /// <summary>Save the settings to the config file</summary>
        public void SaveSettings()
        {
            ((App)Application.Current).ShowPreviewImages = ShowPreviewImages;
            ((App)Application.Current).UnitSystem = SelectedUnitSystem;
            Properties.Settings.Default.Save();
        }
    }
}
