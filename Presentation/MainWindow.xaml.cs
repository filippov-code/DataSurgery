using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using static Core.SurgeryBase;
using Core;
using Encryption;
using System.Security.Cryptography;
using Core.Interfaces;
using System.Windows.Media;
using Core.BitmapImage;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string containerFileFilter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|WAV Files (*.wav)|*.wav|TIFF Files (*.tiff)|*.tiff";
        private ISurgery hideSurgery;
        private ISurgery findSurgery;

        public MainWindow()
        {
            InitializeComponent();

            var availableMethods = Enum.GetValues(typeof(SurgeryMethods));
            hide_surgeryMethodComboBox.ItemsSource = availableMethods;
            hide_surgeryMethodComboBox.SelectedIndex = 0;



        }
        static int SetBit(int value, int index, bool bitValue)
        {
            int result = value;
            int mask = 1 << index;
            if (bitValue)
                result |= mask;
            else
                result &= ~mask;
            return result;
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
            fileDialog.Filter = containerFileFilter;
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
            //byte[] bytes1 = new byte[] { 1, 2, 3, 4, 5, 6 };
            //byte[] bytes2 = new byte[] { 7, 8, 9, 10, 11, 12 };
            //WavSurgery sur = new WavSurgery(@"F:\Programs\Microsoft Visual Studio\Projects\DataSurgery\bin\Debug\net6.0\wav.wav", 1);
            //sur.AddWithLSB(bytes1, 2);
            //sur.AddWithLSB(bytes2, 1);
            //Hide_Log("hide1: " + string.Join(" ", bytes1));
            //Hide_Log("hide2: " + string.Join(" ", bytes2));
            //byte[] rest1 = sur.FindLSB(6, 2, 0);
            //byte[] rest2 = sur.FindLSB(6, 1, 24);
            //Hide_Log("rest1: " + string.Join(" ", rest1));
            //Hide_Log("rest2: " + string.Join(" ", rest2));
            //return;

            //int[] con = { 1, 2, 3, 4, 5, 6, 7, 8 };
            //Log(string.Join(" ", con));
            //byte[] mes = { 1, 2 };
            //Log(string.Join(" ", mes));
            //BmpSurgery sur = new(@"F:\Programs\Microsoft Visual Studio\Projects\DataSurgery\bin\Debug\net6.0\bmp.bmp");
            //sur.BytesForChange = con;
            //sur.AddWithLSB(mes, 4);
            //Log(string.Join(" ", sur.BytesForChange));
            //Log(string.Join(" ", sur.FindLSB(2, 4, 0)));
            //return;

            Log("Сбор данных...");
            string containerPath = hide_containerFilePathTextBox.Text;
            string containerExtension = Path.GetExtension(containerPath);
            string messagePath = hide_messageFilePathTextBox.Text;
            string messageExtension = Path.GetExtension(messagePath);
            string savePath = hide_saveFilePathTextBox.Text;
            int degreeValue =  int.Parse(hide_degreeValueTextBox.Text);
            int degree = (int)Math.Pow(2, degreeValue);
            string password = hide_passwordTextBox.Text;
            string iv = hide_ivTextBox.Text;
            //byte[] containerBytes = File.ReadAllBytes(containerPath);
            byte[] messageBytes = File.ReadAllBytes(messagePath);
            Log("Создание Метатега...");
            Log("Подготовка к шифрованию...");
            AES aes = new AES(password, iv);
            FPE fpe = new FPE(aes);
            byte[] messageEncrypted = aes.Encrypt(messageBytes);
            //Log($"Encrypted message first: {string.Join(" ", messageEncrypted.Take(6).ToArray())}");
            //Log($"Encrypted message length: {messageEncrypted.Length}");
            //Log($"Encrypted message last: {string.Join(" ", messageEncrypted.TakeLast(6).ToArray())}");
            var dsmd = new DSMD(
                messageExtension,
                hide_surgeryMethodComboBox.SelectedIndex,
                degree,
                messageEncrypted.Length,
                MD5.HashData(messageBytes)
            );
            byte[] dsmdABlock = dsmd.GetABlock();
            Log("Шифрование...");
            //Log($"hide dsmd dec: {string.Join(" ", dsmdABlock)}");
            byte[] dsmdEncrypted = fpe.PrefixEncrypt(dsmdABlock);
            //Log($"hide dsmd enc: {string.Join(" ", dsmdEncrypted)}");
            byte[] extensionEncrypted = fpe.PrefixEncrypt(Encoding.UTF8.GetBytes(messageExtension));
            ISurgery surgery = SurgeryFactory.GetSurgery(containerPath);
            surgery.AddWithLSB(dsmdEncrypted, 1);
            Log("Прячем сообщение...");
            byte[] toWrite = extensionEncrypted.Concat(messageEncrypted).ToArray();
            surgery.AddWithLSB(toWrite, degree);
            Log("Сохраняем...");
            surgery.Save(savePath + containerExtension);
            Log("Готово");


            //Log($"Container Length: {containerBytes.Length}");
            //Log($"Message Length: {messageBytes.Length}");
            //AES aes = new AES(password, iv);
            //FPE fpe = new FPE(aes);
            //byte[] sizeHash = MD5.HashData(messageBytes);
            //DSMD dsmd = new DSMD(4, 1, degree, messageBytes.Length, sizeHash.TakeLast(2).ToArray());
            //byte[] dsmdBytes = dsmd.ToBytes();
            //Log($"DSMD Length: {dsmdBytes.Length}");
            //byte[] dsmdFPE = fpe.PrefixEncrypt(dsmdBytes);
            //Log($"DSMD FPE Length: {dsmdFPE.Length}");
            //byte[] messageAES = aes.Encrypt(messageBytes);
            //Log($"Message AES Length: {messageAES.Length}");
            //byte[] readyMessage = dsmdBytes.Concat(messageAES).ToArray();
            //Log($"Ready message Length: {readyMessage.Length}");
            //string containerExtension = Path.GetExtension(containerPath);
            //ISurgery surgery = SurgeryFactory.GetSurgery(containerExtension, containerPath, degree);
            //Log($"{surgery.GetType()} space: {surgery.GetFreeSpace()}");
            //byte[] secret = surgery.HideWithLSB(readyMessage);
            //Log($"Secret Length: {secret.Length}");
            //File.WriteAllBytes(savePath + containerExtension, secret);
            //Log("Done");
        }

        private void Find_chooseStegocontainerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = containerFileFilter;
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
                Log("Сбор данных...");
                string stegoContainerPath = find_stegoContainerFilePathTextBox.Text;
                string savePath = find_saveFilePathTextBox.Text;
                string password = find_passwordTextBox.Text;
                string iv = find_ivTextBox.Text;
                byte[] stegoContainerBytes = File.ReadAllBytes(stegoContainerPath);
                Log("Полготовка к расшифровке...");
                AES aes = new AES(password, iv);
                FPE fpe = new FPE(aes);
                ISurgery surgery = SurgeryFactory.GetSurgery(stegoContainerPath);
                var dsmdEncrypted = surgery.FindLSB(DSMD.Size, 1, 0);

                Log("Расшифровка...");
                var dsmdDecrypted = fpe.PrefixDecrypt(dsmdEncrypted);
                //Log($"find dsmd dec: {string.Join(" ", dsmdDecrypted)}");
                //Log($"find dsmd enc: {string.Join(" ", dsmdEncrypted)}");
                Log("Парсинг Метатега...");
                var dsmd = new DSMD(dsmdDecrypted);
                //Log($"Degree: {dsmd.Degree}");
                //Log($"Extension length: {dsmd.ExtensionSize}");
                //Log($"Method: {dsmd.SurgeryMethod}");
                //Log($"Message size: {dsmd.MessageSize}");
                byte[] encryptedExtension = surgery.FindLSB(dsmd.ExtensionSize, dsmd.Degree, DSMD.Size * 8);
                byte[] decryptedExtension = fpe.PrefixDecrypt(encryptedExtension);
                string extension = DSMD.Encoding.GetString(decryptedExtension);
                //Log($"Extension: {extension}");
                Log("Получаем сообщение...");
                byte[] encryptedMessage = surgery.FindLSB(dsmd.MessageSize, dsmd.Degree, DSMD.Size * 8 + dsmd.ExtensionSize * 8 / dsmd.Degree);
                //Log($"Encrypted message first: {string.Join(" ", encryptedMessage.Take(6).ToArray())}");
                //Log($"Encrypted message length: {encryptedMessage.Length}");
                //Log($"Encrypted message last: {string.Join(" ", encryptedMessage.TakeLast(6).ToArray())}");
                byte[] message = aes.Decrypt(encryptedMessage);

                //if (MD5.HashData(message).TakeLast(2).SequenceEqual(dsmd.HashTail))
                //    Log(i.ToString());

                Log("Сохраняем...");
                File.WriteAllBytes(savePath + "." + extension, message);
                Log("Готово...");
            }
            catch (Exception ex)
            {
                Log($"Не удалось расшифровать: {ex.Message}");
                //throw;
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
                hideSurgery = SurgeryFactory.GetSurgery(containerPath);
                int degree = (int)Math.Pow(2, int.Parse(hide_degreeValueTextBox.Text));
                freeSpace = hideSurgery.GetFreeSpace(degree);
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

            Slider slider = (Slider)sender;
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
}
