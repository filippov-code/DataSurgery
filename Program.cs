using Core;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

//Проект в черновом варианте

int degree = 1;
TiffSurgery png = new TiffSurgery("tiff.tiff", degree);
byte[] message = File.ReadAllBytes("message.txt");
byte[] hidden = png.HideWithLSB(message);
File.WriteAllBytes("hidden.tiff", hidden);
TiffSurgery hiddenTiff = new TiffSurgery("hidden.tiff", degree);
byte[] secret = hiddenTiff.FindLSB(message.Length * 8);
File.WriteAllBytes("secret.txt", secret);
Console.WriteLine(@"\\\\\ Check /////");
Console.WriteLine("Equal: " + message.SequenceEqual(secret));
Console.WriteLine("message bits: " + string.Join(" ", message.Take(10)));
Console.WriteLine("message find: " + string.Join(" ", secret.Take(10)));

return;

//int[] array = new int[10];
//int[] elements = new int[] { 1, 2, 3, 4, 5, 6 };
//var t = new TestSurgery();
//t.ReplaceElementsInArray(array, elements, 4);
//Console.WriteLine(string.Join(" ", array));

//TestSurgery surgery = new TestSurgery();
//byte[] container = new byte[2];
//byte[] message = Encoding.UTF8.GetBytes("H");
//Console.WriteLine("Contain before: " + string.Join(" ", container));
//Console.WriteLine("Message before: " + string.Join(" ", message));
//surgery.WriteMessageLSB(container, message, 4);
//message = surgery.ReadMessageLSB(container, message.Length, 4);
//Console.WriteLine("Contain after: " + string.Join(" ", container));
//Console.WriteLine("Message after: " + string.Join(" ", message));
//Console.WriteLine("Message: " + Encoding.UTF8.GetString(message));

//int degree = 2;
//GifSurgery gifSurgery = new("gif.gif", degree);
//byte[] message = File.ReadAllBytes("message.txt");
//var secretgif = gifSurgery.HideWithLSB(message);
//File.WriteAllBytes("secret.gif", secretgif);
//gifSurgery = new GifSurgery("secret.gif", degree);
//byte[] secret = gifSurgery.FindLSB(message.Length * 8);
//File.WriteAllBytes("secret.txt", secret);
//return;

//int degree = 1;
//JpegSurgery jpeg = new JpegSurgery("jpeg.jpg", degree);
//byte[] message = File.ReadAllBytes("message.txt");
//byte[] hidden = jpeg.HideWithLSB(message);
////return;
////File.WriteAllBytes("test_dct.jpg", hidden);
//JpegSurgery hiddenJpeg = new JpegSurgery("test_dct.jpg", degree);
//hiddenJpeg.tmpBeforeSave = jpeg.tmpBeforeSave;
//byte[] secret = hiddenJpeg.FindLSB(message.Length * 8);
//File.WriteAllBytes("findedMessage.txt", secret);
//Console.WriteLine(@"||||| Check |||||");
//Console.WriteLine("Equal: " + message.SequenceEqual(secret));
//Console.WriteLine("message bits: " + string.Join(" ", message.TakeLast(20)));
//Console.WriteLine("message find: " + string.Join(" ", secret.TakeLast(20)));

//return;

//int degree = 3;
//PngSurgery png = new PngSurgery("png.png", degree);
//byte[] message = File.ReadAllBytes("message.txt");
//byte[] hidden = png.HideWithLSB(message);
//File.WriteAllBytes("hidden.png", hidden);
//PngSurgery hiddenPng = new PngSurgery("hidden.png", degree);
//byte[] secret = hiddenPng.FindLSB(message.Length * 8);
//File.WriteAllBytes("findedMessage.txt", secret);
//Console.WriteLine(@"\\\\\ Check /////");
//Console.WriteLine("Equal: " + message.SequenceEqual(secret));
//Console.WriteLine("message bits: " + string.Join(" ", message.Take(10)));
//Console.WriteLine("message find: " + string.Join(" ", secret.Take(10)));

//return;


byte[] messageBytes = File.ReadAllBytes("message.txt");
byte[] fileBytes = File.ReadAllBytes("sample.bmp");
int offset = BitConverter.ToInt32(fileBytes[10..14]);
BitArray messageBits = new BitArray(messageBytes);

for(int i = 0; i < messageBits.Length; i++)
{
    fileBytes[offset + i] &= 0b11111110;
    fileBytes[offset + i] |= (byte)(messageBits[i] ? 1 : 0);
}

File.WriteAllBytes("ENCRYPTED.bmp", fileBytes);

bool[] secretValues = new bool[messageBits.Length];
for(int i = 0; i < messageBits.Length; i++)
{
    secretValues[i] = fileBytes[offset + i] % 2 == 1 ? true : false;
}

BitArray secretBits = new BitArray(secretValues);
byte[] secretBytes = new byte[secretBits.Length / 8];
secretBits.CopyTo(secretBytes, 0);
File.WriteAllBytes("secret.txt", secretBytes);

