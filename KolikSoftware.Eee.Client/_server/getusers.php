<?php
    $link = mysql_connect("localhost", "skeee", "kvetinka") or die("Could not connect");

    mysql_select_db("skeee") or die("Could not select database");

	$myUserID = $_GET["myUserID"];
	$myPasswordHash = $_GET["myPasswordHash"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
    	$query = "SELECT * FROM eee_User WHERE State>0";
	    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

	    print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	    while ($user = mysql_fetch_object($result))
	    {
	    	print '<User>';
	    	print "<UserID>$user->UserID</UserID>";
	    	print "<Login>$user->Login</Login>";
	    	print "<State>$user->State</State>";
	    	print '</User>';
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
