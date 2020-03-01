using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom
{
    class CMain
    {
        static void Main( string[] args )
        {
            CChatClient client = new CChatClient();

            Console.WriteLine( "<Please enter the name...>" );
            string sName = Console.ReadLine();

            bool bOK = client.Connect( "127.0.0.1", 4099 );
            if( bOK )
            {
                client.SendName( sName );
                Console.WriteLine( "<You can press any key to start entering text...>\n" );

                while( true )
                {
                    while( Console.KeyAvailable == false )
                    {
                        client.Run();
                        System.Threading.Thread.Sleep( 1 );
                    }

                    Console.WriteLine( "\n<Press enter to send text...>" );
                    string sMessage = Console.ReadLine();
                    //client.SendBroadcast( DateTime.Now.Ticks.ToString() );
                    client.SendBroadcast( sMessage );
                    Console.WriteLine( "<...Message sent, press any key to start entering text again...>\n" );
                }
            }

        }
    }
}
