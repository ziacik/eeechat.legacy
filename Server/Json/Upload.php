<?php
	require_once("Common.php");
	
	function DoUpload()
	{
		$fileName = $_POST["FileName"];
		
		/// Try to validate :)
		if (strpos($filename, '..'))
		{
			Return("Invalid Path.");
			return;
		}
				
		$targetPath = "../Upload/" . $fileName;
		$isFirst = $_POST["IsFirst"];
		$data = $_POST["Data"];
		
		if ($isFirst)
			$mode = "wb";
		else
			$mode = "ab";
		
		$outFile = fopen($targetPath, $mode);

		if (!$finalFile)
		{
			Return("Could not create final file.");
		}

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
	
	ConnectValidateUser($_GET["myUserID"], $_GET["myPasswordHash"]);
	DoUpload();
?>
