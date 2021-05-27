using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaRewardsReporter.Core.Interfaces;
using AdaRewardsReporter.Core.Models;

namespace AdaRewardsReporter.Core
{
    public class RewardsReporter
    {
        private readonly ICardanoBlockchainClient _blockchainClient;

        public RewardsReporter(ICardanoBlockchainClient blockchainClient)
        {
            _blockchainClient = blockchainClient ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<RewardsPerEpochSummary>> GenerateReportAsync(string stakeAddress, OrderBy orderBy)
        {
            var rewardsHistory = await GetRewardsHistoryAsync(stakeAddress, orderBy); 
            var epochs = await GetEpochsDetailsAsync(rewardsHistory.Select(x => x.Epoch), orderBy);
            var stakePoolForEachEpoch = await GetMapOfStakePoolForEachEpochAsync(rewardsHistory);

            var rewardsSummary = ProduceRewardsPerEpochSummary(rewardsHistory.ToList(), epochs.ToList(), stakePoolForEachEpoch);
            return rewardsSummary;
        }

        private async Task<IEnumerable<RewardPerEpoch>> GetRewardsHistoryAsync(string stakeAddress, OrderBy orderBy)
        {
            var stakeRewardsUri = string.Format("accounts/{0}/rewards?order={1}", stakeAddress, orderBy.ToString().ToLower());
            var rewardsHistory = await _blockchainClient.QueryAsync<List<RewardPerEpoch>>(stakeRewardsUri);
            return rewardsHistory;
        }

        private async Task<IEnumerable<CardanoEpoch>> GetEpochsDetailsAsync(IEnumerable<int> epochNumbers, OrderBy orderBy)
        {
            var epochsTasks = epochNumbers.Select(e => _blockchainClient.QueryAsync<CardanoEpoch>($"epochs/{e}?orderBy={orderBy.ToString().ToLower()}"));
            var epochsTasksResolved = await Task.WhenAll(epochsTasks);
            return epochsTasksResolved;
        }

        private async Task<Dictionary<int, StakePool>> GetMapOfStakePoolForEachEpochAsync(IEnumerable<RewardPerEpoch> rewardsHistory)
        {
            var poolIdForEachEpoch = new Dictionary<int, string>();
            rewardsHistory.ToList().ForEach(x =>  poolIdForEachEpoch.Add(x.Epoch, x.PoolId));
            var uniquePoolIds = rewardsHistory.Select(x => x.PoolId).Distinct();

            List<StakePool> stakePools = new List<StakePool>();
            foreach (var poolId in uniquePoolIds)
            {
                var stakePool = await _blockchainClient.QueryAsync<StakePool>($"pools/{poolId}/metadata");
                stakePool.PoolId = poolId;
                stakePools.Add(stakePool);
            }
            
            var stakePoolForEachEpoch = new Dictionary<int, StakePool>();
            foreach (var set in poolIdForEachEpoch)
            {
                var stakePool = stakePools.Find(x => x.PoolId.Equals(set.Value));
                stakePoolForEachEpoch.Add(set.Key, stakePool);
            }

            return stakePoolForEachEpoch;
        }

        private IEnumerable<RewardsPerEpochSummary> ProduceRewardsPerEpochSummary(
            List<RewardPerEpoch> rewardsHistory,
            List<CardanoEpoch> epochs,
            Dictionary<int, StakePool> stakePoolForEachEpoch
        )
        {
            var length = rewardsHistory.Count() == epochs.Count()
                ? rewardsHistory.Count()
                : throw new Exception();

            List<RewardsPerEpochSummary> rewardsSummary = new List<RewardsPerEpochSummary>();
            for (int i = 0; i < length; i++)
            {
                var rewardSummary = new RewardsPerEpochSummary
                {
                    Epoch = epochs[i].Epoch,
                    Amount = ConvertLovelaceToAda(rewardsHistory[i].Amount),
                    RewardsReceivedOn = ConvertUnixTimestampToRewardsReceivedDate(epochs[i].EndTime)
                };
                rewardsSummary.Add(rewardSummary);
            }

            foreach (var summary in rewardsSummary)
            {
                var stakePool = stakePoolForEachEpoch[summary.Epoch];
                var stakePoolDescription = $"{stakePool.Name} [{stakePool.Ticker}]";
                summary.StakePool = stakePoolDescription;
            }

            return rewardsSummary;
        }

        private decimal ConvertLovelaceToAda(string amount)
        {
            var lovelaceToAdaRate = 0.000001M;
            var amountInLovelace = Int32.Parse(amount);
            var amountInAda = (decimal)(amountInLovelace * lovelaceToAdaRate);
            return amountInAda;
        }

        private string ConvertUnixTimestampToRewardsReceivedDate(int epochEndTime)
        {
            // Rewards for a given epoch X are paid out at the end of the next epoch, X+1
            var daysInOneEpoch = 5;
            var epochEndDate = DateTimeOffset.FromUnixTimeSeconds(epochEndTime);
            var nextEpochEndDate = epochEndDate.AddDays(daysInOneEpoch);
            var shortDateString = nextEpochEndDate.DateTime.ToShortDateString();
            return shortDateString;
        }
    }
}