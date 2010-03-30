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
using KolikSoftware.Eee.Client.Notifications;
using KolikSoftware.Eee.Client.PluginCore;

namespace KolikSoftware.Eee.Client
{
    public partial class BackgroundServiceController : Component, IEeeService
    {
        #region Private Members
        Dictionary<BackgroundWorker, InvokeInfo> Workers { get; set; }
        #endregion

        public string ApplicationVersion { get; set; }

        public event EventHandler<EventArgs> GetUsersFinished;

        protected virtual void OnGetUsersFinished(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.GetUsersFinished;
            if (handler != null) handler(this, e);
        }

        public bool FirstGetMessages { get; set; }

        public IServiceConfiguration Configuration
        {
            get
            {
                return PluginHelper.Services[0].Configuration;
            }
        }

        public User CurrentUser
        {
            get
            {
                return PluginHelper.Services[0].CurrentUser;
            }
        }

        void DoForAllServices(Action<IEeeService> a)
        {
            foreach (IEeeService service in PluginHelper.Services)
            {
                a(service);
            }
        }

        public void CommitMessage(Post message)
        {
            DoForAllServices(s => s.CommitMessage(message));
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
            this.FirstGetMessages = true;
            this.Workers = new Dictionary<BackgroundWorker, InvokeInfo>();
        }

        public BackgroundServiceController(IContainer container)
        {
            container.Add(this);
            this.FirstGetMessages = true;
            InitializeComponent();
            this.Workers = new Dictionary<BackgroundWorker, InvokeInfo>();
        }

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
            DoForAllServices(s => DoConnect(s, 0, login, password));
        }

