<?php
	require_once("common.php");

	ConnectValidateUser($_POST["myUserID"], $_POST["myPasswordHash"]);

	$result = ConnectRunQuery("SELECT * FROM eee_Room", "Unable to fetch rooms.");

	echo '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	while ($room = mysql_fetch_object($result))
	{
		echo '<Room>';
		echo "<RoomID>$room->RoomID</RoomID>";
		echo "<Name>$room->Name</Name>";
		echo '</Room>';
	}

	echo '</EeeDataSet>';

	mysql_free_result($result);
?>
