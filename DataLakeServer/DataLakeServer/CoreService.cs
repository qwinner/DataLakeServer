using System;
using System.IO;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;
using System.Diagnostics;


namespace DataLakeServer
{
    public partial class CoreService : ServiceBase
    {
        private string[] _processAddress;
        private object _lockerForLog = new object();
        private string _logPath = string.Empty;
        public CoreService()
        {
            InitializeComponent();
            try
            {
                string strProcessAddress = ConfigurationManager.AppSettings["ProcessAddress"].ToString();
                if (strProcessAddress.Trim() != "")
                {
                    this._processAddress = strProcessAddress.Split(',');
                }
                else
                {
                    throw new Exception("read configuration ProcessAddress failed ，ProcessAddress is null！");
                }
                this._logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerWatcherLog");
                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }
            }
            catch (Exception ex)
            {
                this.SaveLog("Watcher() init error ：" + ex.Message.ToString());
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.StartWatch();
            }
            catch (Exception ex)
            {
                this.SaveLog("OnStart() error：" + ex.Message.ToString());
            }
        }

        protected override void OnStop()
        {
        }

        private void StartWatch()
        {
            if (this._processAddress != null)
            {
                if (this._processAddress.Length > 0)
                {
                    foreach (string str in _processAddress)
                    {
                        if (str.Trim() != "")
                        {
                            if (File.Exists(str.Trim()))
                            {
                                this.ScanProcessList(str.Trim());
                            }
                        }
                    }
                }
            }
        }


        private void ScanProcessList(string address)
        {
            Process[] arrayProcess = Process.GetProcesses();
            foreach (Process p in arrayProcess)
            {
                if (p.ProcessName != "System" && p.ProcessName != "Idle")
                {
                    try
                    {
                        if (this.FormatPath(address) == this.FormatPath(p.MainModule.FileName.ToString()))
                        {
                            this.WatchProcess(p, address);
                            return;
                        }
                    }
                    catch
                    {
                        this.SaveLog("process(" + p.Id.ToString() + ")(" + p.ProcessName.ToString() + ")is forbiden for explore full path！");
                    }
                }
            }

            Process process = new Process();
            process.StartInfo.FileName = address;
            process.Start();
            this.WatchProcess(process, address);
        }
        private void WatchProcess(Process process, string address)
        {
            ProcessRestart objProcessRestart = new ProcessRestart(process, address);
            Thread thread = new Thread(new ThreadStart(objProcessRestart.RestartProcess));
            thread.Start();
        }
        private string FormatPath(string path)
        {
            return path.ToLower().Trim().TrimEnd('\\');
        }

        public void SaveLog(string content)
        {
            try
            {
                lock (_lockerForLog)
                {
                    FileStream fs;
                    fs = new FileStream(Path.Combine(this._logPath, DateTime.Now.ToString("yyyyMMdd") + ".log"), FileMode.OpenOrCreate);
                    StreamWriter streamWriter = new StreamWriter(fs);
                    streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    streamWriter.WriteLine("[" + DateTime.Now.ToString() + "]：" + content);
                    streamWriter.Flush();
                    streamWriter.Close();
                    fs.Close();
                }
            }
            catch
            {
            }
        }
    }
    public class ProcessRestart
    {
        private Process _process;
        private string _address;

        public ProcessRestart()
        { }

        public ProcessRestart(Process process, string address)
        {
            this._process = process;
            this._address = address;
        }

        public void RestartProcess()
        {
            try
            {
                while (true)
                {
                    this._process.WaitForExit();
                    this._process.Close();
                    this._process.StartInfo.FileName = this._address;
                    this._process.Start();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                CoreService objProcessWatcher = new CoreService();
                objProcessWatcher.SaveLog("RestartProcess() error，monitor has been canceled the watcher of ("
                    + this._process.Id.ToString() + ")(" + this._process.ProcessName.ToString()
                    + "), error description is ：" + ex.Message.ToString());
            }
        }
    }
}
