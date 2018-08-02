using Microsoft.ML.Runtime.Api;

namespace PackageAnalyzer.Models
{
    public class PackageRelease
    {
        [Column("0")]
        public string Version;

        [Column("1")]
        public float DownloadCount;

        [Column("2")]
        public float Month;

        [Column("3")]
        public float Published;
         
        [Column("4")]
        public float SemVerUpgrade;

        [Column("5")]
        public float ReleaseDiff;
    }
}