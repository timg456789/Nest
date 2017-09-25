using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nest
{
    class NestClient
    {
        private readonly HttpClient httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

        public NestClient(string accessToken)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public byte[] GetSnapshotJpg(NestCamera camera)
        {
            return httpClient.GetByteArrayAsync(camera.SnapshotUrl).Result;
        }

        public List<NestCamera> GetCameras()
        {
            var summary = GetNestSummary();
            JToken devices = summary["devices"];
            var cameras = devices["cameras"].Values<JProperty>();

            List<NestCamera> camerasParsed = new List<NestCamera>();

            foreach (var camera in cameras)
            {
                var cameraJson = camera.Value.ToString();
                camerasParsed.Add(JsonConvert.DeserializeObject<NestCamera>(cameraJson));
            }

            return camerasParsed;
        }

        /// <remarks>
        /// HttpClient will not send default headers on a redirect for security reasons.
        /// Nest API requires following redirects.
        /// </remarks>
        public JObject GetNestSummary()
        {
            JObject nestSummary;

            HttpResponseMessage response = httpClient
                .GetAsync("https://developer-api.nest.com")
                .Result;

            if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
            {
                var redirectedTextResponse = httpClient.GetStringAsync(response.Headers.Location).Result;
                nestSummary = JObject.Parse(redirectedTextResponse);
            }
            else
            {
                nestSummary = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            }

            return nestSummary;
        }
    }
}
