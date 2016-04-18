using NUnit.Engine;
using System.Xml;
using System;
using NUnit.Engine.Services;
using NUnit.Engine.Runners;
using NUnit.Common;
using System.IO;
using System.Linq;

namespace Guinunit.Utilities
{
    public static class TestManager
    {
        public static TestPackage CreatePackage(string path)
        {
            var package = new TestPackage(path);

            package.AddSetting(PackageSettings.ShadowCopyFiles, true);
            package.AddSetting(PackageSettings.ProcessModel, "InProcess");
            package.AddSetting(PackageSettings.DomainUsage, "Single");
            package.AddSetting(PackageSettings.WorkDirectory, Path.GetDirectoryName(path));

            return package;
        }

        public static ServiceContext CreateServices(string path)
        {
            var services = new ServiceContext();
            services.Add(new SettingsService(true));
            services.Add(new DomainManager());
            services.Add(new ExtensionService());
            services.Add(new DriverService());
            services.Add(new RecentFilesService());
            services.Add(new ProjectService());
            services.Add(new RuntimeFrameworkService());
            services.Add(new DefaultTestRunnerFactory());
            services.Add(new TestAgency(path, 0));
            services.Add(new ResultService());
            services.Add(new TestFilterService());
            return services;
        }

        public static XmlNode LoadModule(string path)
        {
            var services = CreateServices(path);

            using (var serviceManager = services.ServiceManager)
            {
                serviceManager.StartServices();

                XmlNode result;
                using (ITestRunner runner = new MasterTestRunner(services, CreatePackage(path)))
                    result = runner.Explore(TestFilter.Empty);

                serviceManager.StopServices();

                return result;
            }
        }

        public static TestFilter CreateFilter(XmlNode node)
        {
            var filterBuilder = new TestFilterBuilder();

            node.SelectNodes("//test-case/@fullname")
                .OfType<XmlAttribute>()
                .Select(x => x.Value)
                .ToList()
                .ForEach(filterBuilder.AddTest);

            return filterBuilder.GetFilter();
        }
    }
}
