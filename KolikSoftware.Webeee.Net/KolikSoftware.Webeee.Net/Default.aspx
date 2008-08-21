<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KolikSoftware.Webeee.Net._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Webeee 2007</title>

    <script src="Script/Common.js" type="text/javascript"></script>

    <link href="Skins/Default/reset.css" rel="stylesheet" type="text/css" />
    <link href="Skins/Default/style.css" rel="stylesheet" type="text/css" />
    <link href="Skins/Default/Common.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <asp:Panel ID="scriptPanel" runat="server" Visible="false">

        <script type="text/javascript">
        var hasFocus = 0;
        var blinking = 0;
        var blinkPhase = 1;
        
        window.onfocus = function() 
        {
            hasFocus = 1;
            StopBlinking();
        }
        
        window.onblur = function()
        {
            hasFocus = 0;
        }        
        </script>

    </asp:Panel>
    <form id="mainForm" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
    <div id="wrap">
        <div id="header">
            <h1>
                Webeee.net
            </h1>
            <sub>
                web klient siete Eee
            </sub>
            <ul id="nav">
                <li><a href="mailto:koliksoftware@gmail.com">Kontakt</a></li>
                <li>
                    <asp:Panel ID="loginPanel" runat="server">
                        Login
                        <asp:TextBox ID="loginText" runat="server"></asp:TextBox>
                        Heslo
                        <asp:TextBox ID="passwordText" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:Button ID="connectButton" runat="server" Text="Pripoji" OnClick="connectButton_Click" />
                    </asp:Panel>
                    <asp:Panel ID="toolbarPanel" runat="server" Visible="false">
                        <asp:Button ID="disconnectButton" runat="server" Text="Odpoji" OnClick="disconnectButton_Click" />
                    </asp:Panel>
                </li>
            </ul>
        </div>
        <div id="content">
            
            <div id="ConnectionProblemsDiv" style="display: none;">
                Problémy s pripojením... prosím, èakajte.
             </div>
            <div id="ConnectionProblemsDescDiv" style="color: Red;">
            </div>
            
            <asp:Panel ID="workspacePanel" runat="server" Visible="false" CssClass="Workspace">
                <div id="MessagesDiv" style="width: 100%; height: 100%; border: solid 1px Black; overflow: scroll;">
                    <div id="MessagesContentDiv" style="padding: 1em 1em 1em 1em;">
                    </div>
                    <div id="ScrollFooterDiv" style="height: 1px;">
                    </div>
                </div>
            
                <div id="UsersContentDiv">
                </div>
            
                <div style="width: 100%;">
                    <textarea id="sendMessageText" cols="123" rows="6" style="width: 100%; height: 60px; background-color: #e6e6fa;" onkeypress="SendByKey(event)"></textarea>
                </div>
            
                <div id="RoomsContentDiv">
                </div>
            
                <input type="button" id="sendMessageButton" title="Odosla" value="Odosla" onclick="Send()" />

                <asp:Image ID="smilxeButton" runat="server" ImageUrl="~/Images/Smile/smile.gif" />
                
                
                <asp:Panel ID="smilePanel" runat="server">
                    <img src="Images/Smile/bad.gif" />
                    <img src="Images/Smile/bash.gif" />
                    <img src="Images/Smile/beta.gif" />
                    <img src="Images/Smile/beee.gif" />
                    <img src="Images/Smile/beer.gif" />
                    <br />
                    <img src="Images/Smile/yo.gif" />
                    <img src="Images/Smile/blow.gif" />
                    <img src="Images/Smile/blush.gif" />
                    <img src="Images/Smile/bye.gif" />
                    <img src="Images/Smile/clap.gif" />
                    <br />
                    <img src="Images/Smile/cool.gif" />
                    <img src="Images/Smile/rtfm.gif" />
                    <img src="Images/Smile/firefox.gif" />
                    <br />
                    <img src="Images/Smile/bleh.gif" />
                    <img src="Images/Smile/dontknow.gif" />
                    <img src="Images/Smile/drunk.gif" />
                    <img src="Images/Smile/eat.gif" />
                    <img src="Images/Smile/exclamation.gif" />
                    <br />
                    <img src="Images/Smile/dance.gif" />
                    <img src="Images/Smile/disgust.gif" />
                    <img src="Images/Smile/haha.gif" />
                    <img src="Images/Smile/help.gif" />
                    <img src="Images/Smile/lol.gif" />
                    <img src="Images/Smile/smile.gif" />
                    <img src="Images/Smile/sorry.gif" />
                    <br />
                    <img src="Images/Smile/mad.gif" />
                    <img src="Images/Smile/nono.gif" />
                    <img src="Images/Smile/notme.gif" />
                    <img src="Images/Smile/nyam.gif" />
                    <img src="Images/Smile/ok.gif" />
                    <img src="Images/Smile/play.gif" />
                    <br />
                    <img src="Images/Smile/puke.gif" />
                    <img src="Images/Smile/question.gif" />
                    <img src="Images/Smile/rolleyes.gif" />
                    <img src="Images/Smile/sad.gif" />
                    <img src="Images/Smile/secret.gif" />
                    <img src="Images/Smile/shocking.gif" />
                    <img src="Images/Smile/crazy.gif" />
                    <img src="Images/Smile/cry.gif" />
                    <br />
                    <img src="Images/Smile/lama.gif" />            
                    <img src="Images/Smile/starwars.gif" />
                    <img src="Images/Smile/sun.gif" />
                    <br />
                    <img src="Images/Smile/shutup.gif" />
                    <img src="Images/Smile/thinking.gif" />
                    <img src="Images/Smile/threat.gif" />
                    <img src="Images/Smile/thumbdown.gif" />
                    <img src="Images/Smile/thumbup.gif" />
                    <img src="Images/Smile/wallbash.gif" />
                    <img src="Images/Smile/wink.gif" />                    
                </asp:Panel>
                
                <ajax:CollapsiblePanelExtender ID="smileCollapsibleExtender" runat="server" AutoCollapse="false" AutoExpand="false" TargetControlID="smilePanel" CollapseControlID="smilxeButton" Collapsed="true" CollapsedSize="0" ExpandedSize="300" ExpandControlID="smilxeButton" ScrollContents="true" ExpandDirection="Vertical">
                </ajax:CollapsiblePanelExtender>
            </asp:Panel>
            
            <ajax:ResizableControlExtender ID="resizablePanelExtender" runat="server" MinimumHeight="60" MinimumWidth="50" TargetControlID="workspacePanel" HandleCssClass="HandleImage" ResizableCssClass="ResizingImage" OnClientResize="OnWorkspaceResize">
            </ajax:ResizableControlExtender>
            
            <asp:Panel ID="getMessagesTriggerPanel" runat="server" Visible="false">
                <script type="text/javascript">
                        setTimeout('GetUsers()', 4000);
                        setTimeout('GetRooms()', 4500);
                        setTimeout('GetMessages()', 5000);
                </script>
            </asp:Panel>
        </div>
    </div>
    </form>
</body>
</html>
