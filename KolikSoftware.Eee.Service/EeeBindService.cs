using System;
using System.Net;
using System.Web;
using System.IO;
using System.Collections;
using System.Data;
using System.Security;
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Threading;

namespace KolikSoftware.Eee.Service
{
    public class EeeBindService : IEeeService
    {
        public string ServiceUrl { get; set; }

        private EeeDataSet.UserRow currentUser;
        private string passwordHash = null;

        int messageIdToStartAt = 0;

        ProxySettings proxySettings = null;

        string client;
        string clientVersion;
        string versionNumber;

        public int RequestsMade { get; private set; }
        public int BytesReceived { get; private set; }

        public EeeBindService(string phpServiceAddress, ProxySettings proxySettings, bool clientInstalled, string clientVersion, string versionNumber)
        {
            this.ServiceUrl = phpServiceAddress;
            this.proxySettings = proxySettings;

#if DEBUG
            this.client = "KolikSoftware.Eee.Client.Devel";
#else
            if (clientInstalled)
                this.client = "KolikSoftware.Eee.Client.Installed";
            else
                this.client = "KolikSoftware.Eee.Client.Binary";
#endif

            this.clientVersion = clientVersion;
            this.versionNumber = versionNumber;
        }

        public EeeDataSet.UserRow CurrentUser
        {
            get
            {                
                return this.currentUser;
            }
        }

        public string PasswordHash
        {
            get
            {
                return this.passwordHash;
            }
        }

        public bool RegisterUser(string login, SecureString password, int color)
        {
            //TODO: compatibility issue. remove
            EeeDataSet.LoginUserRow user = GetUser(login);
            if (user != null)
                return false;

            string salt = Security.CreateSalt(6);
            string passwordHash = Security.CreatePasswordHash(password, salt);

            string response = Invoke("register.php", "login", login, "passwordHash", passwordHash, "salt", salt, "color", color);
            return response == "OK";
        }

        public bool CheckUser(string login, string passwordHash)
        {
            string response = Invoke("checkuser.php", "login", login, "passwordHash", passwordHash);
            return response == "OK";
        }

        public void AutoLogoff()
        {
            Invoke("autologoff.php");
        }

        public bool ChangeState(int userId, string hash, UserState state, string comment)
        {
            string response = Invoke("changestate.php", "myUserID", userId, "myPasswordHash", hash, "state", (int)state, "comment", comment, "client", this.clientVersion);
            return response == "OK";
        }

        public EeeDataSet.LoginUserRow GetUser(string login)
        {
            EeeDataSet ds = InvokeToDataSet("getuser.php", "login", login);

            if (ds.LoginUser.Count > 0)
                return ds.LoginUser[0];
            else
                return null;
        }

        public string Version()
        {
            return Invoke("version.php");
        }

        public void AddReport(string name, string value)
        {
            Invoke("addreport.php", "name", name, "value", value);
        }

        public void SendFeedback(string from, string mail, string feedbackType, string description)
        {
            Invoke("userfeedback.php", "From", from, "Mail", mail, "Type", feedbackType, "Description", description);
        }

        #region Service Invoker
        protected string Invoke(string page, params object[] paramsAndValues)
        {
            string response = InvokeAny(false, page, "<EeeResponse>", "</EeeResponse>", paramsAndValues);
            return response.Substring("<EeeResponse>".Length, response.Length - "<EeeResponse>".Length - "</EeeResponse>".Length);
        }

        protected string InvokeLong(string page, params object[] paramsAndValues)
        {
            string response = InvokeAny(true, page, "<EeeResponse>", "</EeeResponse>", paramsAndValues);
            return response.Substring("<EeeResponse>".Length, response.Length - "<EeeResponse>".Length - "</EeeResponse>".Length);
        }

        protected virtual EeeDataSet InvokeToDataSet(string page, params object[] paramsAndValues)
        {
            bool invokeLong = false;

            if (page == "streammessages.php")
                invokeLong = true;

            EeeDataSet dataSet = new EeeDataSet();

            try
            {
                string response = InvokeAny(invokeLong, page, "<EeeDataSet xmlns=\"http://tempuri.org/EeeDataSet.xsd\">", "</EeeDataSet>", paramsAndValues);

                using (StringReader reader = new StringReader(response))
                {
                    dataSet.ReadXml(reader, XmlReadMode.Auto);
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.StartsWith("The server committed a protocol violation."))
                    return dataSet;
                else
                    throw;
            }

            return dataSet;
        }

