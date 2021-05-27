using System.Threading.Tasks;

namespace AdaRewardsReporter.Core.Interfaces
{
    public interface ICardanoBlockchainClient
    {
        Task<T> QueryAsync<T>(string resourceUri);
    }
}