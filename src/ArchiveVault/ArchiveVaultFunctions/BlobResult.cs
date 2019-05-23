using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveVaultFunctions
{
    public class BlobResult
    {
        public string FileName { get; set; }
        public string SpFilePath { get; set; }
        public string ConfidentialityLevel { get; set; }
        public string RetentionPeriod { get; set; }
    }
}
