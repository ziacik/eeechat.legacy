using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using KolikSoftware.Eee.Service;
using System.Reflection;
using System.Security;
using System.Drawing;
using System.IO;
using System.Web;
using System.Windows.Forms;
using System.Threading;
using KolikSoftware.Eee.Service.Exceptions;

namespace KolikSoftware.Eee.Client
{
    public partial class BackgroundServiceController : Component
    {
        #region Private Members
        bool disconnecting = false;
        bool forceDisconnect;
        IEeeService service = null;
        List<InvocationParameters> senderInvocations = new List<InvocationParameters>();
        List<InvocationParameters> receiverInvocations = new List<InvocationParameters>();
        List<InvocationParameters> downloadInvocations = new List<InvocationParameters>();
        List<InvocationParameters> uploadInvocations = new List<InvocationParameters>();
        #endregion

        #region Events
        public class RegisteredEventArgs : EventArgs
        {
            public static readonly new RegisteredEventArgs Empty = new RegisteredEventArgs();
        }

        public event EventHandler<RegisteredEventArgs> Registered;

        protected virtual void OnRegistered(RegisteredEventArgs e)
        {
            EventHandler<RegisteredEventArgs> handler = Registered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class RegisterFailedEventArgs : EventArgs
        {
            public static readonly new RegisterFailedEventArgs Empty = new RegisterFailedEventArgs();
        }

        public event EventHandler<RegisterFailedEventArgs> RegisterFailed;

        protected virtual void OnRegisterFailed(RegisterFailedEventArgs e)
        {
            EventHandler<RegisterFailedEventArgs> handler = RegisterFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class SucessfulRequestEventArgs : EventArgs
        {
            public static readonly new SucessfulRequestEventArgs Empty = new SucessfulRequestEventArgs();
        }

        public event EventHandler<SucessfulRequestEventArgs> SucessfulRequest;

        protected virtual void OnSucessfulRequest(SucessfulRequestEventArgs e)
        {
            EventHandler<SucessfulRequestEventArgs> handler = SucessfulRequest;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ErrorOccuredEventArgs : EventArgs
        {
            Exception error;

            public ErrorOccuredEventArgs(Exception error)
            {
                this.error = error;
            }

            public Exception Error
            {
                get
                {
                    return this.error;
                }
            }
        }

        public event EventHandler<ErrorOccuredEventArgs> ErrorOccured;

        protected virtual void OnErrorOccured(ErrorOccuredEventArgs e)
        {
            EventHandler<ErrorOccuredEventArgs> handler = ErrorOccured;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ConnectedEventArgs : EventArgs
        {
            public static readonly new ConnectedEventArgs Empty = new ConnectedEventArgs();
        }

        public event EventHandler<ConnectedEventArgs> Connected;

        protected virtual void OnConnected(ConnectedEventArgs e)
        {
            EventHandler<ConnectedEventArgs> handler = Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class LoginFailedEventArgs : EventArgs
        {
            public static readonly new LoginFailedEventArgs Empty = new LoginFailedEventArgs();
        }

        public event EventHandler<LoginFailedEventArgs> LoginFailed;

        protected virtual void OnLoginFailed(LoginFailedEventArgs e)
        {
            EventHandler<LoginFailedEventArgs> handler = LoginFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class DisconnectedEventArgs : EventArgs
        {
            public static readonly new DisconnectedEventArgs Empty = new DisconnectedEventArgs();
            public bool NoClear { get; set; }            
        }

        public event EventHandler<DisconnectedEventArgs> Disconnected;

        protected virtual void OnDisconnected(DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = Disconnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class GetRoomsFinishedEventArgs : EventArgs
        {
            //private EeeDataSet.RoomDataTable rooms;

            //public GetRoomsFinishedEventArgs(EeeDataSet.RoomDataTable rooms)
            //    : base()
            //{
            //    this.rooms = rooms;
            //}

            //public EeeDataSet.RoomDataTable Rooms
            //{
            //    get
            //    {
            //        return this.rooms;
            //    }
            //}
        }

        public event EventHandler<GetRoomsFinishedEventArgs> GetRoomsFinished;

        protected virtual void OnGetRoomsFinished(GetRoomsFinishedEventArgs e)
        {
            EventHandler<GetRoomsFinishedEventArgs> handler = GetRoomsFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class GetUsersFinishedEventArgs : EventArgs
        {
            //private EeeDataSet.UserDataTable users;

            //public GetUsersFinishedEventArgs(EeeDataSet.UserDataTable users)
            //    : base()
            //{
            //    this.users = users;
            //}

            //public EeeDataSet.UserDataTable Users
            //{
            //    get
            //    {
            //        return this.users;
            //    }
            //}
        }

        public event EventHandler<GetUsersFinishedEventArgs> GetUsersFinished;

        protected virtual void OnGetUsersFinished(GetUsersFinishedEventArgs e)
        {
            EventHandler<GetUsersFinishedEventArgs> handler = GetUsersFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class GetMessagesFinishedEventArgs : EventArgs
        {
            //private EeeDataSet.MessageDataTable messages;

            //public GetMessagesFinishedEventArgs(EeeDataSet.MessageDataTable messages)
            //    : base()
            //{
            //    this.messages = messages;
            //}

            //public EeeDataSet.MessageDataTable Messages
            //{
            //    get
            //    {
            //        return this.messages;
            //    }
            //}
        }

        public event EventHandler<GetMessagesFinishedEventArgs> GetMessagesFinished;

        protected virtual void OnGetMessagesFinished(GetMessagesFinishedEventArgs e)
        {
            EventHandler<GetMessagesFinishedEventArgs> handler = GetMessagesFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        //public event EventHandler<CommandProcessor.UserStateChangedEventArgs> UserStateChanged;

        //protected virtual void OnUserStateChanged(CommandProcessor.UserStateChangedEventArgs e)
        //{
        //    EventHandler<CommandProcessor.UserStateChangedEventArgs> handler = UserStateChanged;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        //public event EventHandler<CommandProcessor.ExternalUserStateChangedEventArgs> ExternalUserStateChanged;

        //protected virtual void OnExternalUserStateChanged(CommandProcessor.ExternalUserStateChangedEventArgs e)
        //{
        //    EventHandler<CommandProcessor.ExternalUserStateChangedEventArgs> handler = ExternalUserStateChanged;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        public class UpdatesAvailableEventArgs : EventArgs
        {
            //EeeDataSet.UpdateDataTable updates;

            //public UpdatesAvailableEventArgs(EeeDataSet.UpdateDataTable updates)
            //{
            //    this.updates = updates;
            //}

            //public EeeDataSet.UpdateDataTable Updates
            //{
            //    get
            //    {
            //        return this.updates;
            //    }
            //}
        }

        public event EventHandler<UpdatesAvailableEventArgs> UpdatesAvailable;

        protected virtual void OnUpdatesAvailable(UpdatesAvailableEventArgs e)
        {
            EventHandler<UpdatesAvailableEventArgs> handler = UpdatesAvailable;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class DownloadFinishedEventArgs : EventArgs
        {
            string filePath;

            public string FilePath
            {
                get
                {
                    return this.filePath;
                }
            }

            string link;

            public string Link
            {
                get
                {
                    return this.link;
                }
            }
            
            public DownloadFinishedEventArgs(string filePath, string link)
            {
                this.filePath = filePath;
                this.link = link;
            }
        }

        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        protected virtual void OnDownloadFinished(DownloadFinishedEventArgs e)
        {
            EventHandler<DownloadFinishedEventArgs> handler = DownloadFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class DownloadFailedEventArgs : EventArgs
        {
            string link;

            public string Link
            {
                get
                {
                    return this.link;
                }
            }

            Exception error;

            public Exception Error
            {
                get
                {
                    return this.error;
                }
            }

            public DownloadFailedEventArgs(string link, Exception error)
            {
                this.link = link;
                this.error = error;
            }
        }

        public event EventHandler<DownloadFailedEventArgs> DownloadFailed;

        protected virtual void OnDownloadFailed(DownloadFailedEventArgs e)
        {
            EventHandler<DownloadFailedEventArgs> handler = DownloadFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class UploadFinishedEventArgs : EventArgs
        {
            string file;

            public string File
            {
                get
                {
                    return this.file;
                }
            }
            
            string link;

            public string Link
            {
                get
                {
                    return this.link;
                }
            }

            object parameter;

            public object Parameter
            {
                get
                {
                    return this.parameter;
                }
            }
            
            public UploadFinishedEventArgs(string file, string link, object parameter)
            {
                this.file = file;
                this.link = link;
                this.parameter = parameter;
            }
        }

        public event EventHandler<UploadFinishedEventArgs> UploadFinished;

        protected virtual void OnUploadFinished(UploadFinishedEventArgs e)
        {
            EventHandler<UploadFinishedEventArgs> handler = UploadFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class UploadFailedEventArgs : EventArgs
        {
            string link;

            public string Link
            {
                get
                {
                    return this.link;
                }
            }

            Exception error;

            public Exception Error
            {
                get
                {
                    return this.error;
                }
            }

            public UploadFailedEventArgs(string link, Exception error)
            {
                this.link = link;
                this.error = error;
            }
        }

        public event EventHandler<UploadFailedEventArgs> UploadFailed;

        protected virtual void OnUploadFailed(UploadFailedEventArgs e)
        {
            EventHandler<UploadFailedEventArgs> handler = UploadFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public BackgroundServiceController()
        {
            InitializeComponent();
            this.MessagesToCommit = new List<int>();
        }

        public BackgroundServiceController(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.MessagesToCommit = new List<int>();
        }

        public IEeeService Service
        {
            get
            {
                return this.service;
            }
            set
            {
                this.service = value;
            }
        }

        #region Public
        public List<int> MessagesToCommit { get; private set; }

        //public EeeDataSet.UserRow CurrentUser
        //{
        //    get
        //    {
        //        if (this.service == null)
        //            return null;
        //        else
        //            return this.service.CurrentUser;
        //    }
        //}

        public void ConnectUser(string login, SecureString password)
        {
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "ConnectUser", login, password);
        }

        public void DisconnectUser(bool force)
        {
            CheckUser();
            RemoveInvocations();
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "DisconnectUser");
            this.disconnecting = true;
            this.forceDisconnect = force;
        }

        public void RegisterUser(string login, SecureString password, int color)
        {
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "RegisterUser", login, password, color);
        }

        public void SetAwayMode(string comment)
        {
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "SetAwayMode", comment);
        }

        public void ResetAwayMode()
        {
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "ResetAwayMode");
        }

        public void GetRooms()
        {
            CheckUser();
            AddInvocation(InvocationScope.Receiver, InvocationType.Method, null, "GetRooms");
        }

        public void GetUsers()
        {
            CheckUser();
            AddInvocation(InvocationScope.Receiver, InvocationType.Method, null, "GetUsers");
        }

        public void GetMessages()
        {
            CheckUser();

            string commit = GetCommitMessages();

            AddInvocation(InvocationScope.Receiver, InvocationType.Method, null, "GetMessages", commit);
        }

        private string GetCommitMessages()
        {
            StringBuilder builder = new StringBuilder();

            foreach (int messageId in this.MessagesToCommit)
            {
                if (builder.Length > 0)
                    builder.Append(',');

                builder.Append(messageId);
            }

            this.MessagesToCommit.Clear();

            string commit = builder.ToString();
            return commit;
        }

        public void GetMessages(TimeSpan timeToWait)
        {
            CheckUser();

            string commit = GetCommitMessages();

            AddInvocation(InvocationScope.Receiver, InvocationType.Method, DateTime.Now + timeToWait, "GetMessages", commit);
        }

        public void GetUpdates()
        {
            CheckUser();
            AddInvocation(InvocationScope.Receiver, InvocationType.Method, null, "GetUpdates", Properties.Settings.Default.LatestUpdateNo);
        }

        public void AddMessage(int roomId, string recipientLogin, string message)
        {
            CheckUser();
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "AddMessage", roomId, recipientLogin, message, "");
        }

        public void SendFeedback(string from, string mail, string feedbackType, string description)
        {
            AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "SendFeedback", from, mail, feedbackType, description);
        }

        public void DownloadFile(string link, string destinationDir)
        {
            CheckUser();

            string fileName = link;
            int lastSlashIdx = fileName.LastIndexOf('/');
            fileName = fileName.Substring(lastSlashIdx + 1);
            string filePath = Path.Combine(destinationDir, fileName);

            foreach (InvocationParameters parameters in this.downloadInvocations)
            {
                if (parameters.Arguments[1].Equals(filePath))
                    return;
            }

            AddInvocation(InvocationScope.Download, InvocationType.Method, null, "DownloadFile", link, filePath);
        }

        public void UploadFile(string filePath, object parameter)
        {
            CheckUser();
            AddInvocation(InvocationScope.Upload, InvocationType.Method, null, "UploadFile", filePath, parameter);
        }

        public void AddReport(string name, string value)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Public Helpers
        string basePath = Path.GetDirectoryName(Application.UserAppDataPath);

        /// <summary>
        /// Gets a path to save user data according to current user and context. User must be logged in.
        /// </summary>
        public string GetPathToSave(string context)
        {
            //if (this.CurrentUser == null)
            //    throw new Exception("Current User not set.");
            
            //return Path.Combine(this.basePath, this.CurrentUser.Login + @"\" + context);
            return null;
        }
        #endregion

        #region Properties
        public int UploadInvocationsCount
        {
            get
            {
                int pending = this.uploadInvocations.Count;
                
                if (this.uploadingWorker.IsBusy)
                    return pending + 1;
                else
                    return pending;
            }
        }

        public int DownloadInvocationsCount
        {
            get
            {
                int pending = this.downloadInvocations.Count;

                if (this.downloadingWorker.IsBusy)
                    return pending + 1;
                else
                    return pending;
            }
        }
        #endregion

        #region Private
        private void CheckUser()
        {
            //if (this.CurrentUser == null)
            //    throw new InvalidOperationException("The user is not logged.");
        }

        private object Invoke(InvocationParameters invocationParameters)
        {
            object result = null;

            if (invocationParameters.InvocationType == InvocationType.Method)
                result = typeof(IEeeService).InvokeMember(invocationParameters.Name, BindingFlags.InvokeMethod, null, this.service, invocationParameters.Arguments);
            else
                typeof(IEeeService).InvokeMember(invocationParameters.Name, BindingFlags.SetProperty, null, this.service, invocationParameters.Arguments);

            return result;
        }

        private void ProcessResult(InvocationParameters invocationParameters)
        {
            switch (invocationParameters.Name)
            {
                case "ConnectUser":
                    ProcessConnectUser(invocationParameters);
                    break;
                case "DisconnectUser":
                    ProcessDisconnectUser(invocationParameters);
                    break;
                case "RegisterUser":
                    ProcessRegisterUser(invocationParameters);
                    break;
                case "GetRooms":
                    ProcessGetRooms(invocationParameters);
                    break;
                case "GetUsers":
                    ProcessGetUsers(invocationParameters);
                    break;
                case "GetMessages":
                    ProcessGetMessages(invocationParameters);
                    break;
                case "GetUpdates":
                    ProcessGetUpdates(invocationParameters);
                    break;
                case "DownloadFile":
                    ProcessDownloadFile(invocationParameters);
                    break;
                case "UploadFile":
                    ProcessUploadFile(invocationParameters);
                    break;
            }
        }

        void ProcessRegisterUser(InvocationParameters invocationParameters)
        {
            if (invocationParameters.Result.Equals(true))
                OnRegistered(RegisteredEventArgs.Empty);
            else
                OnRegisterFailed(RegisterFailedEventArgs.Empty);
        }

        void ProcessDownloadFile(InvocationParameters invocationParameters)
        {
            string link = (string)invocationParameters.Arguments[0];
            string filePath = (string)invocationParameters.Arguments[1];

            OnDownloadFinished(new DownloadFinishedEventArgs(filePath, link));
        }

        void ProcessDownloadFile(InvocationParameters invocationParameters, Exception error)
        {
            string link = (string)invocationParameters.Arguments[0];

            OnDownloadFailed(new DownloadFailedEventArgs(link, error));
        }

        void ProcessUploadFile(InvocationParameters invocationParameters)
        {
            if (invocationParameters.Result != null)
                OnUploadFinished(new UploadFinishedEventArgs((string)invocationParameters.Arguments[0], (string)invocationParameters.Result, invocationParameters.Arguments[1]));
            else
                OnUploadFailed(new UploadFailedEventArgs((string)invocationParameters.Arguments[0], new Exception("Internal Error at ProcessUploadFile.")));
        }

        void ProcessUploadFile(InvocationParameters invocationParameters, Exception error)
        {
            OnUploadFailed(new UploadFailedEventArgs((string)invocationParameters.Arguments[0], error));
        }

        void ProcessGetUpdates(InvocationParameters invocationParameters)
        {
            //using (EeeDataSet.UpdateDataTable updates = invocationParameters.Result as EeeDataSet.UpdateDataTable)
            //{
            //    if (updates.Count > 0)
            //        OnUpdatesAvailable(new UpdatesAvailableEventArgs(updates));
            //}
        }

        void ProcessGetMessages(InvocationParameters invocationParameters)
        {
            //using (EeeDataSet.MessageDataTable messages = invocationParameters.Result as EeeDataSet.MessageDataTable)
            //{
            //    if (messages.Count > 0)
            //    {
            //        for (int i = 0; i < messages.Count; i++)
            //        {
            //            EeeDataSet.MessageRow message = messages[i];

            //            bool continueProcessing;
            //            this.processor.ProcessMessage(message, out continueProcessing);

            //            if (continueProcessing == false)
            //            {
            //                message.Delete();
            //                i--;
            //            }
            //        }

            //        messages.AcceptChanges();
            //    }

            //    OnGetMessagesFinished(new GetMessagesFinishedEventArgs(messages));
            //}
        }

        void ProcessGetUsers(InvocationParameters invocationParameters)
        {
            //using (EeeDataSet.UserDataTable users = invocationParameters.Result as EeeDataSet.UserDataTable)
            //{
            //    if (users.Count > 0)
            //    {
            //        OnGetUsersFinished(new GetUsersFinishedEventArgs(users));
            //    }
            //}
        }

        void ProcessGetRooms(InvocationParameters invocationParameters)
        {
            //using (EeeDataSet.RoomDataTable rooms = invocationParameters.Result as EeeDataSet.RoomDataTable)
            //{
            //    if (rooms.Count > 0)
            //    {
            //        OnGetRoomsFinished(new GetRoomsFinishedEventArgs(rooms));
            //    }
            //}
        }

        private void ProcessDisconnectUser(InvocationParameters invocationParameters)
        {
            ProcessDisconnectUser(invocationParameters, false);
        }

        private void ProcessDisconnectUser(InvocationParameters invocationParameters, bool noClear)
        {
            this.disconnecting = false;
            DisposeProcessor();
            OnDisconnected(new DisconnectedEventArgs() { NoClear = noClear });
        }

        private void ProcessConnectUser(InvocationParameters invocationParameters)
        {
            if (invocationParameters.Result.Equals(true))
            {
                InitProcessor();
                OnConnected(ConnectedEventArgs.Empty);
            }
            else
            {
                OnLoginFailed(LoginFailedEventArgs.Empty);
            }
        }

        private void InitProcessor()
        {
            //this.processor = new CommandProcessor(this.notificator, this.CurrentUser.UserID, this.CurrentUser.Login);
            //this.processor.IdentifyRequested += new EventHandler<CommandProcessor.IdentifyRequestedEventArgs>(processor_IdentifyRequested);
            //this.processor.UserStateChanged += new EventHandler<CommandProcessor.UserStateChangedEventArgs>(processor_UserStateChanged);
            //this.processor.ExternalUserStateChanged += new EventHandler<CommandProcessor.ExternalUserStateChangedEventArgs>(processor_ExternalUserStateChanged);
        }

        private void DisposeProcessor()
        {
            //this.processor = null;
        }

        //void processor_UserStateChanged(object sender, CommandProcessor.UserStateChangedEventArgs e)
        //{
        //    OnUserStateChanged(e);
        //}

        //void processor_ExternalUserStateChanged(object sender, CommandProcessor.ExternalUserStateChangedEventArgs e)
        //{
        //    OnExternalUserStateChanged(e);
        //}

        //void processor_IdentifyRequested(object sender, CommandProcessor.IdentifyRequestedEventArgs e)
        //{
        //    //TODO: implement
        //}

        void AddInvocation(InvocationScope invocationScope, InvocationType invocationType, DateTime? invocationTime, string name, params object[] arguments)
        {
            if (this.disconnecting) return;

            List<InvocationParameters> invocationList = null;

            switch (invocationScope)
            {
                case InvocationScope.Receiver:
                    invocationList = this.receiverInvocations;
                    break;
                case InvocationScope.Sender:
                    invocationList = this.senderInvocations;
                    break;
                case InvocationScope.Download:
                    invocationList = this.downloadInvocations;
                    break;
                case InvocationScope.Upload:
                    invocationList = this.uploadInvocations;
                    break;
            }

            InvocationParameters parameters = new InvocationParameters();
            parameters.InvocationScope = invocationScope;
            parameters.InvocationType = invocationType;
            parameters.InvocationTime = invocationTime;
            parameters.Name = name;
            parameters.Arguments = arguments;

            int index = 0;

            if (invocationList.Count > 0)
            {
                while (index < invocationList.Count && (invocationList[index].InvocationTime == null || invocationTime >= invocationList[index].InvocationTime))
                {
                    index++;
                }
            }

            invocationList.Insert(index, parameters);
        }

        private void RemoveInvocations()
        {
            this.senderInvocations.Clear();
            this.receiverInvocations.Clear();
            this.downloadInvocations.Clear();
            this.uploadInvocations.Clear();
        }
        #endregion

        void invocationTimer_Tick(object sender, EventArgs e)
        {
            CheckNextInvocation(this.senderInvocations, this.sendingWorker);
            CheckNextInvocation(this.receiverInvocations, this.receivingWorker);
            CheckNextInvocation(this.downloadInvocations, this.downloadingWorker);
            CheckNextInvocation(this.uploadInvocations, this.uploadingWorker);
        }

        void CheckNextInvocation(List<InvocationParameters> invocationList, BackgroundWorker invocationWorker)
        {
            if (invocationWorker.IsBusy == false && invocationList.Count > 0)
            {
                InvocationParameters invocationParameters = invocationList[0];
                if (invocationParameters.InvocationTime == null || DateTime.Now >= invocationParameters.InvocationTime)
                {
                    invocationList.RemoveAt(0);
                    invocationWorker.RunWorkerAsync(invocationParameters);
                }
            }
        }

        void invocationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            InvocationParameters invocationParameters = (InvocationParameters)e.Argument;

            try
            {
                invocationParameters.Result = Invoke(invocationParameters);
                e.Result = invocationParameters;
            }
            catch (TargetInvocationException ex)
            {
                throw new InvocationException(ex.InnerException, invocationParameters);
            }
            catch (Exception ex)
            {
                throw new InvocationException(ex, invocationParameters);
            }
        }

        void invocationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                OnSucessfulRequest(SucessfulRequestEventArgs.Empty);
                ProcessResult((InvocationParameters)e.Result);
            }
            else
            {
                if (e.Error is InvocationException)
                {
                    InvocationException error = e.Error as InvocationException;

                    if (this.disconnecting && this.forceDisconnect)
                    {
                        ProcessResult(error.InvocationParameters);
                    }
                    else if (error.InnerException is DisconnectedException)
                    {
                        RemoveInvocations();
                        ProcessDisconnectUser(error.InvocationParameters, true);
                        OnErrorOccured(new ErrorOccuredEventArgs(error.InnerException));
                    }
                    else if (error.InvocationParameters.Name == "UploadFile")
                    {
                        ProcessUploadFile(error.InvocationParameters, error.InnerException);
                    }
                    else if (error.InvocationParameters.Name == "DownloadFile")
                    {
                        ProcessDownloadFile(error.InvocationParameters, error.InnerException);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(error.InnerException));
                        AddInvocation(error.InvocationParameters.InvocationScope, error.InvocationParameters.InvocationType, DateTime.Now.AddSeconds(1), error.InvocationParameters.Name, error.InvocationParameters.Arguments); //TODO: seconds
                    }
                }
                else
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e.Error));
                }
            }
        }

        public void ConnectJabber()
        {
            if (!this.externalServicesWorker.IsBusy)
                this.externalServicesWorker.RunWorkerAsync();
        }

        public void RenewJabber()
        {
            if (!this.externalServicesWorker2.IsBusy)
                this.externalServicesWorker2.RunWorkerAsync();
        }

        void externalServicesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
            //    if (!string.IsNullOrEmpty(Properties.Settings.Default.JabberID))
            //        this.service.JabberConnect(Properties.Settings.Default.JabberID, Security.Decrypt(Properties.Settings.Default.JabberPassword, true));
            //}
            //finally
            //{
            //    Thread.Sleep(30000);
            //}
        }

