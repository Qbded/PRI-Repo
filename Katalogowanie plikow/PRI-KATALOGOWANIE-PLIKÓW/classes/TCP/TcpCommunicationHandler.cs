using PRI_KATALOGOWANIE_PLIKÓW.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes.TCP
{
    class TcpCommunicationHandler
    {



        private static readonly int PORT = 100;
        
        /// <summary>
        /// Maximum byte count of data sent in a single data packet
        /// </summary>
        private static readonly int MAX_DATA_PACKET_SIZE = 1024;
        /// <summary>
        /// Length of data packets is stored in int variables, whitch
        /// take 4 bytes each. Therefore, 4 consecutive bytes from
        /// stream must be interpreted as such.
        /// </summary>
        private static readonly int DATA_SIZE_BYTE_ARRAY_LENGTH = 4;

        TcpListener serverListener;

        BackgroundWorker serverBGWorker;
        object _serverThreadLock;
        List<BackgroundWorker> clientBGWorkers;
        List<object> _clientThreadLocks;


        public TcpCommunicationHandler()
        {
            serverListener = new TcpListener(IPAddress.Any, PORT);
            clientBGWorkers = new List<BackgroundWorker>();
            _clientThreadLocks = new List<object>();
            //tcpListener

            StartServer();
        }


        public DistributedNetworkFile GetRequestedFile()
        {
            //TODO: Input proper logic
            return null;
        }


#region server threading logic
        /// <summary>
        /// Starts a background thread using TcpListener to await 
        /// requests.
        /// </summary>
        private void StartServer()
        {
            serverBGWorker = new BackgroundWorker();
            _serverThreadLock = new object();
            serverBGWorker.WorkerSupportsCancellation = true;
            serverBGWorker.WorkerReportsProgress = true;
            serverBGWorker.DoWork += 
                new DoWorkEventHandler(RunServerEvent);
            serverBGWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(ServerStoppedEvent);
            List<object> doWorkArguments = new List<object>()
            {
                _serverThreadLock
            };
            serverBGWorker.RunWorkerAsync(doWorkArguments);
        }

        private void RunServerEvent(object sender, DoWorkEventArgs e)
        {
            // bgWorker.CancellationPending and e.Cancel are both
            // used as cancellation flags for differend methods.
            // Make sure they are in-sync
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            
            RunServer(ref bgWorker, ref e);
        }

        /// <summary>
        /// Runs server socket until cancellation of bgWorker is 
        /// requested
        /// </summary>
        /// <param name="bgWorker"></param>
        /// <param name="e"></param>
        private void RunServer(ref BackgroundWorker bgWorker,
            ref DoWorkEventArgs e)
        {
            List<object> arg = e.Argument as List<object>;
            object _threadLock = arg.ElementAt(0) as object;

            // Start server listener
            try
            {
                serverListener = new TcpListener(IPAddress.Any, PORT);
                serverListener.Start();
            }
            catch(Exception ex)
            {
                e.Result = "Exception occured when starting " +
                    "serverListener object: " + ex.Message;
            }

            while (!bgWorker.CancellationPending)
            {
                lock (_threadLock)
                {
                    if (e.Cancel == true && bgWorker.CancellationPending == false)
                    {
                        bgWorker.CancelAsync();
                    }
                    if (e.Cancel == false && bgWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                    }
                }

                TcpClient tcpClient = serverListener.AcceptTcpClient();
                //NetworkStream networkStream = tcpClient.GetStream();
                Thread thread = new Thread(
                    new ParameterizedThreadStart(AcceptClientRequest));
                thread.Start(tcpClient);
            }

            // Stop serverListener after receiving worker cancellation
            // signal
            try
            {
                serverListener.Stop();
            }
            catch(SocketException ex) {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void StopServer()
        {
            if (serverBGWorker.IsBusy && 
                !serverBGWorker.CancellationPending)
            {
                serverBGWorker.CancelAsync();
            }

            try
            {
                serverListener.Stop();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ServerStoppedEvent(object sender, 
            RunWorkerCompletedEventArgs e)
        {
            if (!e.Error.Message.Equals(""))
            {
                System.Windows.Forms.MessageBox.Show(e.Error.Message);
            }

            if (!((String)e.Result).Equals("")){
                System.Windows.Forms.MessageBox.Show((String)e.Result);
            }
        }
#endregion


#region client threading logic
        private void StartClient()
        {
            BackgroundWorker clientBGWorker = new BackgroundWorker();
            clientBGWorkers.Add(clientBGWorker);
            object _threadLock = new object();
            _clientThreadLocks.Add(_threadLock);
            clientBGWorker.WorkerSupportsCancellation = true;
            clientBGWorker.WorkerReportsProgress = true;
            clientBGWorker.DoWork += new DoWorkEventHandler(RunClientEvent);
            clientBGWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(StopClientEvent);
            List<object> doWorkArgs = new List<object>()
            {
                _threadLock
            };
        }

        private void RunClientEvent(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = sender as BackgroundWorker;

            RunClient(ref bgWorker, ref e);
        }

        private void RunClient(ref BackgroundWorker sender,
            ref DoWorkEventArgs e)
        {

        }

        private void StopClient(BackgroundWorker client)
        {
            client.CancelAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">Index, under whitch the client can be 
        /// found on the clientBGWorkers list</param>
        private void StopClient(int index)
        {
            BackgroundWorker client = clientBGWorkers[index];
            StopClient(client);
        }

        private void StopClientEvent(object sender, RunWorkerCompletedEventArgs e)
        {

        }


        private void CloseAllClients()
        {
            int index = 0;
            foreach(BackgroundWorker bgw in clientBGWorkers)
            {
                object _threadLock = _clientThreadLocks.ElementAt(index);
                if (bgw.IsBusy)
                {
                    lock (_threadLock)
                    {
                        bgw.CancelAsync();
                    }

                    index++;
                }
            }
        }

        private void CleanClientWorkerList()
        {
            int index = 0;
            foreach(BackgroundWorker bgw in clientBGWorkers)
            {
                if (!bgw.IsBusy)
                {
                    clientBGWorkers.RemoveAt(index);
                    _clientThreadLocks.RemoveAt(index);
                }

                index++;
            }
        }
        #endregion


#region socket communication methods
        // All methods in this region are to be called by background
        // threads
        private void AcceptClientRequest(object parameter)
        {
            TcpClient tcpClient = parameter as TcpClient;
            NetworkStream networkStream = tcpClient.GetStream();

            int firstByte = networkStream.ReadByte();
            if(firstByte == TcpRequestCodebook.INITIALIZE[0])
            {
                int requestCode = networkStream.ReadByte();
                if(TcpRequestCodebook.IsRequest(
                    requestCode, TcpRequestCodebook.SEND_FILE))
                {
                    SendFileRequestCallback(networkStream);
                }
            }

            try
            {
                networkStream.Close();
                tcpClient.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void InterpretRequest(NetworkStream networkStream, 
            int request)
        {
            if (TcpRequestCodebook.IsRequest(
                request, TcpRequestCodebook.SEND_FILE))
            {
                DistributedNetworkFile dnFile = GetRequestedFile();
                SendFileRequestCallback(networkStream, dnFile);
            }
            else
            {
                // Incorrect request
            }
        }


        public byte[] RequestFile(DistributedNetworkUser targetUser,
            DistributedNetworkFile file)
        {
            //TODO: Input proper logic
            return null;
        }

        public void SendFileRequestCallback(NetworkStream networkStream,
            DistributedNetworkFile dnFile)
        {
            
        }


        public void SendFile(DistributedNetworkUser targetUser,
            DistributedNetworkFile file)
        {
            //TODO: Input proper logic
        }





        //private byte[] CreateMessageDataPacket(byte[] request,
        //    String base64String)
        //{
        //    MemoryStream memoryStream = new MemoryStream();

        //    byte[] initializingBytes = TcpRequestCodebook.INITIALIZE;
        //    memoryStream.Write(
        //        initializingBytes, 0, initializingBytes.Length);

        //    byte[] requestBytes = request;
        //    memoryStream.Write(
        //        request, 0, request.Length);

        //    byte[] stringBytes = StringToByteArray(base64String);
        //    int stringByteCount = stringBytes.Length;
        //    byte[] 
        //}


        private byte[] CreateFileDataPacket(byte[] request,
            byte[] dataToSend)
        {
            MemoryStream memoryStream = new MemoryStream();

            byte[] initializingBytes = TcpRequestCodebook.INITIALIZE;
            memoryStream.Write(
                initializingBytes, 0, initializingBytes.Length);

            byte[] requestBytes = request;
            memoryStream.Write(
                requestBytes, 0, requestBytes.Length);

            byte[] dataLengthBytes = IntToByteArray(
                dataToSend.Length);
            memoryStream.Write(
                dataLengthBytes, 0, dataLengthBytes.Length);

            byte[] separatorBytes = TcpRequestCodebook.SEPARATOR;
            memoryStream.Write(
                separatorBytes, 0, separatorBytes.Length);

            byte[] dataBytes = dataToSend;
            memoryStream.Write(
                dataBytes, 0, dataBytes.Length);

            return memoryStream.ToArray();
        }


        private String EncodeStringBase64(String unencodedString)
        {
            if (unencodedString.Equals(String.Empty))
            {
                return String.Empty;
            }

            byte[] unencodedStringBytes = 
                Encoding.UTF8.GetBytes(unencodedString);
            return Convert.ToBase64String(unencodedStringBytes);
        }

        private String DecodeStringBase64(String encodedString)
        {
            if (encodedString.Equals(String.Empty))
            {
                return String.Empty;
            }

            byte[] encodedStringBytes = 
                Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(encodedStringBytes);
        }

        private byte[] StringToByteArray(String str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        private String ByteArrayToString(byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }

        private int GetStringByteCount(String str)
        {
            return UTF8Encoding.UTF8.GetByteCount(str);
        }


        /// <summary>
        /// Converts byte to integer representing the byte value
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ByteToInt(byte b)
        {
            return b;
        }

        /// <summary>
        /// Uses first 4 bytes from byte array to create integer value;
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ByteArrayToInt(byte[] b)
        {
            return BitConverter.ToInt32(b, 0);
        }

        /// <summary>
        /// Converts int to 4-element byte array.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>4-element byte array.</returns>
        public static byte[] IntToByteArray(int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            //if (BitConverter.IsLittleEndian)
            //{
            //    Array.Reverse(bytes);
            //}
            return bytes;
        }


        #endregion
    }
}
