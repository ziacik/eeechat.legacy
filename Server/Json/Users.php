<?
	require_once("Common.php");
	require_once("CommonUser.php");

	function Run()
	{
		ValidateUser($_GET["Login"], $_GET["PasswordHash"]);

		$result = RunQuery("SELECT * FROM eee_User WHERE State > 0", "Unable to get active users.");
		$first = true;
		
		echo "[";

		while ($user = mysql_fetch_object($result))
		{
			if ($first)
				$first = false;
			else
				echo ",";

			echo json_encode(CreateUserFromRow($user));
		}
		
		echo "]";

		Free($result);
	}
	
	ConnectDo('Run');
?>
