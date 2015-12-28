using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DataLakeServer.Core
{
    public class ChatService : WebSocketBehavior
    {
        private string _suffix;

        public ChatService()
            : this(null)
        {
        }

        public ChatService(string suffix)
        {
            _suffix = suffix ?? String.Empty;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Sessions.Broadcast(e.Data + _suffix);
        }
    }
}
