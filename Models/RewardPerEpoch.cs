using Newtonsoft.Json;

namespace AdaRewardsReporter.Core.Models
{
    public class RewardPerEpoch
    {
        public int Epoch { get; set; }
        public string Amount { get; set; }
        [JsonProperty("pool_id")]
        public string PoolId { get; set; }
    }
}