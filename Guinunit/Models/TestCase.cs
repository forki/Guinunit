using MyToolkit.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

namespace Guinunit.Models
{
    public class TestCase : ObservableObject
    {
        private TestCaseStatus status;
        private string message;
        private bool isExpanded;

        public TestCaseStatus Status
        {
            get { return status; }
            set { Set(ref status, value); }
        }

        public string Message
        {
            get { return message; }
            set { Set(ref message, value); }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { Set(ref isExpanded, value); }
        }

        public string Name { get; }
        public ObservableCollection<TestCase> TestCases { get; }

        public TestCase(XmlNode node, IDictionary<string, TestCase> testCaseLookup)
        {
            testCaseLookup.Add(node.Attributes.GetNamedItem("fullname").Value, this);

            var testSuites = node.SelectNodes("./test-suite")
                                 .OfType<XmlNode>()
                                 .Select(x => new TestCase(x, testCaseLookup))
                                 .ToList();

            var testCases = node.SelectNodes("./test-case")
                                .OfType<XmlNode>()
                                .Select(x => new TestCase(x, testCaseLookup))
                                .ToList();

            TestCases = new ObservableCollection<TestCase>(testSuites.Concat(testCases));

            isExpanded = testSuites.Any();

            Name = node.Attributes.GetNamedItem("name").Value;
        }
    }
}
