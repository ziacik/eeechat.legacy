using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace KolikSoftware.Eee.Client.Notifications
{
    public enum IconNotificationType
    {
        Blink,
        Change,
        Nothing
    }

    public enum NotificationClickAction
    {
        Activate,
        Restart
    }

    public partial class NotificationManager : Component, IBindableComponent
    {
        public NotificationManager()
        {
            InitializeComponent();
        }

        public NotificationManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region Private
        struct NotificationInfo
        {
            public string Caption;
            public Color Color;
            public string Message;
            public MessageType MessageType;
            public NotificationClickAction ClickAction;
        }

        List<NotificationInfo> notifications = new List<NotificationInfo>();

        bool notifyIconActive = false;
        #endregion

        #region Events
        public class ActivateEventArgs : EventArgs
        {
            public static readonly new ActivateEventArgs Empty = new ActivateEventArgs();
        }

        public event EventHandler<ActivateEventArgs> Activate;

        protected virtual void OnActivate(ActivateEventArgs e)
        {
            EventHandler<ActivateEventArgs> handler = Activate;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Properties
        bool enabled = false;

        [DefaultValue(false)]
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;

                if (value == false)
                    Stop();
            }
        }

        IconNotificationType iconNotificationType = IconNotificationType.Blink;

        [DefaultValue(IconNotificationType.Blink)]
        public IconNotificationType IconNotificationType
        {
            get
            {
                return this.iconNotificationType;
            }
            set
            {
                this.iconNotificationType = value;
            }
        }

        bool allowNotifications = true;

        [DefaultValue(true)]
        public bool AllowNotifications
        {
            get
            {
                return this.allowNotifications;
            }
            set
            {
                this.allowNotifications = value;
                if (value == false)
                    this.notifications.Clear();
            }
        }

        bool connectionBlinking = false;

        [DefaultValue(false)]
        public bool ConnectionBlinking
        {
            get
            {
                return this.connectionBlinking;
            }
            set
            {
                this.connectionBlinking = value;
            }
        }

        bool connectionNotifications = true;

        [DefaultValue(true)]
        public bool ConnectionNotifications
        {
            get
            {
                return this.connectionNotifications;
            }
            set
            {
                this.connectionNotifications = value;
            }
        }

        private NotifyIcon notifyIcon = null;

        [DefaultValue(null)]
        public NotifyIcon NotifyIcon
        {
            get
            {
                return this.notifyIcon;
            }
            set
            {
                if (this.notifyIcon != null)
                    this.notifyIcon.BalloonTipClicked -= notifyIcon_BalloonTipClicked;

                this.notifyIcon = value;

                this.notifyIcon.BalloonTipClicked += notifyIcon_BalloonTipClicked;
            }
        }

        bool awayMode = false;

        [DefaultValue(false)]
        public bool AwayMode
        {
            get
            {
                return this.awayMode;
            }
            set
            {
                this.awayMode = value;
                ResetNotifyIcon();
            }
        }

        Icon normalIcon = null;

        [DefaultValue(null)]
        public Icon NormalIcon
        {
            get
            {
                return this.normalIcon;
            }
            set
            {
                this.normalIcon = value;
            }
        }

        Icon awayIcon = null;

        [DefaultValue(null)]
        public Icon AwayIcon
        {
            get
            {
                return this.awayIcon;
            }
            set
            {
                this.awayIcon = value;
            }
        }

        Icon messageIcon = null;

        [DefaultValue(null)]
        public Icon MessageIcon
        {
            get
            {
                return this.messageIcon;
            }
            set
            {
                this.messageIcon = value;
            }
        }
        #endregion

        #region Public
        public void AddNotification(string caption, int argb, string message, MessageType messageType)
        {
            AddNotification(caption, argb, message, messageType, NotificationClickAction.Activate);
        }

        public void AddNotification(string caption, int argb, string message, MessageType messageType, NotificationClickAction clickAction)
        {
            if (this.enabled || messageType == MessageType.System)
            {
                NotificationInfo notificationInfo = new NotificationInfo();
                notificationInfo.Caption = caption;
                notificationInfo.Color = Color.FromArgb(argb);
                notificationInfo.Message = message;
                notificationInfo.MessageType = messageType;
                notificationInfo.ClickAction = clickAction;

                AddIfNeeded(notificationInfo);
                BlinkIfNeeded(notificationInfo);
            }
        }

        public bool IsBlinking
        {
            get
            {
                return this.blinkingTimer.Enabled;
            }
        }

        public void StopBlinking()
        {
            this.blinkingTimer.Enabled = false;
            ResetNotifyIcon();
        }
        #endregion

        #region Private Methods
        void Stop()
        {
            HideNotification();
            RemoveAllNotifications();
            this.blinkingTimer.Enabled = false;
            ResetNotifyIcon();
        }

        void AddIfNeeded(NotificationInfo notificationInfo)
        {
            bool canAdd = true;

            if (notificationInfo.MessageType == MessageType.Connection)
                canAdd = canAdd && this.ConnectionNotifications;

            /// If the AlwaysNotifyAboutPersonalMessage is set, and this is a personal message, notify about it even if AllowNotifications is false.
            bool allowNotificationException = false;

            if (notificationInfo.MessageType == MessageType.Private && Properties.Settings.Default.AlwaysNotifyAboutPersonalMessage)
                allowNotificationException = true;

            if (notificationInfo.MessageType == MessageType.System || (canAdd && (this.AllowNotifications || allowNotificationException)))
            {
                this.notifications.Add(notificationInfo);
                if (this.notifications.Count == 1)
                    ShowNotification(notificationInfo);
            }
        }

        void BlinkIfNeeded(NotificationInfo notificationInfo)
        {
            bool canBlink = this.IconNotificationType != IconNotificationType.Nothing && notificationInfo.MessageType != MessageType.System;

            if (notificationInfo.MessageType == MessageType.Connection)
                canBlink = canBlink && this.ConnectionBlinking;

            if (canBlink && this.blinkingTimer.Enabled == false)
                this.blinkingTimer.Enabled = true;
        }

        void ShowNotification(NotificationInfo notification)
        {
            switch (notification.MessageType)
            {
                case MessageType.Private:
                    notification.Caption = "Private message from '" + notification.Caption + "':";
                    notification.Message = "[ " + notification.Message + " ]";
                    break;
                case MessageType.Public:
                    notification.Caption = "Message from '" + notification.Caption + "':";
                    break;
                case MessageType.Connection:
                    notification.Message = "[ " + notification.Message + " ]";
                    break;
            }

            if (notification.Message == null || notification.Message.Trim().Length == 0)
                notification.Message = Properties.Resources.EmptyMessage;

            this.notifyIcon.ShowBalloonTip(1000 * 4, notification.Caption, notification.Message, ToolTipIcon.Info); //TODO: 4
            this.notifyIcon.Tag = notification;
            this.notificationTimer.Start();
        }

        void HideNotification()
        {
        }

        void ShowNextNotification()
        {
            if (this.notifications.Count > 0)
            {
                NotificationInfo notificationInfo = this.notifications[0];
                ShowNotification(notificationInfo);
            }
        }

        void RemoveOneNotification()
        {
            this.notifications.RemoveAt(0);
        }

        void RemoveAllNotifications()
        {
            this.notifications.Clear();
        }

        void ResetNotifyIcon()
        {
            if (this.AwayMode)
                this.notifyIcon.Icon = this.AwayIcon;
            else
                this.notifyIcon.Icon = this.NormalIcon;
        }

        void FlipNotifyIcon()
        {
            if (this.notifyIconActive && this.IconNotificationType == IconNotificationType.Blink)
            {
                ResetNotifyIcon();
                this.notifyIconActive = false;
            }
            else
            {
                this.notifyIcon.Icon = this.MessageIcon;
                this.notifyIconActive = true;
            }
        }
        #endregion

        #region Event Handlers
        void notificationTimer_Tick(object sender, EventArgs e)
        {
            if (this.notifications.Count > 0)
                RemoveOneNotification();

            if (this.notifications.Count > 0)
                ShowNextNotification();
            else
                HideNotification();
        }

        void blinkingTimer_Tick(object sender, EventArgs e)
        {
            FlipNotifyIcon();
        }

        void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            NotificationInfo notificationInfo = (NotificationInfo)this.notifyIcon.Tag;

            if (notificationInfo.ClickAction == NotificationClickAction.Activate)
                OnActivate(ActivateEventArgs.Empty);
            else if (notificationInfo.ClickAction == NotificationClickAction.Restart)
                Application.Restart();
        }
        #endregion

        #region IBindableComponent Members
        BindingContext bindingContext;
        ControlBindingsCollection bindings;

        public BindingContext BindingContext
        {
            get
            {
                if (this.bindingContext == null)
                    this.bindingContext = new BindingContext();
                return this.bindingContext;
            }
            set
            {
                this.bindingContext = value;
            }
        }

        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (this.bindings == null)
                    this.bindings = new ControlBindingsCollection(this);
                return this.bindings;
            }
        }
        #endregion
    }

    public enum MessageType
    {
        Public,
        Private,
        Connection,
        System
    }
}
