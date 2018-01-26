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

        public void Shutdown()
        {
            networkSocket.Close();
            tcpCom.Shutdown();
        }

        public void RequestFile(DistributedNetworkUser user,
            List<DistributedNetworkFile> files)
        {
            Console.WriteLine("DistributedNetwork:RequestFile()");

            String downloadDir;

            if (files.Count == 1)
            {
                // Mamy tylko jeden plik, sprawdzamy czy mamy do czynienia z przesyłaniem katalogu.
                if (files[0].realFileName.Equals("EXTERNAL_CATALOG.FDB") && files[0].realFilePath.Equals("TO_DETERMINE"))
                {
                    // Tak
                    downloadDir = ConfigManager.ReadString(ConfigManager.EXTERNAL_DATABASES_LOCATION);
                }
                else
                {
                    // Nie
                    downloadDir = ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION);
                    files[0].realFileName = Path.GetFileName(files[0].realFilePath);
                    if (!Directory.Exists(downloadDir))
                    {
                        Directory.CreateDirectory(downloadDir);
                    }
                }
            }
            else
            {
                // Standardowe wysyłanie plików
                downloadDir = ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION);
                foreach (DistributedNetworkFile dnFile in files)
                {
                    dnFile.realFileName = Path.GetFileName(dnFile.realFilePath);
                }
            }

            tcpCom.RequestFile(files,
                downloadDir + "/",
                user);
        }
    }
}
