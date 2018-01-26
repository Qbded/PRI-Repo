using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    [Serializable]
    public class DistributedNetworkFile
    {
        public bool allowDistribution;
        public bool allowUnpromptedDistribution;

        public String realFilePath;
        public String realFileName;

        public long fileSize;

        public DistributedNetworkFile(String fileName, String filePath, long fileSize, bool allowDistribution, bool allowUnpromptedDistribution)
        {
            this.realFileName = fileName;
            this.realFilePath = filePath;
            this.fileSize = fileSize;
            this.allowDistribution = allowDistribution;
            this.allowUnpromptedDistribution = allowUnpromptedDistribution;
        }

        public bool IsPresentInLocalCatalogue()
        {
            bool result = true;
            return result;
        }
    }
}
