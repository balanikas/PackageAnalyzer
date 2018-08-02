using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Runtime.Internal.Internallearn;
using NuGet;

namespace PackageAnalyzer.DataCollection
{
    class PackageDataCollector
    {
        public static void GetData()
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://nuget.episerver.com/feed/Packages.svc/");
        
            var packages = repo.FindPackagesById("episerver.commerce").ToList();

            packages = packages.Where (item => item.IsReleaseVersion()).ToList();


            var lines = new List<string>();
            IPackage previous = null;
            Console.WriteLine(String.Join("\t\t", "Version", "DownloadCount", "Month", "Published", "VerType", "DaysDiff"));

            foreach (var p in packages.Where(x => x.Version >= new SemanticVersion(7,6,0,0)).OrderBy(x => x.Published))
            {
                var list = new List<string>()
                {
                    p.Version.ToFullString(),
                    p.DownloadCount.ToString(),
                    p.Published.Value.Month.ToString(),
                    p.Published.Value.Ticks.ToString(),
                    ((int)DetectSemVerUpgrade(previous, p)).ToString(),
                    DetectPublishedDateDiff(previous, p).Days.ToString(),
                };

                var line = String.Join(",", list);
                if (line.Contains("?"))
                {
                    continue;
                }

                Console.WriteLine(String.Join("\t\t", list));

                lines.Add(line);
              
                previous = p;
            }

            File.Delete(Path.Combine(Environment.CurrentDirectory, "data-train.csv"));
            File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "data-train.csv"), String.Join("\n", lines));

        }

        static TimeSpan DetectPublishedDateDiff(IPackage previous, IPackage current)
        {
            return previous == null ? TimeSpan.Zero : current.Published.Value - previous.Published.Value;
        }

        static VersionType DetectSemVerUpgrade(IPackage previous, IPackage current)
        {
            if (previous == null)
            {
                return VersionType.Major;
            }

            var (previousmajor, previousminor, previouspatch) = Split(previous.Version);
            var (major, minor, patch) = Split(current.Version);

            if (major > previousmajor) return VersionType.Major;
            if (minor > previousminor) return VersionType.Minor;
            if (patch > previouspatch) return VersionType.Patch;

            return VersionType.None;

            (int, int, int ) Split(SemanticVersion version)
            {
                var splitted = version.ToFullString().Split('.');
                return (Int32.Parse(splitted[0]), Int32.Parse(splitted[1]), Int32.Parse(splitted[2]));
            }
        }

        
    }

    public enum VersionType
    {
        None = 0,
        Major = 1, 
        Minor = 2, 
        Patch = 3
    }
}
