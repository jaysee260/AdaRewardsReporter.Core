using Newtonsoft.Json;

namespace AdaRewardsReporter.Core.Models
{
    public class CardanoAddress
    {
        [JsonProperty("stake_address")]
        public string StakeAddress { get; set; }
        public string Type { get; set; }
    }
}