        protected string InvokeAny(bool invokeLong, string page, string openTag, string closeTag, object[] paramsAndValues)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            for (int i = 0; i < paramsAndValues.Length; i++)
            {
                if (i != 0)
                    builder.Append("&");

                string param = paramsAndValues[i].ToString();
                i++;

                object objVal = paramsAndValues[i];
                if (objVal == null) objVal = "";

                string value = HttpUtility.UrlEncode(objVal.ToString());
                value = value.Replace("+", "%20");

                builder.Append(param);
                builder.Append("=");
                builder.Append(value);
            }

            string query = builder.ToString();

            string url = this.ServiceUrl;
            
            if (url.EndsWith("/") == false)
                url += "/";

            Uri uri;
            
            if (page.StartsWith("http://"))
                uri = new Uri(page);
            else
                uri = new Uri(url + page);

            HttpWebRequest request = null;
            WebResponse response = null;

            //TODO: Static proxy
            request = (HttpWebRequest)HttpWebRequest.Create(uri);

            if (this.proxySettings.Server.Length > 0)
                request.Proxy = new WebProxy(this.proxySettings.Server);
            else
                request.Proxy = WebRequest.GetSystemWebProxy();

            //if (this.proxySettings.NoCredentials == false)
            {
                if (this.proxySettings.User.Length > 0)
                    request.Proxy.Credentials = new NetworkCredential(this.proxySettings.User, this.proxySettings.Password, this.proxySettings.Domain);
                else
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(query);

            //request.ServicePoint.ConnectionLimit = 10;
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            if (invokeLong)
                request.Timeout = 1200000;

            //using
            Stream requestStream = request.GetRequestStream();
            //{
                requestStream.Write(data, 0, data.Length);
            //}

            this.RequestsMade++;

            using (response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string text = reader.ReadToEnd();

                        this.BytesReceived += System.Text.Encoding.UTF8.GetByteCount(text);

                        int startIdx = text.IndexOf(openTag);
                        int endIdx = text.IndexOf(closeTag);

                        if (startIdx < 0 || endIdx < 0)
                        {
                            startIdx = text.IndexOf("<EeeResponse>");
                            endIdx = text.IndexOf("</EeeResponse>");

                            if (startIdx < 0 || endIdx < 0)
                            {
                                throw new WebException("Bad response: " + text);
                            }
                            else
                            {
                                string eeeResponse = text.Substring(startIdx + "<EeeResponse>".Length, endIdx - startIdx - "<EeeResponse>".Length);

                                if (eeeResponse == "Invalid password or user ID." && this.CurrentUser != null)
                                    throw new DisconnectedException();
                                else
                                    throw new WebException(eeeResponse);
                            }
                        }

                        return AdjustResponse(page, openTag, closeTag, text.Substring(startIdx, endIdx - startIdx + closeTag.Length));
                    }
                }
            }
        }

        protected void InvokeBind(string page, params object[] paramsAndValues)
        {
            string openTag = "<EeeDataSet xmlns=\"http://tempuri.org/EeeDataSet.xsd\">";
            string closeTag = "</EeeDataSet>";

            StringBuilder stringStream = new StringBuilder();

            HttpWebRequest request = BuildRequest(page, paramsAndValues);            

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = null;

                        do
                        {
                            line = reader.ReadLine();
                            stringStream.Append(line);

                            string text = stringStream.ToString();

                            int startIdx = text.IndexOf(openTag);
                            int endIdx = text.IndexOf(closeTag);

                            if (startIdx < 0 || endIdx < 0)
                            {
                                startIdx = text.IndexOf("<EeeResponse>");
                                endIdx = text.IndexOf("</EeeResponse>");

                                if (startIdx >= 0 && endIdx >= 0)
                                {
                                    string eeeResponse = text.Substring(startIdx + "<EeeResponse>".Length, endIdx - startIdx - "<EeeResponse>".Length);

                                    if (eeeResponse == "Invalid password or user ID." && this.CurrentUser != null)
                                        throw new DisconnectedException();
                                    else
                                        throw new WebException(eeeResponse);
                                }
                            }
                            else
                            {
                                text = text.Substring(startIdx, endIdx - startIdx + 1);
                                stringStream.Remove(startIdx, endIdx - startIdx + 1);

                                EeeDataSet dataSet = new EeeDataSet();

                                using (StringReader dataReader = new StringReader(text))
                                {
                                    dataSet.ReadXml(dataReader, XmlReadMode.Auto);
                                }

                                foreach (EeeDataSet.MessageRow messageRow in dataSet.Message)
                                {
                                    OnMessageAvailable(new MessageAvailableEventArgs(messageRow));
                                }
                            }
                        }
                        while (line != null);
                    }
                }
            }
        }

        private HttpWebRequest BuildRequest(string page, object[] paramsAndValues)
        {
            string query = BuildQuery(paramsAndValues);
            string url = this.ServiceUrl;

            if (url.EndsWith("/") == false)
                url += "/";

            Uri uri;

            if (page.StartsWith("http://"))
                uri = new Uri(page);
            else
                uri = new Uri(url + page);

            HttpWebRequest request = null;

            //TODO: Static proxy
            request = (HttpWebRequest)HttpWebRequest.Create(uri);

            if (this.proxySettings.Server.Length > 0)
                request.Proxy = new WebProxy(this.proxySettings.Server);
            else
                request.Proxy = WebRequest.GetSystemWebProxy();

            //if (this.proxySettings.NoCredentials == false)
            {
                if (this.proxySettings.User.Length > 0)
                    request.Proxy.Credentials = new NetworkCredential(this.proxySettings.User, this.proxySettings.Password, this.proxySettings.Domain);
                else
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(query);

            //request.ServicePoint.ConnectionLimit = 10;
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            request.Timeout = 120000;

            //using
            Stream requestStream = request.GetRequestStream();
            //{
            requestStream.Write(data, 0, data.Length);
            //}
            return request;
        }

        protected string BuildQuery(object[] paramsAndValues)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < paramsAndValues.Length; i++)
            {
                if (i != 0)
                    builder.Append("&");

                string param = paramsAndValues[i].ToString();
                i++;

                object objVal = paramsAndValues[i];
                if (objVal == null) objVal = "";

                string value = HttpUtility.UrlEncode(objVal.ToString());
                value = value.Replace("+", "%20");

                builder.Append(param);
                builder.Append("=");
                builder.Append(value);
            }

            string query = builder.ToString();
            return query;
        }



        protected virtual string AdjustResponse(string page, string openTag, string closeTag, string response)
        {
            return response;
        }
        #endregion

        #region IEeeService Members
        public bool ConnectUser(string login, SecureString password)
        {
            if (this.currentUser != null)
                DisconnectUser();

            //TODO: compatibility issue. remove
            EeeDataSet.LoginUserRow user = GetUser(login);
            if (user == null)
                return false;

            string passwordHash = Security.CreatePasswordHash(password, user.Salt);

            if (ChangeState(user.UserID, passwordHash, UserState.Connected, ""))
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
            {
                return false;
            }
        }        

        public void DisconnectUser()
        {
            if (this.currentUser == null)
                return;

            ChangeState(this.currentUser.UserID, this.passwordHash, UserState.Disconnected, "");

            this.currentUser.Table.Dispose();
            this.currentUser = null;
        }

        public void SetAwayMode(string comment)
        {
            if (ChangeState(this.currentUser.UserID, this.passwordHash, UserState.Away, comment))
                this.currentUser.State = (int)UserState.Away;
        }

        public void ResetAwayMode()
        {
            if (ChangeState(this.currentUser.UserID, this.passwordHash, UserState.Connected, ""))
                this.currentUser.State = (int)UserState.Connected;
        }

        public EeeDataSet.RoomDataTable GetRooms()
        {
            EeeDataSet ds = InvokeToDataSet("getrooms.php", "myUserID", this.currentUser.UserID, "myPasswordHash", this.passwordHash);
            return ds.Room;
        }

        public EeeDataSet.UserDataTable GetUsers()
        {
            EeeDataSet ds = InvokeToDataSet("getusers.php", "myUserID", this.currentUser.UserID, "myPasswordHash", this.passwordHash);
            return ds.User;
        }

        public event EventHandler<MessageAvailableEventArgs> MessageAvailable;

        protected virtual void OnMessageAvailable(MessageAvailableEventArgs e)
        {
            EventHandler<MessageAvailableEventArgs> handler = this.MessageAvailable;
            if (handler != null) handler(this, e);
        }

        public EeeDataSet.MessageDataTable GetMessages()
        {
            //EeeDataSet ds = InvokeToDataSet("getmessages.php", "myUserID", this.currentUser.UserID, "myPasswordHash", this.passwordHash, "fromID", this.messageIdToStartAt);
            EeeDataSet ds = InvokeToDataSet("streammessages.php", "myUser", this.currentUser.Login, "fromID", this.messageIdToStartAt);

            if (ds.Message.Count > 0)
            {
                int maxId = (int)ds.Message.Compute("Max(" + ds.Message.MessageIDColumn.ColumnName + ")", null);
                this.messageIdToStartAt = maxId + 1;
            }

            //InvokeBind("streammessages.php", "myUser", this.currentUser.Login);

            return ds.Message;
        }

        int maxMessageId = 0;

        public EeeDataSet.MessageDataTable GetMessagesTran()
        {
            EeeDataSet ds = InvokeToDataSet("getmessages.php", "myUserID", this.currentUser.UserID, "myPasswordHash", this.passwordHash, "fromID", this.messageIdToStartAt);

            if (ds.Message.Count > 0)
            {
                int maxId = (int)ds.Message.Compute("Max(" + ds.Message.MessageIDColumn.ColumnName + ")", null);
                this.maxMessageId = maxId;
            }

            return ds.Message;
        }

        public void GetMessagesCommit()
        {
            this.messageIdToStartAt = this.maxMessageId + 1;
        }

        public EeeDataSet.UpdateDataTable GetUpdates(int lastUpdateId)
        {
            EeeDataSet ds = InvokeToDataSet("getupdates.php", "myUserID", this.currentUser.UserID, "myPasswordHash", this.passwordHash, "fromID", lastUpdateId, "client", this.client, "version", this.versionNumber);
            return ds.Update;
        }

        public virtual bool AddMessage(int roomId, string recipientLogin, string message, string externalFrom)
        {
            string response = Invoke("addmessage.php", "myUserID", this.currentUser.UserID, "myPasswordHash", this.passwordHash, "roomID", roomId, "toUserLogin", recipientLogin, "message", message, "externalFrom", externalFrom);
            return response == "OK";
        }

        public string UploadFile(string filePath, object parameter)
        {
            string fileId = RemoveDiacriticsAndSpecials(Path.GetFileName(filePath));

            using (FileStream stream = File.OpenRead(filePath))
            {
                int count;

                byte[] buffer = new byte[502400];

                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string tempFilePath = Path.GetTempFileName();

                    try
                    {
                        using (FileStream tempStream = File.OpenWrite(tempFilePath))
                        {
                            tempStream.Write(buffer, 0, count);
                        }

                        if (!UploadFile(fileId, tempFilePath))
                            return null;
                    }
                    finally
                    {
                        try { File.Delete(tempFilePath); }
                        catch { }
                    }
                }
            }

            string url = this.ServiceUrl;

            if (url.EndsWith("/") == false)
                url += "/";

            return url + "Upload/" + fileId;
        }

        string RemoveDiacriticsAndSpecials(string text)
        {
            text = text.Normalize(System.Text.NormalizationForm.FormD);

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];

                if (CharUnicodeInfo.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark)
                {
                    if ((currentChar >= '0' && currentChar <= '9') || (currentChar >= 'a' && currentChar <= 'z') || (currentChar >= 'A' && currentChar <= 'Z') || (currentChar == '-') || (currentChar == '_') || (currentChar == '.'))
                        builder.Append(text[i]);
                    else
                        builder.Append('-');
                }
            }

            return builder.ToString();
        }

        bool UploadFile(string fileId, string filePath)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "binary/octet-stream");

                //TODO: Parameters to POST

                client.QueryString.Add("myUserID", this.currentUser.UserID.ToString());
                client.QueryString.Add("myPasswordHash", this.passwordHash);
                client.QueryString.Add("fileId", fileId);

                if (this.proxySettings.Server.Length > 0)
                    client.Proxy = new WebProxy(this.proxySettings.Server);
                else
                    client.Proxy = WebRequest.GetSystemWebProxy();

                //if (this.proxySettings.NoCredentials == false)
                {
                    if (this.proxySettings.User.Length > 0)
                        client.Proxy.Credentials = new NetworkCredential(this.proxySettings.User, this.proxySettings.Password, this.proxySettings.Domain);
                    else
                        client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                string url = this.ServiceUrl;

                if (url.EndsWith("/") == false)
                    url += "/";

                byte[] responseBytes = client.UploadFile(url + "upload.php", "POST", filePath);
                string response = Encoding.ASCII.GetString(responseBytes);
                response = response.Substring("<EeeResponse>".Length, response.Length - "<EeeResponse>".Length - "</EeeResponse>".Length);
                return response == "OK";
            }
        }

        public void DownloadFile(string link, string file)
        {
            using (WebClient client = new WebClient())
            {
                if (this.proxySettings.Server.Length > 0)
                    client.Proxy = new WebProxy(this.proxySettings.Server);
                else
                    client.Proxy = WebRequest.GetSystemWebProxy();

                //if (this.proxySettings.NoCredentials == false)
                {
                    if (this.proxySettings.User.Length > 0)
                        client.Proxy.Credentials = new NetworkCredential(this.proxySettings.User, this.proxySettings.Password, this.proxySettings.Domain);
                    else
                        client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                if (link.IndexOf('?') >= 0)
                    link = link + "&nocache=" + Guid.NewGuid().ToString();
                else
                    link = link + "?nocache=" + Guid.NewGuid().ToString();

                Uri uri = new Uri(link);

                client.DownloadFile(uri, file);
            }
        }
        #endregion

        #region Jabber
        public void JabberConnect(string jabberId, string password)
        {
            string response = InvokeAny(true, "http://kolik.aspone.cz/connect.aspx", "<EeeResponse>", "</EeeResponse>", new object[] { "login", jabberId, "password", password, "eeeUser", this.CurrentUser.Login, "eeePasswordHash", this.passwordHash });
            if (response.Contains("OK") == false && response.Contains("Another") == false)
                throw new WebException("Jabber response: " + response); //TODO: should be not a WebException
        }

        public void JabberRenew(string jabberId)
        {
            string response = InvokeAny(false, "http://kolik.aspone.cz/renew.aspx", "<EeeResponse>", "</EeeResponse>", new object[] { "login", jabberId });
            if (response.Contains("OK") == false)
                throw new WebException("Jabber renew response: " + response); //TODO: should be not a WebException
        }

        public void JabberSend(string jabberId, string externalUser, string text, string externalNick)
        {
            string response = InvokeAny(false, "http://kolik.aspone.cz/send.aspx", "<EeeResponse>", "</EeeResponse>", new object[] { "myPasswordHash", this.passwordHash, "login", jabberId, "to", externalUser, "message", text, "nick", externalNick });
            if (response.Contains("OK") == false)
                throw new WebException("Jabber send response: " + response); //TODO: should be not a WebException
        }

        public void JabberDisconnect(string jabberId)
        {
            string response = InvokeAny(false, "http://kolik.aspone.cz/disconnect.aspx", "<EeeResponse>", "</EeeResponse>", new object[] { "myPasswordHash", this.passwordHash, "login", jabberId });
            if (response.Contains("OK") == false)
                return; //TODO: log ("Jabber disconnect response: " + response); //TODO: should be not a WebException
        }
        #endregion

        public void GetUsersCommit()
        {
            throw new NotImplementedException();
        }
    }
}
