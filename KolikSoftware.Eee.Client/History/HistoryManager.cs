using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using KolikSoftware.Eee.Service;
using KolikSoftware.Eee.Client.Helpers;

namespace KolikSoftware.Eee.Client.History
{
    public partial class HistoryManager : Component
    {
        DateTime historyDate = DateTime.Today;

        public HistoryManager()
        {
            InitializeComponent();
        }

        public HistoryManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        //EeeDataSet currentData = new EeeDataSet();
        //EeeDataSet.MessageDataTable pendingMessages;

        //public EeeDataSet.MessageDataTable Current
        //{
        //    get
        //    {
        //        return this.currentData.Message;
        //    }
        //}

        //EeeDataSet.MessageDataTable history;

        //public EeeDataSet.MessageDataTable History
        //{
        //    get
        //    {
        //        return this.history;
        //    }
        //}

        BackgroundServiceController serviceController;

        public BackgroundServiceController ServiceController
        {
            get
            {
                return this.serviceController;
            }
            set
            {
                if (this.serviceController != null)
                {
                    this.serviceController.Connected -= serviceController_Connected;
                    this.serviceController.Disconnected -= serviceController_Disconnected;
                }

                this.serviceController = value;

                this.serviceController.Connected += serviceController_Connected;
                this.serviceController.Disconnected += serviceController_Disconnected;
            }
        }

        void serviceController_Disconnected(object sender, BackgroundServiceController.DisconnectedEventArgs e)
        {
            //Unload();
        }

        void serviceController_Connected(object sender, BackgroundServiceController.ConnectedEventArgs e)
        {
            Load();
        }

        //public void Save(EeeDataSet.MessageDataTable messages)
        //{
            //if (messages.Count > 0)
            //{
            //    if (this.pendingMessages == null)
            //        this.pendingMessages = messages;
            //    else
            //        this.pendingMessages.Merge(messages);

            //    if (this.savingWorker.IsBusy == false)
            //    {
            //        MergePending();
            //        this.savingWorker.RunWorkerAsync();
            //    }
            //}
        //}

        void Load()
        {
            //string historyDir = this.serviceController.GetPathToSave("History");
            //historyDir = Path.Combine(historyDir, this.historyDate.Year.ToString());

            //if (Directory.Exists(historyDir) == false)
            //    Directory.CreateDirectory(historyDir);

            //string fileName = Path.Combine(historyDir, this.historyDate.ToString("yyyy-MM-dd") + ".xml");

            //if (File.Exists(fileName))
            //{
            //    try
            //    {
            //        this.currentData.ReadXml(fileName);
            //    }
            //    catch (Exception ex)
            //    {
            //        Trace.WriteLine("Could not load history. " + ex.Message);
            //    }
            //}
        }

        //public EeeDataSet.MessageDataTable Load(DateTime date)
        //{
        //    if (this.history != null)
        //        this.history.DataSet.Dispose();

        //    string historyDir = this.serviceController.GetPathToSave("History");
        //    historyDir = Path.Combine(historyDir, date.Year.ToString());

        //    if (Directory.Exists(historyDir) == false)
        //        return null;

        //    string fileName = Path.Combine(historyDir, date.ToString("yyyy-MM-dd") + ".xml");

        //    if (File.Exists(fileName) == false)
        //        return null;

        //    EeeDataSet dataSet = new EeeDataSet();
        //    dataSet.ReadXml(fileName);

        //    this.history = dataSet.Message;
        //    return this.history;
        //}

        //void Unload()
        //{
        //    this.currentData.Dispose();
        //    this.currentData = new EeeDataSet();

        //    if (this.pendingMessages != null)
        //    {
        //        this.pendingMessages.Dispose();
        //        this.pendingMessages = null;
        //    }
        //}

        //void MergePending()
        //{
        //    this.currentData.Message.Merge(this.pendingMessages);
        //    this.pendingMessages = null;
        //}

        //void DoSave()
        //{
        //    string historyDir = this.serviceController.GetPathToSave("History");
        //    historyDir = Path.Combine(historyDir, this.historyDate.Year.ToString());

        //    if (Directory.Exists(historyDir) == false)
        //        Directory.CreateDirectory(historyDir);

        //    string fileName = Path.Combine(historyDir, this.historyDate.ToString("yyyy-MM-dd") + ".xml");

        //    this.currentData.WriteXml(fileName);
        //}

        //void savingWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    DoSave();
        //}

        //void savingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (this.pendingMessages != null && this.pendingMessages.Count > 0)
        //    {
        //        MergePending();
        //        this.savingWorker.RunWorkerAsync();
        //    }
        //}
    }
}
