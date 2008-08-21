using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KolikSoftware.Eee.Client.Helpers
{
    class MessageVisibilityHelper
    {
        static readonly MessageVisibilityHelper instance = new MessageVisibilityHelper();

        public static MessageVisibilityHelper Instance
        {
            get
            {
                return MessageVisibilityHelper.instance;
            }
        }

        #region Regexes
        Dictionary<string, Regex> regexCache = new Dictionary<string, Regex>();

        Regex GetFromRoomRegex(int selectedRoomId)
        {
            string key = "FromRoom" + selectedRoomId;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + selectedRoomId + "$", RegexOptions.Compiled);
        }

        Regex GetIgnoredRoomsRegex(List<int> ignoredRooms)
        {
            string key = "IgnoredRooms";
            string regex = "(";

            foreach (int roomId in ignoredRooms)
            {
                regex += roomId + "|";
            }

            regex = regex.Substring(0, regex.Length - 1) + ")";
            key += regex;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + regex + "$", RegexOptions.Compiled);
        }

        Regex GetIgnoredUsersRegex(List<int> ignoredUsers)
        {
            string key = "IgnoredUsers";
            string regex = "(";

            foreach (int userId in ignoredUsers)
            {
                regex += userId + "|";
            }

            regex = regex.Substring(0, regex.Length - 1) + ")";
            key += regex;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + regex + "$", RegexOptions.Compiled);
        }

        Regex GetFromMeRegex(int myUserId)
        {
            string key = "FromMe" + myUserId;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + myUserId + "$", RegexOptions.Compiled);
        }

        Regex GetFromUserRegex(int selectedUserId)
        {
            string key = "FromUser" + selectedUserId;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + selectedUserId + "$", RegexOptions.Compiled);
        }

        Regex GetBodyToUserRegex(int selectedUserId, string selectedUserLogin)
        {
            string key = "BodyToUser" + selectedUserId;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + selectedUserLogin + ":", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        Regex GetBodyToMeRegex(int myUserId, string myLogin)
        {
            string key = "BodyToMe" + myUserId;

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex("^" + myLogin + ":", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        Regex GetBodyToAnybodyRegex()
        {
            string key = "BodyToAnybody";

            if (this.regexCache.ContainsKey(key))
                return this.regexCache[key];
            else
                return this.regexCache[key] = new Regex(@"^\w*:\s", RegexOptions.Compiled | RegexOptions.Singleline);
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Returns true if the message from the user should be visible.
        /// </summary>
        public bool UserVisible(string fromUserId, string messageText, int selectedUserId, string selectedUserLogin, bool meSelected, List<int> ignoredUsers, int myUserId, string myLogin)
        {
            if (selectedUserId == 0 && meSelected == false && ignoredUsers.Count == 0)
                return true;
            /// If the sender is ignored, do not show.
            else if (ignoredUsers.Count > 0 && GetIgnoredUsersRegex(ignoredUsers).IsMatch(fromUserId))
                return false;
            /// If I am selected only, show if I am the recipient or if I am the sender and somebody is recipient.
            else if (selectedUserId == 0 && meSelected)
                return GetBodyToMeRegex(myUserId, myLogin).IsMatch(messageText) || (GetFromMeRegex(myUserId).IsMatch(fromUserId) && GetBodyToAnybodyRegex().IsMatch(messageText));
            /// If I am selected and some user is selected, show if the message is from me to him or from him to me.
            else if (selectedUserId != 0 && meSelected)
                return (GetFromMeRegex(myUserId).IsMatch(fromUserId) && GetBodyToUserRegex(selectedUserId, selectedUserLogin).IsMatch(messageText)) || (GetFromUserRegex(selectedUserId).IsMatch(fromUserId) && GetBodyToMeRegex(myUserId, myLogin).IsMatch(messageText));
            /// If some user is selected but not me, show if the message is from me to him or the message if from him.
            else if (selectedUserId != 0 && !meSelected)
                return (GetFromMeRegex(myUserId).IsMatch(fromUserId) && GetBodyToUserRegex(selectedUserId, selectedUserLogin).IsMatch(messageText)) || GetFromUserRegex(selectedUserId).IsMatch(fromUserId);

            return true;
        }

        /// <summary>
        /// Returns true if the message from the room should be visible.
        /// </summary>
        public bool RoomVisible(string roomId, int selectedRoomId, List<int> ignoredRooms)
        {
            /// If the room is ignored, do not show.
            if (ignoredRooms.Count > 0 && GetIgnoredRoomsRegex(ignoredRooms).IsMatch(roomId))
                return false;
            /// If some room is selected and it is not from that room, do not show.
            else if (selectedRoomId != 0)
                return GetFromRoomRegex(selectedRoomId).IsMatch(roomId);
            else
                return true;
        }

        public bool IsAddressedMessage(string message)
        {
            return GetBodyToAnybodyRegex().IsMatch(message);
        }
        #endregion
    }
}
