using Newtonsoft.Json;

namespace NestTools.Models
{
    public class NestStructureJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("away")]
        public string Away { get; set; }
    }
}
