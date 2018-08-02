using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;
using PackageAnalyzer.Analyzers;
using PackageAnalyzer.DataCollection;
using PackageAnalyzer.Models;

namespace PackageAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            PackageDataCollector.GetData();
            var model = Analyzer.Train().Result;

            // Evaluate(model);
            var release = new PackageRelease
            {
                DownloadCount = 1000,
                //Published = DateTime.UtcNow.AddMonths(2).Ticks,
                //ReleaseDiff = 60,
                SemVerUpgrade = (long)VersionType.Patch,
                Version = "12.5.8"
            };

            var prediction = model.Predict(release);
            Console.WriteLine("Predicted month: {0}", prediction.Month );

            Console.ReadKey();
        }
    }
}
