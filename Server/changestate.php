<?php
    /* Connecting, selecting database */
    $link = mysql_connect("localhost", "eeechatn_kolik", "kuratko")
        or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

    mysql_query("SET NAMES latin2");

	$myUserID = $_POST["myUserID"];
	$myPasswordHash = $_POST["myPasswordHash"];
	$state = $_POST["state"];
	$comment = $_POST["comment"];
	$client = $_POST["client"];
	
	if ($client == null || $client == '')
		$client = 'Unknown';

    $query = "SELECT UserID, Login, State FROM eee_User WHERE UserID = $myUserID AND Password = '$myPasswordHash'";
    $result = mysql_query($query) or die("Query failed");

    if ($user = mysql_fetch_object($result))
    {
    	if (($state > 0) && $user->State == 0)
			$query = "UPDATE eee_User SET Access = Now(), LoginTime = Now(), State=$state, Client='$client' WHERE UserID=$user->UserID";
		else
			$query = "UPDATE eee_User SET Access = Now(), State=$state WHERE UserID=$user->UserID";

	    mysql_query($query) or die("Query failed");

	    $query = "INSERT INTO eee_Message(FromUserID, ToUserID, Message, Time, Seen) VALUES($user->UserID, 0, '[STATE $user->Login;$state;$comment;$client]', Now(), 0)";
	    mysql_query($query) or die("Query failed");

	    print "<EeeResponse>OK</EeeResponse>";
    }
    else
    {
	    print "<EeeResponse>BAD login</EeeResponse>";
    }

    /* Free resultset */
    mysql_free_result($result);

    /* Closing connection */
    mysql_close($link);
?>
