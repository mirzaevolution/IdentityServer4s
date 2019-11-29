using Newtonsoft.Json;

namespace MGR.MainAppWebForms.Models
{
    public class HelloResponse
    {
        [JsonProperty]
        public string Message { get; set; }
    }
}