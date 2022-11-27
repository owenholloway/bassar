namespace Bassza.Api.Dtos
{
    public class RequestDto
    {
        public EndPointDto EndPointDto;
        public Dictionary<string, string> BodyData = new Dictionary<string, string>();
        public Dictionary<string, string> UrlData = new Dictionary<string, string>();
    }
}