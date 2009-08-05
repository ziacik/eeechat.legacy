<?php
	$myFile = "UserFeedbacks.txt";
	$fh = fopen($myFile, 'a') or die('Cant open file');
	$data = "Feedback Type: " . $_POST["Type"] . "\nFrom: " . $_POST["From"] . "\nMail: " . $_POST["Mail"] . "\n\nDescription:\n" . $_POST["Description"] . "\n____________________________________\n";
	fwrite($fh, $data);
	fclose($fh);

	print '<EeeResponse>OK</EeeResponse>';
/*	require("class.phpmailer.php");

	$mail = new PHPMailer();
	$mail->CharSet = 'iso-8859-2'; // nastavení kodování e-mailu

	$mail->IsSMTP(); // použijeme k odeslání e-mailu SMTP server
	$mail->SMTPAuth = true; // je nutná SMTP autorizace
	$mail->Host = 'smtp.gmail.com'; // adresa SMTP serveru
	$mail->Username = '5kolik'; // uživatelské jméno
	$mail->Password = 'xxxxx'; // uživatelské heslo
	$mail->Port = 465;

	$mail->From = 'kolik777@gmail.com'; // e-mailová adresa odesílatele
	$mail->FromName = "Kolik"; // celé jméno odesílatele
	$mail->AddAddress('5kolik@gmail.com'); // e-mailová adresa p&#345;íjemce

	$mail->Subject = 'Eee Client User Feedback';
	$mail->Body = 'Feedback Type: ' . $_POST["Type"] . '\nFrom: ' . $_POST["From"] . '\nMail: ' . $_POST["Mail"] . '\n\nDescription:\n' . $_POST["Description"];
	$mail->WordWrap = 50; // zalomení t&#283;la zprávy po 50 znacích

	if (!$mail->Send())
	{
		die('Mailer Error: ' . $mail->ErrorInfo);
	}
	else
	{
		print '<EeeResponse>OK</EeeResponse>';
	}*/
?>


