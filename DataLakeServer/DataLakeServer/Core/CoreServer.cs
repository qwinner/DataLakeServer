using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataLakeServer.Core
{
    public class CoreServer
    {
        private CoreServer instance = new CoreServer();

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
            
           
        }

        ~CoreServer()
        {

        }
    }
}
