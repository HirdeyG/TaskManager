using MBIA.AutomatedTesting.Framework;
using MBIA.AutomatedTesting.Framework.Enums;
using MBIA.AutomatedTesting.Framework.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager
{
    [AutomatedTestCase]
    public class TaskManagerTestCases : MBIA.AutomatedTesting.Framework.IAutomatedTestCases
    {
        /// <summary>
        /// Main entry point function to exectue test cases
        /// </summary>
        /// <param name="driverType"></param>
        /// <param name="resultsScreenShotPath"></param>
        /// <returns></returns>
        public TestCaseResultSummary ExecuteTestCase(DriverType driverType, string resultsScreenShotPath, string applicationURL, bool downloadFiles)
        {
            TestCaseResultSummary retVal = new TestCaseResultSummary();
            retVal.TestCaseResults = new List<TestCaseResult>();

            TestCaseResult launchedResult = new TestCaseResult();
            IWebDriver iWebDriver = null;
            try
            {
                iWebDriver = DriverHelper.CreateInstance(driverType);
                if (iWebDriver != null)
                {
                    WebDriverWait wait = new WebDriverWait(iWebDriver, TimeSpan.FromSeconds(120));

                    iWebDriver.Navigate().GoToUrl(applicationURL);
                    iWebDriver.Manage().Window.Maximize();
                    Thread.Sleep(8000);
                    Screenshot ss1 = ((ITakesScreenshot)iWebDriver).GetScreenshot();
                    ss1.SaveAsFile(resultsScreenShotPath + "\\1 - AppLaunch.jpg", OpenQA.Selenium.ScreenshotImageFormat.Jpeg);

                    var appLaunch = Application_Launch(iWebDriver);
                    retVal.TestCaseResults.Add(appLaunch);

                    if (appLaunch.Result == false)
                    {
                        iWebDriver.Close();
                        iWebDriver.Dispose();
                        return retVal;
                    }
                    var output1 = ManageTasks_SearchData(iWebDriver, resultsScreenShotPath, wait);
                    retVal.TestCaseResults.Add(output1);

                    if(output1.Result==true)
                    {
                        var output2 = ManageTask_Edit(iWebDriver, resultsScreenShotPath, wait);
                        retVal.TestCaseResults.Add(output2);
                    }

                    iWebDriver.Close();
                    iWebDriver.Dispose();
                }
            }
            catch (Exception ex)
            {
                LoggerBase.Logger.Error("Error while launching application", ex);
                string retVal3 = "Error while launching application (" + ex.Message + ")<br/><br/>";
                launchedResult.TestCase = "Launch Application";
                launchedResult.Result = false;
                launchedResult.DetailedException = ex.Message + " " + ex.StackTrace;
                launchedResult.Message = retVal3;
                retVal.TestCaseResults.Add(launchedResult);
                LoggerBase.Logger.Info(retVal3);
                if (iWebDriver != null)
                {
                    iWebDriver.Close();
                    iWebDriver.Dispose();
                }
            }
            return retVal;
        }
        /// <summary>
        /// Method to verify application launch
        /// </summary>
        /// <param name="iWebDriver"></param>
        /// <returns></returns>
        public TestCaseResult Application_Launch(IWebDriver iWebDriver)
        {
            TestCaseResult launchedResult = new TestCaseResult();
            try
            {
                if (iWebDriver.FindElement(By.XPath("//span[contains(text(),'My Tasks')]")).Displayed)
                {
                    string retVal1 = "Application launched successfully<br/><br/>";
                    launchedResult.TestCase = "Launch Application";
                    launchedResult.Result = true;
                    launchedResult.Message = retVal1;
                    LoggerBase.Logger.Info(retVal1);
                    return launchedResult;
                }
            }
            catch (Exception ex)
            {
                LoggerBase.Logger.Info(ex.Message + " " + ex.StackTrace);
                launchedResult.DetailedException = ex.Message + " " + ex.StackTrace;
            }

            launchedResult.TestCase = "Launch Application";
            launchedResult.Result = false;
            launchedResult.Message = "Application launching failed<br/><br/>";
            return launchedResult;
        }

        /// <summary>
        /// Method for verifying Manage Tasks searched data
        /// </summary>
        /// <param name="iWebDriver"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public TestCaseResult ManageTasks_SearchData(IWebDriver iWebDriver, string resultsScreenShotPath, WebDriverWait wait)
        {
            TestCaseResult result = new TestCaseResult();
            result.TestCase = "Verification of search data on Manage Tasks page";

            ElementHelper eleHelper = new ElementHelper(iWebDriver);
            string retVal = "Data populated for searched Task <br/><br/>";
            try
            {
               
                eleHelper.DoAction(FindBy.XPath, "//span[contains(text(),'Manage Tasks')]", MBIA.AutomatedTesting.Framework.Enums.Action.Click); //Clicking on CUSIP Master
                LoggerBase.Logger.Info("Clicked on Manage Tasks");
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@id='txtSearchDesc']")));
                eleHelper.DoAction(FindBy.XPath, "//input[@id='txtSearchDesc']", MBIA.AutomatedTesting.Framework.Enums.Action.SendKeys, "Access to Source Code (Fall recertification)");
                LoggerBase.Logger.Info("Entered task name");
                eleHelper.DoAction(FindBy.XPath, "//button[@type='submit']", MBIA.AutomatedTesting.Framework.Enums.Action.Click);
                LoggerBase.Logger.Info("Clicked on Submit button");
                Thread.Sleep(8000);
                //wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//table[@class='table table-bordered']/tbody/tr[2]/td/a[@class='glyphicon glyphicon-edit']")));

                try
                {
                    if (iWebDriver.FindElement(By.XPath("//table[@class='table table-bordered']/tbody/tr[2]/td/a[@class='glyphicon glyphicon-edit']")).Displayed)
                    {
                        Console.WriteLine("Data populated");
                        LoggerBase.Logger.Info("Data is populated on page");
                    }
                }
                catch (Exception ex)
                {
                    
                    retVal = "Data is not populated on page";
                    Console.WriteLine("Data not populated");
                    LoggerBase.Logger.Info("Data is not populated on page");
                }

                Thread.Sleep(3000);
                Screenshot ss2 = ((ITakesScreenshot)iWebDriver).GetScreenshot();
                ss2.SaveAsFile(resultsScreenShotPath + "\\2 - ManageTasks.jpg", OpenQA.Selenium.ScreenshotImageFormat.Jpeg); //Screenshot of displayed data
                result.Result = true;
            }
            catch (Exception ex)
            {
                LoggerBase.Logger.Error("Error while verifying data on Manage Tasks page", ex);
                result.Result = false;
                result.DetailedException = ex.Message + " " + ex.StackTrace;
                retVal = "Error while verifying data on Manage Tasks page (" + ex.Message + ")<br/><br/>";
            }
            result.Message = retVal;
            return result;
        }
        /// <summary>
        /// Method for searching CUSIP record by clicking on Manage Task Edit button
        /// </summary>
        /// <param name="iWebDriver"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public TestCaseResult ManageTask_Edit(IWebDriver iWebDriver, string resultsScreenShotPath, WebDriverWait wait)
        {
            TestCaseResult result = new TestCaseResult();
            result.TestCase = "Manage Tasks Add/Edit";

            ElementHelper eleHelper = new ElementHelper(iWebDriver);
            string retVal = "Manage Tasks Add/Edit successfully after clicking on edit icon <br/><br/>";
            try
            {
              

                try
                {
                    if (iWebDriver.FindElement(By.XPath("//table[@class='table table-bordered']/tbody/tr[2]/td/a[@class='glyphicon glyphicon-edit']")).Displayed)
                    {
                        eleHelper.DoAction(FindBy.XPath, "//table[@class='table table-bordered']/tbody/tr[2]/td/a[@class='glyphicon glyphicon-edit']", MBIA.AutomatedTesting.Framework.Enums.Action.Click);
                        LoggerBase.Logger.Info("Clicked on first search result edit");
                        Thread.Sleep(8000);
                        if (iWebDriver.FindElement(By.XPath("//input[@id='Name']")).Displayed)
                        {
                            Console.WriteLine("Data populated");
                            LoggerBase.Logger.Info("Data is populated on page");
                        }
                    }
                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Data not populated");
                    LoggerBase.Logger.Info("Data is not populated on page");
                    retVal = "Data is not populated on page";
                }

                Thread.Sleep(3000);
                Screenshot ss2 = ((ITakesScreenshot)iWebDriver).GetScreenshot();
                ss2.SaveAsFile(resultsScreenShotPath + "\\3 - EditManageTasks.jpg", OpenQA.Selenium.ScreenshotImageFormat.Jpeg); //Screenshot of displayed data
                result.Result = true;
            }
            catch (Exception ex)
            {
                LoggerBase.Logger.Error("Error while populating data for Edit Task", ex);
                result.Result = false;
                result.DetailedException = ex.Message + " " + ex.StackTrace;
                retVal = "Error while populating data for Edit Task (" + ex.Message + ")<br/><br/>";
            }
            result.Message = retVal;
            return result;
        }

    }
}