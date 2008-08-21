<?php
    /* Connecting, selecting database */
    $link = mysql_connect("localhost", "skeee", "kvetinka")
        or die("Could not connect");

    mysql_select_db("skeee") or die("Could not select database");

	$myUserID = $_GET["myUserID"];
	$myPasswordHash = $_GET["myPasswordHash"];
	$state = $_GET["state"];
	$comment = $_GET["comment"];

    $query = "SELECT UserID, Login, State FROM eee_User WHERE UserID = $myUserID AND Password = '$myPasswordHash'";
    $result = mysql_query($query) or die("Query failed");

    if ($user = mysql_fetch_object($result))
    {
    	if ( ($state>0) && $user->State==0)
			$query = "UPDATE eee_User SET Access = Now(), LoginTime = Now(), State=$state WHERE UserID=$user->UserID";
		else
			$query = "UPDATE eee_User SET Access = Now(), State=$state WHERE UserID=$user->UserID";

	    mysql_query($query) or die("Query failed");

	    $query = "INSERT INTO eee_Message(FromUserID, ToUserID, Message, Time, Seen) VALUES($user->UserID, 0, '[STATE $user->Login;$state;$comment]', Now(), 0)";
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
