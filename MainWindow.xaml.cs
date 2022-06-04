using FoodAI.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FoodAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() is true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }

        private async void MakePredictionAsync(string fileName)
        {
            string url = "https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/a98eb639-862d-47c6-af0c-fc61e01f2cdb/classify/iterations/Iteration1/image";
            string predictionKey = "a7cab5091b45432e9eb8bb181d0a4131";
            string contentType = "application/octet-stream";
            var file = File.ReadAllBytes(fileName);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
                
                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction>? predictions = JsonConvert.DeserializeObject<CustomVision>(responseString)?.Predictions?.ToList();

                    predictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}
