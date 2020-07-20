using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiVersioningDemo.Middleware
{
    public interface IVersionMigrationHelper
    {
        public T GetRequestBody<T>();

        public void SetRequestBody(object body);
        
        public void SetRequestBody(string body);
        
        public T GetResponseBody<T>();

        public void SetResponseBody(object body);
        
        public void SetResponseBody(string body);

        public HttpContext GetHttpContext();
    }
}