using System.ServiceProcess;


namespace DataLakeServer
{
    public partial class CoreService : ServiceBase
    {
        public CoreService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {
        }
    }
}
