<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UsersPanel.ascx.cs" Inherits="KolikSoftware.Webeee.Net.Controls.UsersPanel" %>

Blabla

<asp:UpdatePanel ID="usersUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:RadioButtonList runat="server" ID="usersList" DataTextField="Login" DataValueField="UserID" AutoPostBack="True">
        </asp:RadioButtonList>
    </ContentTemplate>
</asp:UpdatePanel>