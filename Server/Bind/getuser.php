<?php
	require_once("common.php");

	$login = $_POST["login"];

	$result = ConnectRunQuery("SELECT UserID, Salt, Color FROM eee_User WHERE Login='$login'", "Unable to fetch user data.");

	echo '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	while ($user = mysql_fetch_object($result))
	{
		echo '<LoginUser>';
		echo "<UserID>$user->UserID</UserID>";
		echo "<Salt>$user->Salt</Salt>";
		echo "<Color>$user->Color</Color>";
		echo '</LoginUser>';
	}

	echo '</EeeDataSet>';

	mysql_free_result($result);
?>
