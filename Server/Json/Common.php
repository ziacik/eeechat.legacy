<?
   	class ActionResult
	{
		public $Result;
	}
	
	function FormatTime($time)
	{
		return "/Date(" . (strtotime($time) * 1000) . ")/";
	}
    
	function Result($result)
	{
		$actionResult = new ActionResult();
		$actionResult->Result = $result;
		echo json_encode($actionResult);
	}

	function CreateConnection()
	{
		$connection = mysql_connect("localhost", "eeechatn_kolik", "XXX");
		if (!$connection) throw new Exception("Could not connect.");
		
		$ok = mysql_select_db("eeechatn_eeechatdb");
		if (!$ok) throw new Exception("Could not select database.");
		
		$ok = mysql_query("SET NAMES latin2"); 
		if (!$ok) throw new Exception("Could not set charset.");
		
		return $connection;
	}
	
	function CloseConnection($connection)
	{
		mysql_close($connection);
	}
	
	function ConnectDo($callbackFunction, $optionalParam1 = 0, $optionalParam2 = 0)
	{
		$connection = CreateConnection();
		$result = 0;
		$error = 0;
		
		try
		{
			if ($optionalParam2)
				$result = $callbackFunction($optionalParam1, $optionalParam2);
			else if ($optionalParam1)
				$result = $callbackFunction($optionalParam1);
			else
				$result = $callbackFunction();
		}
		catch (Exception $e)
		{
			$error = $e->getMessage();
			if ($connection)
				$error = $error . " : " . mysql_error($connection); //TODO: Remove!
			Result($error);
		}

		if ($connection)
		{
			CloseConnection($connection);
		}
		
		if ($error)
			die();
		
		return $result;
	}
	
	function ConnectRunQuery($query, $errorText, $freeResult = 0)
	{
		$result = ConnectDo('RunQuery', $query, $errorText);
		
		if ($freeResult)
			mysql_free_result($result);
		else
			return $result;
	}
	
	function ConnectValidateUser($login, $password, $getUser = 0)
	{
		if (!$login)
			$login = '(missing)';
		if (!$password)
			$password = '(missing)';
	
		$user = ConnectDo('ValidateUser', $login, $password);
		
		if ($getUser)
			return $user;
	}
    
	function Free($result)
	{
		mysql_free_result($result);
	}

	function Escape($parameter)
	{
		return mysql_real_escape_string($parameter);
	}
	
	function LogToFile($logName, $data)
	{
		$fh = fopen($logName, 'a');
		if (!$fh) throw new Exception('Cant open file');		
		fwrite($fh, "(At " . date(DATE_RFC822) . "):  " . $data . "\r\n");
		fclose($fh);		
	}
	
	function RunQuery($query, $errorText)
	{
		$result = mysql_query($query);
		if (!$result) throw new Exception($errorText);		
		return $result;
	}
	
	function ValidateUser($login, $passwordHash)
	{
		$query = sprintf("SELECT * FROM eee_User WHERE Login = '%s' AND Password = '%s' AND State > 0", Escape($login), Escape($passwordHash));
		$result = RunQuery($query, 'Unable to fetch user data. ');		
		$row = mysql_fetch_object($result);    
		mysql_free_result($result);
		
		if (!$row)
			throw new Exception('Invalid password or user ID. ' );
			
		return $row;
	}
?>