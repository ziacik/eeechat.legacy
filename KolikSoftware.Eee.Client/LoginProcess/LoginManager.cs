using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Security.Windows.Forms;
using System.Windows.Forms;
using System.Security;

namespace KolikSoftware.Eee.Client.LoginProcess
{
    public partial class LoginManager : Component
    {
        public LoginManager()
        {
            InitializeComponent();
        }

        public LoginManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        enum LoginPhase
        {
            Disconnected = 0,
            Phase1,
            Phase2,
            Connected
        }

        enum Operation
        {
            None,
            Connecting,
            Disconnecting
        }

        Operation operation = Operation.None;
        LoginPhase loginPhase = LoginPhase.Disconnected;
        bool wantToLogin = false;
        bool wantToLogout = false;
        bool forceLogout;
        
        string user;
        SecureString password;

        void IncreaseLoginPhase()
        {
            this.loginPhase = (LoginPhase)(((int)this.loginPhase) + 1);

            if (this.loginPhase == LoginPhase.Connected)
            {
                OnConnected(ConnectedEventArgs.Empty);
                OnAfterLogin(AfterLoginEventArgs.Empty);
                if (this.wantToLogout)
                    DoLogout(this.forceLogout);
            }
        }

        #region Properties
        BackgroundServiceController serviceController = null;

        [DefaultValue(null)]
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
                    this.serviceController.GetRoomsFinished -= serviceController_GetRoomsFinished;
                    this.serviceController.GetUsersFinished -= serviceController_GetUsersFinished;
                    this.serviceController.Disconnected -= serviceController_Disconnected;
                    this.serviceController.LoginFailed -= serviceController_LoginFailed;
                }

                this.serviceController = value;

                this.serviceController.Connected += serviceController_Connected;
                this.serviceController.GetUsersFinished += serviceController_GetUsersFinished;
                this.serviceController.GetRoomsFinished += serviceController_GetRoomsFinished;
                this.serviceController.Disconnected += serviceController_Disconnected;
                this.serviceController.LoginFailed += serviceController_LoginFailed;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.loginPhase == LoginPhase.Connected;
            }
        }
        #endregion

        #region Events
        public class BeforeLoginEventArgs : EventArgs
        {
            public static readonly new BeforeLoginEventArgs Empty = new BeforeLoginEventArgs();
        }

        public event EventHandler<BeforeLoginEventArgs> BeforeLogin;

        protected virtual void OnBeforeLogin(BeforeLoginEventArgs e)
        {
            EventHandler<BeforeLoginEventArgs> handler = BeforeLogin;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class AfterLoginEventArgs : EventArgs
        {
            public static readonly new AfterLoginEventArgs Empty = new AfterLoginEventArgs();
        }

        public event EventHandler<AfterLoginEventArgs> AfterLogin;

        protected virtual void OnAfterLogin(AfterLoginEventArgs e)
        {
            EventHandler<AfterLoginEventArgs> handler = AfterLogin;
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

        public class DisconnectedEventArgs : EventArgs
        {
            public static readonly new DisconnectedEventArgs Empty = new DisconnectedEventArgs();
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

        public class InvalidPasswordEventArgs : EventArgs
        {
            public static readonly new InvalidPasswordEventArgs Empty = new InvalidPasswordEventArgs();
        }

        public event EventHandler<InvalidPasswordEventArgs> InvalidPassword;

        protected virtual void OnInvalidPassword(InvalidPasswordEventArgs e)
        {
            EventHandler<InvalidPasswordEventArgs> handler = InvalidPassword;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public void Login(Form callerForm, bool mustShowDialog)
        {
            using (UserCredentialsDialog dialog = new UserCredentialsDialog())
            {
                dialog.Flags =
                    UserCredentialsDialogFlags.GenericCredentials |
                    UserCredentialsDialogFlags.ShowSaveCheckbox |
                    UserCredentialsDialogFlags.ExpectConfirmation;

                if (mustShowDialog)
                    dialog.Flags = dialog.Flags | UserCredentialsDialogFlags.AlwaysShowUI;

                if (Properties.Settings.Default.SaveCredentials)
                {
                    dialog.SaveChecked = true;
                }
                else
                {
                    dialog.Password = null;
                    dialog.User = Properties.Settings.Default.DefaultUser;
                }

                if (dialog.ShowDialog(callerForm) == DialogResult.OK)
                {
                    this.user = dialog.User;
                    this.password = dialog.Password.Copy();

                    DoLogin();

                    if (dialog.SaveChecked == false)
                        dialog.Password.Clear();

                    Properties.Settings.Default.SaveCredentials = dialog.SaveChecked;
                    Properties.Settings.Default.DefaultUser = dialog.User;
                    Properties.Settings.Default.Save();

                    dialog.ConfirmCredentials(true);
                }
            }
        }

        public void Logout(bool force)
        {
            DoLogout(force);
        }

        void DoLogin()
        {
            CheckServiceController();

            if (this.loginPhase != LoginPhase.Disconnected)
            {
                this.wantToLogin = true;
                if (this.operation != Operation.Disconnecting)
                    DoLogout(true);
            }

            if (this.loginPhase == LoginPhase.Disconnected)
            {
                this.wantToLogin = false;

                OnBeforeLogin(BeforeLoginEventArgs.Empty);

                this.serviceController.ConnectUser(this.user, this.password);
            }
        }

        void DoLogout(bool force)
        {
            CheckServiceController();

            if (this.loginPhase != LoginPhase.Connected)
            {
                if (this.operation == Operation.Connecting)
                {
                    this.wantToLogout = true;
                    this.forceLogout = force;
                }
            }

            if (this.loginPhase == LoginPhase.Connected)
            {
                this.wantToLogout = false;
                OnBeforeLogin(BeforeLoginEventArgs.Empty);
                this.serviceController.DisconnectUser(force);
            }
        }

        void CheckServiceController()
        {
            if (this.serviceController == null)
                throw new InvalidOperationException("The service controller has to be set.");
        }

        void serviceController_LoginFailed(object sender, BackgroundServiceController.LoginFailedEventArgs e)
        {
            this.loginPhase = LoginPhase.Disconnected;
            
            OnDisconnected(DisconnectedEventArgs.Empty);
            OnAfterLogin(AfterLoginEventArgs.Empty);
            OnInvalidPassword(InvalidPasswordEventArgs.Empty);
        }

        void serviceController_Disconnected(object sender, BackgroundServiceController.DisconnectedEventArgs e)
        {
            this.loginPhase = LoginPhase.Disconnected;

            OnDisconnected(DisconnectedEventArgs.Empty);
            OnAfterLogin(AfterLoginEventArgs.Empty);
            
            if (this.wantToLogin)
                DoLogin();
        }

        void serviceController_GetUsersFinished(object sender, BackgroundServiceController.GetUsersFinishedEventArgs e)
        {
            IncreaseLoginPhase();
        }

        void serviceController_GetRoomsFinished(object sender, BackgroundServiceController.GetRoomsFinishedEventArgs e)
        {
            IncreaseLoginPhase();
        }

        void serviceController_Connected(object sender, BackgroundServiceController.ConnectedEventArgs e)
        {
            IncreaseLoginPhase();

            this.serviceController.GetUsers();
            this.serviceController.GetRooms();
            this.serviceController.GetUpdates();
        }
    }
}
