<?php
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
			echo "<EeeResponse>" . $error . "</EeeResponse>";			
		}

		if ($connection)
		{
			echo "Closing connection.<br />";    
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
	
	function ConnectValidateUser($userId, $password, $getUser = 0)
	{
		if (!$userId)
			$userId = '(missing)';
		if (!$password)
			$password = '(missing)';
	
		$result = ConnectDo('ValidateUser', $userId, $password);
		
		if ($getUser)
			return $result;
		else
			mysql_free_result($result);
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
	
	function ValidateUser($userId, $passwordHash)
	{
		$result = RunQuery("SELECT * FROM eee_User WHERE UserID=$userId AND Password = '$passwordHash' AND State>0", 'Unable to fetch user data.');		
		$row = mysql_fetch_object($result);    
		mysql_free_result($result);
		
		if (!$row)
			throw new Exception('Invalid password or user ID.');
			
		return $row;
	}
?>