﻿using Core;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using Core;


//int t = -91;
//Console.WriteLine(Convert.ToString(-91, 2));
//return;

int degree = 1;
JpegSurgery jpeg = new JpegSurgery("jpeg.jpg", degree);
byte[] message = File.ReadAllBytes("message.txt");
byte[] hidden = jpeg.HideWithLSB(message);
//return;
//File.WriteAllBytes("test_dct.jpg", hidden);
JpegSurgery hiddenJpeg = new JpegSurgery("test_dct.jpg", degree);
hiddenJpeg.tmpBeforeSave = jpeg.tmpBeforeSave;
byte[] secret = hiddenJpeg.FindLSB(message.Length * 8);
File.WriteAllBytes("findedMessage.txt", secret);
Console.WriteLine(@"||||| Check |||||");
Console.WriteLine("Equal: " + message.SequenceEqual(secret));
Console.WriteLine("message bits: " + string.Join(" ", message.TakeLast(20)));
Console.WriteLine("message find: " + string.Join(" ", secret.TakeLast(20)));

return;

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
