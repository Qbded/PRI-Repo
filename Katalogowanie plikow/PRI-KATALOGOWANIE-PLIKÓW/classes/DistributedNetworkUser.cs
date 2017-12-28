using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    class DistributedNetworkUser
    {
        public bool IsNetworkOwner { get; }
        public String Alias { get; }
        public IPAddress IPAddress { get; }


        public DistributedNetworkUser(bool isOwner, String alias, 
            IPAddress IPAddress)
        {
            this.IsNetworkOwner = isOwner;
            this.Alias = alias;
            this.IPAddress = IPAddress;
        }
    }
}
