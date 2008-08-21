<?php
    $link = mysql_connect("localhost", "skeee", "kvetinka") or die("Could not connect");

    mysql_select_db("skeee") or die("Could not select database");

    mysql_query("SET NAMES latin2");

	$myUserID = $_GET["myUserID"];
	$myPasswordHash = $_GET["myPasswordHash"];
	$fromID = $_GET["fromID"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
		$query = "SELECT M.MessageID, M.FromUserID, M.RoomID, M.ToUserID, M.Message, M.Time, M.Seen, U.Login, U.Color";
		$query = $query . " FROM eee_Message as M, eee_User as U, eee_User as Ja";
		$query = $query . " WHERE Ja.UserID=$myUserID AND M.FromUserID=U.UserID";
		$query = $query . " AND	( (M.ToUserID=$myUserID AND M.Seen=0) OR (M.MessageID>=$fromID AND M.Time>=Ja.LoginTime AND (M.ToUserID IS NULL OR M.ToUserID=0 OR M.FromUserID=$myUserID)) )";
		$query = $query . " ORDER BY M.Time";

	    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

	    print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	    while ($message = mysql_fetch_object($result))
	    {
	    	$time = gmdate('Y-m-d\TH\:i\:s', strtotime($message->Time)) . '.0000000-00:00';
			print '<Message>';
	    	print "<MessageID>$message->MessageID</MessageID>";
	    	print "<FromUserID>$message->FromUserID</FromUserID>";
	    	print "<ToUserID>$message->ToUserID</ToUserID>";
	    	if (isset($message->RoomID))
				print "<RoomID>$message->RoomID</RoomID>";

	    	print "<Message>" . htmlspecialchars($message->Message) . "</Message>";
	    	print "<Time>$time</Time>";
	    	print "<Seen>$message->Seen</Seen>";
	    	print "<Login>$message->Login</Login>";
	    	print "<Color>$message->Color</Color>";
	    	print '</Message>';
	    }

	    print '</EeeDataSet>';

		$query = "UPDATE eee_Message SET Seen=1 WHERE ToUserID=$myUserID AND Seen=0";
		mysql_query($query);

		$query = "UPDATE eee_User SET Access = Now() WHERE UserID=$myUserID";
		mysql_query($query);
	}
	else
	{
		print "<EeeResponse>Invalid password or user ID.</EeeResponse>";
	}

    mysql_free_result($result);

    mysql_close($link);
?>

