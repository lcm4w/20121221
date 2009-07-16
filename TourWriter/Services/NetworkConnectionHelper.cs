using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace TourWriter.Services
{
    class NetworkConnectionHelper
    {
        [Flags]
        enum ConnectionState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        [DllImport("wininet", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref ConnectionState lpdwFlags, int dwReserved);

        /// <summary>
        /// Test connectivity using the InternetGetConnectedState function from the system DLL Winnet.
        /// </summary>
        /// <returns>True if connection was successful</returns>
        public static bool IsOffline()
        {
            ConnectionState state = 0;
            InternetGetConnectedState(ref state, 0);
            if (((int)ConnectionState.INTERNET_CONNECTION_OFFLINE & (int)state) != 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Test connectivity using System.Net.NetworkInformation.Ping class.
        /// </summary>
        /// <param name="address">Address to connect to</param>
        /// <returns>True if connection was successful</returns>
        public static bool Ping(string address)
        {
            //set options ttl=128 and no fragmentation
            PingOptions options = new PingOptions(128, true);

            Ping ping = new Ping();

            //32 empty bytes buffer
            byte[] data = new byte[32];

            int timeout = 1000;
            PingReply reply = ping.Send(address, timeout, data, options);

            if (reply != null && reply.Status == IPStatus.Success)
                return true;

            return false;
        }
    }
}
