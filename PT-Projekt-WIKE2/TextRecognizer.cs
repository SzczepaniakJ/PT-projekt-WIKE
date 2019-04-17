﻿using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PT_Projekt_WIKE2 {
    public sealed class TextRecognizer {
        public TextRecognizer() {
            SubscriptionKey = "bfe5fa4dc9464c6da966cb9870591222";
            URI = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/";
        }

        private string SubscriptionKey { get; }
        private string URI { get; }

        public async Task<string> RecognizeText(string imageFilePath) {
            return await RequestTextRegognition(imageFilePath);
        }

        private async Task<string> RequestTextRegognition(string imageFilePath) {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

            HttpResponseMessage response;

            byte[] imageData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent byteArrayContent = new ByteArrayContent(imageData)) {
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response = await httpClient.PostAsync(URI + "recognizeText?mode=Printed", byteArrayContent);
            }

            if (response.IsSuccessStatusCode) {
                string operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                string responseStatus;

                while (true) {
                    response = await httpClient.GetAsync(operationLocation);

                    responseStatus = await response.Content.ReadAsStringAsync();

                    if (responseStatus.Contains("\"status\":\"Succeeded\"")) {
                        break;
                    }

                    else {
                        Thread.Sleep(1000);
                    }
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                return JToken.Parse(responseBody).ToString();
            }

            return string.Empty;
        }

        private byte[] GetImageAsByteArray(string imageFilePath) {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read)) {
                BinaryReader binaryReader = new BinaryReader(fileStream);

                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}