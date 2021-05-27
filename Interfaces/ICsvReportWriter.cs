using System.Collections.Generic;

namespace AdaRewardsReporter.Core.Interfaces
{
    public interface ICsvReportWriter
    {
        void WriteReport<T>(IEnumerable<T> records);
    }
}