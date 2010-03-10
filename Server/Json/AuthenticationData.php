<?
	require_once("Common.php");
	
	class AuthenticationData
	{
		public $Salt;
	}
	
	function Run()
	{		
		$data = new AuthenticationData();	

		$login = Escape($_GET["Login"]);
		$result = RunQuery("SELECT Salt FROM eee_User WHERE Login='$login'", "");	

		if ($result)
		{
			$user = mysql_fetch_object($result);
			Free($result);
			
			if ($user)
				$data->Salt = $user->Salt;
		}
		
		/// So that caller does not know if requested login is or not in database.
		if (empty($data->Salt))
			$data->Salt = "Test"; 
			
		echo json_encode($data);
	}
	
	ConnectDo('Run');
?>