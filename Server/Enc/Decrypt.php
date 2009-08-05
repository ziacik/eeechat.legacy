<?php

$base = file_get_contents('Public.cer');
$publicKey = openssl_pkey_get_public($base);
$decryptedData = "";
$decryptedChunk = "";

$chunks = explode('<-chunk->', $message);

foreach ($chunks as $chunk)
{
    if ($chunk != '')
    {
        openssl_public_decrypt($data, $decryptedChunk, $publicKey);
        $decryptedData .= $decryptedChunk;
    }
}

openssl_free_key($publicKey);

$message = $decryptedData;

?>