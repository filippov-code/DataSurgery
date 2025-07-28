using DataSurgery.Core;
using DataSurgery.Core.Enums;
using DataSurgery.Core.Interfaces;
using DataSurgery.Domain;
using DataSurgery.Domain.Enums;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Presentation.WPF;

/// <summary>
/// Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string _containerFileFilter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|WAV Files (*.wav)|*.wav|TIFF Files (*.tiff)|*.tiff";
    private ISurgery _hideSurgery;
    private ISurgery _findSurgery;
    private SurgeryService _service;

    public MainWindow()
    {
        InitializeComponent();

        var availableMethods = Enum.GetValues(typeof(SurgeryMethods));
        hide_surgeryMethodComboBox.ItemsSource = availableMethods;
        hide_surgeryMethodComboBox.SelectedIndex = 0;
        _service = new SurgeryService();
    }

    private void Hide_chooseMessageButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog fileDialog = new OpenFileDialog();
        bool? fileSelected = fileDialog.ShowDialog();
        if (fileSelected == null || !fileSelected.Value)
            return;

        hide_messageFilePathTextBox.Text = fileDialog.FileName;
        CheckMessageAndContainerSizes();
    }

    private void Hide_chooseContainerButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Filter = _containerFileFilter;
        bool? fileSelected = fileDialog.ShowDialog();

        if (fileSelected == null || !fileSelected.Value)
            return;

        hide_containerFilePathTextBox.Text = fileDialog.FileName;
        CheckMessageAndContainerSizes();
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
        try
        {
            var containerPath = hide_containerFilePathTextBox.Text;
            var messagePath = hide_messageFilePathTextBox.Text;
            var savePath = hide_saveFilePathTextBox.Text;
            var degreeValue = int.Parse(hide_degreeValueTextBox.Text);
            var degree = (int)Math.Pow(2, degreeValue);
            var password = hide_passwordTextBox.Text;
            var iv = hide_ivTextBox.Text;
            var surgeyMethod = hide_surgeryMethodComboBox.SelectedIndex;

            var progressHandler = (HideProcessSteps step) =>
            {
                var log = step switch
                {
                    HideProcessSteps.DataCollection => "Сбор данных...",
                    HideProcessSteps.MetatagCreating => "Создание Метатега...",
                    HideProcessSteps.Encryption => "Шифрование...",
                    HideProcessSteps.Hiding => "Прячем сообщение...",
                    HideProcessSteps.Saving => "Сохраняем...",
                    HideProcessSteps.Done => "Готово",
                    _ => null
                };

                if (log is not null)
                    Log(log);
            };

            _service.Hide(
                containerPath,
                messagePath,
                savePath,
                degree,
                password,
                iv,
                (SurgeryMethods)surgeyMethod,
                progressHandler
            );
        }
        catch (Exception ex)
        {
            Log($"Не удалось спрятать: {ex.Message}");
        }
    }

    private void Find_chooseStegocontainerButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Filter = _containerFileFilter;
        bool? fileSelected = fileDialog.ShowDialog();
        if (fileSelected != null && fileSelected.Value)
        {
            find_stegoContainerFilePathTextBox.Text = fileDialog.FileName;
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
        try
        {
            string stegoContainerPath = find_stegoContainerFilePathTextBox.Text;
            string savePath = find_saveFilePathTextBox.Text;
            string password = find_passwordTextBox.Text;
            string iv = find_ivTextBox.Text;

            var processHandler = (FindProcessSteps step) =>
            {
                var log = step switch
                {
                    FindProcessSteps.DataCollection => "Сбор данных...",
                    FindProcessSteps.DecryptionPreparing => "Подготовка к расшифровке...",
                    FindProcessSteps.Decryption => "Расшифровка...",
                    FindProcessSteps.MetatagParsing => "Парсинг Метатега...",
                    FindProcessSteps.MessageExtracting => "Получаем сообщение...",
                    FindProcessSteps.Saving => "Сохраняем...",
                    FindProcessSteps.Done => "Готово",
                    _ => null
                };

                if (log is not null)
                    Log(log);
            };

            _service.Find(
                stegoContainerPath,
                savePath,
                password,
                iv,
                processHandler
            );
        }
        catch (Exception ex)
        {
            Log($"Не удалось расшифровать: {ex.Message}");
        }
    }

    #region UIElements
    private bool CheckMessageAndContainerSizes()
    {
        string messagePath = hide_messageFilePathTextBox.Text;
        string containerPath = hide_containerFilePathTextBox.Text;
        long messageSize = 0, freeSpace = 0;

        if (File.Exists(messagePath))
        {
            FileInfo fileInfo = new FileInfo(messagePath);
            messageSize = fileInfo.Length + 16 - fileInfo.Length % 16 + DSMD.Size;
            WriteTextInTextBlock(hide_messagePathInfoTextBlock, $"Суммарный размер: {messageSize} байт", Brushes.Gray);
        }
        if (File.Exists(containerPath))
        {
            _hideSurgery = SurgeryFactory.GetSurgery(containerPath);
            int degree = (int)Math.Pow(2, int.Parse(hide_degreeValueTextBox.Text));
            freeSpace = _hideSurgery.GetFreeSpace(degree);
            WriteTextInTextBlock(hide_containerPathInfoTextBlock, $"Доступное место: {freeSpace} байт", Brushes.Gray);
        }
        if (messageSize > 0 && freeSpace > 0)
        {
            bool enoughtSpace = messageSize <= freeSpace;
            WriteTextInTextBlock(hide_messagePathInfoTextBlock, null, enoughtSpace ? Brushes.Green : Brushes.Red);
            WriteTextInTextBlock(hide_containerPathInfoTextBlock, null, enoughtSpace ? Brushes.Green : Brushes.Red);
            return enoughtSpace;
        }

        return false;
    }

    private void WriteTextInTextBlock(TextBlock textBlock, string text, Brush textColor)
    {
        if (text != null)
        {
            textBlock.Text = text;
        }
        if (textColor != null)
        {
            textBlock.Foreground = textColor;
        }
    }

    private void Hide_degreeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsInitialized)
            return;

        var slider = (Slider)sender;
        slider.SelectionEnd = e.NewValue;
        hide_degreeValueTextBox.Text = Math.Round(e.NewValue).ToString();

        CheckMessageAndContainerSizes();
    }

    public void Log(string log)
    {
        hide_logTextBox.Text += log + "\n";
        find_logTextBox.Text += log + "\n";
    }
    #endregion
}
