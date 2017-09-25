using System;
using Newtonsoft.Json;

namespace Nest
{
    class NestCamera
    {
        [JsonProperty("is_online")]
        public bool IsOnline { get; set; }

        [JsonProperty("is_streaming")]
        public bool IsStreaming { get; set; }

        [JsonProperty("last_is_online_change")]
        public DateTime LastIsOnlineChange { get; set; }

        [JsonProperty("snapshot_url")]
        public string SnapshotUrl { get; set; }
    }
}
