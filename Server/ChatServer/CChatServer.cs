using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace ChatRoom
{
    class CChatServer
    {
        public CChatServer()
        {
        }

        public void Bind( int iPort )
        {
            m_iPort = iPort;

            m_theListener = new TcpListener( IPAddress.Any, iPort );
            //IPAddress[] ipAddresses = Dns.GetHostAddresses( "127.0.0.1" );
            //m_theListener = new TcpListener( ipAddresses[0], iPort );

            m_theListener.Start();

            Console.WriteLine( "Chat Server port " + iPort + " binded..." );
        }

        public void RunAccept()
        {
            System.Threading.Thread theThread = new System.Threading.Thread( _RunTCPClients );
            theThread.Start();

            while ( true )
            {

                TcpClient theClient = m_theListener.AcceptTcpClient();
                Console.WriteLine( "A client has connected..." );

                lock( m_setTCPClients )
                {
                    m_setTCPClients.Add( theClient );
                }
            }

        }

        void _RunTCPClients()
        {
            while ( true )
            {
                if( m_setTCPClients.Count == 0 ) continue;

                TcpClient[] aTcpClients = null;
                lock( m_setTCPClients )
                {
                    aTcpClients = new TcpClient[ m_setTCPClients.Count ];
                    m_setTCPClients.CopyTo( aTcpClients );
                }

                foreach ( TcpClient theClient in aTcpClients )
                {
                    bool bRemove = false;

                    if( _IsConnected( theClient ) == false )
                    {
                        bRemove = true;
                    }
                    else
                    {
                        try
                        {
                            if( theClient.Available > 0 )
                            {
                                _HandleReceiveMessages( theClient );
                            }
                        }
                        catch( Exception )
                        {
                            bRemove = true;
                        }
                    }

                    if( bRemove )
                    {
                        lock( m_setTCPClients )
                        {
                            if( m_setTCPClients.Contains( theClient ) )
                            {
                                Console.WriteLine( "A client name: " + m_dicTCPClientNames[ theClient ] + " has disconnected..." );
                                m_setTCPClients.Remove( theClient );
                                m_dicTCPClientNames.Remove( theClient );
                            }
                        }
                    }
                }
            }
        }

        void _HandleReceiveMessages( TcpClient theClient )
        {
            NetworkStream theClientStream = theClient.GetStream();

            int iNumBytes = theClient.Available;
            byte[] aBuffer = new byte[ iNumBytes ];

            int iBytesRead = theClientStream.Read( aBuffer, 0, iNumBytes );

            string sRequest = System.Text.Encoding.ASCII.GetString( aBuffer ).Substring( 0, iBytesRead );

            if ( sRequest.StartsWith( "LOGINNAME:", StringComparison.OrdinalIgnoreCase ) )
            {
                string[] aTokens = sRequest.Split( ':' );
                m_dicTCPClientNames.Add( theClient, aTokens[ 1 ] );
                Console.WriteLine( "...and the client name is: " + aTokens[1] );
            }
            else if ( sRequest.StartsWith( "BROADCAST:", StringComparison.OrdinalIgnoreCase ) )
            {
                string[] aTokens = sRequest.Split( ':' );
                string sMessage = aTokens[ 1 ];

                TcpClient[] aTcpClients = null;
                lock( m_setTCPClients )
                {
                    aTcpClients = new TcpClient[ m_setTCPClients.Count ];
                    m_setTCPClients.CopyTo( aTcpClients );
                }

                foreach( TcpClient client in aTcpClients )
                {
                    if( theClient != client )
                    {
                        string sMsgRequest = "MESSAGE:" + m_dicTCPClientNames[ theClient ] + ":" + sMessage;
                        byte[] aRequestBuffer = System.Text.Encoding.ASCII.GetBytes( sMsgRequest );

                        client.GetStream().Write( aRequestBuffer, 0, aRequestBuffer.Length );
                    }
                }
            }
        }

        bool _IsConnected( TcpClient theClient )
        {
            if( theClient.Connected == false ) return false;

            try
            {
                if( theClient.Client.Poll( 0, SelectMode.SelectRead ) )
                {
                    byte[] buff = new byte[ 1 ];
                    if( theClient.Client.Receive( buff, SocketFlags.Peek ) == 0 ) return false;
                }
            }
            catch( Exception )
            {
                return false;
            }

            return true;
        }

        //
        //
        TcpListener m_theListener = null;
        HashSet<TcpClient> m_setTCPClients = new HashSet<TcpClient>();
        Dictionary<TcpClient, string> m_dicTCPClientNames = new Dictionary<TcpClient, string>();
        int m_iPort;
    }
}
