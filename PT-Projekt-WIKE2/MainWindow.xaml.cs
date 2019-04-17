using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PT_Projekt_WIKE2 {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            TextRecognizer textRecognizer = new TextRecognizer();

            string responseBody = await textRecognizer.RecognizeText(@"C:\Users\Bartosz\Desktop\epyc.jpg");

            ResponseBodyTextBlock.Text = responseBody;
        }
    }
}