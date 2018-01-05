using PRI_KATALOGOWANIE_PLIKÓW.classes.TCP;
using System;
using System.Collections.Generic;
using System.IO;
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

        public DistributedNetwork(Main_form mainForm)
        {
            networkSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            tcpCom = new TcpCommunicationHandler(mainForm);
        }

        ~DistributedNetwork()
        {
            networkSocket.Close();
        }


        public void RequestFile(DistributedNetworkUser user,
            DistributedNetworkFile file)
        {
            String downloadDir = Environment.CurrentDirectory + "/Downloaded";
            String fileName = Path.GetFileName(file.realFilePath);
            if (!Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }
            tcpCom.RequestFile(file,
                downloadDir + "/" + fileName,
                user);
        }
    }
}
