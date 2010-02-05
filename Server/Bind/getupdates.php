<?php
	require_once("common.php");

	ConnectValidateUser($_POST["myUserID"], $_POST["myPasswordHash"]);

	$client = $_POST["client"];
	$version = $_POST["version"];
	$fromID = $_POST["fromID"];

	echo '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

	$result = ConnectRunQuery("SELECT * FROM eee_Updates WHERE UpdateID > $fromID AND Client = '$client' AND Version >= '$version' AND Cumulative = 1 ORDER BY UpdateID Desc LIMIT 1", "Unable to fetch cumulative updates.");	
	$update = mysql_fetch_object($result);
	mysql_free_result($result);	

	if ($update)
	{
		echo '<Update>';
		echo "<UpdateID>$update->UpdateID</UpdateID>";
		echo "<Name>$update->Name</Name>";
		echo "<Description>$update->Description</Description>";
		echo "<Link>$update->Link</Link>";
		echo '</Update>';
	}
	else
	{
		$result = ConnectRunQuery("SELECT * FROM eee_Updates WHERE UpdateID > $fromID AND Client = '$client' AND Version = '$version'", "Unable to fetch updates.");

		while ($update = mysql_fetch_object($result))
		{
			echo '<Update>';
			echo "<UpdateID>$update->UpdateID</UpdateID>";
			echo "<Name>$update->Name</Name>";
			echo "<Description>$update->Description</Description>";
			echo "<Link>$update->Link</Link>";
			echo '</Update>';
		}
	    
		mysql_free_result($result);	    
	}

	echo '</EeeDataSet>';
?>
