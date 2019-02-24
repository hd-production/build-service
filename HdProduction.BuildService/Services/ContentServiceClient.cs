using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HdProduction.BuildService.Services
{
    public interface IContentServiceClient
    {
        Task<string> UploadFileAsync(string path);
        string GetDownloadLink(string fileKey);
    }

    public class ContentServiceClient : IContentServiceClient
    {
        private static readonly HttpClient HttpClient;

        static ContentServiceClient()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("access-key", "25666AB4CE0D3512E1C1FD1BA044C460E4814F66A9ABFCD4A81FAF7C2BE9A9C9");
        }

        private readonly string _contentServiceUrl;

        public ContentServiceClient(string contentServiceUrl)
        {
            _contentServiceUrl = contentServiceUrl;
        }

        public async Task<string> UploadFileAsync(string path)
        {
            var fileName = Path.GetFileName(path);
            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StreamContent(File.OpenRead(path)), "file", fileName);
                using (var response = await HttpClient.PostAsync(_contentServiceUrl, content))
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            return fileName;
        }

        public string GetDownloadLink(string fileKey)
        {
            return Path.Combine(_contentServiceUrl, fileKey);
        }
    }
}