        void externalServicesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO:
            //if (e.Error != null)
              //  throw e.Error;
            //else
                ConnectJabber();
        }

        void externalServicesWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Properties.Settings.Default.JabberID))
            //    this.service.JabberRenew(Properties.Settings.Default.JabberID);
        }

        internal void SendJabber(string externalUser, string text, string externalNick)
        {
            CheckUser();
            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.JabberID))
                AddInvocation(InvocationScope.Sender, InvocationType.Method, null, "JabberSend", Properties.Settings.Default.JabberID, externalUser, text, externalNick);
        }

        internal void DisconnectJabber()
        {
        //    if (!string.IsNullOrEmpty(Properties.Settings.Default.JabberID))
        //    {
        //        try
        //        {
        //            this.service.JabberDisconnect(Properties.Settings.Default.JabberID);
        //        }
        //        catch
        //        {
        //            //TODO: Log?
        //        }
        //    }
        }
    }

    enum InvocationScope
    {
        Sender,
        Receiver,
        Download,
        Upload
    }

    enum InvocationType
    {
        Method,
        Property
    }

    struct InvocationParameters
    {
        public InvocationScope InvocationScope;
        public InvocationType InvocationType;
        public DateTime? InvocationTime;
        public string Name;
        public object[] Arguments;
        public object Result;
    }

    class InvocationException : Exception
    {
        InvocationParameters invocationParameters;

        public InvocationException(Exception innerException, InvocationParameters invocationParameters)
            : base("Invocation exception", innerException)
        {
            this.invocationParameters = invocationParameters;
        }

        public InvocationParameters InvocationParameters
        {
            get
            {
                return this.invocationParameters;
            }
        }
    }
}
