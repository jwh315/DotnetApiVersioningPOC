using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ApiVersioningDemo.Middleware
{
    public class VersionMigrationHelper : IVersionMigrationHelper
    {
        private readonly HttpContext _httpContext;

        private string _requestBody;

        private string _responseBody;
        
        public VersionMigrationHelper(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public T GetRequestBody<T>()
        {
            var body = JsonConvert.DeserializeObject<T>(_requestBody);

            return body;
        }

        public void SetRequestBody(object body)
        {
            var json = JsonConvert.SerializeObject(body);

            _requestBody = json;
        }
        
        public void SetRequestBody(string body)
        {
            _requestBody = body;
        }

        public T GetResponseBody<T>()
        {
            var body = JsonConvert.DeserializeObject<T>(_responseBody);

            return body;
        }

        public void SetResponseBody(object body)
        {
            var json = JsonConvert.SerializeObject(body);

            _responseBody = json;
        }
        
        public void SetResponseBody(string body)
        {
            _responseBody = body;
        }

        public HttpContext GetHttpContext()
        {
            return _httpContext;
        }
    }
}