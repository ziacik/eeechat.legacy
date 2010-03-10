<?
	class User
	{
		public $Id;
		public $Login;
		public $State;
		public $Color;
		public $Comment;
		public $Client;
		public $ConnectedSince;
	}
	
	function GetUser($login)
	{	
		$query = sprintf("SELECT UserID, Login, State, Color, Client, LoginTime FROM eee_User WHERE Login='%s'", Escape($login));
		$result = RunQuery($query, "Unable to fetch user data.");

		if ($result)
		{
			$user = mysql_fetch_object($result);
			Free($result);
			
			return CreateUserFromRow($user);
		}
	}
	
	function CreateUserFromRow($user)
	{	
		if ($user)
		{
			$data = new User();	
			$data->Id = $user->UserID;
			$data->Login = $user->Login;
			$data->State = $user->State;
			$data->Color = $user->Color;
			$data->Client = $user->Client;
			$data->ConnectedSince = FormatTime($user->LoginTime); 
			
			return $data;
		}
	}	
?>