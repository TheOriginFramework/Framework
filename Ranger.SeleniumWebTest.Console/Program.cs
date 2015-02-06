using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranger.SeleniumWebTest.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            var navigation = driver.Navigate();
            
            navigation.GoToUrl("http://exam.microsysolution.com");

            // put data.
            IWebElement userNameElement = driver.FindElement(By.Name("UserName"));
            IWebElement passwordElement = driver.FindElement(By.Name("Password"));

            userNameElement.SendKeys("DA000159");
            passwordElement.SendKeys("DA000159");

            userNameElement.Submit();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url == "http://exam.microsysolution.com/TS/TestSession");
            
            driver.Close();
        }
    }
}
