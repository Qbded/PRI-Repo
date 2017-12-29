using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes.TCP
{
    static class TcpRequestCodebook
    {
        public static readonly String REQUEST_FILE = "request file";
        public static readonly String REQUEST_CATALOG = "request catalog";

        public static readonly byte[] INITIALIZE = { 2 };
        public static readonly byte[] SEPARATOR = { 4 };
    }
}
