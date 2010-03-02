using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class RoomStatePlugin : IMainFormPlugin
    {
        public Room SelectedRoom { get; set; }

        Dictionary<string, Room> RoomsByName { get; set; }
        Dictionary<ToolStripButton, Room> RoomsByButton { get; set; }
        Dictionary<string, ToolStripButton> ButtonsByRoomName { get; set; }

        public MainForm Form { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;

            this.RoomsByButton = new Dictionary<ToolStripButton, Room>();
            this.RoomsByName = new Dictionary<string, Room>();
            this.ButtonsByRoomName = new Dictionary<string, ToolStripButton>();
        }

        public void SetRooms(IList<Room> rooms)
        {
            this.RoomsByName.Clear();

            foreach (Room room in rooms)
            {
                this.RoomsByName.Add(room.Name, room);
                AddRoomButton(room);
            }

            this.SelectedRoom = this.RoomsByName["Pokec"];
            UpdateRoomIcons();
        }

        void UpdateRoomIcons()
        {
            int index = 0;

            foreach (ToolStripItem item in this.Form.RoomsToolStrip.Items)
            {
                if (index >= 10) return;
                item.ImageIndex = index++;
            }
        }

        void AddRoomButton(Room room)
        {
            ToolStripButton button = new ToolStripButton(room.Name);
            button.ImageScaling = ToolStripItemImageScaling.None;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.CheckOnClick = true;
            button.CheckedChanged += new EventHandler(button_CheckedChanged);
            button.MouseUp += new MouseEventHandler(button_MouseUp);

            this.Form.RoomsToolStrip.Items.Add(button);

            this.RoomsByButton[button] = room;
            this.ButtonsByRoomName[room.Name] = button;
        }

        void button_MouseUp(object sender, MouseEventArgs e)
        {
        }

        bool FreezeEvents { get; set; }

        void button_CheckedChanged(object sender, EventArgs e)
        {
            if (this.FreezeEvents) return;
            this.FreezeEvents = true;

            try
            {
                ToolStripButton button = sender as ToolStripButton;

                if (button.Checked)
                {
                    foreach (ToolStripItem roomItem in this.Form.RoomsToolStrip.Items)
                    {
                        if (roomItem is ToolStripButton)
                        {
                            ToolStripButton roomButton = roomItem as ToolStripButton;

                            if (roomButton != button)
                                roomButton.Checked = false;
                        }
                    }

                    this.SelectedRoom = this.RoomsByButton[button];
                }
                else
                {
                    this.SelectedRoom = this.RoomsByName["Pokec"];
                }

                //TODO: RearrangeMessages();

            }
            finally
            {
                this.FreezeEvents = false;
            }
        }

        public Room GetRoom(string roomName)
        {
            Room room = null;

            if (!string.IsNullOrEmpty(roomName))
                this.RoomsByName.TryGetValue(roomName, out room);

            return room;       
        }
    }
}
