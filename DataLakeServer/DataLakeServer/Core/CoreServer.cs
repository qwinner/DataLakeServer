using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace DataLakeServer.Core
{
    public class CoreServer
    {
        private CoreServer instance = new CoreServer();
        private WebSocketServer mWssv;
        public CoreServer GetInstance()
        {
            return instance;
        }
        private CoreServer()
        {
            InitInstance();
        }

        public void InitInstance()
        {    
            
            mWssv = new WebSocketServer(4649);
            mWssv.AddWebSocketService<EchoService>("/Echo");
            mWssv.AddWebSocketService<ChatService>("/Chat");
            mWssv.AddWebSocketService<ChatService>("/ChatWithNyan", () => new ChatService(" Nyan!"));
            mWssv.Start();            
        }

        ~CoreServer()
        {
            mWssv.Stop();
        }
    }
}
