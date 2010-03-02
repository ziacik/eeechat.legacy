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
using System.Threading;
using KolikSoftware.Eee.Service.Exceptions;
using KolikSoftware.Eee.Service.Domain;
using KolikSoftware.Eee.Client.MainFormPlugins;

namespace KolikSoftware.Eee.Client
{
    public partial class BackgroundServiceController : Component, IEeeService
    {
        #region Private Members
        Dictionary<BackgroundWorker, InvokeInfo> Workers { get; set; }
        #endregion

        public event EventHandler<EventArgs> GetUsersFinished;

        protected virtual void OnGetUsersFinished(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.GetUsersFinished;
            if (handler != null) handler(this, e);
        }


        public IServiceConfiguration Configuration
        {
            get
            {
                return this.Service.Configuration;
            }
        }

        public User CurrentUser
        {
            get
            {
                return this.Service.CurrentUser;
            }
        }

        public void CommitMessage(Post message)
        {
            this.Service.CommitMessage(message);
        }

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
                if (e.Error is DisconnectedException)
                    OnDisconnected(DisconnectedEventArgs.Empty);

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

        public event EventHandler<EventArgs> GetRoomsFinished;

        protected virtual void OnGetRoomsFinished(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.GetRoomsFinished;
            if (handler != null) handler(this, e);
        }


        public class GetMessagesFinishedEventArgs : EventArgs
        {
            public IList<Post> Messages { get; private set; }

            public GetMessagesFinishedEventArgs(IList<Post> messages)
            {
                this.Messages = messages;
            }
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

        public MainForm Form { get; set; }

        public BackgroundServiceController()
        {
            InitializeComponent();
            this.Workers = new Dictionary<BackgroundWorker, InvokeInfo>();
        }

        public BackgroundServiceController(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.Workers = new Dictionary<BackgroundWorker, InvokeInfo>();
        }

        public IEeeService Service { get; set; }


        #region Public
        #region Invoke Core
        class InvokeInfo
        {
            public int SleepSecs { get; set; }
            public Action Work { get; set; }
            public Func<object> Query { get; set; }
            public Action<object> Success { get; set; }
            public Action<Exception> Error { get; set; }
            public object Result { get; set; }
        }

        public void InvokeInBackground(int sleepSecs, Action work, Action<object> success, Action<Exception> error)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            InvokeInfo info = new InvokeInfo()
            {
                SleepSecs = sleepSecs,
                Work = work,
                Success = success,
                Error = error
            };
            this.Workers.Add(worker, info);
            worker.RunWorkerAsync(info);
        }

        public void QueryInBackground(int sleepSecs, Func<object> query, Action<object> success, Action<Exception> error)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            InvokeInfo info = new InvokeInfo()
            {
                SleepSecs = sleepSecs,
                Query = query,
                Success = success,
                Error = error
            };
            this.Workers.Add(worker, info);
            worker.RunWorkerAsync(info);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            if (!e.Cancelled)
            {
                InvokeInfo info = this.Workers[worker];
                if (e.Error != null)
                    info.Error(e.Error);
                else
                    info.Success(e.Result);
            }

            this.Workers.Remove(worker);

            worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
            worker.DoWork -= worker_DoWork;
            worker.Dispose();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            InvokeInfo info = (InvokeInfo)e.Argument;

            if (info.SleepSecs > 0)
                Thread.Sleep(info.SleepSecs * 1000);
            
            if (info.Work != null)
                info.Work();

            if (info.Query != null)
                e.Result = info.Query();
        }
        #endregion

        public void Connect(string login, SecureString password)
        {
            DoConnect(0, login, password);
        }

