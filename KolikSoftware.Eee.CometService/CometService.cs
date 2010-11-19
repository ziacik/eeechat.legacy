﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using KolikSoftware.Eee.Service;
using KolikSoftware.Eee.Service.Domain;
using KolikSoftware.Eee.Service.Exceptions;
using KolikSoftware.Eee.Service.Core;

namespace KolikSoftware.Eee.CometService
{
    public class CometService : EeeServiceBase, IEeeService
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

        class RegisterArguments
        {
            public string Login { get; set; }
            public string PasswordHash { get; set; }
            public string Salt { get; set; }
            public int Color { get; set; }
        }

        public DynamicArgumentsHelper ArgumentsHelper { get; set; }
        public string ApplicationVersion { get; set; }

        public CometService()
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

            this.CurrentUser = Action<User>("UserState", "Login", login, "PasswordHash", this.PasswordHash, "State", (int)UserState.Connected, "Client", this.ApplicationVersion);
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
            if (this.CurrentUser == null)
                return null;

            var users = QueryList<User>("Users", () => this.CurrentUser.Login, () => this.PasswordHash);

            foreach (var user in users)
            {
                DownloadUserImage(user);
            }

            return users;
        }

        void DownloadUserImage(User user)
        {
            using (var client = RequestFactory.Instance.CreateClient(this.ProxySettings))
            {
                //TODO: Hardcoded
                var imageUrl = "http://www.eeechat.net/Avatars/" + user.Login + "?nocache";

                var userImageDir = Path.GetDirectoryName(user.ImagePath);

                if (!Directory.Exists(userImageDir))
                    Directory.CreateDirectory(userImageDir);

                try
                {
                    client.DownloadFile(imageUrl, user.ImagePath);
                }
                catch (Exception ex)
                {
                    //TODO:
                }
            }
        }

        public IList<Room> GetRooms()
        {
            if (this.CurrentUser == null)
                return null;

            return QueryList<Room>("Rooms", () => this.CurrentUser.Login, () => this.PasswordHash);
        }

        public IList<Post> GetMessages()
        {
            //TODO: prerobit nejako genericky?
            try
            {
                if (this.CurrentUser == null)
                    return null;

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

            if (this.CurrentUser == null)
                return null;

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

        public bool RegisterUser(string login, SecureString password, int color)
        {
            var salt = SecurityHelper.CreateSalt(6);
            var hash = SecurityHelper.CreatePasswordHash(password, salt);

            var args = new RegisterArguments { Login = login, Color = color, Salt = salt, PasswordHash = hash };

            try
            {
                var result = Action("Register", () => args.Login, () => args.Color, () => args.Salt, () => args.PasswordHash);

                if (result.Result != "OK")
                    throw new ServiceException(result.Result); //TODO:
                else
                    return true;
                //TODO: throw new ServiceException(result.Result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ReplyTo(Post post, string message)
        {
            SendMessage(post.Room, post.From, message);
        }

        public void SendMessage(Room room, User recipient, string message)
        {
            if (this.CurrentUser == null)
                return;

            if (recipient != null && recipient.Login.IndexOf('@') >= 0)
                return;

            PostArguments args = new PostArguments(room, recipient, message);
            ActionResult result;

            if (recipient != null)
                result = Action("Post", () => this.CurrentUser.Login, () => this.PasswordHash, () => args.Room, () => args.Recipient, () => args.Text);
            else
                result = Action("Post", () => this.CurrentUser.Login, () => this.PasswordHash, () => args.Room, () => args.Text);

            if (result.Result != "OK")
                throw new ServiceException(result.Result);
        }

        public void UploadFile(UploadInfo info)
        {
            if (this.CurrentUser == null)
                return;

            info.IsFirst = true;

            byte[] buffer = new byte[100000];

            using (FileStream stream = new FileStream(info.FilePath, FileMode.Open, FileAccess.Read))
            {
                int count;

                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (count == buffer.Length)
                    {
                        info.Data = buffer;
                    }
                    else
                    {
                        info.Data = new byte[count];
                        Array.Copy(buffer, info.Data, count);
                    }

                    info.IsLast = stream.Position == stream.Length;

                    ActionResult result = Action("Upload", () => info.FileName, () => info.IsFirst, () => info.IsLast, () => info.Comment, () => info.Data);

                    if (result.Result != "OK")
                        throw new ServiceException(result.Result);

                    info.IsFirst = false;
                }
            }
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
