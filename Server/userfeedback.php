<?php
	$myFile = "UserFeedbacks.txt";
	$fh = fopen($myFile, 'a') or die('Cant open file');
	$data = "Feedback Type: " . $_POST["Type"] . "\nFrom: " . $_POST["From"] . "\nMail: " . $_POST["Mail"] . "\n\nDescription:\n" . $_POST["Description"] . "\n____________________________________\n";
	fwrite($fh, $data);
	fclose($fh);

	print '<EeeResponse>OK</EeeResponse>';
/*	require("class.phpmailer.php");

	$mail = new PHPMailer();
	$mail->CharSet = 'iso-8859-2'; // nastaven� kodov�n� e-mailu

	$mail->IsSMTP(); // pou�ijeme k odesl�n� e-mailu SMTP server
	$mail->SMTPAuth = true; // je nutn� SMTP autorizace
	$mail->Host = 'smtp.gmail.com'; // adresa SMTP serveru
	$mail->Username = '5kolik'; // u�ivatelsk� jm�no
	$mail->Password = 'xxxxxxxxxxx'; // u�ivatelsk� heslo
	$mail->Port = 465;

	$mail->From = 'kolik777@gmail.com'; // e-mailov� adresa odes�latele
	$mail->FromName = "Kolik"; // cel� jm�no odes�latele
	$mail->AddAddress('5kolik@gmail.com'); // e-mailov� adresa p&#345;�jemce

	$mail->Subject = 'Eee Client User Feedback';
	$mail->Body = 'Feedback Type: ' . $_POST["Type"] . '\nFrom: ' . $_POST["From"] . '\nMail: ' . $_POST["Mail"] . '\n\nDescription:\n' . $_POST["Description"];
	$mail->WordWrap = 50; // zalomen� t&#283;la zpr�vy po 50 znac�ch

	if (!$mail->Send())
	{
		die('Mailer Error: ' . $mail->ErrorInfo);
	}
	else
	{
		print '<EeeResponse>OK</EeeResponse>';
	}*/
?>


