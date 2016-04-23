using System;

namespace Guinunit.Utilities
{
    public interface ITestRunnerTarget
    {
        TimeSpan Duration { get; set; }
        int PassedCount { get; set; }
        int FailedCount { get; set; }
        int SkippedCount { get; set; }
        bool IsRunningTests { get; set; }
    }
}