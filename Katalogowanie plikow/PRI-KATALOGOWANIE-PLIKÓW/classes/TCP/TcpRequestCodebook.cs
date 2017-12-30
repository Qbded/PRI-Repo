﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes.TCP
{
    static class TcpRequestCodebook
    {
        //public static readonly String REQUEST_FILE = "request file";
        //public static readonly String REQUEST_CATALOGUE = "request catalogue";

        public static readonly byte[] INITIALIZE = { 2 };
        public static readonly byte[] SEPARATOR = { 4 };

        public static readonly byte[] SEND_FILE = { 20 };
        public static readonly byte[] SEND_CATALOGUE = { 21 };

        public static readonly byte[] START_SENDING_FILE = { 120 };
        public static readonly byte[] CONTINUE_SENDING_FILE = { 121 };
        public static readonly byte[] SENDING_FILE_FRAGMENT = { 122 };
        public static readonly byte[] DONE_SENDING_FILE = { 123 };

        public static readonly byte[] TERMINATE = { 255 };

        //public static readonly int INITIALIZE = 2;
        //public static readonly int SEPARATOR = 4;

        //public static readonly int REQUEST_FILE = 20;
        //public static readonly int REQUEST_CATALOGUE = 21;

        //public static readonly int START_SENDING_FILE = 120;
        //public static readonly int CONTINUE_SENDING_FILE = 121;
        //public static readonly int SENDING_FILE_FRAGMENT = 122;
        //public static readonly int DONE_SENDING_FILE = 123;


        public static bool IsRequest(int request, byte requestCode)
        {
            if (request > 255 || request < 0) return false;
            if (request == requestCode) return true;
            else return false;
        }

        public static bool IsRequest(byte[] request, byte[] requestCode)
        {
            return request.SequenceEqual(requestCode);
        }

        public static bool IsRequest(int request, byte[] requestCode)
        {
            if(requestCode.Length == 1)
            {
                return IsRequest(request, requestCode[0]);
            }
            else
            {
                byte[] requestBytes = IntToByteArray(request);
                return IsRequest(requestBytes, requestCode);
            }
        }
    }
}
