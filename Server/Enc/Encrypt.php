<?php

$base = file_get_contents('Public.cer');
$publicKey = openssl_pkey_get_public($base);
$encryptedData = "";

openssl_public_encrypt($data, $encryptedData, $publicKey);
openssl_free_key($publicKey);

?>