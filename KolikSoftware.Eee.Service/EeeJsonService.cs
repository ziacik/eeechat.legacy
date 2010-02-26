using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;
using KolikSoftware.Eee.Service.Core;
using System.Security;

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
            return QueryList<Post>("Messages", () => this.CurrentUser.Login, () => this.PasswordHash, () => this.ArgumentsHelper.FromId, () => this.ArgumentsHelper.Commit);
        }

        public void CommitMessage(Post message)
        {
            if (message.Id >= this.ArgumentsHelper.FromId)
                this.ArgumentsHelper.FromId = message.Id + 1;

            this.ArgumentsHelper.MessagesToCommit.Add(message.Id.ToString());
        }
    }
}
