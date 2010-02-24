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
        public User CurrentUser { get; private set; }

        public AuthenticationData GetAuthenticationData(string login)
        {
            return Query<AuthenticationData>("AuthenticationData", "Login", login);
        }

        public bool ConnectUser(string login, SecureString password)
        {
            if (this.CurrentUser != null)
                DisconnectUser();

            AuthenticationData authenticationData = GetAuthenticationData(login);
            string passwordHash = SecurityHelper.CreatePasswordHash(password, authenticationData.Salt);

            /*if (ChangeState(user.UserID, passwordHash, UserState.Connected, ""))
            {
                EeeDataSet.UserDataTable userDataTable = new EeeDataSet.UserDataTable();

                userDataTable.BeginLoadData();
                userDataTable.Merge(user.Table);
                userDataTable[0].Login = login;
                userDataTable[0].State = (int)UserState.Connected;
                userDataTable.EndLoadData();

                this.currentUser = userDataTable[0];
                this.messageIdToStartAt = 0;
                this.passwordHash = passwordHash;
                return true;
            }
            else
            {*/
                return false;
            //}
        }

        public void DisconnectUser()
        {
            if (this.CurrentUser == null)
                return;

            //ChangeState(this.currentUser.UserID, this.passwordHash, UserState.Disconnected, "");

            this.CurrentUser = null;
        }
    }
}
