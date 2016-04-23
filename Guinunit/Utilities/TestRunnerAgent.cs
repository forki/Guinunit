using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guinunit.Models;
using NUnit.Engine;
using NUnit.Engine.Runners;

namespace Guinunit.Utilities
{
    public class TestRunnerAgent : IDisposable
    {
        private readonly ConcurrentQueue<RunnerMessage> messageQueue = new ConcurrentQueue<RunnerMessage>();
        private readonly ITestRunnerTarget target;

        private SemaphoreSlim semaphore = new SemaphoreSlim(0, int.MaxValue);
        private RunningState state;

        public TestRunnerAgent(ITestRunnerTarget target)
        {
            this.target = target;
            Task.Factory.StartNew(ProcessMessages, TaskCreationOptions.LongRunning);
        }

        private async void ProcessMessages()
        {
            while (true)
            {
                await semaphore.WaitAsync();

                RunnerMessage message;
                if (!messageQueue.TryDequeue(out message))
                    continue;

                message.Handle(this);
            }
        }

        public void Dispose()
        {
            semaphore?.Dispose();
            semaphore = null;

            state?.Dispose();
            state = null;
        }

        private void SendMessage(RunnerMessage message)
        {
            messageQueue.Enqueue(message);
            semaphore.Release();
        }

        public void StartTests(IList<TestModule> testModules)
        {
            SendMessage(new StartMessage(testModules));
        }

        public void Update(TestModule testModule, string report)
        {
            SendMessage(new UpdateMessage(testModule, report));
        }

        private void Finish(TestModule testModule)
        {
            SendMessage(new FinishMessage(testModule));
        }

        public void Cancel()
        {
            SendMessage(new CancelMessage());
        }

        private void StopRun()
        {
            state.Dispose();
            state = null;
            target.IsRunningTests = false;
        }

        private async Task StartRun(TestModule testModule)
        {
            var testListener = new TestEventListener(this, testModule);

            var services = TestManager.CreateServices(testModule.Path);
            using (var serviceManager = services.ServiceManager)
            {
                serviceManager.StartServices();
                using (var runner = new MasterTestRunner(services, TestManager.CreatePackage(testModule.Path)))
                {
                    state.Cancellation.Token.Register(() =>
                    {
                        runner.StopRun(true);
                        serviceManager.StopServices();
                        runner.Dispose();
                        serviceManager.Dispose();
                        foreach (var testCase in testModule.TestCaseMap.Values.Where(x => x.Status == TestCaseStatus.Running))
                            testCase.Status = TestCaseStatus.Cancelled;
                    });

                    runner.Run(testListener, testModule.Filter);
                    Finish(testModule);
                    serviceManager.StopServices();
                }
            }
        }

        private class RunningState : IDisposable
        {
            public CancellationTokenSource Cancellation { get; } = new CancellationTokenSource();

            public IList<TestModule> TestModules { get; }

            public DateTime StartTime { get; } = DateTime.Now;

            public RunningState(IList<TestModule> testModules)
            {
                TestModules = testModules;
            }

            public void Dispose()
            {
                Cancellation.Cancel();
                Cancellation.Dispose();
            }
        }

        private abstract class RunnerMessage
        {
            public abstract void Handle(TestRunnerAgent agent);
        }

        private class StartMessage : RunnerMessage
        {
            public IList<TestModule> TestModules { get; }

            public StartMessage(IList<TestModule> testModules)
            {
                TestModules = testModules;
            }

            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state != null || TestModules.Count < 1)
                    return;

                agent.state = new RunningState(TestModules);
                agent.target.Duration = TimeSpan.Zero;
                agent.target.PassedCount = 0;
                agent.target.FailedCount = 0;
                agent.target.SkippedCount = 0;

                foreach (var testModule in TestModules)
                {
                    foreach (var testCase in testModule.TestCaseMap.Values)
                        testCase.Status = TestCaseStatus.NotRun;

                    Task.Run(async () => await agent.StartRun(testModule), agent.state.Cancellation.Token);
                }

                agent.target.IsRunningTests = true;
            }
        }

        private class UpdateMessage : RunnerMessage
        {
            public TestModule TestModule { get; }
            public string Report { get; }

            public UpdateMessage(TestModule testModule, string report)
            {
                TestModule = testModule;
                Report = report;
            }

            public override void Handle(TestRunnerAgent agent)
            {

            }
        }

        private class FinishMessage : RunnerMessage
        {
            public TestModule TestModule { get; }

            public FinishMessage(TestModule testModule)
            {
                TestModule = testModule;
            }

            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state == null)
                    return;

                agent.state.TestModules.Remove(TestModule);

                if (agent.state.TestModules.Count < 1)
                    agent.StopRun();
            }
        }

        private class CancelMessage : RunnerMessage
        {
            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state != null)
                    agent.StopRun();
            }
        }

        private class TestEventListener : ITestEventListener
        {
            private readonly TestModule testModule;
            private readonly TestRunnerAgent agent;

            public TestEventListener(TestRunnerAgent agent, TestModule testModule)
            {
                this.agent = agent;
                this.testModule = testModule;
            }

            public void OnTestEvent(string report)
            {

            }
        }
    }
}