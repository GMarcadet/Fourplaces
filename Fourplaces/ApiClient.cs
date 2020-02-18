using Common.Api.Dtos;
using Fourplaces.DTO;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TD.Api.Dtos;

namespace Fourplaces
{
	public class ApiClient
	{
        public static int NONE_IMAGE = 0;

		private readonly HttpClient _client = new HttpClient();

		public static string ACCESS_TOKEN = "access_token";
		public static string REFRESH_TOKEN = "refresh_token";

		public async Task<HttpResponseMessage> Execute(HttpMethod method, string url, object data = null, string token = null)
		{
			HttpRequestMessage request = new HttpRequestMessage(method, url);

			if (data != null)
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
			}

			if (token != null)
			{
				request.Headers.Add("Authorization", $"Bearer {token}");
			}

			return await _client.SendAsync(request);
		}

		public async Task<T> ReadFromResponse<T>(HttpResponseMessage response)
		{
			string result = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(result);
		}

        public async Task<ImageItem> PublishMediaFile(MediaFile file)
        {
            Console.WriteLine("Publishing image...");

            // prepare access token
            HttpClient client = new HttpClient();
            byte[] imageData = MediaFileToByteArray(file);

            SessionStorage storage = SessionStorage.GetStorage();
            string accessToken = storage.Get(ApiClient.ACCESS_TOKEN) as string;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, APIResources.buildImagePublicationURI());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
            requestContent.Add(imageContent, "file", "file.jpg");

            request.Content = requestContent;

            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ApiClient apiClient = new ApiClient();
                Response<ImageItem> imageResponse = await apiClient.ReadFromResponse<Response<ImageItem>>(response);
                ImageItem item = imageResponse.Data;

                Console.WriteLine("Image Uploded !");

                return item;
            }
            else
            {

                Console.WriteLine("Publishing image failure: Invalid status code: " + response.StatusCode);
            }
            return new ImageItem();
        }

        /**
         * 
         */ 
        private static byte[] MediaFileToByteArray(MediaFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.GetStream().CopyTo(memoryStream);
                file.Dispose();
                return memoryStream.ToArray();
            }
        }
    }
}
