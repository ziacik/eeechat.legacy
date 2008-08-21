using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using KolikSoftware.Eee.Service;
using System.Collections.Generic;
using System.Xml;
using KolikSoftware.Eee.Client.Helpers;
using System.Text;
using KolikSoftware.Eee.Processor;
using System.IO;
using KolikSoftware.Webeee.Net.Controls;

namespace KolikSoftware.Webeee.Net
{
    public class MessageHelper
    {
        static readonly MessageHelper instance = new MessageHelper();

        public static MessageHelper Instance
        {
            get
            {
                return MessageHelper.instance;
            }
        }

        EeeDataSet.RoomDataTable rooms;

        public EeeDataSet.RoomDataTable Rooms
        {
            get
            {
                return this.rooms;
            }
            set
            {
                this.rooms = value;
            }
        }

        public string ConvertMessages(EeeDataSet.MessageDataTable messages, EeeDataSet.UserDataTable users, int myUserId, out bool hasUserChanges)
        {
            hasUserChanges = false;

            if (messages.Count > 0)
            {
                EeeDataSet.RoomRow mainRoom = null;

                if (this.rooms.Count > 0)
                    mainRoom = this.rooms[0];                

                /// Add notifications & set attributes.
                for (int i = 0; i < messages.Count; i++)
                {
                    EeeDataSet.MessageRow message = messages[i];

                    hasUserChanges |= CommandProcessor.Instance.ProcessMessage(message, users);

                    if (string.IsNullOrEmpty(message.Message))
                    {
                        message.Delete();
                        i--;
                    }
                    else
                    {
                        EeeDataSet.RoomRow roomRow = this.rooms.FindByRoomID(message.RoomID);

                        if (roomRow != null && roomRow != mainRoom)
                        {
                            message.Room = roomRow.Name;
                        }
                    }
                }

                messages.AcceptChanges();

                StringBuilder builder = new StringBuilder();

                if (messages.Count > 0)
                {
                    List<string> links = new List<string>();

                    XmlDocument transformedXml = MessageTextProcessor.Instance.ProcessMessages(messages, links);

                    foreach (XmlNode messageNode in transformedXml.SelectNodes("/root/message"))
                    {
                        string messageIdStr = messageNode.Attributes["messageId"].Value;

                        if (messageIdStr == null)
                            throw new ApplicationException("Wrong template: messageId not specified.");

                        int messageId;

                        if (!int.TryParse(messageIdStr, out messageId))
                            throw new ApplicationException("Wrong template: messageId format no good.");

                        EeeDataSet.MessageRow messageRow = messages.FindByMessageID(messageId);

                        if (messageRow == null)
                            throw new ApplicationException("Wrong template: message with specified id not found.");

                        builder.Append("<div>");
                        builder.Append(messageNode.InnerXml);
                        builder.Append("</div>\r\n");
                    }
                }

                return builder.ToString();
            }
            else
            {
                return "";
            }
        }

        public string ConvertUsers(EeeDataSet.UserDataTable users, int selectedUserId)
        {
            if (users.Count > 0)
            {
                RadioButtonList list = new RadioButtonList();
                list.RepeatDirection = RepeatDirection.Horizontal;
                list.RepeatLayout = RepeatLayout.Flow;
                list.Style.Add("font-family", "Arial");
                list.Style.Add("font-size", "11px");
                list.ID = "usersList";

                bool selectedAny = false;

                string style = "";

                ListItem allItem = new ListItem();
                allItem.Text = "Všetci";
                allItem.Value = "0";
                allItem.Attributes.Add("style", style);
                list.Items.Add(allItem);

                foreach (EeeDataSet.UserRow userRow in users)
                {
                    if (userRow.State != 0)
                    {
                        style = "";

                        ListItem item = new ListItem();
                        item.Text = userRow.Login;
                        item.Value = userRow.UserID.ToString();

                        if (userRow.UserID == selectedUserId)
                        {
                            item.Selected = true;
                            selectedAny = true;
                        }

                        if (userRow.State == 2)
                            style += "color: Red;";

                        item.Attributes.Add("style", style);
                        list.Items.Add(item);
                    }
                }

                if (!selectedAny)
                {
                    allItem.Selected = true;
                }

                StringBuilder builder = new StringBuilder();

                using (StringWriter textWriter = new StringWriter(builder))                
                using (HtmlTextWriter writer = new HtmlTextWriter(textWriter))
                {
                    list.RenderControl(writer);
                }

                return builder.ToString();
            }
            else
            {
                return "";
            }
        }

        public string ConvertRooms(EeeDataSet.RoomDataTable roomTable)
        {
            RadioButtonList list = new RadioButtonList();
            list.RepeatDirection = RepeatDirection.Horizontal;
            list.RepeatLayout = RepeatLayout.Flow;
            list.Style.Add("font-family", "Arial");
            list.Style.Add("font-size", "11px");
            list.ID = "roomsList";

            bool first = true;

            foreach (EeeDataSet.RoomRow roomRow in roomTable)
            {
                ListItem item = new ListItem();
                item.Text = roomRow.Name;
                item.Value = roomRow.RoomID.ToString();
                item.Selected = first;
                item.Attributes.Add("style", "");
                list.Items.Add(item);
                first = false;
            }

            StringBuilder builder = new StringBuilder();

            using (StringWriter textWriter = new StringWriter(builder))
            using (HtmlTextWriter writer = new HtmlTextWriter(textWriter))
            {
                list.RenderControl(writer);
            }

            return builder.ToString();
        }
    }
}