        void DoConnect(IEeeService service, int sleepSecs, string login, SecureString password)
        {
            InvokeInBackground(
                sleepSecs,
                () => service.Connect(login, password),
                r => OnConnected(ConnectedEventArgs.Empty),
                e =>
                {
                    if (e is ServiceException && ((ServiceException)e).Type == ServiceException.ExceptionType.BadLogin)
                    {
                        OnLoginFailed(LoginFailedEventArgs.Empty);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(e));
                        /// Try reconnecting in 30 secs.
                        DoConnect(service, 30, login, password);
                    }
                }
            );
        }

        public void Disconnect(bool force)
        {
            DoForAllServices(s => DoDisconnect(s, 0, force));
        }

        void DoDisconnect(IEeeService service, int sleepSecs, bool force)
        {
            //RemoveInvocations();

            InvokeInBackground(
                sleepSecs,
                () => service.Disconnect(),
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
                        DoDisconnect(service, 3, force);
                    }
                }
            );
        }

        public IList<User> GetUsers()
        {
            DoForAllServices(s => DoGetUsers(s, 0));
            return null;
        }

        void DoGetUsers(IEeeService service, int sleepSecs)
        {
            QueryInBackground(
                sleepSecs,
                () => service.GetUsers(),
                r =>
                {
                    IList<User> users = (IList<User>)r;
                    this.Form.GetPlugin<UserStatePlugin>().SetUsers(users);
                    OnGetUsersFinished(EventArgs.Empty);
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoGetUsers(service, 3);
                }
            );
        }

        public IList<Room> GetRooms()
        {
            DoForAllServices(s => DoGetRooms(s, 0));
            return null;
        }

        void DoGetRooms(IEeeService service, int sleepSecs)
        {
            QueryInBackground(
                sleepSecs,
                () => service.GetRooms(),
                r => 
                {
                    IList<Room> rooms = (IList<Room>)r;
                    this.Form.GetPlugin<RoomStatePlugin>().SetRooms(rooms);
                    OnGetRoomsFinished(EventArgs.Empty);
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoGetRooms(service, 3);
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
            DoForAllServices(s => DoGetMessages(s, 0, 0));
            return null;
        }

        public IList<Post> GetMessagesSafe()
        {
            DoForAllServices(s => DoGetMessagesSafe(s, 0, false));
            return null;
        }

        void DoGetMessages(IEeeService service, int sleepSecs, int retryNo)
        {
            QueryInBackground(
                sleepSecs,
                () => 
                {
                    return service.GetMessages();
                },
                r => 
                {
                    IList<Post> posts = (IList<Post>)r;
                    this.Form.GetPlugin<UserStatePlugin>().SetUsersToPosts(posts);
                    AddPostsToBrowser(service, posts);
                    this.FirstGetMessages = false;
                    OnSucessfulRequest(SucessfulRequestEventArgs.Empty);
                    OnGetMessagesFinished(new GetMessagesFinishedEventArgs(posts));
                    DoGetMessages(service, service.Configuration.MessageGetInterval, 0);
                },
                e =>
                {
                    if (IsConnectionProblem(e))
                    {
                        /// In case this is a "Connection Problem", switch to safe mode.
                        /// If success, return to normal mode.
                        DoGetMessagesSafe(service, service.Configuration.MessageGetInterval, true);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(e));

                        int delay;

                        if (service.Configuration.MessageGetInstantRetryCount > retryNo)
                            delay = service.Configuration.MessageGetInstantRetryDelay;
                        else
                            delay = service.Configuration.MessageGetRetryDelay;

                        DoGetMessages(service, delay, retryNo + 1);
                    }
                }
            );
        }

        void AddPostsToBrowser(IEeeService service, IList<Post> posts)
        {
            if (posts.Count > 0)
            {
                bool canScroll = true; //TODO:
                bool willScroll = false;

                foreach (Post post in posts)
                {
                    post.Sent = post.Sent.ToLocalTime();

                    /// The notification is added when the post is not from me, and the room is not ignored, and the user is not ignored.
                    /// If the NotifyAboutIgnoredPersonalMessages is set, also show the notification if the room or user is ignored, but it is for me personally.
                    bool fromMe = post.From.Login == service.CurrentUser.Login;
                    //bool roomIgnored = IsRoomIgnored(post.Room.);
                    //bool userIgnored = IsUserIgnored(post.FromUserID);
                    //bool ignored = roomIgnored || userIgnored;

                    bool isPrivate = post.To != null;
                    bool forMe = isPrivate && post.To.Login == service.CurrentUser.Login;
                    bool showForMe = forMe && Properties.Settings.Default.NotifyAboutIgnoredPersonalMessages;

                    service.CommitMessage(post);

                    if (!this.Form.GetPlugin<CommandPostPlugin>().ProcessCommandPost(post))
                    {
                        if (!fromMe /*&& (ignored == false || showForMe)*/)
                        {
                            MessageType postType = forMe ? MessageType.Private : MessageType.Public;
                            this.Form.notificationManager.AddNotification(post.From.Login, post.From.Color, post.Text, postType);
                        }

                        this.Form.GetPlugin<BrowserPlugin>().AddMessage(post, this.FirstGetMessages, false);
                        willScroll = canScroll;
                    }
                }

                if (willScroll)
                    this.Form.GetPlugin<BrowserPlugin>().ScrollDown();
            }
        }

        void DoGetMessagesSafe(IEeeService service, int sleepSecs, bool returnToNormalModeOnSuccess)
        {
            QueryInBackground(
                sleepSecs,
                () =>
                {
                    return service.GetMessagesSafe();
                },
                r =>
                {
                    IList<Post> posts = (IList<Post>)r;
                    this.Form.GetPlugin<UserStatePlugin>().SetUsersToPosts(posts);
                    AddPostsToBrowser(service, posts);
                    this.FirstGetMessages = false;
                    OnSucessfulRequest(SucessfulRequestEventArgs.Empty);
                    OnGetMessagesFinished(new GetMessagesFinishedEventArgs(posts));

                    if (returnToNormalModeOnSuccess)
                        DoGetMessages(service, service.Configuration.MessageGetInterval, 0);
                    else
                        DoGetMessagesSafe(service, service.Configuration.MessageGetSafeInterval, false);
                },
                e =>
                {
                    OnErrorOccured(new ErrorOccuredEventArgs(e));
                    DoGetMessagesSafe(service, service.Configuration.MessageGetSafeInterval, returnToNormalModeOnSuccess);
                }
            );
        }

        public void SendMessage(Room room, User recipient, string message)
        {
            DoForAllServices(s => DoSendMessage(s, 0, room, recipient, message, null));
        }

        void DoSendMessage(IEeeService service, int retryNo, Room room, User recipient, string message, Post post)
        {
            int sleepSecs = 0;

            if (retryNo > 5)
                sleepSecs = 60;
            else if (retryNo > 0)
                sleepSecs = 5;

            BrowserPlugin browserPlugin = this.Form.GetPlugin<BrowserPlugin>();

            if (post == null)
            {
                //TODO: Review
                if (recipient != null && message.StartsWith(recipient.Login + ":"))
                    message = message.Substring(recipient.Login.Length + 1).Trim();

                post = new Post()
                {
                    From = service.CurrentUser,
                    GlobalId = Guid.NewGuid().ToString(),
                    Room = room,
                    To = recipient,
                    Sent = DateTime.Now,
                    Text = HttpUtility.HtmlEncode(message)
                };

                browserPlugin.AddMessage(post, false, false);
                browserPlugin.SetPostPending(post);
                browserPlugin.ScrollDown();
            }

            InvokeInBackground(
                sleepSecs,
                () => 
                {
                    service.SendMessage(room, recipient, message);
                },
                r =>
                {
                    browserPlugin.SetPostSent(post);
                },
                e =>
                {
                    if (e is ServiceException && ((ServiceException)e).Type == ServiceException.ExceptionType.UnknownRecipient)
                    {
                        post.Text = "<i>Message undeliverable - recipient unknown</i><br />" + post.Text;
                        browserPlugin.UpdatePost(post);
                    }
                    else
                    {
                        OnErrorOccured(new ErrorOccuredEventArgs(e));
                        DoSendMessage(service, retryNo + 1, room, recipient, message, post);
                    }
                }
            );
        }

        public void RegisterUser(string login, SecureString password, int color)
        {
        }

        public void SetAwayMode(string comment)
        {
        }

        public void ResetAwayMode()
        {
        }


        public void GetUpdates()
        {
        }

        public void AddMessage(int roomId, string recipientLogin, string message)
        {
        }

        public void SendFeedback(string from, string mail, string feedbackType, string description)
        {
        }

        public void DownloadFile(string link, string destinationDir)
        {
            string fileName = link;
            int lastSlashIdx = fileName.LastIndexOf('/');
            fileName = fileName.Substring(lastSlashIdx + 1);
            string filePath = Path.Combine(destinationDir, fileName);

            //foreach (InvocationParameters parameters in this.downloadInvocations)
            //{
            //    if (parameters.Arguments[1].Equals(filePath))
            //        return;
            //}

        }

        public void UploadFile(string filePath, object parameter)
        {
        }

        public void AddReport(string name, string value)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region IEeeService Members

        public string ServiceUrl
        {
            get
            {
                if (PluginHelper.Services.Count == 0 || PluginHelper.Services[0].Configuration == null)
                    return null;
                else
                    return PluginHelper.Services[0].Configuration.ServiceUrl;
            }
            set
            {
                if (PluginHelper.Services.Count > 0 && PluginHelper.Services[0].Configuration != null)
                    PluginHelper.Services[0].Configuration.ServiceUrl = value;
            }
        }

        public ProxySettings ProxySettings
        {
            get
            {
                if (PluginHelper.Services.Count == 0)
                    return null;
                else
                    return PluginHelper.Services[0].ProxySettings;
            }
            set
            {
                if (PluginHelper.Services.Count > 0)
                    PluginHelper.Services[0].ProxySettings = value;
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
