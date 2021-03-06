﻿using PRI_KATALOGOWANIE_PLIKÓW.classes;
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
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes.TCP
{
    public class TcpCommunicationHandler
    {
        Main_form uiForm;


        private static readonly int PORT = 8080;
        
        /// <summary>
        /// Maximum byte count of data sent in a single data packet
        /// </summary>
        private static readonly int MAX_DATA_PACKET_SIZE = 4 * 1024;
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
        bool surpress_errors = true;


        public TcpCommunicationHandler(Main_form mainForm)
        {
            this.uiForm = mainForm;

            IPAddress listen_address;
            try
            {
                listen_address = IPAddress.Parse(ConfigManager.ReadString(ConfigManager.TCP_COMM_IP_ADDRESS));
            }
            catch
            {
                listen_address = IPAddress.Any;
            }

            serverListener = new TcpListener(listen_address, PORT);
            clientBGWorkers = new List<BackgroundWorker>();
            _clientThreadLocks = new List<object>();
            //tcpListener

            StartServer(uiForm);
        }

        public void Shutdown()
        {
            foreach (BackgroundWorker client in clientBGWorkers)
            {
                StopClient(client);
            }
            StopServer();
        }


        public void RequestFile(List<DistributedNetworkFile> dnFileList, String localDownloadDir,
            DistributedNetworkUser targetUser)
        {
            // Console.WriteLine("TcpCommunicationHandler:RequestFile");
            List<object> args = new List<object>()
            {
                dnFileList,
                localDownloadDir
            };
            StartClient(targetUser, TcpRequestCodebook.SEND_FILE, args);
        }


#region server threading logic
        /// <summary>
        /// Starts a background thread using TcpListener to await 
        /// requests.
        /// </summary>
        private void StartServer(Main_form form)
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
                form,
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

                // Console.WriteLine("Server's local endpoint: " +
                //    ((IPEndPoint)serverListener.LocalEndpoint).Address.ToString() + ":" +
                //    ((IPEndPoint)serverListener.LocalEndpoint).Port.ToString());
                // Console.WriteLine("Awaiting connection...");
                TcpClient tcpClient = null;
                try
                {
                    tcpClient = serverListener.AcceptTcpClient();
                }
                catch(Exception ex)
                {
                    //DisplayMessageBoxInMainForm(mainForm, "Exception while accepting client connection: " + ex.Message);
                }
                // Console.WriteLine("Connection request received from " +
                //    ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString() + ":" +
                //    ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString());
                if (!bgWorker.CancellationPending)
                {
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
            }

            // Stop serverListener after receiving worker cancellation
            // signal
            try
            {
                serverListener.Stop();
            }
            catch(SocketException ex) {
                // Console.WriteLine(ex.Message.ToString());
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
                // Console.WriteLine(e.Message);
            }
        }

        private void ServerStoppedEvent(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (!e.Error.Message.Equals(""))
                {
                    System.Windows.Forms.MessageBox.Show(e.Error.Message);
                }

                if (!((String)e.Result).Equals(""))
                {
                    System.Windows.Forms.MessageBox.Show((String)e.Result);
                }
            }
        }
#endregion


