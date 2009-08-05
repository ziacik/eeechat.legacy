<?php
    require_once("Open.php");

	$fromID = $_POST["fromID"];

	$query = "SELECT M.MessageID, M.FromUserID, M.RoomID, M.ToUserID, M.Message, M.Time, M.Seen, U.Login, U.Color";
	$query = $query . " FROM eee_Message as M, eee_User as U, eee_User as Ja";
	$query = $query . " WHERE Ja.UserID=$myUserID AND M.FromUserID=U.UserID";
	$query = $query . " AND	( (M.ToUserID=$myUserID AND M.Seen=0) OR (M.MessageID>=$fromID AND M.Time>=Ja.LoginTime AND (M.ToUserID IS NULL OR M.ToUserID=0 OR M.FromUserID=$myUserID)) )";
	$query = $query . " ORDER BY M.Time";

    $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

    $data = '';

    while ($message = mysql_fetch_object($result))
    {
    	$time = gmdate('Y-m-d\TH\:i\:s', strtotime($message->Time)) . '.0000000-00:00';
		$data = '<Message>';
    	$data = $data . "<MessageID>$message->MessageID</MessageID>";
    	$data = $data . "<FromUserID>$message->FromUserID</FromUserID>";
    	$data = $data . "<ToUserID>$message->ToUserID</ToUserID>";

        if (isset($message->RoomID))
			$data = $data . "<RoomID>$message->RoomID</RoomID>";

    	$data = $data . "<Message>" . htmlspecialchars($message->Message) . "</Message>";
    	$data = $data . "<Time>$time</Time>";
    	$data = $data . "<Seen>$message->Seen</Seen>";
    	$data = $data . "<Login>$message->Login</Login>";
    	$data = $data . "<Color>$message->Color</Color>";
    	$data = $data . '</Message>';
    }

    require("Encrypt.php");

    print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';
    print base64_encode($encryptedData);
    print '</EeeDataSet>';


	$query = "UPDATE eee_Message SET Seen=1 WHERE ToUserID=$myUserID AND Seen=0";
	mysql_query($query);

	$query = "UPDATE eee_User SET Access = Now() WHERE UserID=$myUserID";
	mysql_query($query);


    include("Close.php");
?>

