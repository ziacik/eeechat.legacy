<?
	require_once("Common.php");
	
	class Message
	{
		public $Id;
		public $GlobalId;
		public $From;
		public $Private;
		public $Room;
		public $Sent;
		public $Text;
	}
	
	class MessageUser
	{
		public $Id;
		public $Login;
		public $Color;
	}
	
	class MessageRoom
	{
		public $Id;
		public $Name;
	}
	
	function CreateMessageFromRow($message)
	{
		if ($message)
		{
			$data = new Message();
			$data->Id = $message->MessageID;
			$data->GlobalId = "TEST";
			$data->From = new MessageUser();
			$data->From->Id = $message->FromUserID;
			$data->From->Login = $message->FromLogin;
			$data->From->Color = $message->FromColor;
			if ($message->ToUserID > 0)
			{
				$data->To = new MessageUser();
				$data->To->Id = $message->ToUserID;
				$data->To->Login = $message->ToLogin;
				$data->To->Color = $message->ToColor;
			}
			$data->Room = new MessageRoom();
			$data->Room->Id = $message->RoomID;
			$data->Room->Name = $message->Name;			
			$data->Sent = FormatTime($message->Time);
			$data->Text = htmlspecialchars($message->Message);
			return $data;
		}
	}
	
	function IsUserTimestampHigher($userId, $timeStamp)
	{
		$result = ConnectRunQuery("SELECT Access FROM eee_User WHERE UserID=$userId", "Unable to fetch user timestamp.");
		$row = mysql_fetch_object($result);
		
		$isHigher = ($row->Access) > $timeStamp;
		mysql_free_result($result);
		
		return $isHigher;
	}
	
	function SetUserTimestamp($userId, $timeStamp)
	{
		ConnectRunQuery("UPDATE eee_User SET Access = '$timeStamp' WHERE UserID=$userId", "Unable to set user timestamp.");
	}	
	
	function DoCycle($userId, $fromId, $timeOut)
	{
		$last = false;
		
		$startTime = date("Y-m-d H:i:s");
		SetUserTimestamp($userId, $startTime);		

		for ($cycleNo = 1; $cycleNo < $timeOut; $cycleNo++)
		{		
			if ($any || connection_aborted())
				break;
				
			if (IsUserTimestampHigher($userId, $startTime))
				break;
				
			$query = "SELECT ".
					    "M.MessageID, ".
					    "R.RoomID, ".
					    "R.Name, ".
					    "F.UserID as FromUserID, ".
					    "F.Login as FromLogin, ".
					    "F.Color as FromColor, ".
					    "T.UserID as ToUserID, ".
					    "T.Login as ToLogin, ".
					    "T.Color as ToColor, ".
					    "M.ToUserID, ".
					    "M.Message, ".
					    "M.Time ".
					"FROM ".
					    "eee_Message as M ".
					    "INNER JOIN eee_User as F ON M.FromUserID = F.UserID ".
					    "INNER JOIN eee_Room as R ON (M.RoomID = R.RoomID) OR (M.RoomID = 0 AND R.RoomID = 1) ".
					    "LEFT JOIN eee_User as T ON M.ToUserID = T.UserID ".
					"WHERE ".
						"(M.Time >= date(now()) OR (M.Seen = 0 AND M.ToUserID IS NOT NULL AND M.ToUserID > 0)) ".
						"AND M.MessageID >= $fromId ".
						"AND ".
						"( ".
						    "M.ToUserID IS NULL ".
						    "OR M.ToUserID = 0 ".
						    "OR M.ToUserID = $userId ".
						    "OR M.FromUserID = $userId ".
						") ".
					"ORDER BY ".
					    "M.MessageID";
					    
			$result = ConnectRunQuery($query, "Unable to get messages.");

			$first = true;
			$any = false;

			while ($message = mysql_fetch_object($result))
			{
				if ($first)
					echo "[";
				else
					echo ",";
				
				echo json_encode(CreateMessageFromRow($message));
				
				$fromId = $message->MessageID + 1;
				$any = true;
				$first = false;
			}

			if ($any)
				echo "]";
			else
				sleep(1);
		}
		
		if (!$any)
		    Result("NOMESSAGES");
	}

	set_time_limit(0);

	$user = ConnectValidateUser($_GET["Login"], $_GET["PasswordHash"], 1);
	$userId = $user->UserID;

	$fromId = $_GET["FromId"];
	$commit = $_GET["Commit"];
        $timeOut = $_GET["Timeout"];

        if (!isset($timeOut))
            $timeOut = 300;

	if ($commit)
		ConnectRunQuery("UPDATE eee_Message SET Seen=1 WHERE MessageId IN ($commit)", "Unable to commit messages.");

	DoCycle($userId, $fromId, $timeOut);
?>
		