<?php

header("Cache-Control: no-store, must-revalidate");
header("Expires: Mon, 26 Jul 1997 05:00:00 GMT");
header("Content-Type: text/plain");


$link = mysql_connect("localhost", "eeechatn_kolik", "XXX") or die("Could not connect");
mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");
mysql_query("SET NAMES latin2");

/*$myUserID = $_GET["myUserID"];
$myPasswordHash = $_GET["myPasswordHash"];
$fromID = $_GET["fromID"];*/

$fromID = $_POST["fromID"];
$myUser = $_POST["myUser"];

$userQuery = "SELECT UserID FROM eee_User WHERE Login='$myUser' AND State>0";

$result = mysql_query($userQuery) or die("<EeeResponse>Query failed</EeeResponse>");
$user = mysql_fetch_object($result);

$myUserID = $user->UserID;

if (!$user)
{
	mysql_free_result($result);
	mysql_close($link);
	
	die("<EeeResponse>Invalid password or user ID.</EeeResponse>");
}

mysql_free_result($result);
mysql_close($link);

$last = false;

while ($user && ! $last && ! connection_aborted())
{
	$link = mysql_connect("localhost", "eeechatn_kolik", "XXX") or die("Could not connect");
	mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");
	mysql_query("SET NAMES latin2");

	$query = "SELECT M.MessageID, M.FromUserID, M.RoomID, M.ToUserID, M.Message, M.Time, M.Seen, U.Login, U.Color";
	$query = $query . " FROM eee_Message as M, eee_User as U, eee_User as Ja";
	$query = $query . " WHERE Ja.UserID=$myUserID AND M.FromUserID=U.UserID";
	$query = $query . " AND	( (M.ToUserID=$myUserID AND M.Seen=0) OR (M.MessageID>=$fromID AND M.Time>=Ja.LoginTime AND (M.ToUserID IS NULL OR M.ToUserID=0 OR M.FromUserID=$myUserID)) )";
	$query = $query . " ORDER BY M.Time";

	$result = mysql_query($query) or die("<EeeResponse>Message query failed.</EeeResponse>");

	$first = true;
	$last = false;

	while ($message = mysql_fetch_object($result))
	{
		if ($first)
			print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

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

		$fromID = $message->MessageID + 1;
		$last = true;
		$first = false;
	}

	if ($last)
	{
		print '</EeeDataSet>';

		$query = "UPDATE eee_Message SET Seen=1 WHERE ToUserID=$myUserID AND Seen=0";
		mysql_query($query);
	}

	$query = "UPDATE eee_User SET Access = Now() WHERE UserID=$myUserID";
	mysql_query($query);

	$result = mysql_query($userQuery) or die("<EeeResponse>Query failed</EeeResponse>");
	$user = mysql_fetch_object($result);    

	mysql_free_result($result);
	mysql_close($link);

	print 'X';
	flush();
	sleep(1);
}

?>
