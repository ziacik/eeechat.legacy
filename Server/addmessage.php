<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "kuratko") or die("<EeeResponse>Could not connect.</EeeResponse>");

    mysql_select_db("eeechatn_eeechatdb") or die("<EeeResponse>Could not select database.</EeeResponse>");
    
    mysql_query("SET NAMES latin2");

	$myPasswordHash = $_POST["myPasswordHash"];
	$myUserID = $_POST["myUserID"];
	$roomID = $_POST["roomID"];
	$toUserLogin = $_POST["toUserLogin"];
	$message = $_POST["message"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
	    $query = "SELECT UserID FROM eee_User WHERE Login='$toUserLogin'";
	    $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");
	    
	    if ($toUser = mysql_fetch_object($result))
	    	$toUserID = $toUser->UserID;
	    else
	    	$toUserID = 0;

	    $query = "INSERT INTO eee_Message(FromUserID, ToUserID, RoomID, Message, Time, Seen) VALUES($myUserID, $toUserID, $roomID, '$message', Now(), 0)";
	    mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

	    print "<EeeResponse>OK</EeeResponse>";
	}
	else
	{
		print "<EeeResponse>Invalid password or user ID.</EeeResponse>";
	}

    mysql_free_result($result);

    mysql_close($link);
?>
