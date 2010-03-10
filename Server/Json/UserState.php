<?
	require_once("Common.php");
	require_once("CommonUser.php");

	function Run()
	{  
		$login = $_POST["Login"];
		$hash = $_POST["PasswordHash"];
		$state = $_POST["State"];
		$comment = $_POST["Comment"];
		$client = $_POST["Client"];

		if ($client == null || $client == '')
			$client = 'Unknown';

		$query = sprintf("SELECT UserID, State FROM eee_User WHERE Login = '%s' AND Password = '%s'", Escape($login), Escape($hash));
		$result = RunQuery($query, "Unable to fetch user.");
		$user = mysql_fetch_object($result);
		mysql_free_result($result);

		if ($user)
		{
			if (($state > 0) && $user->State == 0)
				$query = sprintf("UPDATE eee_User SET Access = Now(), LoginTime = Now(), State = %d, Client = '%s' WHERE Login = '%s'", $state, Escape($client), Escape($login));
			else
				$query = sprintf("UPDATE eee_User SET Access = Now(), State= %d WHERE Login = '%s'", $state, Escape($login));

			$query2 = sprintf("INSERT INTO eee_Message(FromUserID, ToUserID, Message, Time, Seen) VALUES (%d, 0, '[STATE %s;%d;%s;%s]', Now(), 0)", $user->UserID, Escape($login), $state, Escape($comment), Escape($client));

			//TODO: Those two should run in transaction.
			RunQuery($query, "Unable to set user state.");
			RunQuery($query2, "Unable to insert State message.");

			echo json_encode(GetUser($login));
		}
		else
		{
			Result("BADLOGIN");
		}	
	}  

	ConnectDo('Run');
?>
