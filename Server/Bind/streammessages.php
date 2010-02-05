<?php
	require_once("common.php");
	
	function IsUserTimestampHigher($userId, $timeStamp)
	{
		$result = ConnectRunQuery("SELECT Access FROM eee_User WHERE UserID=$userId", 'Unable to fetch user timestamp.');				
		$row = mysql_fetch_object($result);
		
		echo "Current timestamp: $timeStamp    User timestamp: " . $row->Access . "<br />";
		
		$isHigher = ($row->Access) > $timeStamp;
		mysql_free_result($result);
		
		return $isHigher;
	}
	
	function SetUserTimestamp($userId, $timeStamp)
	{
		ConnectRunQuery("UPDATE eee_User SET Access = '$timeStamp' WHERE UserID=$userId", 'Unable to set user timestamp.');
	}	
	
	function DoCycle($userId, $fromId)
	{
		$last = false;
		
		$startTime = date("Y-m-d H:i:s");
		SetUserTimestamp($userId, $startTime);		

		for ($cycleNo = 1; $cycleNo < 120; $cycleNo++)
		{		
			if ($any || connection_aborted())
				break;
				
			if (IsUserTimestampHigher($userId, $startTime))
				break;

			$query = "SELECT M.MessageID, M.FromUserID, M.RoomID, M.ToUserID, M.Message, M.Time, M.Seen, U.Login, U.Color";
			$query = $query . " FROM eee_Message as M, eee_User as U, eee_User as Ja";
			$query = $query . " WHERE Ja.UserID=$userId AND M.FromUserID=U.UserID";
			$query = $query . " AND	( (M.ToUserID=$userId AND M.Seen=0) OR (M.MessageID>=$fromId AND M.Time>=Ja.LoginTime AND (M.ToUserID IS NULL OR M.ToUserID=0 OR M.FromUserID=$userId)) )";
			$query = $query . " ORDER BY M.Time";

			$result = ConnectRunQuery($query, "Unable to get messages.");

			$first = true;
			$any = false;

			while ($message = mysql_fetch_object($result))
			{
				if ($first)
					echo '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

				$time = gmdate('Y-m-d\TH\:i\:s', strtotime($message->Time)) . '.0000000-00:00';
				
				echo '<Message>';
				echo "<MessageID>$message->MessageID</MessageID>";
				echo "<FromUserID>$message->FromUserID</FromUserID>";
				echo "<ToUserID>$message->ToUserID</ToUserID>";
				
				if (isset($message->RoomID))
					echo "<RoomID>$message->RoomID</RoomID>";

				echo "<Message>" . htmlspecialchars($message->Message) . "</Message>";
				echo "<Time>$time</Time>";
				echo "<Seen>$message->Seen</Seen>";
				echo "<Login>$message->Login</Login>";
				echo "<Color>$message->Color</Color>";
				echo '</Message>';
				
				$fromId = $message->MessageID + 1;
				$any = true;
				$first = false;
			}

			if ($any)
				echo '</EeeDataSet>';
			else
				sleep(1);
		}
		
		if (!$any)
		    echo "<EeeResponse>Timeout</EeeResponse>";
	}

	set_time_limit(0);

	//TODO: ConnectValidateUser($_POST["myUserID"], $_POST["myPasswordHash"]);

	$fromId = $_POST["fromID"];
	$myUser = $_POST["myUser"];
	$guid = $_POST["guid"];
	$commit = $_POST["commit"];

	$userQuery = "SELECT UserID FROM eee_User WHERE Login='$myUser' AND State>0";
	
	$result = ConnectRunQuery("SELECT UserID FROM eee_User WHERE Login='$myUser' AND State>0", "Unable to fetch user.");	
	$user = mysql_fetch_object($result);
	mysql_free_result($result);
	
	$userId = $user->UserID;

	if (!$user)
		die("<EeeResponse>Invalid password or user ID.</EeeResponse>");

	if ($commit)
		ConnectRunQuery("UPDATE eee_Message SET Seen=1 WHERE MessageId IN ($commit)", "Unable to commit messages.");

	DoCycle($userId, $fromId);
?>
	