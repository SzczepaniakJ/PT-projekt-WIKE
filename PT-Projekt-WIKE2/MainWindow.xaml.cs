using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tesseract;

namespace PT_Projekt_WIKE2 {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        Bitmap image;
        string path = Path.Combine(Environment.CurrentDirectory, @"obrazek.bmp");

        public object Convert(object value)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            if(TesseractCheckBox.IsChecked == true)
            {
                var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractAndCube);
                var page = ocr.Process(image);
                ResponseBodyTextBlock.Text = page.GetText();
            }
            else
            {
                TextRecognizer textRecognizer = new TextRecognizer(KeyTextBox.Text);
                
                string responseBody = await textRecognizer.RecognizeText(path);

                ResponseBodyTextBlock.Text = responseBody;
            }            
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wszystkie pliki (.)|*.*|BMP (.bmp)|*.bmp|PNG (.png)|*.png|JPG (.jpg)|*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                switch (System.IO.Path.GetExtension(openFileDialog.FileName))
                {
                    case ".bmp":
                        image = new Bitmap(openFileDialog.FileName);
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".png":
                        image = new Bitmap(openFileDialog.FileName);
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        image = new Bitmap(openFileDialog.FileName);
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        
                        break;
                    default:
                        image = null;
                        MessageBox.Show("Zle rozszerzenie pliku");
                        break;
                }
            }
        }
    }
}