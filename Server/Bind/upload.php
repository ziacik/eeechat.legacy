<?php
	require_once("common.php");
	
	function DoUpload()
	{
		$fileId = $_GET["fileId"];
		$targetPath = "Upload/" . $fileId;

		if (is_uploaded_file($_FILES['file']['tmp_name']))
		{
			$uploadedFile = fopen($_FILES['file']['tmp_name'], "rb");

			if (!$uploadedFile)
				die("<EeeResponse>Could not open uploaded file.</EeeResponse>");

			$finalFile = fopen($targetPath, "ab");

			if (!$finalFile)
				die("<EeeResponse>Could not open final file.</EeeResponse>");

			while (!feof($uploadedFile))
			{
				if (!fwrite($finalFile, fread($uploadedFile, 1024)))
				{
					fclose($uploadedFile);
					fclose($finalFile);
					die("<EeeResponse>Error writing final file.</EeeResponse>");
					return false;
				}
			}

			fclose($uploadedFile);
			fclose($finalFile);

			print "<EeeResponse>OK</EeeResponse>";
		}
		else
		{
			print("<EeeResponse>File not uploaded.</EeeResponse>");
		}
		
	}
	
	ConnectValidateUser($_GET["myUserID"], $_GET["myPasswordHash"]);
	DoUpload();
?>
