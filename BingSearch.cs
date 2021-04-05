using System;
using System.IO;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace seo_bot_docker
{
    public static class BingSearch
    {
        [FunctionName("BingSearch")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"BingSearch C# Timer trigger function executed at: {DateTime.Now}");

            string env = Environment.GetEnvironmentVariable("ENVIRONMENT");

            if (string.IsNullOrEmpty(env)) {
                env = "dev";
            }
            
            log.LogInformation($"Using {env} environment");

            string path = "";
            string driver = "";

            if (env.Equals("local")) {
                path = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                driver = "chromedriver.exe";
            }
            else {
                path = "/usr/bin/";
                driver = "chromedriver";
            }

            MyChromeDriver myChromeDriver = new MyChromeDriver(path, driver, log);
            myChromeDriver.Setup();
            myChromeDriver.RunBingSearch();
            myChromeDriver.TearDown();
        }
    }
}
