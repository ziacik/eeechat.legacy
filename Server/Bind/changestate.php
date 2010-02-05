<?php
	require_once("common.php");

	$myUserID = $_POST["myUserID"];
	$myPasswordHash = $_POST["myPasswordHash"];
	$state = $_POST["state"];
	$comment = $_POST["comment"];
	$client = $_POST["client"];
	
	if ($client == null || $client == '')
		$client = 'Unknown';

	$result = ConnectRunQuery("SELECT UserID, Login, State FROM eee_User WHERE UserID = $myUserID AND Password = '$myPasswordHash'", "Unable to fetch user.");
	$user = mysql_fetch_object($result);
	mysql_free_result($result);
	
	if ($user)
	{
		if (($state > 0) && $user->State == 0)
			$query = "UPDATE eee_User SET Access = Now(), LoginTime = Now(), State=$state, Client='$client' WHERE UserID=$user->UserID";
		else
			$query = "UPDATE eee_User SET Access = Now(), State=$state WHERE UserID=$user->UserID";

		//TODO: Those two should run in transaction.
		ConnectRunQuery($query, "Unable to set user state.");		
		ConnectRunQuery("INSERT INTO eee_Message(FromUserID, ToUserID, Message, Time, Seen) VALUES($user->UserID, 0, '[STATE $user->Login;$state;$comment;$client]', Now(), 0)", "Unable to insert State message.");

		echo "<EeeResponse>OK</EeeResponse>";
	}
	else
	{
	    echo "<EeeResponse>BAD login</EeeResponse>";
	}	
?>
