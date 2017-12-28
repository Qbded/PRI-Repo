using PRI_KATALOGOWANIE_PLIKÓW.classes.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    class DistributedNetwork
    {
        List<DistributedNetworkUser> users;

        Socket networkSocket;
        //TcpMessageTransferer tcpMessageTransferer = new TcpMessageTransferer();
        //TcpFileTransferer tcpFileTransferer = new TcpFileTransferer();
        TcpCommunicationHandler tcpCom;

        public DistributedNetwork()
        {
            networkSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            tcpCom = new TcpCommunicationHandler();
        }

        ~DistributedNetwork()
        {
            networkSocket.Close();
        }


        public void RequestFile(DistributedNetworkUser user)
        {

        }
    }
}
