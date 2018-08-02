using Microsoft.ML.Runtime.Api;

namespace PackageAnalyzer.Models
{
    public class PackageReleasePrediction
    {
        [ColumnName("Score")]
        public float Month;
    }
}