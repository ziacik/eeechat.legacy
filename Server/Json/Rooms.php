<?
	require_once("Common.php");
	require_once("CommonRoom.php");
	
	function Run()
	{
		ValidateUser($_GET["Login"], $_GET["PasswordHash"]);

		$result = RunQuery("SELECT * FROM eee_Room", "Unable to fetch rooms.");
		$first = 1;
		
		echo "[";

		while ($room = mysql_fetch_object($result))
		{
			if ($first == 1)
				$first = 0;
			else
				echo ",";

			echo json_encode(CreateRoomFromRow($room));
		}
		
		echo "]";

		Free($result);
	}
	
	ConnectDo('Run');
?>
