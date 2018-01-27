using Newtonsoft.Json;

namespace Nest.Models
{
    public class NestStructureJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("away")]
        public string Away { get; set; }
    }
}
