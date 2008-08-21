<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "kuratko") or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

	$myUserID = $_POST["myUserID"];
	$myPasswordHash = $_POST["myPasswordHash"];
	$fromID = $_POST["fromID"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
		$query = "SELECT * FROM eee_Room";;

	    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

	    print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	    while ($room = mysql_fetch_object($result))
	    {
			print '<Room>';
	    	print "<RoomID>$room->RoomID</RoomID>";
	    	print "<Name>$room->Name</Name>";
	    	print '</Room>';
	    }

	    print '</EeeDataSet>';
	}
	else
	{
		print "<EeeResponse>Invalid password or user ID.</EeeResponse>";
	}

    mysql_free_result($result);

    mysql_close($link);
?>
