using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchDog
{
    public partial class watchDog : Form
    {
        private string[] _processAddress;
        private object _lockerForLog = new object();
        private string _logPath = string.Empty;
        private List<ProcessRestart> processList;
        public watchDog()
        {
            InitializeComponent();
            processList = new List<ProcessRestart>();
            Init();
        }

        private void Init()
        {
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

        private void watchDog_Load(object sender, EventArgs e)
        {
            //设置鼠标放在托盘图标上面的文字 
            StartWatch();
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
                        //this.SaveLog("process(" + p.Id.ToString() + ")(" + p.ProcessName.ToString() + ")is forbiden for explore full path！");
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
            processList.Add(objProcessRestart);
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

        private void watchDog_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach(var val in this.processList)
            {
                val.Stop();
                Thread.Sleep(100);
            }
        }


    }

    public class ProcessRestart
    {
        private Process _process;
        private string _address;
        private bool enable;

        public ProcessRestart()
        { }

        public ProcessRestart(Process process, string address)
        {
            this._process = process;
            this._address = address;
            enable = true;
        }

        public void Stop()
        {
            enable = false;
        }

        public void RestartProcess()
        {
            try
            {
                while (enable)
                {
                    this._process.WaitForExit();
                    this._process.Close();
                    this._process.StartInfo.FileName = this._address;
                    this._process.Start();
                    Thread.Sleep(1000);
                }
            }
            catch
            {
            }
        }
    }
}
