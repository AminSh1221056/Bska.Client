
namespace Bska.Client.UI.Helper
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Json;
    using Newtonsoft.Json;
    using System.IO;
    using System.Xml;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Bska.Client.Domain.Entity;
    using System.Net;
    using System.Net.Security;
    using System.Threading;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using Bska.Client.Repository.Model;

    public class WebAPIHelper
    {
        HttpClient client = UserLog.UniqueInstance.Client;

        public async Task<CustomerModel> GetCustomerAsync(string path)
        {
            CustomerModel customer = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadAsAsync<CustomerModel>();
            }
            return customer;
        }

        public T GetTItems<T>(string url)
        {
            T data =default(T);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var task = client.SendAsync(request)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;

                    var jsonTask = response.Content.ReadAsAsync<T>();
                    jsonTask.Wait();
                    data = jsonTask.Result;
                });
            try
            {
                task.Wait();
                return data;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        
        public async Task<XmlDocument> GetXmlFormatAsync(string url)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            var doc = new XmlDocument();
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                var s = await responseContent.ReadAsStreamAsync();
                doc.Load(s);
            }
            return doc;
        }
    }

    public class EchoClientHandler : HttpMessageHandler
    {
        private string _acceptRange;
        private string _content;
        public EchoClientHandler(string acceptReange, string content)
        {
            this._acceptRange = acceptReange;
            this._content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                var response = new HttpResponseMessage();
                return response;
            });
        }
    }

    public class JsonContent : HttpContent
    {
        private readonly JToken _value;

        public JsonContent(JToken value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream,
            TransportContext context)
        {
            var jw = new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            _value.WriteTo(jw);
            jw.Flush();
            return Task.FromResult<object>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
