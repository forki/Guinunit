using NUnit.Engine;
using System.Collections.Generic;
using System.IO;

namespace Guinunit.Models
{
    public class TestModule
    {
        public string Path { get; }
        public TestFilter Filter { get; }
        public IDictionary<string, TestCase> TestCaseMap { get; }
        public FileSystemWatcher Watcher { get; }

        public TestModule(string path, TestFilter filter, IDictionary<string, TestCase> testCaseMap, FileSystemWatcher watcher)
        {
            Path = path;
            Filter = filter;
            TestCaseMap = testCaseMap;
            Watcher = watcher;
        }
    }
}
