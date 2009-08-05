<?php

$publicKey = openssl_pkey_get_public(file_get_contents('./puk'));
$dataToEncrypt = "This is the data that I would like to encrypt with the public key."; 
$encryptedData = "";

openssl_public_encrypt($dataToEncrypt, $encryptedData, $publicKey);
openssl_free_key($pub_key);

echo(base64_encode($encryptedData));

?>