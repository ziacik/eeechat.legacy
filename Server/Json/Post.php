<?php
	require_once("Common.php");
	require_once("CommonRoom.php");
	require_once("CommonUser.php");
	
	function Run()
	{
		$user = ValidateUser($_POST["Login"], $_POST["PasswordHash"], 1);
		$userId = $user->UserID;
		
		$recipient = $_POST["Recipient"];
		$room = $_POST["Room"];		
		$text = $_POST["Text"];
		
		$room = GetRoom($room);
		$roomId = $room->Id;

		$recipientId = 0;
		
		if (!empty($recipient))
		{
			$recipient = GetUser($recipient);
			
			if ($recipient)
				$recipientId = $recipient->Id;
			
			if (!$recipientId)
			{
				Result("UnknownRecipient");
				return;
			}
		}
					
		$query = sprintf("INSERT INTO eee_Message(FromUserID, ToUserID, RoomID, Message, Time, Seen) VALUES(%d, %d, %d, '%s', Now(), 0)", $userId, $recipientId, $roomId, Escape($text));
		RunQuery($query, "Unable to insert message. uid: $userId, rec: $recipientId, rm: " . $_POST["Room"] . ", rmid: $roomId, q: $query");
		
		Result("OK");
	}

	ConnectDo('Run');	
?>
