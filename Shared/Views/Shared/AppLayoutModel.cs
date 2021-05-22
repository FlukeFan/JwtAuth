using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Hosting;

namespace AuthEx.Shared.Views.Shared
{
    public class AppLayoutModel
    {
        private static IList<Menu> _menus;

        public string Title;
        public string BaseUrl;
        public bool EnableBlazorServerSide;
        public bool EnableBlazorClientSide;
        public string ClientStyles;
        public string CustomScript;

        public IList<Menu> GetMenus(IWebHostEnvironment host)
        {
            if (_menus != null)
                return _menus;

            lock (typeof(AppLayoutModel))
            {
                if (_menus == null)
                {
                    var menus = new List<Menu>();
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

                        var menuElement = (XmlElement)doc.SelectSingleNode("//Menu");
                        var menu = new Menu
                        {
                            Position = int.Parse(menuElement.GetAttribute("Position")),
                            Text = menuElement.GetAttribute("Text"),
                        };

                        if (menuElement.HasAttribute("Path"))
                        {
                            menu.Path = menuElement.GetAttribute("Path");
                        }
                        else
                        {
                            menu.Items = menuElement.SelectNodes("MenuItem")
                                .Cast<XmlElement>()
                                .Select(e => new Menu
                                {
                                    Text = e.GetAttribute("Text"),
                                    Path = e.GetAttribute("Path"),
                                }).ToList();
                        }

                        menus.Add(menu);
                    }

                    _menus = menus.OrderBy(m => m.Position).ToList();
                }
            }

            _menus.Insert(0, new Menu { Text = "Page 1", Path = "/Home/Page1" });
            _menus.Insert(1, new Menu { Text = "Page 2", Path = "/Home/Page2" });

            return _menus;
        }
    }

    public class Menu
    {
        public int Position;
        public string Text;
        public string Path;

        public IList<Menu> Items;
    }

    public static class AppLayoutExtensions
    {
        public static AppLayoutModel LayoutModel(this IRazorPage page)
        {
            return (AppLayoutModel)page.ViewContext.ViewData["appLayoutModel"];
        }

        public static void SetAppLayout(
            this IRazorPage page,
            string title,
            string baseUrl,
            bool enableBlazorServerSide = false,
            bool enableBlazorClientSide = false,
            string clientStyles = null,
            string customScript = null)
        {
            page.Layout = "_appLayout";

            var model = new AppLayoutModel
            {
                Title = title,
                BaseUrl = baseUrl,
                EnableBlazorServerSide = enableBlazorServerSide,
                EnableBlazorClientSide = enableBlazorClientSide,
                ClientStyles = clientStyles,
                CustomScript = customScript,
            };

            page.ViewContext.ViewData["appLayoutModel"] = model;
        }
    }
}
