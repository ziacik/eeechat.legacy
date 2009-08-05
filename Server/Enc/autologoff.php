<?php
    /* Connecting, selecting database */
    $link = mysql_connect("localhost", "eeechatn_kolik", "xxx")
        or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

    $query = "SELECT * FROM eee_User WHERE State>0 AND Access<(Now() - INTERVAL 2 HOUR)";
    $result = mysql_query($query) or die("Query failed");

    while ($user = mysql_fetch_object($result))
    {
    	$query = "UPDATE eee_User SET Access = Now(), State=0 WHERE UserID=$user->UserID";
	    mysql_query($query) or die("Query failed");

	    $query = "INSERT INTO eee_Message(FromUserID, ToUserID, Message, Time, Seen) VALUES($user->UserID, 0, '[STATE $user->Name;0;Autodisconnect]', Now(), 0)";
	    mysql_query($query) or die("Query failed");
	}

    print "<EeeResponse>OK</EeeResponse>";

    /* Free resultset */
    mysql_free_result($result);

    /* Closing connection */
    mysql_close($link);
?>
