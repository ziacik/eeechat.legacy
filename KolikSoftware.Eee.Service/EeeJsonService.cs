using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;
using KolikSoftware.Eee.Service.Core;
using System.Security;
using KolikSoftware.Eee.Service.Exceptions;

namespace KolikSoftware.Eee.Service
{
    public class EeeJsonService : EeeServiceBase, IEeeService
    {        
        public class DynamicArgumentsHelper
        {
            public DynamicArgumentsHelper()
            {
                this.MessagesToCommit = new List<string>();
            }

            public int FromId { get; set; }
            public List<string> MessagesToCommit { get; set; }

            public string Commit
            {
                get
                {
                    return String.Join(",", this.MessagesToCommit.ToArray());
                }
            }

            /// <summary>
            /// Timeout for safe get.
            /// </summary>
            public int Timeout
            {
                get
                {
                    return 0;
                }
            }
        }

        class PostArguments
        {
            public string Room { get; set; }
            public string Recipient { get; set; }
            public string Text { get; set; }

            public PostArguments(Room room, User recipient, string text)
            {
                this.Room = room.Name;
                if (recipient != null)
                    this.Recipient = recipient.Login;
                this.Text = text;
            }
        }

        public DynamicArgumentsHelper ArgumentsHelper { get; set; }

        public EeeJsonService()
        {
            this.ArgumentsHelper = new DynamicArgumentsHelper();
            this.Configuration = new BindServiceConfiguration();
        }

        public AuthenticationData GetAuthenticationData(string login)
        {
            return Query<AuthenticationData>("AuthenticationData", "Login", login);
        }

        public void Connect(string login, SecureString password)
        {
            if (this.CurrentUser != null)
                Disconnect();

            AuthenticationData authenticationData = GetAuthenticationData(login);
            CreateHash(authenticationData, password);

            this.CurrentUser = Action<User>("UserState", "Login", login, "PasswordHash", this.PasswordHash, "State", (int)UserState.Connected);
        }

        public void Disconnect()
        {
            if (this.CurrentUser == null)
                return;

            Action<User>("UserState", "Login", this.CurrentUser.Login, "PasswordHash", this.PasswordHash, "State", (int)UserState.Disconnected);

            this.CurrentUser = null;
        }

        public IList<User> GetUsers()
        {
            return QueryList<User>("Users", () => this.CurrentUser.Login, () => this.PasswordHash);
        }

        public IList<Room> GetRooms()
        {
            return QueryList<Room>("Rooms", () => this.CurrentUser.Login, () => this.PasswordHash);
        }

        public IList<Post> GetMessages()
        {
            //TODO: prerobit nejako genericky?
            try
            {
                IList<Post> posts = LongQueryList<Post>("Messages", () => this.CurrentUser.Login, () => this.PasswordHash, () => this.ArgumentsHelper.FromId, () => this.ArgumentsHelper.Commit);
                this.ArgumentsHelper.MessagesToCommit.Clear();
                return posts;
            }
            catch (ServiceException ex)
            {
                if (ex.Type == ServiceException.ExceptionType.NoMessages)
                    return new List<Post>();
                else
                    throw;
            }
        }

        public IList<Post> GetMessagesSafe()
        {
            //TODO: prerobit nejako genericky?
            try
            {
                IList<Post> posts = QueryList<Post>("Messages", () => this.CurrentUser.Login, () => this.PasswordHash, () => this.ArgumentsHelper.FromId, () => this.ArgumentsHelper.Commit, () => this.ArgumentsHelper.Timeout);
                this.ArgumentsHelper.MessagesToCommit.Clear();
                return posts;
            }
            catch (ServiceException ex)
            {
                if (ex.Type == ServiceException.ExceptionType.NoMessages)
                    return new List<Post>();
                else
                    throw;
            }
        }

        public void SendMessage(Room room, User recipient, string message)
        {
            PostArguments args = new PostArguments(room, recipient, message);
            ActionResult result;

            if (recipient != null)
                result = Action("Post", () => this.CurrentUser.Login, () => this.PasswordHash, () => args.Room, () => args.Recipient, () => args.Text);
            else
                result = Action("Post", () => this.CurrentUser.Login, () => this.PasswordHash, () => args.Room, () => args.Text);

            if (result.Result != "OK")
                throw new ServiceException(result.Result);
        }

        public void CommitMessage(Post message)
        {
            if (message.Id >= this.ArgumentsHelper.FromId)
                this.ArgumentsHelper.FromId = message.Id + 1;

            if (message.To != null && message.To.Login == this.CurrentUser.Login)
                this.ArgumentsHelper.MessagesToCommit.Add(message.Id.ToString());
        }
    }
}
