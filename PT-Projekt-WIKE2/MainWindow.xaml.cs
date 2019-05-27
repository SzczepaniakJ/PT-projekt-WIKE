using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Linq;
using Tesseract;
using PT_Projekt_WIKE2.Electronics;

namespace PT_Projekt_WIKE2 {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Thread electronicsLoader = new Thread(() =>
            {
                string json = File.ReadAllText(@"Data/electronics.json");
                electronicsDictionary = new ElectronicsDictionary(json);
            });

            electronicsLoader.Start();
            electronicsLoader.Join();
        }

        Bitmap image;
        string path = Path.Combine(Environment.CurrentDirectory, @"obrazek.bmp");
        int engineMode = 2;
        ElectronicsDictionary electronicsDictionary;

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
            if(TesseractRadioButton.IsChecked == true)
            {
                var ocr = new TesseractEngine("./tessdata", "eng", engineMode.ToString());
                var page = ocr.Process(image);
                ResponseBodyTextBlock.Text = page.GetText();
            }
            else if(MicrosoftRadioButton.IsChecked == true)
            {
                TextRecognizer textRecognizer = new TextRecognizer(KeyTextBox.Text);
                
                string responseBody = await textRecognizer.RecognizeText(path);

                ResponseBodyTextBlock.Text = responseBody;
            }            
        }

        private void menuLoadImage_Click(object sender, RoutedEventArgs e)
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

        private void TesseractSelectMode_Click(object sender, RoutedEventArgs e)
        {
            TesseractModePopup.IsOpen = true;
        }

        private void SaveTesseractMode_Click(object sender, RoutedEventArgs e)
        {
            TesseractModePopup.IsOpen = false;
        }

        private void TesseractAndCube_Click(object sender, RoutedEventArgs e)
        {
            Tesseract_Only.IsChecked = false;
            Cube_Only.IsChecked = false;
            engineMode = 2;
        }

        private void TesseractOnly_Click(object sender, RoutedEventArgs e)
        {
            Tesseract_and_Cube.IsChecked = false;
            Cube_Only.IsChecked = false;
            engineMode = 0;
        }

        private void CubeOnly_Click(object sender, RoutedEventArgs e)
        {
            Tesseract_Only.IsChecked = false;
            Tesseract_and_Cube.IsChecked = false;
            engineMode = 1;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            /*string[] lines = ResponseBodyTextBlock.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            List<Tuple<ElectronicElement, int>> list = new List<Tuple<ElectronicElement, int>>();

            foreach (string line in lines)
            {
                if (line.Length > 6)
                {
                    list.Add(electronicsDictionary.FindClosest(ResponseBodyTextBlock.Text));
                }
            }

            var closestElement = list.OrderBy(element => element.Item2).ToList().FirstOrDefault(); */
            var closestElement = electronicsDictionary.FindClosest(ResponseBodyTextBlock.Text);
            TextBlock.Text = closestElement.Item1.Name + "\n" + closestElement.Item1.Info;
        }

    }
}