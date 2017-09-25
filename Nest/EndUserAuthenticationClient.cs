using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Nest
{
    class EndUserAuthenticationClient
    {
        /// <summary>
        /// 1. Send end-users to your authentication URL provided when signing up for developer program.
        /// 2. When the end-user accepts your integration the end-user will receive a pin.
        /// 3. The end-user needs to provide the here to grant access to their devices through the access token.
        /// </summary>
        /// <remarks>
        /// An access token is currently good for 10 years and should be stored persistently for all future requests.
        /// A pin can only be used to grant a single access token.
        /// A new pin can be retrieved to grant a new access token.
        /// Prior access tokens are still valid.
        /// All access tokens will appear on the nest app and can be de-authorized by the end-user.
        /// </remarks>
        public EndUserAuthentication CreateAuthTokenFromPin(string productId, string productSecret, string customerPin)
        {
            string responseBody;
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", productId),
                    new KeyValuePair<string, string>("client_secret", productSecret),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", customerPin)
                });
                HttpResponseMessage result = client.PostAsync("https://api.home.nest.com/oauth2/access_token",content).Result;
                responseBody = result.Content.ReadAsStringAsync().Result;
            }

            var endUserAuthentication = JsonConvert.DeserializeObject<EndUserAuthentication>(responseBody);

            return endUserAuthentication;
        }
    }
}
