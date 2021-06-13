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
    }
}