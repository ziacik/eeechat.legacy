using System;
using KolikSoftware.Eee.Service;

namespace KolikSoftware.Eee.Processor
{
    #region Generic Command EventArgs
    public class CommandEventArgs : EventArgs
    {
        protected string param1 = null;
        protected string param2 = null;
        protected string param3 = null;

        public string Param1
        {
            get
            {
                return param1;
            }
        }

        public string Param2
        {
            get
            {
                return param2;
            }
        }

        public string Param3
        {
            get
            {
                return param3;
            }
        }

        public CommandEventArgs(string param1)
        {
            this.param1 = param1;
        }

        public CommandEventArgs(string param1, string param2)
        {
            this.param1 = param1;
            this.param2 = param2;
        }

        public CommandEventArgs(string param1, string param2, string param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
        }
    }
    #endregion

    public delegate void CommandEventHandler(object sender, CommandEventArgs e);

    public class CommandProcessor
    {
        protected int myUserID;
        protected string myLogin;
        protected Notificator notificator;

        #region Events
        public class IdentifyRequestedEventArgs : EventArgs
        {
            public static readonly new IdentifyRequestedEventArgs Empty = new IdentifyRequestedEventArgs();
        }

        public event EventHandler<IdentifyRequestedEventArgs> IdentifyRequested;

        protected virtual void OnIdentifyRequested(IdentifyRequestedEventArgs e)
        {
            EventHandler<IdentifyRequestedEventArgs> handler = IdentifyRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class UserStateChangedEventArgs : EventArgs
        {
            private string userLogin;

            public string UserLogin
            {
                get
                {
                    return this.userLogin;
                }
                set
                {
                    this.userLogin = value;
                }
            }

            private UserState userState;

            public UserState UserState
            {
                get
                {
                    return this.userState;
                }
                set
                {
                    this.userState = value;
                }
            }

            private string comment;

            public string Comment
            {
                get
                {
                    return this.comment;
                }
                set
                {
                    this.comment = value;
                }
            }

            private int userID;

            public int UserID
            {
                get
                {
                    return this.userID;
                }
                set
                {
                    this.userID = value;
                }
            }

            string client;

            public string Client
            {
                get
                {
                    return this.client;
                }
                set
                {
                    this.client = value;
                }
            }

            public UserStateChangedEventArgs(int userID, string userLogin, UserState userState, string comment, string client)
            {
                this.userLogin = userLogin;
                this.userState = userState;
                this.comment = comment;
                this.userID = userID;
                this.client = client;
            }
        }

        public event EventHandler<UserStateChangedEventArgs> UserStateChanged;

        protected virtual void OnUserStateChanged(UserStateChangedEventArgs e)
        {
            EventHandler<UserStateChangedEventArgs> handler = UserStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ExternalUserStateChangedEventArgs : EventArgs
        {
            string userName;

            public string UserName
            {
                get
                {
                    return this.userName;
                }
                set
                {
                    this.userName = value;
                }
            }

            UserState userState;

            public UserState UserState
            {
                get
                {
                    return this.userState;
                }
                set
                {
                    this.userState = value;
                }
            }

            string comment;

            public string Comment
            {
                get
                {
                    return this.comment;
                }
                set
                {
                    this.comment = value;
                }
            }

            string userId;

            public string UserId
            {
                get
                {
                    return this.userId;
                }
                set
                {
                    this.userId = value;
                }
            }

            public ExternalUserStateChangedEventArgs(string userId, string userName, UserState userState, string comment)
            {
                this.userName = userName;
                this.userState = userState;
                this.comment = comment;
                this.userId = userId;
            }
        }

        public event EventHandler<ExternalUserStateChangedEventArgs> ExternalUserStateChanged;

        protected virtual void OnExternalUserStateChanged(ExternalUserStateChangedEventArgs e)
        {
            EventHandler<ExternalUserStateChangedEventArgs> handler = ExternalUserStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public CommandProcessor(Notificator notificator, int myUserID, string myLogin)
        {
            this.myUserID = myUserID;
            this.myLogin = myLogin;
            this.notificator = notificator;
        }

        public virtual void ProcessMessage(EeeDataSet.MessageRow message, out bool continueProcessing)
        {
            if (IsCommand(message))
            {
                continueProcessing = false;
                Command(message);
            }
            else
            {
                message.ColorHex = message.Color.ToString("X6");
                continueProcessing = true;
            }            
        }

        public virtual bool IsCommand(EeeDataSet.MessageRow message)
        {
            if (message.Message == null || message.Message.Length < 2)
                return false;
            else
                return message.Message[0] == '[' && message.Message[message.Message.Length - 1] == ']';
        }

        /// <summary>
        /// Processes a command.
        /// </summary>
        /// <returns>True if command is accepted and the message is not intended to be further processed.</returns>
        private void Command(EeeDataSet.MessageRow message)
        {
            string msg = message.Message;

            string cmd = msg.Trim();

            string[] commands = {"IDENTIFY", "STATE", "EXTERNALSTATE"};

            string command = null;
            string[] paramList = null;

            foreach (string commandName in commands)
            {
                if (msg.IndexOf(commandName) == 1)
                {
                    command = commandName;

                    int paramsLen = msg.Length - commandName.Length - 3;

                    if (paramsLen > 0)
                    {
                        paramList = msg.Substring(commandName.Length + 2, paramsLen).Split(';');
                    }
                }
            }

            try
            {
                switch (command)
                {
                    case "IDENTIFY":
                        OnIdentifyRequested(IdentifyRequestedEventArgs.Empty);
                        message.Message = "";
                        return;
                    case "STATE":
                        OnUserStateChanged(new UserStateChangedEventArgs(message.FromUserID, message.Login, (UserState)int.Parse(GetParam(paramList, 1)), GetParam(paramList, 2), GetParam(paramList, 3)));
                        return;
                    case "EXTERNALSTATE":
                        OnExternalUserStateChanged(new ExternalUserStateChangedEventArgs(message.ExternalFrom, GetParam(paramList, 0), GetExternalState(GetParam(paramList, 1)), GetParam(paramList, 2)));
                        return;
                }
            }
            catch (Exception ex)
            {
                this.notificator.ReportError(this, "Chyba pri spracovaní príkazu (" + ex.Message + ").", ex);
            }

            message.Message = "";
        }

        UserState GetExternalState(string stateName)
        {
            if (stateName == "Off")
                return UserState.Disconnected;
            else if (stateName == "On")
                return UserState.Connected;
            else
                return UserState.Away;
        }

        protected string GetParam(string[] paramList, int index)
        {
            if (paramList == null || paramList.Length <= index)
                return null;
            else
                return paramList[index];
        }

        protected string GetJoinedParams(string[] paramList, int fromIndex)
        {
            if (paramList == null || paramList.Length <= fromIndex)
                return null;
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                for (int i = fromIndex; i < paramList.Length; i++)
                {
                    sb.Append(paramList[i]);
                    if (i < paramList.Length - 1)
                        sb.Append(";");
                }

                return sb.ToString();
            }
        }

    }
}
