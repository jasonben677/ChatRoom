using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Net;

namespace ChatRoom
{
    class CChatClient
    {
        public CChatClient()
        {
        }

        public bool Connect( string sAddress, int iPort )
        {
            m_sAddress = sAddress;
            m_iPort = iPort;

            m_theClient = new TcpClient();

            try
            {
                IPHostEntry host = Dns.GetHostEntry( sAddress );
                //var address = ( from h in host.AddressList where h.AddressFamily == AddressFamily.InterNetwork select h ).First();
                IPAddress address = null;
                foreach( IPAddress h in host.AddressList )
                {
                    if( h.AddressFamily == AddressFamily.InterNetwork )
                    {
                        address = h;
                        break;
                    }
                }

                m_theClient.Connect( address.ToString(), iPort );
                Console.WriteLine( "Connected to Chat Server: " + sAddress + ":" + iPort + "\n" );

                return true;
            }
            catch( Exception e )
            {
                Console.WriteLine( "Exception happened: " + e.ToString() );
                return false;
            }
        }

        public void SendName( string sName )
        {
            string sRequest = "LOGINNAME:" + sName;
            byte[] aRequestBuffer = System.Text.Encoding.ASCII.GetBytes( sRequest );

            m_theClient.GetStream().Write( aRequestBuffer, 0, aRequestBuffer.Length );
        }
        public void SendBroadcast( string sMessage )
        {
            string sRequest = "BROADCAST:" + sMessage;
            byte[] aRequestBuffer = System.Text.Encoding.ASCII.GetBytes( sRequest );

            m_theClient.GetStream().Write( aRequestBuffer, 0, aRequestBuffer.Length );
        }

        public void Run()
        {
            if( m_theClient.Available > 0 )
            {
                _HandleReceiveMessages( m_theClient );
            }
        }

        public void _HandleReceiveMessages( TcpClient theClient )
        {
            NetworkStream theClientStream = theClient.GetStream();

            int iNumBytes = theClient.Available;
            byte[] aBuffer = new byte[ iNumBytes ];

            int iBytesRead = theClientStream.Read( aBuffer, 0, iNumBytes );

            string sRequest = System.Text.Encoding.ASCII.GetString( aBuffer ).Substring( 0, iBytesRead );

            if( sRequest.StartsWith( "MESSAGE:", StringComparison.OrdinalIgnoreCase ) )
            {
                string[] aTokens = sRequest.Split( ':' );
                string sName = aTokens[ 1 ];
                string sMessage = aTokens[ 2 ];
                Console.WriteLine( sName + " said: " + sMessage );
            }
        }

        //
        //
        TcpClient m_theClient = null;
        string m_sAddress = "";
        int m_iPort;
    }
}
