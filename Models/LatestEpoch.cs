using Newtonsoft.Json;

namespace AdaRewardsReporter.Core.Models
{
    public class LatestEpoch
    {
        public int Epoch { get; set; }
        [JsonProperty("start_time")]
        public int StartTime { get; set; }
        [JsonProperty("end_time")]
        public int EndTime { get; set; }
        [JsonProperty("first_block_time")]
        public int FirstBlockTime { get; set; }
        [JsonProperty("last_block_time")]
        public int LastBlockTime { get; set; }
        [JsonProperty("block_count")]
        public int BlockCount { get; set; }
        [JsonProperty("tx_count")]
        public int TxCount { get; set; }
        public string Output { get; set; }
        public string Fees { get; set; }
        [JsonProperty("active_stake")]
        public string ActiveStake { get; set; }
    }
}