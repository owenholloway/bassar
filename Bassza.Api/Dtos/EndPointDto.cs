namespace Bassza.Api.Dtos
{
    public class EndPointDto
    {
        public string Function { get; set; } = "";
        public FunctionTypeEnum FunctionType { get; set; }

        public Dictionary<string, string> UrlData = new Dictionary<string, string>();

    }
}