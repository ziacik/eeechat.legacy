using System;
using System.Security;

namespace KolikSoftware.Eee.Service
{
    /// <summary>
    /// Eee Service Interface.
    /// </summary>
    public interface IEeeService
    {
        int RequestsMade { get; }
        int BytesReceived { get; }

        string PasswordHash { get; }

        EeeDataSet.UserRow CurrentUser { get; }

        string ServiceUrl { get; set; }

        /// <summary>
        /// Connects and logins the user at the web service. Returns false if login fails.
        /// </summary>
        bool ConnectUser(string login, SecureString password);

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
        EeeDataSet.MessageDataTable GetMessages();

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

        void GetMessagesCommit();
    }
}