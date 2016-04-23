using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guinunit.Models;
using NUnit.Engine;
using NUnit.Engine.Internal;
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

            // ReSharper disable once FunctionNeverReturns
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

        public void Cancel()
        {
            SendMessage(new CancelMessage());
        }

        private void StopRun()
        {
            target.Duration = DateTime.Now - state.StartTime;

            state.Dispose();
            state = null;

            target.IsRunningTests = false;
        }

        private void StartRun(TestModule testModule)
        {
            var testListener = new TestEventListener(this, testModule);

            var services = TestManager.CreateServices(testModule.Path);
            using (var serviceManager = services.ServiceManager)
            {
                serviceManager.StartServices();
                using (var runner = new MasterTestRunner(services, TestManager.CreatePackage(testModule.Path)))
                {
                    var r = runner;
                    var sm = serviceManager;

                    var registration = state.Cancellation.Token.Register(() =>
                    {
                        r.StopRun(true);
                        sm.StopServices();
                        r.Dispose();
                        sm.Dispose();
                        foreach (var testCase in testModule.TestCaseMap.Values.Where(x => x.Status == TestCaseStatus.Running))
                            testCase.Status = TestCaseStatus.Cancelled;
                    });

                    using (registration)
                    {
                        runner.Run(testListener, testModule.Filter);
                        SendMessage(new FinishMessage(testModule));
                        serviceManager.StopServices();
                    }
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
                Cancellation.Dispose();
            }
        }

        private abstract class RunnerMessage
        {
            public abstract void Handle(TestRunnerAgent agent);
        }

        private class StartMessage : RunnerMessage
        {
            private readonly IList<TestModule> testModules;

            public StartMessage(IEnumerable<TestModule> testModules)
            {
                this.testModules = testModules.ToList();
            }

            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state != null || testModules.Count < 1)
                    return;

                agent.state = new RunningState(testModules);
                agent.target.Duration = TimeSpan.Zero;
                agent.target.PassedCount = 0;
                agent.target.FailedCount = 0;
                agent.target.SkippedCount = 0;

                foreach (var testModule in testModules)
                {
                    foreach (var testCase in testModule.TestCaseMap.Values)
                        testCase.Status = TestCaseStatus.NotRun;

                    Task.Run(() => agent.StartRun(testModule), agent.state.Cancellation.Token);
                }

                agent.target.IsRunningTests = true;
            }
        }

        private class UpdateMessage : RunnerMessage
        {
            private readonly TestModule testModule;
            private readonly string report;

            public UpdateMessage(TestModule testModule, string report)
            {
                this.testModule = testModule;
                this.report = report;
            }

            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state != null)
                    agent.target.Duration = DateTime.Now - agent.state.StartTime;

                var node = XmlHelper.CreateXmlNode(report);

                var testName = node.Attributes?.GetNamedItem("fullname")?.Value;
                if (string.IsNullOrWhiteSpace(testName))
                    return;

                TestCase testCase;
                if (!testModule.TestCaseMap.TryGetValue(testName, out testCase))
                    return;

                var increaseCounter = 0;

                switch (node.Name)
                {
                    case "start-suite":
                    case "start-test":
                        testCase.Status = TestCaseStatus.Running;
                        return;

                    case "test-case":
                        increaseCounter = 1;
                        break;

                    case "test-suite":
                        break;

                    default:
                        return;
                }

                var result = node.Attributes.GetNamedItem("result").Value;

                switch (result)
                {
                    case "Skipped":
                        testCase.Status = TestCaseStatus.Skipped;
                        testCase.Message = node.SelectSingleNode("./reason/message")?.InnerText ?? "";
                        agent.target.SkippedCount += increaseCounter;
                        break;

                    case "Passed":
                        testCase.Status = TestCaseStatus.Passed;
                        agent.target.PassedCount += increaseCounter;
                        break;

                    case "Failed":
                        testCase.Status = TestCaseStatus.Failed;
                        testCase.Message = node.SelectSingleNode("./failure/message")?.InnerText ?? "";
                        agent.target.FailedCount += increaseCounter;
                        break;

                    default:
                        throw new Exception($"Unhandled test result: {result}.");
                }
            }
        }

        private class FinishMessage : RunnerMessage
        {
            private readonly TestModule testModule;

            public FinishMessage(TestModule testModule)
            {
                this.testModule = testModule;
            }

            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state == null)
                    return;

                agent.state.TestModules.Remove(testModule);

                if (agent.state.TestModules.Count < 1)
                    agent.StopRun();
            }
        }

        private class CancelMessage : RunnerMessage
        {
            public override void Handle(TestRunnerAgent agent)
            {
                if (agent.state == null)
                    return;

                agent.state.Cancellation.Cancel();
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
                agent.SendMessage(new UpdateMessage(testModule, report));
            }
        }
    }
}