using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager.TaskManagerTestCases testcase = new TaskManager.TaskManagerTestCases();
            testcase.ExecuteTestCase(MBIA.AutomatedTesting.Framework.Enums.DriverType.InternetExplorer, @"\\mbamk-iis5t\wwwroot\Automated_Testing\Testcases\TaskManager", "https://taskmanager-test.mbia.com/", false);
        }
    }
}
