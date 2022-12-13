using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Bassza.Api.Features
{
    public static class OlemsController
    {

        private static CookieContainer CookieContainer { get; set; } = new CookieContainer();
        
        public static HttpResponseMessage? RunRequest(this RequestDto requestDto)
        {
            switch (requestDto.EndPointDto.FunctionType)
            {
                case FunctionTypeEnum.POST:
                    return RunRequestPost(requestDto).Result;
                
                case FunctionTypeEnum.GET:
                    return RunRequestGet(requestDto).Result;
                    break;
                
                default:
                    return null;
                    break;
            }
            
        }

        public static void AddCookie(string cookie)
        {
            cookie = cookie.Trim();
            var cookieSplit = cookie.Split("=");
            if (cookieSplit.Length != 2) return;

            if (cookieSplit[0].Equals("path")) return;
            
            var cookieObj = new Cookie(cookieSplit[0], cookieSplit[1],"/", Endpoints.BaseUrl);
            
            CookieContainer.Add(new Uri($"https://{Endpoints.BaseUrl}"), cookieObj);
            
        }

        private static async Task<HttpResponseMessage> RunRequestPost(RequestDto requestDto)
        {
            var request = BuildRequest(requestDto);
            var client = CommonRequestSetup(requestDto);
        
            var bodySb = new StringBuilder();

            var formData = requestDto.BodyData.ToList();
            var postData = new FormUrlEncodedContent(formData);

            return await client.PostAsync(request.ToString(), postData);
        }
        
        private static async Task<HttpResponseMessage> RunRequestGet(RequestDto requestDto)
        {
            var request = BuildRequest(requestDto);
            
            return await CommonRequestSetup(requestDto).GetAsync(request.ToString());
        }

        private static HttpClient CommonRequestSetup(RequestDto requestDto)
        {
            //var request = WebRequest.Create(uriStringBuilder.ToString());
            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };

            handler.AllowAutoRedirect = false;
            
            var client = new HttpClient(handler);

            client.Timeout = TimeSpan.FromMinutes(5);
            client.BaseAddress = new Uri($"https://{Endpoints.BaseUrl}");
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/avif", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/signed-exchange", 0.8));

            return client;
        }

        private static UriBuilder BuildRequest(RequestDto requestDto)
        {
            var uriStringBuilder = new UriBuilder(Endpoints.BaseUrl);
            uriStringBuilder.Path = requestDto.EndPointDto.Function;
            uriStringBuilder.Scheme = "https";
            uriStringBuilder.Port = -1;

            
            if (requestDto.UrlData.Count < 1 && requestDto.EndPointDto.UrlData.Count < 1) return uriStringBuilder;

            var query = HttpUtility.ParseQueryString(uriStringBuilder.Query);
            
            foreach (var data in requestDto.UrlData)
            {
                query[data.Key] = data.Value;
                //HttpUtility.UrlEncode()
            }

            foreach (var data in requestDto.EndPointDto.UrlData)
            {
                query[data.Key] = data.Value;
            }

            uriStringBuilder.Query = query.ToString();

            return uriStringBuilder;
        }
    }
}