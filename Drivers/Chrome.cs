using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace seo_bot_docker
{
    public class MyChromeDriver {
        private ChromeDriver _chromeDriver;
        private string _pathToDriver;
        private string _driverApp;
        private ILogger _log;

        private SearchHelper _searchHelper;
        private int _googleResultsPage = 1;
        private int _bingResultsPage = 1;
 
        public MyChromeDriver(string pathToDriver, string driverApp, ILogger log) {
            _pathToDriver = pathToDriver;
            _driverApp = driverApp;
            _log = log;
            _searchHelper = new SearchHelper();
        }

        public void Setup() {
            _log.LogInformation($"Looking for {_driverApp} in path: {_pathToDriver}");

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless");
            chromeOptions.AddArguments("--no-sandbox");
            chromeOptions.AddArguments("--disable-gpu");
            
            var service = ChromeDriverService.CreateDefaultService(_pathToDriver, _driverApp);
            
            try {
                _chromeDriver = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(2));
                _chromeDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            }
            catch (Exception ex) {
                _log.LogError(ex.Message);
            }
        }
        public void RunGoogleSearch(string url) {
            // Navigate to Google
            _log.LogInformation("Opening Google...");
            _chromeDriver.Navigate().GoToUrl(url);
            int timeoutSeconds = 60;

            string query = _searchHelper.GetRandomSearch();
            
            // Create new wait timer and set it to 10 seconds
            var wait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(timeoutSeconds));
            _log.LogInformation("Waiting for search page to load...");
            _log.LogInformation($"Searching for phrase '{query}'");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name("q"))).SendKeys(query);
            
            // Wait until Google Search button is visible
            _log.LogInformation("Waiting for search button to become visible...");
            IWebElement searchButton = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(timeoutSeconds)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Name("q")));
            
            _log.LogInformation("Clicking search button...");
            searchButton.SendKeys(Keys.Enter);

            // Wait until search results stats appear which confirms that the search finished
            _log.LogInformation("Waiting for result stats...");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("result-stats")));

            // Find result stats and assign to variable name resultStats
            var resultStats = _chromeDriver.FindElement(By.Id("result-stats"));

            // Find a search result in the list
            var results = _chromeDriver.FindElements(By.ClassName("g"));
            
            while (!EvaluateResults(results) && _googleResultsPage <= 10) {
                // Get the next page of the results
                IWebElement nextLink = _chromeDriver.FindElement(By.Id("pnnext"));
                _googleResultsPage++;
                
                _log.LogInformation("Clicking through the next page...");
                nextLink.Click();
                
                // Wait until search results stats appear which confirms that the search finished
                _log.LogInformation("Waiting for result stats...");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("result-stats")));

                // Find a search result in the list
                results = _chromeDriver.FindElements(By.ClassName("g"));
            }
        }

        public void RunBingSearch(string url) {
            // Navigate to Google
            _log.LogInformation("Opening Bing...");
            _chromeDriver.Navigate().GoToUrl(url);
            int timeoutSeconds = 60;

            string query = _searchHelper.GetRandomSearch();
            
            // Create new wait timer and set it to 10 seconds
            var wait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(timeoutSeconds));
            _log.LogInformation("Waiting for search page to load...");
            _log.LogInformation($"Searching for phrase '{query}'");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("sb_form_q"))).SendKeys(query);
            
            // Wait until Search button is visible
            _log.LogInformation("Waiting for search button to become visible...");
            //IWebElement searchButton = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(timeoutSeconds)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("sb_form_go")));
            IWebElement searchButton = _chromeDriver.FindElement(By.Id("sb_form_q"));
            
            _log.LogInformation("Clicking search button...");
            searchButton.SendKeys(Keys.Enter);

            // Wait until search results stats appear which confirms that the search finished
            _log.LogInformation("Waiting for result stats...");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("sb_count")));

            // Find a search result in the list
            var results = _chromeDriver.FindElements(By.Id("b_results"));

            while (!EvaluateResults(results) && _bingResultsPage <= 10) {
                // Get the next page of the results
                IWebElement nextLink = _chromeDriver.FindElement(By.CssSelector("#b_results > li.b_pag > nav > ul > li:last-child"));
                _log.LogInformation("Clicking through the next page...");
                nextLink.Click();
                _bingResultsPage++;

                // Wait until search results stats appear which confirms that the search finished
                _log.LogInformation("Waiting for result stats...");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("sb_count")));

                // Find a search result in the list
                results = _chromeDriver.FindElements(By.Id("b_results"));
            }

        }

        private bool EvaluateResults(System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> results) {
            _log.LogInformation("Printing the result links...");
            var foundIt = false;
            foreach (var result in results) {
                if (foundIt) { break; }
                _log.LogInformation(result.Text);
                string url = result.FindElement(By.TagName("a")).GetAttribute("href");
                _log.LogInformation(url);
                if (!string.IsNullOrEmpty(url) && url.Contains(_searchHelper.GetTargetDomain())) {
                    _log.LogInformation("Following link to desired site...");
                    var myPage = result.FindElement(By.TagName("a"));
                    myPage.Click();
                    foundIt = true;
                    _log.LogInformation("Finished navigating to site...");
                    return true;
                }
            }
            return false;
        }

        public void TearDown() {
            _chromeDriver.Quit();
        }
    }
}