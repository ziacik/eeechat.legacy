<?php
	require_once("common.php");

	ConnectValidateUser($_POST["myUserID"], $_POST["myPasswordHash"]);
	
    	$result = ConnectRunQuery("SELECT * FROM eee_User WHERE State>0", "Unable to get active users.");

	echo '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	while ($user = mysql_fetch_object($result))
	{
		echo '<User>';
		echo "<UserID>$user->UserID</UserID>";
		echo "<Login>$user->Login</Login>";
		echo "<State>$user->State</State>";
		echo "<Client>$user->Client</Client>";                
		echo "<LoginTime>" . strtr($user->LoginTime, " ", "T") . "+02:00" . "</LoginTime>";                
		echo '</User>';
	}

	echo '</EeeDataSet>';

	mysql_free_result($result);
?>
