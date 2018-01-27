
using Newtonsoft.Json;

namespace NestTools
{
    public class EndUserAuthentication
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
