using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AuthEx.Shared
{
    public static class Extensions
    {
        public static string AppFolder(this IWebHostEnvironment host, string path = null)
        {
            var rootPath = Path.Combine(host.ContentRootPath, "..");

            if (host.IsDevelopment())
                while (!File.Exists(Path.Combine(rootPath, "AuthEx.sln")))
                    rootPath = Path.Combine(rootPath, "..");

            return path != null
                ? Path.Combine(rootPath, path)
                : rootPath;
        }
    }
}
