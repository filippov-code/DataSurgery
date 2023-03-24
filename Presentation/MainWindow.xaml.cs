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
using System.Runtime.Intrinsics.Arm;
using Encryption;
using System.Security.Cryptography;
using Core.Interfaces;
using Core.Audio;
using Core.BitmapImage;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string containerFileFilter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|WAV Files (*.wav)|*.wav|TIFF Files (*.tiff)|*.tiff";
        private SurgeryBase? surgery;

        public MainWindow()
        {
            InitializeComponent();

            var availableMethods = Enum.GetValues(typeof(SurgeryMethods));
            hide_surgeryMethodComboBox.ItemsSource = availableMethods;
            hide_surgeryMethodComboBox.SelectedIndex = 0;

            //DSMD dSMD = new DSMD("wavert", 1, 2, 1024, new byte[] { 1, 2});
            //Log(dSMD.BBlockSize.ToString());


        }

        private void Hide_chooseContainerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = containerFileFilter;
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


            string containerPath = hide_containerFilePathTextBox.Text;
            string containerExtension = Path.GetExtension(containerPath);
            string messagePath = hide_messageFilePathTextBox.Text;
            string messageExtension = Path.GetExtension(messagePath);
            string savePath = hide_saveFilePathTextBox.Text;
            int degree = int.Parse(hide_degreeValueTextBox.Text);
            string password = hide_passwordTextBox.Text;
            string iv = hide_ivTextBox.Text;
            byte[] containerBytes = File.ReadAllBytes(containerPath);
            byte[] messageBytes = File.ReadAllBytes(messagePath);
            AES aes = new AES(password, iv);
            FPE fpe = new FPE(aes);
            byte[] messageEncrypted = aes.Encrypt(messageBytes);
            var dsmd = new DSMD(
                messageExtension,
                hide_surgeryMethodComboBox.SelectedIndex,
                degree,
                messageEncrypted.Length,
                MD5.HashData(messageBytes)
            );
            byte[] dsmdABlock = dsmd.GetABlock();
            Log($"hide dsmd dec: {string.Join(" ", dsmdABlock)}");
            byte[] dsmdEncrypted = fpe.PrefixEncrypt(dsmdABlock);
            Log($"hide dsmd enc: {string.Join(" ", dsmdEncrypted)}");
            byte[] extensionEncrypted = fpe.PrefixEncrypt(Encoding.UTF8.GetBytes(messageExtension));
            ISurgery surgery = SurgeryFactory.GetSurgery(containerExtension, containerPath, degree);
            surgery.AddWithLSB(dsmdEncrypted, 1);
            byte[] toWrite = extensionEncrypted.Concat(messageEncrypted).ToArray();
            surgery.AddWithLSB(toWrite, degree);
            surgery.Save(savePath + containerExtension);
            Log("Done");


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
            string stegoContainerPath = find_stegoContainerFilePathTextBox.Text;
            string stegoContainerExtension = Path.GetExtension(stegoContainerPath);
            string savePath = find_saveFilePathTextBox.Text;
            string password = find_passwordTextBox.Text;
            string iv = find_ivTextBox.Text;
            byte[] stegoContainerBytes = File.ReadAllBytes(stegoContainerPath);
            AES aes = new AES(password, iv);
            FPE fpe = new FPE(aes);
            ISurgery surgery = SurgeryFactory.GetSurgery(stegoContainerExtension, stegoContainerPath, 1);
            var dsmdEncrypted = surgery.FindLSB(9, 1, 0);
            Log($"find dsmd enc: {string.Join(" ", dsmdEncrypted)}");
            var dsmdDecrypted = fpe.PrefixDecrypt(dsmdEncrypted);
            Log($"find dsmd dec: {string.Join(" ", dsmdDecrypted)}");
            var dsmd = new DSMD(dsmdDecrypted);
            Log($"degree: {dsmd.Degree}");
            Log($"extension length: {dsmd.ExtensionSize}");
            Log($"method: {dsmd.SurgeryMethod}");
            Log($"message size: {dsmd.MessageSize}");
            string extension = DSMD.Encoding.GetString(surgery.FindLSB(dsmd.ExtensionSize, dsmd.Degree, 9 * 8));
            Log($"extension: {extension}");
            byte[] encryptedMessage = surgery.FindLSB(dsmd.MessageSize, dsmd.Degree, 9*8 + dsmd.ExtensionSize * 8 / dsmd.Degree);
            
                byte[] message = aes.Decrypt(encryptedMessage);

                //if (MD5.HashData(message).TakeLast(2).SequenceEqual(dsmd.HashTail))
                //    Log(i.ToString());

                File.WriteAllBytes(savePath + "." + extension, message);
            }
            catch (Exception ex)
            {
                Log($"Не удалось расшифровать: {ex.Message}");
                throw;
            }
        }

        #region UIElements
        private void Hide_degreeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            slider.SelectionEnd = e.NewValue;
            if (IsInitialized)
                hide_degreeValueTextBox.Text = Math.Round(e.NewValue).ToString();
        }

        public void Log(string log)
        {
            hide_logTextBox.Text += log + "\n";
            find_logTextBox.Text += log + "\n";
        }
        #endregion

    }
}
