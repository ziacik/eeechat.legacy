function StartBlinking()
{
    if ((blinking != 1) && (hasFocus == 0))
    {
        blinking = 1;
        blinkPhase = 1;
        Blink();
    }
}

function StopBlinking()
{
    blinking = 0;
}

function Blink()
{
    if (blinkPhase == 1)
    {
        document.title = "Nová správa";
        blinkPhase = 2;
    }
    else if (blinkPhase == 2)
    {
        document.title = "Webeee.net 2008";
        blinkPhase = 1;
    }
    
    if (blinking == 1)
        setTimeout("Blink()", 1000);
    else
        document.title = "Webeee.net 2008";
}

function SetError(error)
{
    $get('ConnectionProblemsDiv').style.display = "";
    
    if ((error._exceptionType != 'System.Net.WebException') && (error._exceptionType != 'System.Net.Sockets.SocketException'))            
    {
        $get('ConnectionProblemsDescDiv').innerHTML = error._message + " " + error._stackTrace;
    }
}

function ClearError()
{
    $get('ConnectionProblemsDiv').style.display = "none";
    $get('ConnectionProblemsDescDiv').innerHTML = "";
}

 function GetMessages()
 {
    PageMethods.GetMessages(OnGetMessagesSucceeded, OnGetMessagesFailed);
 }
 
function OnGetMessagesSucceeded(result, userContext, methodName)
{
    ClearError();

    if (result != '')
    {
        $get('MessagesContentDiv').innerHTML = $get('MessagesContentDiv').innerHTML + result;
        $get('ScrollFooterDiv').scrollIntoView(false);
        StartBlinking();
    }
        
    PageMethods.GetMessagesCommit(OnGetMessagesCommitSucceeded, OnGetMessagesFailed);
}

function OnGetMessagesFailed(error, userContext, methodName)
{
    SetError(error);
    setTimeout("GetMessages()", 5000);
}

function OnGetMessagesCommitSucceeded(result, userContext, methodName)
{
    ClearError();        
    setTimeout("GetMessages()", 5000);
}    

function OnGetMessagesCommitFailed(error, userContext, methodName)
{
    SetError(error);
    setTimeout("GetMessages()", 5000);
}



function GetUsers()
{
    PageMethods.GetUsers(GetSelectedUserId(), OnGetUsersSucceeded, OnGetUsersFailed);
}

function OnGetUsersSucceeded(result, userContext, methodName)
{
    if (result != '')    
        $get('UsersContentDiv').innerHTML = result;
        
    PageMethods.GetUsersCommit(OnGetUsersCommitSucceeded, OnGetUsersFailed);
}

function OnGetUsersFailed(error, userContext, methodName)
{
    SetError(error);
    setTimeout("GetUsers()", 5000);
}

function OnGetUsersCommitSucceeded(result, userContext, methodName)
{
    setTimeout("GetUsers()", 5000);
}    

function OnGetUsersCommitFailed(error, userContext, methodName)
{
    SetError(error);
    setTimeout("GetUsers()", 5000);
}    


function GetRooms()
{
    PageMethods.GetRooms(OnGetRoomsSucceeded, OnGetRoomsFailed);
}

function OnGetRoomsSucceeded(result, userContext, methodName)
{
    $get('RoomsContentDiv').innerHTML = result;
}

function OnGetRoomsFailed(error, userContext, methodName)
{
    SetError(error);
    setTimeout("GetRooms()", 5000);
}


function Send()
{
    $get("sendMessageButton").disabled = true;
    $get("sendMessageText").disabled = true;
    PageMethods.Send($get("sendMessageText").value, GetSelectedUserId(), GetSelectedRoomId(), OnSendSucceeded, OnSendFailed);
}

function OnSendSucceeded(result, userContext, methodName)
{
    $get("sendMessageText").value = "";
    $get("sendMessageButton").disabled = false;
    $get("sendMessageText").disabled = false;
}

function OnSendFailed(error, userContext, methodName)
{
    SetError(error);
    $get("sendMessageButton").disabled = false;            
    $get("sendMessageText").disabled = false;
}


function GetSelectedUserId()
{
    var usersContentDiv = $get("UsersContentDiv");
    var val = GetSelectedRadioValue(usersContentDiv.childNodes[0].childNodes);
    
    if (val == "")
        val = "0";
        
    return val;
}

function GetSelectedRoomId()
{
    var roomsContentDiv = $get("RoomsContentDiv");
    var val = GetSelectedRadioValue(roomsContentDiv.childNodes[0].childNodes);
    
    if (val == "")
        val = "0";
        
    return val;
}

function GetSelectedRadioValue(buttonGroup)
{
    for (var i = 0; i < buttonGroup.length; i++)
    {
        if (buttonGroup[i].childNodes[0].checked)
        {
            return buttonGroup[i].childNodes[0].value;
        }
    }

    return "";
} 
        

function SendByKey(event)
{
    var key = (event.which) ? event.which : event.keyCode;
    
    if (!event.shiftKey && (key == 13))
        Send();            
}

function OnWorkspaceResize(sender, eventArgs)
{
    var e = sender.get_element();
    PageMethods.OnResize(e.style.width, e.style.height);
}
