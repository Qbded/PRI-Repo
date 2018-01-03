using PRI_KATALOGOWANIE_PLIKÓW.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes.TCP
{
    public class TcpCommunicationHandler
    {



        private static readonly int PORT = 100;
        
        /// <summary>
        /// Maximum byte count of data sent in a single data packet
        /// </summary>
        private static readonly int MAX_DATA_PACKET_SIZE = 4096;
        /// <summary>
        /// Length of data packets is stored in int variables, whitch
        /// take 4 bytes each. Therefore, 4 consecutive bytes from
        /// stream must be interpreted as such.
        /// </summary>
        private static readonly int DATA_SIZE_BYTE_ARRAY_LENGTH = 4;


        #region return codes
        private static readonly int RETURN_OK = 0;
        private static readonly int RETURN_CANCEL = 1;
        private static readonly int RETURN_TIMEOUT = 2;
        private static readonly int RETURN_BAD_REQUEST = 3;
        #endregion


        TcpListener serverListener;

        BackgroundWorker serverBGWorker;
        object _serverThreadLock;
        List<BackgroundWorker> clientBGWorkers;
        List<object> _clientThreadLocks;


        public TcpCommunicationHandler(Main_form mainForm)
        {
            serverListener = new TcpListener(IPAddress.Any, PORT);
            clientBGWorkers = new List<BackgroundWorker>();
            _clientThreadLocks = new List<object>();
            //tcpListener

            StartServer(mainForm);
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
        private void StartServer(Main_form main_Form)
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
                main_Form,
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
            Main_form mainForm = arg.ElementAt(0) as Main_form;
            object _threadLock = arg.ElementAt(1) as object;

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
                List<object> threadArgs = new List<object>()
                {
                    mainForm,
                    tcpClient
                };
                //NetworkStream networkStream = tcpClient.GetStream();
                Thread thread = new Thread(
                    new ParameterizedThreadStart(AcceptClientRequest));
                thread.Start(threadArgs);
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
        private void StartClient(Main_form mainForm, 
            bool[] requestToServer)
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
                mainForm,
                _threadLock,
                requestToServer
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
            List<object> args = parameter as List<object>;
            Main_form mainForm = args.ElementAt(0) as Main_form;
            TcpClient tcpClient = args.ElementAt(1) as TcpClient;
            NetworkStream networkStream = tcpClient.GetStream();

            int firstByte = AwaitNonNegativeByte(networkStream);
            if(firstByte == -1)
            {
                networkStream.Close();
                networkStream.Dispose();
                tcpClient.Close();
                tcpClient.Dispose();
                return;
            } 
            if(firstByte == TcpRequestCodebook.INITIALIZE[0])
            {
                int requestCode = AwaitNonNegativeByte(networkStream);
                if (requestCode == -1)
                {
                    networkStream.Close();
                    networkStream.Dispose();
                    tcpClient.Close();
                    tcpClient.Dispose();
                    return;
                }
                if(TcpRequestCodebook.IsRequest(
                    requestCode, TcpRequestCodebook.SEND_FILE))
                {
                    SendFileRequestCallback(mainForm, networkStream);
                }
                if (TcpRequestCodebook.IsRequest(
                    requestCode, TcpRequestCodebook.SEND_CATALOGUE))
                {
                    SendCatalogueRequestCallback(mainForm, networkStream);
                }
            }

            try
            {
                networkStream.Close();
                networkStream.Dispose();
                tcpClient.Close();
                tcpClient.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /*public int SendFileRequest(NetworkStream networkStream,
            DistributedNetworkFile dnFile)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] fileData;
            using(MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, dnFile);
                fileData = memoryStream.ToArray();
            }
            byte[] packet = CreateFileDataPacket(
                TcpRequestCodebook.SEND_FILE, fileData);
            networkStream.Write(packet, 0, packet.Length);

            packet = null;
            fileData = null;
            binaryFormatter = null;
        }*/
        public int SendFileRequest(NetworkStream networkStream,
            String filePathInCatalogue)
        {
            byte[] serializedFilePath = SerializeString(filePathInCatalogue);
            byte[] packet = CreateTCPDataPacket(
                TcpRequestCodebook.SEND_FILE, serializedFilePath);
            networkStream.Write(packet, 0, packet.Length);
            networkStream.Flush();

            packet = null;
            serializedFilePath = null;

            return 0;
        }


        public int SendFileRequestCallback(Main_form mainForm,
            NetworkStream networkStream)
        {
            // Reading size of upcoming packet
            byte[] sizeBytes = AwaitPacketSize(networkStream);

            int packetSize = ByteArrayToInt(sizeBytes);

            int separator = AwaitNonNegativeByte(networkStream);
            if (separator == -1) {
                return RETURN_TIMEOUT;
            }

            byte[] serializedFilePath = new byte[packetSize];

            serializedFilePath = 
                AwaitDataPacket(networkStream, packetSize);
            String filePathInCatalogue = DeserializeString(serializedFilePath);

            DistributedNetworkFile distributedNetworkFile = DistributedNetworkFile.GetFileByFilePath(filePathInCatalogue);

            if (!distributedNetworkFile.IsPresentInLocalCatalogue())
            {
                byte[] responsePacket = CreateTCPDataPacket(
                    TcpRequestCodebook.FILE_NOT_IN_MY_CATALOGUE,
                    SerializeString("FileNotFound"));

                networkStream.Write(responsePacket, 0, responsePacket.Length);
                networkStream.Flush();
                return RETURN_OK;
            }

            bool canSendFile = false;
            if(distributedNetworkFile.allowUnpromptedDistribution == true)
            {
                canSendFile = true;
            }
            else
            {
                canSendFile = GetUserPermissionToSendFile(mainForm,
                    distributedNetworkFile);
                    
                if(canSendFile == false)
                {
                    byte[] responsePacket = CreateTCPDataPacket(
                        TcpRequestCodebook.REFUSED_TO_SEND_FILE,
                        SerializeString("User refused to send file"));

                    networkStream.Write(responsePacket, 0, responsePacket.Length);
                    networkStream.Flush();
                    return RETURN_OK;
                }
            }

            // serialize and send file

            FileStream fileStream = new FileStream(
                distributedNetworkFile.realFilePath, FileMode.Open);

            long totalBytesRead = 0;
            long remainingFileBytes = distributedNetworkFile.fileSize;
            while (remainingFileBytes > 0)
            {
                long fileFragmentSize =
                    Math.Min(distributedNetworkFile.fileSize,
                        MAX_DATA_PACKET_SIZE);
                byte[] fileFragment = new byte[fileFragmentSize];
                int bytesRead = fileStream.Read(
                    fileFragment, 0, fileFragment.Length);
                totalBytesRead += bytesRead;
                remainingFileBytes -= bytesRead;

                byte[] fileFragmentPacket = CreateTCPDataPacket(
                    TcpRequestCodebook.SENDING_FILE_FRAGMENT,
                    fileFragment);
                networkStream.Write(fileFragmentPacket, 0,
                    fileFragmentPacket.Length);
                networkStream.Flush();

                int initByte = AwaitNonNegativeByte(networkStream);
                if (initByte == -1) return RETURN_TIMEOUT;
                int requestCode = AwaitNonNegativeByte(networkStream);
                if (requestCode == -1) return RETURN_TIMEOUT;
                byte[] receivedPacketSize = AwaitPacketSize(networkStream);
                if (receivedPacketSize.Length == 0) return RETURN_TIMEOUT;
                packetSize = ByteArrayToInt(receivedPacketSize);
                int separatorByte = AwaitNonNegativeByte(networkStream);
                if (separatorByte == -1) return RETURN_TIMEOUT;
                byte[] packetData = AwaitDataPacket(networkStream, packetSize);
                if (packetData.Length > MAX_DATA_PACKET_SIZE) return RETURN_TIMEOUT;

                if(requestCode != TcpRequestCodebook.CONTINUE_SENDING_FILE[0])
                {
                    byte[] responsePacket = CreateTCPDataPacket(
                        TcpRequestCodebook.TERMINATE,
                        SerializeString("Bad response"));
                    networkStream.Write(responsePacket, 0, 
                        responsePacket.Length);
                    networkStream.Flush();
                    return RETURN_BAD_REQUEST;
                }
            }

            byte[] finalMessagePacket = CreateTCPDataPacket(
                TcpRequestCodebook.DONE_SENDING_FILE,
                SerializeString("Entire file has been sent"));
            networkStream.Write(finalMessagePacket, 0, finalMessagePacket.Length);
            networkStream.Flush();
            finalMessagePacket = null;
            finalMessagePacket = CreateTCPDataPacket(
                 TcpRequestCodebook.TERMINATE,
                 SerializeString("End connection"));
            networkStream.Write(finalMessagePacket, 0, finalMessagePacket.Length);
            networkStream.Flush();

            return 0;


            //byte[] serializedDNFile = new byte[packetSize];
            //networkStream.Read(serializedDNFile, 0, packetSize);

            //MemoryStream serializationStream = new MemoryStream(serializedDNFile);
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //DistributedNetworkFile dnFile = 
            //    (DistributedNetworkFile) binaryFormatter.Deserialize(serializationStream);


            //DistributedNetworkFile dnf = new DistributedNetworkFile();
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream ms = new MemoryStream()) {
            //    binaryFormatter.Serialize(ms, dnf);
            //    byte[] serializedFileData = ms.ToArray();
            //}
            // TODO check if file exists
            // TODO logic to ask user for permission to send file
        }


        private bool GetUserPermissionToSendFile(Main_form mainForm,
            DistributedNetworkFile distributedNetworkFile)
        {
            bool canSendFile = false;
            mainForm.Invoke((Action)delegate ()
            {
                canSendFile = mainForm.GrantFileTransferPermission(distributedNetworkFile);
            });
            return canSendFile;
        }

        public int SendCatalogueRequest(NetworkStream networkStream)
        {
            byte[] packet = CreateTCPDataPacket(
                TcpRequestCodebook.SEND_CATALOGUE, new byte[] { 0 });
            networkStream.Write(packet, 0, packet.Length);
            networkStream.Flush();

            packet = null;

            return 0;
        }

        public int SendCatalogueRequestCallback(Main_form mainForm,
            NetworkStream networkStream)
        {
            // Reading size of upcoming packet
            byte[] sizeBytes = AwaitPacketSize(networkStream);

            int packetSize = ByteArrayToInt(sizeBytes);

            int separator = AwaitNonNegativeByte(networkStream);
            if (separator == -1)
            {
                return RETURN_TIMEOUT;
            }

            // We should do some checks in the main form and return their results here.
            // On successful exit bool in returns is set to false and it's string is a non-empty string pointing to our external catalog
            // On any error bool in returns is set to true and it's string is empty.

            Tuple<bool,string> check_returns = new Tuple<bool,string>(true,"");

            mainForm.Invoke((Action)delegate ()
            {
                check_returns = mainForm.ExternalCatalogTransfer();
            });

            if(check_returns.Item1 == false)
            {
                // Everything was fine, we have a path to external catalogue in our returns:
                FileInfo external_catalogue_info = new FileInfo(check_returns.Item2);
                FileStream fileStream = new FileStream(
                check_returns.Item2, FileMode.Open);
                
                long totalBytesRead = 0;
                long remainingFileBytes = external_catalogue_info.Length;
                while (remainingFileBytes > 0)
                {
                    long fileFragmentSize =
                        Math.Min(external_catalogue_info.Length,
                            MAX_DATA_PACKET_SIZE);
                    byte[] fileFragment = new byte[fileFragmentSize];
                    int bytesRead = fileStream.Read(
                        fileFragment, 0, fileFragment.Length);
                    totalBytesRead += bytesRead;
                    remainingFileBytes -= bytesRead;

                    byte[] fileFragmentPacket = CreateTCPDataPacket(
                        TcpRequestCodebook.SENDING_FILE_FRAGMENT,
                        fileFragment);
                    networkStream.Write(fileFragmentPacket, 0,
                        fileFragmentPacket.Length);
                    networkStream.Flush();

                    int initByte = AwaitNonNegativeByte(networkStream);
                    if (initByte == -1) return RETURN_TIMEOUT;
                    int requestCode = AwaitNonNegativeByte(networkStream);
                    if (requestCode == -1) return RETURN_TIMEOUT;
                    byte[] receivedPacketSize = AwaitPacketSize(networkStream);
                    if (receivedPacketSize.Length == 0) return RETURN_TIMEOUT;
                    packetSize = ByteArrayToInt(receivedPacketSize);
                    int separatorByte = AwaitNonNegativeByte(networkStream);
                    if (separatorByte == -1) return RETURN_TIMEOUT;
                    byte[] packetData = AwaitDataPacket(networkStream, packetSize);
                    if (packetData.Length > MAX_DATA_PACKET_SIZE) return RETURN_TIMEOUT;

                    if (requestCode != TcpRequestCodebook.CONTINUE_SENDING_FILE[0])
                    {
                        byte[] responsePacket = CreateTCPDataPacket(
                            TcpRequestCodebook.TERMINATE,
                            SerializeString("Bad response"));
                        networkStream.Write(responsePacket, 0,
                            responsePacket.Length);
                        networkStream.Flush();
                        return RETURN_BAD_REQUEST;
                    }
                }

                byte[] finalMessagePacket = CreateTCPDataPacket(
                    TcpRequestCodebook.DONE_SENDING_FILE,
                    SerializeString("Entire file has been sent"));
                networkStream.Write(finalMessagePacket, 0, finalMessagePacket.Length);
                networkStream.Flush();
                finalMessagePacket = null;
                finalMessagePacket = CreateTCPDataPacket(
                     TcpRequestCodebook.TERMINATE,
                     SerializeString("End connection"));
                networkStream.Write(finalMessagePacket, 0, finalMessagePacket.Length);
                networkStream.Flush();
            }
            else
            {
                // Something went wrong - we return an error to the caller
                
                byte[] responsePacket = CreateTCPDataPacket(
                            TcpRequestCodebook.CANNOT_SEND_CATALOGUE,
                            SerializeString("External catalogue could not be sent. Requested participant appears to have no files to share."));
                networkStream.Write(responsePacket, 0,
                    responsePacket.Length);
                networkStream.Flush();
                return RETURN_OK;
            }

            return 0;
        }

        //public byte[] RequestFile(DistributedNetworkUser targetUser,
        //    DistributedNetworkFile file)
        //{
        //    //TODO: Input proper logic
        //    return null;
        //}


        //public void SendFile(DistributedNetworkUser targetUser,
        //    DistributedNetworkFile file)
        //{
        //    //TODO: Input proper logic
        //}





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


        /// <summary>
        /// Will return first non-negative byte from NetworkStream
        /// or -1 if timeout occurs
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        private int AwaitNonNegativeByte(NetworkStream networkStream)
        {
            DateTime lastReadTime = DateTime.Now;
            TimeSpan timeoutWaitTime = new TimeSpan(0, 0, 
                ConfigManager.ReadInt(
                    ConfigManager.TCP_SECONDS_TO_TIMEOUT));
            int byteRead = networkStream.ReadByte();
            while(byteRead == -1)
            {
                byteRead = networkStream.ReadByte();
                if(byteRead == -1)
                {
                    TimeSpan idleTime = DateTime.Now.Subtract(lastReadTime);
                    if(idleTime.CompareTo(timeoutWaitTime) >= 0)
                    {
                        return -1;
                    }
                }
            }
            return byteRead;
        }


        /// <summary>
        /// Returns bytes read from stream or empty byte array if 
        /// time-out occurs before all bytes could be read
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        private byte[] AwaitPacketSize(NetworkStream networkStream)
        {
            DateTime lastReadTime = DateTime.Now;
            TimeSpan timeoutWaitTime = new TimeSpan(0, 0, 
                ConfigManager.ReadInt(
                    ConfigManager.TCP_SECONDS_TO_TIMEOUT));
            int bytesToRead = DATA_SIZE_BYTE_ARRAY_LENGTH;
            int bytesReadTotal = 0;
            byte[] buffer = new byte[DATA_SIZE_BYTE_ARRAY_LENGTH];
            while(bytesToRead > 0)
            {
                int bytesRead = networkStream.Read(
                    buffer, bytesReadTotal, bytesToRead);
                if (bytesRead > 0)
                {
                    bytesToRead -= bytesRead;
                    bytesReadTotal += bytesRead;
                }
                else
                {
                    TimeSpan idleTime = 
                        DateTime.Now.Subtract(lastReadTime);
                    if(idleTime.CompareTo(timeoutWaitTime) >= 0)
                    {
                        return new byte[0];
                    }
                }
            }
            return buffer;
        }


        /// <summary>
        /// Returns bytes read from stream or array bigger than defined max size
        /// if time-out occurs before all bytes could be read
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="packetSize"></param>
        /// <returns></returns>
        private byte[] AwaitDataPacket(NetworkStream networkStream,
            int packetSize)
        {
            DateTime lastReadTime = DateTime.Now;
            TimeSpan timeoutWaitTime = new TimeSpan(0, 0,
                ConfigManager.ReadInt(
                    ConfigManager.TCP_SECONDS_TO_TIMEOUT));
            int bytesToRead = packetSize;
            int bytesReadTotal = 0;
            byte[] buffer = new byte[packetSize];
            while (bytesToRead > 0)
            {
                int bytesRead = networkStream.Read(
                    buffer, bytesReadTotal, bytesToRead);
                if (bytesRead > 0)
                {
                    bytesToRead -= bytesRead;
                    bytesReadTotal += bytesRead;
                }
                else
                {
                    TimeSpan idleTime =
                        DateTime.Now.Subtract(lastReadTime);
                    if (idleTime.CompareTo(timeoutWaitTime) >= 0)
                    {
                        return new byte[MAX_DATA_PACKET_SIZE + 1];
                    }
                }
            }
            return buffer;
        }


        private byte[] CreateTCPDataPacket(byte[] request,
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

        private byte[] SerializeString(String s)
        {
            return StringToByteArray(EncodeStringBase64(s));
        }

        private String DeserializeString(byte[] b)
        {
            return DecodeStringBase64(ByteArrayToString(b));
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
