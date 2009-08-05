<?php
    require_once("Open.php");

	$roomID = $_POST["roomID"];
	$toUserLogin = $_POST["toUserLogin"];
	$message = $_POST["message"];

    $query = "SELECT UserID FROM eee_User WHERE Login='$toUserLogin'";
    $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

    if ($toUser = mysql_fetch_object($result))
    	$toUserID = $toUser->UserID;
    else
    	$toUserID = 0;
    	
    

    $query = "INSERT INTO eee_Message(FromUserID, ToUserID, RoomID, Message, Time, Seen) VALUES($myUserID, $toUserID, $roomID, '$message', Now(), 0)";
    mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

    print "<EeeResponse>OK</EeeResponse>";

    include("Close.php");
?>
