using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Nest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Nest
{
    public class NestClient
    {
        private readonly HttpClient httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
        private readonly ITestOutputHelper logger;

        public NestClient(string accessToken, ITestOutputHelper logger)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.logger = logger;
        }

        public byte[] GetSnapshotJpg(NestCameraJson cameraJson)
        {
            return httpClient.GetByteArrayAsync(cameraJson.SnapshotUrl).Result;
        }

        public List<NestCameraJson> GetCameras()
        {
            var summary = GetNestSummary();
            JToken devices = summary["devices"];
            var cameras = devices["cameras"].Values<JProperty>();

            List<NestCameraJson> camerasParsed = new List<NestCameraJson>();

            foreach (var camera in cameras)
            {
                var cameraJson = camera.Value.ToString();
                camerasParsed.Add(JsonConvert.DeserializeObject<NestCameraJson>(cameraJson));
            }

            return camerasParsed;
        }

        public List<NestStructureJson> GetStructures()
        {
            var summary = GetNestSummary();

            var structures = summary["structures"].Values<JProperty>();

            List<NestStructureJson> camerasParsed = new List<NestStructureJson>();

            foreach (var structure in structures)
            {
                camerasParsed.Add(JsonConvert.DeserializeObject<NestStructureJson>(structure.Value.ToString()));
            }

            return camerasParsed;
        }
        
        /// <remarks>
        /// HttpClient will not send default headers on a redirect for security reasons.
        /// Nest API requires following redirects.
        /// </remarks>
        public JObject GetNestSummary()
        {
            string nestSummaryResponse;

            HttpResponseMessage response = httpClient
                .GetAsync("https://developer-api.nest.com")
                .Result;

            if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
            {
                var redirectedTextResponse = httpClient.GetStringAsync(response.Headers.Location).Result;
                nestSummaryResponse = redirectedTextResponse;
            }
            else
            {
                nestSummaryResponse = response.Content.ReadAsStringAsync().Result;
            }

            return GetNestSummary(nestSummaryResponse);
        }

        public JObject GetNestSummary(string nestSummaryResponse)
        {
            logger.WriteLine("Parsing: " + nestSummaryResponse);
            return JObject.Parse(nestSummaryResponse);
        }

    }
}
