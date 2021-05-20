using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AuthEx.Home.Views.Home
{
    public static class IndexModel
    {
        public static IEnumerable<WarmupUrl> WarmupUrls(IWebHostEnvironment host)
        {
            var warmupUrls = new List<WarmupUrl>();
            var rootPath = Path.Combine(host.ContentRootPath, "..");

            if (host.IsDevelopment())
                while (!File.Exists(Path.Combine(rootPath, "AuthEx.sln")))
                    rootPath = Path.Combine(rootPath, "..");

            var moduleFiles = Directory.GetFiles(rootPath, "module.xml", SearchOption.AllDirectories);

            foreach (var moduleFile in moduleFiles)
            {
                if (moduleFile.Contains($"{Path.DirectorySeparatorChar}bin"))
                    continue; // skip any build artefacts

                var doc = new XmlDocument();
                doc.Load(moduleFile);

                var warmupElement = (XmlElement)doc.SelectSingleNode("//Warmup");
                warmupUrls.Add(new WarmupUrl
                {
                    Name = warmupElement.GetAttribute("Name"),
                    Url = warmupElement.GetAttribute("Url"),
                });
            }

            return warmupUrls;
        }

        public class WarmupUrl
        {
            public string Name;
            public string Url;
        }
    }
}
