using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using static Core.SurgeryBase;
using Core;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SurgeryBase? surgery;

        public MainWindow()
        {
            InitializeComponent();

            var availableMethods = Enum.GetValues(typeof(SurgeryMethods));
            hide_surgeryMethodComboBox.ItemsSource = availableMethods;
            hide_surgeryMethodComboBox.SelectedIndex = 0;
        }

        private void Hide_chooseContainerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? fileSelected = fileDialog.ShowDialog();
            if (fileSelected == null || !fileSelected.Value)
                return;
            hide_containerFilePathTextBox.Text = fileDialog.FileName;
            string extension = Path.GetExtension(fileDialog.FileName);
            //surgery = Helper.GetSurgery(extension, fileDialog.FileName, (int)hide_degreeSlider.Value);
        }

        private void Hide_chooseMessageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? fileSelected = fileDialog.ShowDialog();
            if (fileSelected != null && fileSelected.Value)
            {
                hide_messageFilePathTextBox.Text = fileDialog.FileName;
            }
        }

        private void Hide_chooseSavePathButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            bool? fileSelected = fileDialog.ShowDialog();
            if (fileSelected != null && fileSelected.Value)
            {
                hide_saveFilePathTextBox.Text = fileDialog.FileName;
            }
        }

        public void Hide_excecuteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Find_chooseStegocontainerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? fileSelected = fileDialog.ShowDialog();
            if (fileSelected != null && fileSelected.Value)
            {
                find_stegocontainerFilePathTextBox.Text = fileDialog.FileName;
            }
        }

        private void Find_chooseSavePathButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            bool? fileSelected = fileDialog.ShowDialog();
            if (fileSelected != null && fileSelected.Value)
            {
                find_saveFilePathTextBox.Text = fileDialog.FileName;
            }
        }
        public void Find_excecuteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        #region UIElements
        private void Hide_degreeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            slider.SelectionEnd = e.NewValue;
            if (IsInitialized)
                hide_degreeValueTextBox.Text = Math.Round(e.NewValue).ToString();
        }

        #endregion

    }
}