        void DoConnect(int sleepSecs, string login, SecureString password)
        {
            InvokeInBackground(
                sleepSecs,
                () => this.Service.Connect(login, password),
                r => OnConnected(ConnectedEventArgs.Empty),
                e =>
                {
                    if (e.Message == "BADLOGIN")
                    {
                        OnLoginFailed(LoginFailedEventArgs.Empty);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(e));
                        /// Try reconnecting in 30 secs.
                        DoConnect(30, login, password);
                    }
                }
            );
        }

        public void Disconnect(bool force)
        {
            DoDisconnect(0, force);
        }

        void DoDisconnect(int sleepSecs, bool force)
        {
            CheckUser();
            //RemoveInvocations();

            InvokeInBackground(
                sleepSecs,
                () => this.Service.Disconnect(),
                r => OnDisconnected(DisconnectedEventArgs.Empty),
                e =>
                {
                    if (force)
                    {
                        OnDisconnected(DisconnectedEventArgs.Empty);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(e));
                        DoDisconnect(3, force);
                    }
                }
            );
        }

        public IList<User> GetUsers()
        {
            DoGetUsers(0);
            return null;
        }

        void DoGetUsers(int sleepSecs)
        {
            QueryInBackground(
                sleepSecs,
                () => this.Service.GetUsers(),
                r =>
                {
                    IList<User> users = (IList<User>)r;
                    this.Form.GetPlugin<UserStatePlugin>().SetUsers(users);
                    OnGetUsersFinished(EventArgs.Empty);
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoGetUsers(3);
                }
            );
        }

        public IList<Room> GetRooms()
        {
            DoGetRooms(0);
            return null;
        }

        void DoGetRooms(int sleepSecs)
        {
            QueryInBackground(
                sleepSecs,
                () => this.Service.GetRooms(),
                r => 
                {
                    IList<Room> rooms = (IList<Room>)r;
                    this.Form.GetPlugin<RoomStatePlugin>().SetRooms(rooms);
                    OnGetRoomsFinished(EventArgs.Empty);
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoGetRooms(3);
                }
            );
        }

        bool IsConnectionProblem(Exception error)
        {
            while (error != null)
            {
                if (error is System.Net.WebException || error is System.Net.Sockets.SocketException)
                    return true;

                error = error.InnerException;
            }

            return false;
        }

        public IList<Post> GetMessages()
        {
            DoGetMessages(0, 0);
            return null;
        }

        public IList<Post> GetMessagesSafe()
        {
            DoGetMessagesSafe(0, false);
            return null;
        }

        void DoGetMessages(int sleepSecs, int retryNo)
        {
            QueryInBackground(
                sleepSecs,
                () => 
                {
                    return this.Service.GetMessages();
                },
                r => 
                {
                    IList<Post> posts = (IList<Post>)r;
                    this.Form.GetPlugin<UserStatePlugin>().SetUsersToPosts(posts);
                    OnSucessfulRequest(SucessfulRequestEventArgs.Empty);
                    OnGetMessagesFinished(new GetMessagesFinishedEventArgs(posts));
                    DoGetMessages(this.Service.Configuration.MessageGetInterval, 0);
                },
                e =>
                {
                    if (IsConnectionProblem(e))
                    {
                        /// In case this is a "Connection Problem", switch to safe mode.
                        /// If success, return to normal mode.
                        DoGetMessagesSafe(this.Service.Configuration.MessageGetInterval, true);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(e));

                        int delay;

                        if (this.Service.Configuration.MessageGetInstantRetryCount > retryNo)
                            delay = this.Service.Configuration.MessageGetInstantRetryDelay;
                        else
                            delay = this.Service.Configuration.MessageGetRetryDelay;

                        DoGetMessages(delay, retryNo + 1);
                    }
                }
            );
        }

        void DoGetMessagesSafe(int sleepSecs, bool returnToNormalModeOnSuccess)
        {
            QueryInBackground(
                sleepSecs,
                () =>
                {
                    return this.Service.GetMessagesSafe();
                },
                r =>
                {
                    IList<Post> posts = (IList<Post>)r;
                    this.Form.GetPlugin<UserStatePlugin>().SetUsersToPosts(posts);
                    OnSucessfulRequest(SucessfulRequestEventArgs.Empty);
                    OnGetMessagesFinished(new GetMessagesFinishedEventArgs(posts));

                    if (returnToNormalModeOnSuccess)
                        DoGetMessages(this.Service.Configuration.MessageGetInterval, 0);
                    else
                        DoGetMessagesSafe(this.Service.Configuration.MessageGetSafeInterval, false);
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoGetMessagesSafe(this.Service.Configuration.MessageGetSafeInterval, returnToNormalModeOnSuccess);
                }
            );
        }

        public void SendMessage(Room room, User recipient, string message)
        {
            DoSendMessage(0, room, recipient, message);
        }

        void DoSendMessage(int retryNo, Room room, User recipient, string message)
        {
            int sleepSecs = 0;

            if (retryNo > 5)
                sleepSecs = 60;
            else if (retryNo > 0)
                sleepSecs = 5;

            InvokeInBackground(
                sleepSecs,
                () => this.Service.SendMessage(room, recipient, message),
                r =>
                {
                    //TODO: commit or what
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoSendMessage(retryNo + 1, room, recipient, message);
                }
            );
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

            //foreach (InvocationParameters parameters in this.downloadInvocations)
            //{
            //    if (parameters.Arguments[1].Equals(filePath))
            //        return;
            //}

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
        //string basePath = Path.GetDirectoryName(Application.UserAppDataPath);

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
        //public int UploadInvocationsCount
        //{
        //    get
        //    {
        //        int pending = this.uploadInvocations.Count;
                
        //        if (this.uploadingWorker.IsBusy)
        //            return pending + 1;
        //        else
        //            return pending;
        //    }
        //}

        //public int DownloadInvocationsCount
        //{
        //    get
        //    {
        //        int pending = this.downloadInvocations.Count;

        //        if (this.downloadingWorker.IsBusy)
        //            return pending + 1;
        //        else
        //            return pending;
        //    }
        //}
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
                result = typeof(IEeeService).InvokeMember(invocationParameters.Name, BindingFlags.InvokeMethod, null, this.Service, invocationParameters.Arguments);
            else
                typeof(IEeeService).InvokeMember(invocationParameters.Name, BindingFlags.SetProperty, null, this.Service, invocationParameters.Arguments);

            return result;
        }

        private void ProcessResult(InvocationParameters invocationParameters)
        {
            switch (invocationParameters.Name)
            {
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
            //if (this.disconnecting) return;

            //List<InvocationParameters> invocationList = null;

            //switch (invocationScope)
            //{
            //    case InvocationScope.Receiver:
            //        invocationList = this.receiverInvocations;
            //        break;
            //    case InvocationScope.Sender:
            //        invocationList = this.senderInvocations;
            //        break;
            //    case InvocationScope.Download:
            //        invocationList = this.downloadInvocations;
            //        break;
            //    case InvocationScope.Upload:
            //        invocationList = this.uploadInvocations;
            //        break;
            //}

            //InvocationParameters parameters = new InvocationParameters();
            //parameters.InvocationScope = invocationScope;
            //parameters.InvocationType = invocationType;
            //parameters.InvocationTime = invocationTime;
            //parameters.Name = name;
            //parameters.Arguments = arguments;

            //int index = 0;

            //if (invocationList.Count > 0)
            //{
            //    while (index < invocationList.Count && (invocationList[index].InvocationTime == null || invocationTime >= invocationList[index].InvocationTime))
            //    {
            //        index++;
            //    }
            //}

            //invocationList.Insert(index, parameters);
        }

        #endregion

        void invocationTimer_Tick(object sender, EventArgs e)
        {
            return;            
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

                    /*if (this.disconnecting && this.forceDisconnect)
                    {
                        ProcessResult(error.InvocationParameters);
                    }
                    else */if (error.InnerException is DisconnectedException)
                    {
                        //RemoveInvocations();
                        //ProcessDisconnectUser(error.InvocationParameters, true);
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

        #region IEeeService Members

        public string ServiceUrl
        {
            get
            {
                if (this.Service == null || this.Service.Configuration == null)
                    return null;
                else
                    return this.Service.Configuration.ServiceUrl;
            }
            set
            {
                if (this.Service != null && this.Service.Configuration != null)
                    this.Service.Configuration.ServiceUrl = value;
            }
        }

        public ProxySettings ProxySettings
        {
            get
            {
                if (this.Service == null)
                    return null;
                else
                    return this.Service.ProxySettings;
            }
            set
            {
                if (this.Service != null)
                    this.Service.ProxySettings = value;
            }
        }

        public AuthenticationData GetAuthenticationData(string login)
        {
            throw new NotSupportedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        #endregion
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
