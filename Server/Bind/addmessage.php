<?php
	require_once("common.php");
	
	ConnectValidateUser($_POST["myUserID"], $_POST["myPasswordHash"]);
	
	$myUserID = $_POST["myUserID"];
	$roomID = $_POST["roomID"];
	$toUserLogin = $_POST["toUserLogin"];
	$message = $_POST["message"];

	$toUserID = 0;

	if (strlen($toUserLogin))
	{
		$result = ConnectRunQuery("SELECT UserID FROM eee_User WHERE Login='$toUserLogin'", "Unable to fetch target user data.");
		
		if ($toUser = mysql_fetch_object($result))
			$toUserID = $toUser->UserID;
		
		mysql_free_result($result);
	}
	
	ConnectRunQuery("INSERT INTO eee_Message(FromUserID, ToUserID, RoomID, Message, Time, Seen) VALUES($myUserID, $toUserID, $roomID, '$message', Now(), 0)", "Unable to insert message.");

	echo "<EeeResponse>OK</EeeResponse>";	    
?>
