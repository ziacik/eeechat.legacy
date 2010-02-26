using System;
using System.Security;
using KolikSoftware.Eee.Service.Domain;
using System.Collections.Generic;

namespace KolikSoftware.Eee.Service
{
    /// <summary>
    /// Eee Service Interface.
    /// </summary>
    public interface IEeeService
    {
        /// <summary>
        /// Service configuration.
        /// </summary>
        IServiceConfiguration Configuration { get; }

        /// <summary>
        /// Proxy settings.
        /// </summary>
        ProxySettings ProxySettings { get; set; }

        /// <summary>
        /// Get data used for authentication of a user specified by login.
        /// </summary>
        AuthenticationData GetAuthenticationData(string login);

        /// <summary>
        /// Connects and logins the user at the web service. Returns false if login fails.
        /// </summary>
        void Connect(string login, SecureString password);

        /// <summary>
        /// Logouts user and disconnects from the web service.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets the list of the connected users.
        /// </summary>
        IList<User> GetUsers();

        /// <summary>
        /// Gets the list of chat rooms.
        /// </summary>
        IList<Room> GetRooms();

        /// <summary>
        /// Gets the list of new messages.
        /// </summary>
        IList<Post> GetMessages();

        /// <summary>
        /// Puts private message into commit queue.
        /// </summary>
        void CommitMessage(Post message);

        /// <summary>
        /// Gets currently connected user. Null if disconnected.
        /// </summary>
        User CurrentUser { get; }

        /*
        bool IsBound { get; }
        int RequestsMade { get; }
        int BytesReceived { get; }

        string PasswordHash { get; }

        EeeDataSet.UserRow CurrentUser { get; }

        string ServiceUrl { get; set; }

        ProxySettings ProxySettings { get; set; }

        /// <summary>
        /// Logouts user and disconnects from the web service.
        /// </summary>
        void DisconnectUser();

        /// <summary>
        /// Checks whether login, password combination is right.
        /// </summary>
        bool CheckUser(string login, string passwordHash);

        /// <summary>
        /// Registers new user. Returns false if the user already exists or register fails.
        /// </summary>
        bool RegisterUser(string login, SecureString password, int color);

        /// <summary>
        /// Sets the away mode and sends a comment.
        /// </summary>
        void SetAwayMode(string comment);

        /// <summary>
        /// Resets the away mode.
        /// </summary>
        void ResetAwayMode();

        /// <summary>
        /// Gets the list of chat rooms.
        /// </summary>
        EeeDataSet.RoomDataTable GetRooms();

        /// <summary>
        /// Gets the list of the connected users.
        /// </summary>
        EeeDataSet.UserDataTable GetUsers();

        /// <summary>
        /// Gets the list of new messages.
        /// </summary>
        EeeDataSet.MessageDataTable GetMessages(string commitMessages);

        /// <summary>
        /// Sends a new message to the server.
        /// </summary>
        bool AddMessage(int roomId, string recipientLogin, string message, string externalFrom);

        /// <summary>
        /// Sends a user feedback.
        /// </summary>
        void SendFeedback(string from, string mail, string feedbackType, string description);

        /// <summary>
        /// If there are any updates, gets update parameters.
        /// </summary>
        EeeDataSet.UpdateDataTable GetUpdates(int lastUpdateId);

        /// <summary>
        /// Sends a specific report to the server.
        /// </summary>
        void AddReport(string name, string value);

        void DownloadFile(string link, string filePath);

        /// <summary>
        /// Uploads file. Returns null if fails or link to file if success.
        /// </summary>
        string UploadFile(string filePath, object parameter);

        /// <summary>
        /// Connect to Jabber. Return only when connection is closed.
        /// </summary>
        void JabberConnect(string jabberId, string password);

        /// <summary>
        /// Renews Jabber connection so that it does not expire and disconnect at server.
        /// </summary>
        void JabberRenew(string jabberId);

        /// <summary>
        /// Sends a message to Jabber user.
        /// </summary>
        void JabberSend(string jabberId, string externalUser, string text, string externalNick);

        /// <summary>
        /// Disconnects from Jabber.
        /// </summary>
        void JabberDisconnect(string jabberId);

        EeeDataSet.MessageDataTable GetMessagesTran();

        void GetMessagesCommit();*/
    }
}