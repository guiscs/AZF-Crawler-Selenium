using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Functions
{
    public class HttpExample
    {
        [FunctionName("HttpExample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var driver = CreateAndStartChromeService();
            var list = BuscarTabelaBrasileirao(driver);
            driver.Quit();

            // var files = Directory.GetFiles(@"/", "*chrom*", SearchOption.AllDirectories);//Directory.GetFiles(Path.GetDirectoryName(@"/usr/"), "*");
            // //var folders = Directory.GetDirectories(Path.GetDirectoryName(@"/usr/"));

            // var retorno = new List<string>();

            // foreach (var item in files)
            // {
            //     retorno.Add(item);
            // }

            //foreach (var item in folders)
            //{
            //    retorno.Add(item);
            //}

            return new OkObjectResult(list);
            //return new OkObjectResult(retorno.OrderBy(x => x).ToList()); //new OkObjectResult(list);            
        }

        private List<TabelaBrasileirao> BuscarTabelaBrasileirao(ChromeDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl("https://globoesporte.globo.com/futebol/brasileirao-serie-a/");
                Thread.Sleep(5000);

                var rowTables = driver.FindElementsByClassName("classificacao__tabela--linha");
                var tabelaBrasileiro = new List<TabelaBrasileirao>();
                if (rowTables != null)
                    foreach (var row in rowTables.Take(20))
                    {
                        var posicao = row.FindElements(By.ClassName("classificacao__equipes--posicao")).FirstOrDefault().Text;
                        var time = row.FindElements(By.ClassName("classificacao__equipes--time")).FirstOrDefault().Text;

                        tabelaBrasileiro.Add(new TabelaBrasileirao()
                        {
                            Posicao = Convert.ToInt32(posicao),
                            Clube = time
                        });

                    }

                return tabelaBrasileiro;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ChromeDriver CreateAndStartChromeService()
        {

            String driverPath = "usr/local/bin"; //$"{Environment.GetEnvironmentVariable("PATH")}";

            var option = new ChromeOptions();

            option.AddArgument("start-maximized");
            option.AddArgument("--headless");

            var pathChrome = getGetPathChrome();
            if (!string.IsNullOrEmpty(pathChrome))
                option.BinaryLocation = pathChrome;

            option.AddArgument("start-maximized");
            option.AddArgument("disable-infobars");
            option.AddArgument("disable-extensions");
            option.AddArgument("disable-gpu");
            option.AddArgument("disable-dev-shm-usage");
            option.AddArgument("--no-sandbox");
            option.AddArgument("disable-web-security");
            option.Proxy = null;

            //try{
            //return new ChromeDriver("/mnt/c/Users/g.mello.pires/repos/AppCrawlerServeless/Functions/bin/output", option, TimeSpan.FromSeconds(120));
            //return new ChromeDriver("/home/site/wwwroot/", option, TimeSpan.FromSeconds(120));
            return new ChromeDriver("/usr/local/bin/", option, TimeSpan.FromSeconds(120));

            //return new ChromeDriver(driverPath, option, TimeSpan.FromSeconds(120));
            // }
            // catch(Exception ex)
            // {
            //     return new ChromeDriver("/home/site/wwwroot/", option, TimeSpan.FromSeconds(120));
            // }            
        }

        private string getGetPathChrome()
        {
            if (File.Exists("/usr/bin/chromium-browser"))
                return "/usr/bin/chromium-browser";
            else if (File.Exists("/usr/bin/chromium"))
                return "/usr/bin/chromium";
            else if (File.Exists("/usr/bin/chrome"))
                return "/usr/bin/chrome";
            else if (File.Exists("/usr/bin/google-chrome"))
                return "/usr/bin/google-chrome";
            else if (File.Exists("/usr/bin/google-chrome-stable"))
                return "/usr/bin/google-chrome-stable";
            else
                return string.Empty;
        }
    }
}
