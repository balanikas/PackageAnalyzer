using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using PackageAnalyzer.DataCollection;
using PackageAnalyzer.Models;

namespace PackageAnalyzer.Analyzers
{
    class Analyzer
    {
        static readonly string _datapath = Path.Combine(Environment.CurrentDirectory, "data-train.csv");
        static readonly string _testdatapath = Path.Combine(Environment.CurrentDirectory,  "data-test.csv");
        static readonly string _modelpath = Path.Combine(Environment.CurrentDirectory,  "model.zip");

        public static async Task<PredictionModel<PackageRelease, PackageReleasePrediction>> Train()
        {
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(_datapath).CreateFrom<PackageRelease>(useHeader: true, separator: ','));


            pipeline.Add(new ColumnCopier(("Month", "Label")));


            pipeline.Add(new CategoricalOneHotVectorizer("Version"));

            pipeline.Add(new ColumnConcatenator("Features",
                "ReleaseDiff",
                "SemVerUpgrade",
                "DownloadCount", 
                "Published"));

            pipeline.Add(new FastTreeRegressor());

            var model = pipeline.Train<PackageRelease, PackageReleasePrediction>();
            await model.WriteAsync(_modelpath);
            return model;
        }

        private static void Evaluate(PredictionModel<PackageRelease, PackageReleasePrediction> model)
        {
            var testData = new TextLoader(_testdatapath).CreateFrom<PackageRelease>(useHeader: true, separator: ',');

            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, testData);
            Console.WriteLine($"Rms = {metrics.Rms}");

        }
    }
}
