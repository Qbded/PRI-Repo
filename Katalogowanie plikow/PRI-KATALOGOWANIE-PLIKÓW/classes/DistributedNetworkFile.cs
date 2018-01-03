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

        public String filePathInCatalogue;
        public String realFilePath;

        public long fileSize;

        public DistributedNetworkFile()
        {

        }


        public static DistributedNetworkFile GetFileByFilePath(String filePath)
        {
            // TODO grab file data based on file's path
            // return distributedNetworkFile
        }


        public bool IsPresentInLocalCatalogue()
        {

        }
    }
}
