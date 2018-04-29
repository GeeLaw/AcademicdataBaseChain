using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace ABCClient
{
    static class Client
    {
        public static readonly Entity BoE = Entity.Entities[0];

        public class Payload
        {
            [JsonProperty("func")]
            public string Function { get; set; }
            [JsonProperty("params")]
            public List<string> Parameters { get; set; }
            [JsonProperty("account")]
            public string Account { get; set; }
        }

        public class Result
        {
            [JsonProperty("success")]
            public bool Success { get; set; }
            [JsonProperty("payloads")]
            public List<string> Payloads { get; set; }
        }

        public static string ChainName { get { throw new NotImplementedException(); } }
        public static string ContractName { get { throw new NotImplementedException(); } }
        public static string ApiKey { get { throw new NotImplementedException(); } }
        public static string InvokeUri { get { return string.Format("https://baas.ink.plus/public-api/call/{0}/{1}/invoke?apikey={2}", ChainName, ContractName, ApiKey); } }
        public static string QueryUri{ get { return string.Format("https://baas.ink.plus/public-api/call/{0}/{1}/query?apikey={2}", ChainName, ContractName, ApiKey); } }

        public static async Task<Result> CallInvokeAsync(Payload payload)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(InvokeUri, content);
                return JsonConvert.DeserializeObject<Result>(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task<Result> CallQueryAsync(Payload payload)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(QueryUri, content);
                return JsonConvert.DeserializeObject<Result>(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task<bool> MarkAsSchoolAsync(string accountAddress, string asAccount)
        {
            return (await CallInvokeAsync(new Payload
            {
                Function = "markAsSchool",
                Parameters = new List<string> { accountAddress },
                Account = asAccount
            })).Success;
        }

        public static async Task<bool> UnmarkAsSchoolAsync(string accountAddress, string asAccount)
        {
            return (await CallInvokeAsync(new Payload
            {
                Function = "unmarkAsSchool",
                Parameters = new List<string> { accountAddress },
                Account = asAccount
            })).Success;
        }

        public static async Task<bool> SetTranscriptAsync(string transcriptId, string transcriptEncrytped, string asAccount)
        {
            return (await CallInvokeAsync(new Payload
            {
                Function = "setTranscript",
                Parameters = new List<string> { transcriptId, transcriptEncrytped },
                Account = asAccount
            })).Success;
        }

        public static async Task<string> QueryTranscriptAsync(string schoolId, string transcriptId)
        {
            var result = await CallQueryAsync(new Payload
            {
                Function = "queryTranscript",
                Parameters = new List<string> { schoolId, transcriptId },
            });
            return result.Success ? result.Payloads[0] : null;
        }
    }
}
