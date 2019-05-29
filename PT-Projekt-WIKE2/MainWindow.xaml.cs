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
using System.Drawing.Imaging;
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

        Bitmap image, image2;
        string path = Path.Combine(Environment.CurrentDirectory, @"obrazek.bmp");
        int engineMode = 2;
        ElectronicsDictionary electronicsDictionary;
        string ApiKey = "";

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
                if(ApiKey == "")
                {
                    MessageBox.Show("Insert API Key!");
                }
                else
                {
                    TextRecognizer textRecognizer = new TextRecognizer(KeyTextBox.Text);

                    string responseBody = await textRecognizer.RecognizeText(path);

                    ResponseBodyTextBlock.Text = responseBody;
                }                
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
                        image2 = image;
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".png":
                        image = new Bitmap(openFileDialog.FileName);
                        image2 = image;
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        image = new Bitmap(openFileDialog.FileName);
                        image2 = image;
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
                        image2 = image;
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".png":
                        image = new Bitmap(openFileDialog.FileName);
                        image2 = image;
                        LoadedImage.Source = (ImageSource)Convert(image);
                        image.Save("obrazek.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        image = new Bitmap(openFileDialog.FileName);
                        image2 = image;
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

        private void insertAPIKey_Click(object sender, RoutedEventArgs e)
        {
            ComputerVisionAPIKey.IsOpen = true;
        }

        private void saveComputerVisionApiKey_Click(object sender, RoutedEventArgs e)
        {
            ApiKey = KeyTextBox.Text;
            ComputerVisionAPIKey.IsOpen = false;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = ResponseBodyTextBlock.Text.Split(new[] { "\n" }, StringSplitOptions.None);
            List<Tuple<ElectronicElement, int>> list = new List<Tuple<ElectronicElement, int>>();

            foreach (string line in lines)
            {
                if (line.Length > 6)
                {
                    list.Add(electronicsDictionary.FindClosest(line.ToLower()));
                }
            }

            var closestElement = list.OrderBy(element => element.Item2).ToList().FirstOrDefault();
            /*var closestElement = electronicsDictionary.FindClosest(ResponseBodyTextBlock.Text);*/
            TextBlock.Text = closestElement.Item1.Name + "\n" + closestElement.Item1.Info;
        }

        public static Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }

        public static Bitmap MakeGrayscale(Bitmap original)
        {
            //make an empty bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    System.Drawing.Color originalColor = original.GetPixel(i, j);

                    //create the grayscale version of the pixel
                    int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
                        + (originalColor.B * .11));

                    //create the color object
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(grayScale, grayScale, grayScale);

                    //set the new image's pixel to the grayscale version
                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            return newBitmap;
        }

        public Bitmap make_bw(Bitmap original, int threshold)
        {

            Bitmap output = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {

                for (int j = 0; j < original.Height; j++)
                {

                    System.Drawing.Color c = original.GetPixel(i, j);

                    int average = ((c.R + c.B + c.G) / 3);

                    if (average < threshold)
                        output.SetPixel(i, j, System.Drawing.Color.Black);

                    else
                        output.SetPixel(i, j, System.Drawing.Color.White);

                }
            }
            return output;
        }

        private Bitmap RotateImage(Bitmap rotateMe, float angle)
        {
            var bmp = new Bitmap(rotateMe.Width + (rotateMe.Width / 2), rotateMe.Height + (rotateMe.Height / 2));

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImageUnscaled(rotateMe, (rotateMe.Width / 4), (rotateMe.Height / 4), bmp.Width, bmp.Height);

            rotateMe = bmp;

            Bitmap rotatedImage = new Bitmap(rotateMe.Width, rotateMe.Height);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotateMe.Width / 2, rotateMe.Height / 2);   //set the rotation point as the center into the matrix
                g.RotateTransform(angle);                                        //rotate
                g.TranslateTransform(-rotateMe.Width / 2, -rotateMe.Height / 2); //restore rotation point into the matrix
                g.DrawImage(rotateMe, new System.Drawing.Point(0, 0));                          //draw the image on the new bitmap
            }

            return rotatedImage;
        }

        private void adjustContrast_Click(object sender, RoutedEventArgs e)
        {
            image = AdjustContrast(image, Int32.Parse(ContrastTextBox.Text));
            LoadedImage.Source = (ImageSource)Convert(image);
        }

        private void grayscale_Click(object sender, RoutedEventArgs e)
        {
            image = MakeGrayscale(image);
            LoadedImage.Source = (ImageSource)Convert(image);
        }

        private void bw_Click(object sender, RoutedEventArgs e)
        {
            //image = make_bw(image, Int32.Parse(ThresholdTextBox.Text));
            image = make_bw(image, Int32.Parse(ThresholdTextBox.Text));
            LoadedImage.Source = (ImageSource)Convert(image);
        }

        private void rotate_Click(object sender, RoutedEventArgs e)
        {
            image = RotateImage(image, Int32.Parse(AngleTextBox.Text));
            LoadedImage.Source = (ImageSource)Convert(image);
        }

        private void slider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ThresholdTextBox.Text = ((int)ThresholdSlider.Value).ToString();
        }

        private void resetImageButton_Click(object sender, RoutedEventArgs e)
        {
            image = image2;
            LoadedImage.Source = (ImageSource)Convert(image);
        }
    }
}