using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    static class AppCryptoDataStorage
    {
        public static byte[] DB_local_key { set; get; }
        public static bool UserAuthorized = false;
    }
}
