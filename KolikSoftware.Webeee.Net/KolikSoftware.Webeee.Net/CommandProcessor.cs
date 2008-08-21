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
        static readonly CommandProcessor instance = new CommandProcessor();

        public static CommandProcessor Instance
        {
            get
            {
                return CommandProcessor.instance;
            }
        }

        public bool ProcessMessage(EeeDataSet.MessageRow message, EeeDataSet.UserDataTable users)
        {
            if (IsCommand(message))
                return Command(message, users);
            else
                message.ColorHex = message.Color.ToString("X6");
            
            return false;
        }

        public bool IsCommand(EeeDataSet.MessageRow message)
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
        bool Command(EeeDataSet.MessageRow message, EeeDataSet.UserDataTable users)
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

            bool hasUserChanges = false;

            try
            {
                switch (command)
                {
                    case "STATE":
                        hasUserChanges = true;
                        StateChanged(users, message.FromUserID, message.Login, (UserState)int.Parse(GetParam(paramList, 1)), GetParam(paramList, 2), GetParam(paramList, 3));
                        break;
                }
            }
            catch (Exception ex)
            {
                //TODO: this.notificator.ReportError(this, "Chyba pri spracovaní príkazu (" + ex.Message + ").", ex);
            }

            message.Message = "";
            return hasUserChanges;
        }

        void StateChanged(EeeDataSet.UserDataTable users, int userId, string login, UserState userState, string comment, string client)
        {
            EeeDataSet.UserRow userRow = users.FindByUserID(userId);

            if (userState == UserState.Disconnected)
            {
                if (userRow != null)
                    userRow.State = 0;
            }
            else if (userRow == null)
            {
                users.AddUserRow(userId, login, (int)userState, 0, "", comment, DateTime.Now, client);
            }
            else if (userRow != null)
            {
                userRow.State = (int)userState;
                userRow.AwayModeComment = comment;
                userRow.Client = client;
            }
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