#region client threading logic
        private void StartClient(DistributedNetworkUser targetUser,
            byte[] requestToServer, List<object> requestArgs)
        {
            // Console.WriteLine("StartClient");
            BackgroundWorker clientBGWorker = new BackgroundWorker();
            clientBGWorkers.Add(clientBGWorker);
            object _threadLock = new object();
            _clientThreadLocks.Add(_threadLock);
            clientBGWorker.WorkerSupportsCancellation = true;
            clientBGWorker.WorkerReportsProgress = true;
            clientBGWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(StopClientEvent);
            //List<object> doWorkArgs = new List<object>()
            //{
            //    UIForm,
            //    _threadLock,
            //    requestToServer,
            //    requestArgs
            //};
            //clientBGWorker.DoWork += new DoWorkEventHandler(RunClientEvent);
            //clientBGWorker.RunWorkerAsync();
            List<object> doWorkArgs = new List<object>()
            {
                uiForm,
                _threadLock,
                requestToServer,
                requestArgs,
                targetUser
            };
            clientBGWorker.DoWork += (s, args) =>
            {
                RunClient(ref doWorkArgs);
            };
            clientBGWorker.RunWorkerAsync();
        }

        //private void RunClientEvent(object sender, DoWorkEventArgs e)
        //{
        //    BackgroundWorker bgWorker = sender as BackgroundWorker;

        //    RunClient(ref bgWorker, ref e);
        //}

        //private void RunClient(ref BackgroundWorker sender,
        //    ref DoWorkEventArgs e)
        //{

        //}

        private void RunClient(ref List<object> args)
        {
            Main_form mainForm = args.ElementAt(0) as Main_form;
            object _threadLock = args.ElementAt(1) as object;
            byte[] requestToServer = args.ElementAt(2) as byte[];

            DistributedNetworkUser targetUser =
                args.ElementAt(4) as DistributedNetworkUser;

            
            // Console.WriteLine("Attempted connection to " + targetUser.IPAddress.ToString());

            if(TcpRequestCodebook.IsRequest(
                requestToServer, TcpRequestCodebook.SEND_FILE))
            {
                // Console.WriteLine("Running client requesting file");
                List<int> sendFileResult = new List<int>();
                List<object> requestArgs = args.ElementAt(3) as List<object>;
                List<DistributedNetworkFile> dnFileList = requestArgs.ElementAt(0) as List<DistributedNetworkFile>;
                String localDownloadDir = requestArgs.ElementAt(1) as String;

                foreach (DistributedNetworkFile dnFile in dnFileList)
                {
                    TcpClient tcpClient = null;
                    NetworkStream networkStream = null;
                    try
                    {
                        tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse(ConfigManager.ReadString(ConfigManager.TCP_COMM_IP_ADDRESS)), PORT));

                        // Żeby czas był w sekundach mnożymy wartość z konfiga o 1000.
                        // Podana wartośc zawsze brana jest ~ *2, zakładam że robi retry połącznia za pierwszym failem...
                        tcpClient.ReceiveTimeout = ConfigManager.ReadInt(ConfigManager.TCP_SECONDS_TO_TIMEOUT) * 1000;

                        tcpClient.Connect(targetUser.IPAddress, PORT);
                        networkStream = tcpClient.GetStream();
                    }
                    catch (Exception ex)
                    {
                        DisplayMessageBoxInMainForm(mainForm, "Błąd podczas nawiązywania połączenia z użytkownikiem o aliasie: " + targetUser.Alias + '\n'
                                                            + "Szczegóły:\n" + ex.Message);
                    }
                    if (tcpClient == null || networkStream == null)
                    {
                        // Coś poszło nie tak z robieniem gniazd.
                        Console.WriteLine("Coś poszło nie tak z robieniem gniazd i/lub streamów");
                    }
                    else
                    {
                        int result = SendFileRequest(networkStream, dnFile, localDownloadDir + dnFile.realFileName, mainForm);
                        sendFileResult.Add(result);
                    }

                    if (networkStream != null) networkStream.Close();
                    if (tcpClient != null) tcpClient.Close();
                }
            }

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

            int firstByte = AwaitNonNegativeByte(networkStream, mainForm);
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
                int requestCode = AwaitNonNegativeByte(networkStream, mainForm);
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
                    // Console.WriteLine("Received client request: send file");
                    SendFileRequestCallback(mainForm, networkStream);
                }
                /*
                else if (TcpRequestCodebook.IsRequest(
                    requestCode, TcpRequestCodebook.SEND_CATALOGUE))
                {
                    // Console.WriteLine("Received client request: send file");
                    SendCatalogueRequestCallback(mainForm, networkStream);
                }
                */
                else
                {
                    // Console.WriteLine("Received unrecognized client request");
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
                // Console.WriteLine(ex.Message);
            }
        }

        
        public int SendFileRequest(NetworkStream networkStream,
            DistributedNetworkFile dnFile, String localDownloadPath, Main_form mainForm)
        {
            Console.WriteLine("Wszedłem do ściągania pliku " + dnFile.realFileName + " o czasie " + DateTime.Now.ToString());
            String realFilePath = dnFile.realFilePath;
            byte[] serializedFilePath;
            if (dnFile.allowUnpromptedDistribution)
            {
                serializedFilePath = SerializeString(realFilePath + "1");
            }
            else
            {
                serializedFilePath = SerializeString(realFilePath + "0");
            }

            byte[] packetPayload = serializedFilePath;

            byte[] packet = CreateTCPDataPacket(
                TcpRequestCodebook.SEND_FILE, packetPayload);
            networkStream.Write(packet, 0, packet.Length);
            networkStream.Flush();
            // Console.WriteLine("SendFileRequest: requested " + dnFile.realFilePath);

            int initByte = AwaitNonNegativeByte(networkStream, mainForm);
            if (initByte == -1)
            {
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_TIMEOUT;
            }
            int requestByte = AwaitNonNegativeByte(networkStream, mainForm);
            if (requestByte == -1)
            {
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_TIMEOUT;
            }
            if (requestByte != TcpRequestCodebook.SENDING_FILE_FRAGMENT[0])
            {
                if (requestByte == TCP.TcpRequestCodebook.NO_EXTERNAL_CATALOG[0])
                {
                    byte[] sizeBytes = AwaitPacketSize(networkStream, mainForm);
                    if (sizeBytes.Length == 0)
                    {
                        return RETURN_TIMEOUT;
                    }
                    int sizeofPacket = ByteArrayToInt(sizeBytes);

                    int separator = AwaitNonNegativeByte(networkStream, mainForm);
                    if (separator == -1)
                    {
                        return RETURN_TIMEOUT;
                    }

                    byte[] serializedData = AwaitDataPacket(networkStream, sizeofPacket, mainForm);
                    string deserializedData = DeserializeString(serializedData);

                    string[] deserializedDataChunks = deserializedData.Split('|');


                    AddAliasInMainForm(mainForm, deserializedDataChunks[0], deserializedDataChunks[1]);
                    DisplayMessageBoxInMainForm(mainForm, "Podany użytkownik nie wygenerował katalogu obiegowego!");
                    AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                    KillProgressWindowInMainForm(mainForm);
                    CheckIfDoneInMainForm(mainForm);
                    return RETURN_CANCEL;
                }
                else
                {
                    IncrementFailedDownloadCountInMainForm(mainForm);
                    DisplayMessageBoxInMainForm(mainForm, "Błąd przesyłania pliku: " + dnFile.realFileName);
                    AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                    KillProgressWindowInMainForm(mainForm);
                    CheckIfDoneInMainForm(mainForm);
                    return RETURN_BAD_REQUEST;
                }
            }
                
            // Console.WriteLine("SendFileRequest: received request:" + requestByte);

            byte[] packetSizeBytes = AwaitPacketSize(networkStream, mainForm);
            if (packetSizeBytes.Length == 0)
            {
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_TIMEOUT;
            }
            int packetSize = ByteArrayToInt(packetSizeBytes);

            int separatorByte = AwaitNonNegativeByte(networkStream, mainForm);

            if (separatorByte == -1)
            {
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_TIMEOUT;
            }
            

            byte[] fileFragmentPacket =
                AwaitDataPacket(networkStream, packetSize, mainForm);

            int bytesReceived = packetSize;
            int bytesWritten = 0;

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(
                localDownloadPath, FileMode.OpenOrCreate);
            }
            catch(Exception ex)
            {
                if (!surpress_errors) DisplayMessageBoxInMainForm(mainForm, "file download stopped, unable to open file stream at " +
                                                                            localDownloadPath + "\n" + ex.Message);
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_CANCEL;
            }
            try
            {
                fileStream.Write(fileFragmentPacket, bytesWritten, fileFragmentPacket.Length);
                fileStream.Flush();
            }
            catch(Exception ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm,
                    "file download stopped, unable to write to file stream at " +
                    localDownloadPath + "\n" + ex.Message);
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_CANCEL;
            }
            bytesWritten += fileFragmentPacket.Length;

            byte[] responsePacket = CreateTCPDataPacket(
                TcpRequestCodebook.CONTINUE_SENDING_FILE,
                new byte[0]);
            try
            {
                networkStream.Write(responsePacket, 0, responsePacket.Length);
                networkStream.Flush();
            }
            catch (Exception ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm,
                    "File download stopped, unable to write to network stream.\n" + ex.Message);
                IncrementFailedDownloadCountInMainForm(mainForm);
                AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                KillProgressWindowInMainForm(mainForm);
                CheckIfDoneInMainForm(mainForm);
                return RETURN_CANCEL;
            }
            // Console.WriteLine("SendFileRequest: sent request: Continue sending file");


            if(!dnFile.realFileName.Equals("EXTERNAL_CATALOG.FDB")) SpawnProgressWindowInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath), (int)dnFile.fileSize);
            while (true)
            {
                initByte = AwaitNonNegativeByte(networkStream, mainForm);
                if (initByte == -1)
                {
                    // Console.WriteLine("Timeout in SendFileRequest");
                    fileStream.Close();
                    IncrementFailedDownloadCountInMainForm(mainForm);
                    AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                    KillProgressWindowInMainForm(mainForm);
                    CheckIfDoneInMainForm(mainForm);
                    return RETURN_TIMEOUT;
                }
                requestByte = AwaitNonNegativeByte(networkStream, mainForm);
                if (requestByte == -1)
                {
                    // Console.WriteLine("Timeout in SendFileRequest");
                    fileStream.Close();
                    IncrementFailedDownloadCountInMainForm(mainForm);
                    AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                    KillProgressWindowInMainForm(mainForm);
                    CheckIfDoneInMainForm(mainForm);
                    return RETURN_TIMEOUT;
                }
                // Console.WriteLine("SendFileRequest: received request: " + requestByte);
                if (!TcpRequestCodebook.IsRequest(
                    requestByte, TcpRequestCodebook.SENDING_FILE_FRAGMENT))
                {
                    //// Console.WriteLine("SendFileRequest() request code received is not SENDING_FILE_FRAGMENT");
                    if (TcpRequestCodebook.IsRequest(
                        requestByte, TcpRequestCodebook.DONE_SENDING_FILE))
                    {
                        // Pobieramy wartość przesłanego nam stringa.
                        byte[] sizeBytes = AwaitPacketSize(networkStream, mainForm);
                        if (sizeBytes.Length == 0)
                        {
                            return RETURN_TIMEOUT;
                        }
                        int sizeofPacket = ByteArrayToInt(sizeBytes);

                        int separator = AwaitNonNegativeByte(networkStream, mainForm);
                        if (separator == -1)
                        {
                            return RETURN_TIMEOUT;
                        }

                        byte[] serializedData = AwaitDataPacket(networkStream, sizeofPacket, mainForm);
                        string deserializedData = DeserializeString(serializedData);
                        
                        // Jeżeli kopiowaliśmy katalog - zmieniamy nazwę kopii roboczej na rzeczywistą nazwę.
                        if (dnFile.realFileName.Equals("EXTERNAL_CATALOG.FDB"))
                        {
                            string file_location = ConfigManager.ReadString(ConfigManager.EXTERNAL_DATABASES_LOCATION);
                            string target_filename = file_location + deserializedData;
                            AttemptToFinalizeInMainForm(mainForm, target_filename);
                        }

                        AddSuccessfulDownloadNameInMainForm(mainForm, dnFile.realFileName);
                        IncrementSuccessfulDownloadCountInMainForm(mainForm);
                        CheckIfDoneInMainForm(mainForm);
                        KillProgressWindowInMainForm(mainForm);
                    }
                    break;
                }
                else
                {
                    packetSizeBytes = null;
                    packetSizeBytes = AwaitPacketSize(networkStream, mainForm);
                    if (packetSizeBytes.Length == 0)
                    {
                        // Console.WriteLine("Timeout in SendFileRequest");
                        fileStream.Close();
                        IncrementFailedDownloadCountInMainForm(mainForm);
                        AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                        KillProgressWindowInMainForm(mainForm);
                        CheckIfDoneInMainForm(mainForm);
                        return RETURN_TIMEOUT;
                    }
                    packetSize = ByteArrayToInt(packetSizeBytes);
                    bytesReceived += packetSize;

                    separatorByte = AwaitNonNegativeByte(networkStream, mainForm);
                    if (separatorByte == -1)
                    {
                        // Console.WriteLine("Timeout in SendFileRequest");
                        fileStream.Close();
                        IncrementFailedDownloadCountInMainForm(mainForm);
                        AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                        KillProgressWindowInMainForm(mainForm);
                        CheckIfDoneInMainForm(mainForm);
                        return RETURN_TIMEOUT;
                    }

                    fileFragmentPacket = null;
                    fileFragmentPacket = AwaitDataPacket(networkStream, packetSize, mainForm);
                    try
                    {
                        fileStream.Write(fileFragmentPacket, 0, fileFragmentPacket.Length);
                        UpdateProgressWindowInMainForm(mainForm);
                        fileStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        if (!surpress_errors)
                        DisplayMessageBoxInMainForm(mainForm,
                            "File download stopped, unable to write to file stream at " +
                            localDownloadPath + "\n" + ex.Message);
                        IncrementFailedDownloadCountInMainForm(mainForm);
                        AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                        KillProgressWindowInMainForm(mainForm);
                        CheckIfDoneInMainForm(mainForm);
                        return RETURN_CANCEL;
                    }
                    bytesWritten += fileFragmentPacket.Length;

                    responsePacket = null;
                    responsePacket = CreateTCPDataPacket(
                        TcpRequestCodebook.CONTINUE_SENDING_FILE,
                        new byte[0]);
                    try
                    {
                        networkStream.Write(responsePacket, 0, responsePacket.Length);
                        networkStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        if (!surpress_errors)
                        DisplayMessageBoxInMainForm(mainForm,
                            "File download stopped, unable to write to network stream at " +
                            localDownloadPath + "\n" + ex.Message);
                        IncrementFailedDownloadCountInMainForm(mainForm);
                        AddFailedDownloadNameInMainForm(mainForm, Path.GetFileName(dnFile.realFilePath));
                        KillProgressWindowInMainForm(mainForm);
                        CheckIfDoneInMainForm(mainForm);
                        return RETURN_CANCEL;
                    }
                    // Console.WriteLine("SendFileRequest: sent request: Continue sending file");
                }
            }

            packet = null;
            serializedFilePath = null;

            fileStream.Close();
            fileStream.Dispose();

            return RETURN_OK;
        }


        public int SendFileRequestCallback(Main_form mainForm,
            NetworkStream networkStream)
        {
            // Reading size of upcoming packet
            bool sendsExternalCatalog = false, unpromptedDistribution = false;
            byte[] sizeBytes = AwaitPacketSize(networkStream, mainForm);
            if (sizeBytes.Length == 0) return RETURN_TIMEOUT;
            int packetSize = ByteArrayToInt(sizeBytes);

            int separator = AwaitNonNegativeByte(networkStream, mainForm);
            if (separator == -1) {
                return RETURN_TIMEOUT;
            }

            byte[] serializedPacketPayload = new byte[packetSize];

            serializedPacketPayload =
                AwaitDataPacket(networkStream, packetSize, mainForm);

            string deserializedPacketPayload = DeserializeString(serializedPacketPayload);

            if(deserializedPacketPayload[deserializedPacketPayload.Length-1].Equals('1'))
            {
                // Kopiujemy bez pytania
                unpromptedDistribution = true;
            }
            else
            {
                // Kopiujemy z pytaniem
                unpromptedDistribution = false;
            }
            
            String filePath = deserializedPacketPayload.Remove(deserializedPacketPayload.Length-1);
            String fileName = "";
            long fileSize = 0;

            if (!filePath.Equals("TO_DETERMINE"))
            {
                // Standardowa logika przesyłania.
                fileName = Path.GetFileName(filePath);
                fileSize = new FileInfo(filePath).Length;
            }
            else
            {
                filePath = ConfigManager.ReadString(ConfigManager.EXTERNAL_DATABASES_LOCATION) +
                           ConfigManager.ReadString(ConfigManager.USER_ALIAS).ToUpper() + "_CATALOG.FDB";

                // Logika pobierania katalogu
                sendsExternalCatalog = true;
                // Najpierw rozkazujemy mainformowi zerwać wszystkie połączenia z bazą
                TerminateDBConnectionsInMainForm(mainForm);

                fileName = Path.GetFileName(filePath);

                if (new FileInfo(filePath).Exists)
                {
                    fileSize = new FileInfo(filePath).Length;
                }
                else
                {
                    fileSize = 0;
                }
            }

            // Console.WriteLine("SendFileRequestCallback: Received request for file " + filePath);

            DistributedNetworkFile distributedNetworkFile = new DistributedNetworkFile(fileName,filePath,fileSize, true, unpromptedDistribution);

            if(!sendsExternalCatalog)
            {
                if (!distributedNetworkFile.IsPresentInLocalCatalogue())
                {
                    byte[] responsePacket = CreateTCPDataPacket(
                        TcpRequestCodebook.FILE_NOT_IN_MY_CATALOGUE,
                        SerializeString("FileNotFound"));

                    try
                    {
                        networkStream.Write(responsePacket, 0, responsePacket.Length);
                        networkStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        if (!surpress_errors)
                        DisplayMessageBoxInMainForm(mainForm,
                            "Cannot send \"file not found\" response: " +
                            ex.Message);
                    }
                    return RETURN_OK;
                }
            } 
            else
            {
                // Sprawdzamy czy został wygenerowany katalog obiegowy
                FileInfo externalCatalogChecker = new FileInfo(distributedNetworkFile.realFilePath);

                if(!externalCatalogChecker.Exists)
                {
                    string[] username_grabber = fileName.Split('_');
                    
                    // Katalog obiegowy nie istnieje, wysyłamy przerwanie.
                    byte[] responsePacket = CreateTCPDataPacket(
                        TcpRequestCodebook.NO_EXTERNAL_CATALOG,
                        SerializeString(username_grabber[0] + '|' + ConfigManager.ReadString(ConfigManager.TCP_COMM_IP_ADDRESS)));

                    try
                    {
                        networkStream.Write(responsePacket, 0, responsePacket.Length);
                        networkStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        if (!surpress_errors)
                        DisplayMessageBoxInMainForm(mainForm,
                            "Cannot send \"file not found\" response: " +
                            ex.Message);
                    }
                    return RETURN_CANCEL;
                }
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

                    try
                    {
                        networkStream.Write(responsePacket, 0, responsePacket.Length);
                        networkStream.Flush();
                    }
                    catch(Exception ex)
                    {
                        if (!surpress_errors)
                        DisplayMessageBoxInMainForm(mainForm,
                            "Cannot send \"refused to send file\" response, " + 
                            ex.Message);
                    }
                    return RETURN_OK;
                }
            }
            // Console.WriteLine("SendFileRequestCallback: canSendFile == " + canSendFile);

            // serialize and send file

            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(distributedNetworkFile.realFilePath);
            }
            catch(Exception ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm,
                    "File upload halted, cannot acquire file information, " +
                    ex.Message);
                return RETURN_CANCEL;
            }
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(
                    distributedNetworkFile.realFilePath, FileMode.Open);
            }
            catch(Exception ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm,
                    "File upload halted, cannot open file stream: " +
                    ex.Message);
                return RETURN_CANCEL;
            }

            long totalBytesRead = 0;
            long remainingFileBytes = fileInfo.Length;
            // Console.WriteLine("SendFileRequestCallback: remainingFileBytes == " + remainingFileBytes);
            while (remainingFileBytes > 0)
            {
                long fileFragmentSize =
                    Math.Min(remainingFileBytes, MAX_DATA_PACKET_SIZE);
                byte[] fileFragment = new byte[fileFragmentSize];
                int bytesRead = 0;
                try
                {
                    bytesRead = fileStream.Read(fileFragment, 0, fileFragment.Length);
                }
                catch(Exception ex)
                {
                    if (!surpress_errors)
                    DisplayMessageBoxInMainForm(mainForm,
                        "File upload halted, unable to read from file stream: " +
                        ex.Message);
                    return RETURN_CANCEL;
                }
                totalBytesRead += bytesRead;
                remainingFileBytes -= bytesRead;

                byte[] fileFragmentPacket = CreateTCPDataPacket(
                    TcpRequestCodebook.SENDING_FILE_FRAGMENT,
                    fileFragment);
                // Console.WriteLine("SendFileRequestCallback: Sending file fragment");
                try
                {
                    networkStream.Write(fileFragmentPacket, 0,
                        fileFragmentPacket.Length);
                    networkStream.Flush();
                }
                catch (Exception ex)
                {
                    if (!surpress_errors)
                    DisplayMessageBoxInMainForm(mainForm,
                        "File upload halted, unable to write to network stream: " +
                        ex.Message);
                    return RETURN_CANCEL;
                }

                int initByte = AwaitNonNegativeByte(networkStream, mainForm);
                if (initByte == -1)
                {
                    fileStream.Close();
                    return RETURN_TIMEOUT;
                }
                int requestCode = AwaitNonNegativeByte(networkStream, mainForm);
                if (requestCode == -1)
                {
                    fileStream.Close();
                    return RETURN_TIMEOUT;
                }
                byte[] receivedPacketSize = AwaitPacketSize(networkStream, mainForm);
                if (receivedPacketSize.Length == 0)
                {
                    fileStream.Close();
                    return RETURN_TIMEOUT;
                }
                packetSize = ByteArrayToInt(receivedPacketSize);
                int separatorByte = AwaitNonNegativeByte(networkStream, mainForm);
                if (separatorByte == -1)
                {
                    fileStream.Close();
                    return RETURN_TIMEOUT;
                }
                byte[] packetData = AwaitDataPacket(networkStream, packetSize, mainForm);
                if (packetData.Length > MAX_DATA_PACKET_SIZE)
                {
                    fileStream.Close();
                    return RETURN_TIMEOUT;
                }

                if(requestCode != TcpRequestCodebook.CONTINUE_SENDING_FILE[0])
                {
                    byte[] responsePacket = CreateTCPDataPacket(
                        TcpRequestCodebook.TERMINATE,
                        SerializeString("Bad response"));
                    try
                    {
                        networkStream.Write(responsePacket, 0,
                            responsePacket.Length);
                        networkStream.Flush();
                    }
                    catch(Exception ex)
                    {
                        if (!surpress_errors)
                        DisplayMessageBoxInMainForm(mainForm,
                            "Unable to send \"bad response\" response, " +
                            "cannot write to network stream.\n" + ex.Message);
                    }
                    return RETURN_BAD_REQUEST;
                }
            }

            fileStream.Close();
            string final_message;
            if(sendsExternalCatalog)
            {
                final_message = fileName;
            }
            else
            {
                final_message = "Entire file has been sent";
            }
            byte[] finalMessagePacket = CreateTCPDataPacket(
                TcpRequestCodebook.DONE_SENDING_FILE,
                SerializeString(final_message));
            try
            {
                networkStream.Write(finalMessagePacket, 0, finalMessagePacket.Length);
                networkStream.Flush();
            }
            catch (Exception ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm,
                    "Cannot finalize file uplodad, unable to write to network stream: " +
                    ex.Message);
                return RETURN_CANCEL;
            }
            finalMessagePacket = null;
            finalMessagePacket = CreateTCPDataPacket(
                 TcpRequestCodebook.TERMINATE,
                 SerializeString("End connection"));
            try
            {
                networkStream.Write(finalMessagePacket, 0, finalMessagePacket.Length);
                networkStream.Flush();
            }
            catch(Exception ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm,
                    "File upload connection: cannot request connection termination: " +
                    ex.Message);
            }

            return RETURN_OK;


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
                canSendFile = mainForm.RequestFileTransferPermission(distributedNetworkFile);
            });
            return canSendFile;
        }

        private void DisplayMessageBoxInMainForm(Main_form mainFormRef,
            String msg)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.DisplayMessageBoxFromAnotherThread(msg);
            });
        }

        private void AddSuccessfulDownloadNameInMainForm(Main_form mainFormRef,
            String filename)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.AddSuccessfulDownloadNameFromAnotherThread(filename);
            });
        }

        private void AddFailedDownloadNameInMainForm(Main_form mainFormRef,
            String filename)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.AddFailedDownloadNameFromAnotherThread(filename);
            });
        }

        private void IncrementSuccessfulDownloadCountInMainForm(Main_form mainFormRef)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.IncrementSuccessfulDownloadCountFromAnotherThread();
            });
        }

        private void IncrementFailedDownloadCountInMainForm(Main_form mainFormRef)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.IncrementFailedDownloadCountFromAnotherThread();
            });
        }

        private void SpawnProgressWindowInMainForm(Main_form mainFormRef, string fileName, int fileSize)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.SpawnProgressWindowFromAnotherThread(fileName,fileSize);
            });
        }

        private void UpdateProgressWindowInMainForm(Main_form mainFormRef)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.UpdateProgressWindowFromAnotherThread();
            });
        }

        private void KillProgressWindowInMainForm(Main_form mainFormRef)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.KillProgressWindowFromAnotherThread();
            });
        }

        private void CheckIfDoneInMainForm(Main_form mainFormRef)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.CheckIfDoneFromAnotherThread();
            });
        }

        private void AddAliasInMainForm(Main_form mainFormRef, 
            String alias,
            String address)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.AddAliasFromAnotherThread(alias,address);
            });
        }

        private string GrabExternalCatalogPathInMainForm(Main_form mainFormRef)
        {
            string result_from_main = "";

            mainFormRef.Invoke((Action)delegate ()
            {
                result_from_main = mainFormRef.GrabExternalCatalogPathFromAnotherThread();
            });

            return result_from_main;
        }

        private void TerminateDBConnectionsInMainForm(Main_form mainFormRef)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.TerminateDBConnectionsFromAnotherThread();
            });
        }

        private void AttemptToFinalizeInMainForm(Main_form mainFormRef, string target_filename)
        {
            mainFormRef.Invoke((Action)delegate ()
            {
                mainFormRef.AttemptToFinalizeFromAnotherThread(target_filename);
            });
        }

        /// <summary>
        /// Will return first non-negative byte from NetworkStream
        /// or -1 if timeout occurs
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        private int AwaitNonNegativeByte(NetworkStream networkStream, Main_form mainForm)
        {
            DateTime lastReadTime = DateTime.Now;
            TimeSpan timeoutWaitTime = new TimeSpan(0, 0, 
                ConfigManager.ReadInt(
                    ConfigManager.TCP_SECONDS_TO_TIMEOUT));
            int byteRead = -1;
            try
            {
                byteRead = networkStream.ReadByte();
                while (byteRead == -1)
                {
                    byteRead = networkStream.ReadByte();
                    if (byteRead == -1)
                    {
                        TimeSpan idleTime = DateTime.Now.Subtract(lastReadTime);
                        if (idleTime.CompareTo(timeoutWaitTime) >= 0)
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm, "Execution stopped due to socket exception: " + ex.Message);
                return -1;
            }
            catch (IOException ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm, "Execution stopped due to I/O exception: " + ex.Message);
                return -1;
            }
            return byteRead;
        }


        /// <summary>
        /// Returns bytes read from stream or empty byte array if 
        /// time-out occurs before all bytes could be read
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        private byte[] AwaitPacketSize(NetworkStream networkStream, Main_form mainForm)
        {
            DateTime lastReadTime = DateTime.Now;
            TimeSpan timeoutWaitTime = new TimeSpan(0, 0, 
                ConfigManager.ReadInt(
                    ConfigManager.TCP_SECONDS_TO_TIMEOUT));
            int bytesToRead = DATA_SIZE_BYTE_ARRAY_LENGTH;
            int bytesReadTotal = 0;
            byte[] buffer = new byte[DATA_SIZE_BYTE_ARRAY_LENGTH];
            try
            {
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
                            return new byte[0];
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm, "Execution stopped due to socket exception: " + ex.Message);
                return new byte[0];
            }
            catch (IOException ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm, "Execution stopped due to I/O exception: " + ex.Message);
                return new byte[0];
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
            int packetSize, Main_form mainForm)
        {
            DateTime lastReadTime = DateTime.Now;
            TimeSpan timeoutWaitTime = new TimeSpan(0, 0,
                ConfigManager.ReadInt(
                    ConfigManager.TCP_SECONDS_TO_TIMEOUT));
            int bytesToRead = packetSize;
            int bytesReadTotal = 0;
            byte[] buffer = new byte[packetSize];
            try
            {
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
            }
            catch (SocketException ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm, "Execution stopped due to socket exception: " + ex.Message);
                return new byte[MAX_DATA_PACKET_SIZE + 1];
            }
            catch (IOException ex)
            {
                if (!surpress_errors)
                DisplayMessageBoxInMainForm(mainForm, "Execution stopped due to I/O exception: " + ex.Message);
                return new byte[MAX_DATA_PACKET_SIZE + 1];
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
