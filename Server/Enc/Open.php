<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "xxx") or die("<EeeResponse>Could not connect.</EeeResponse>");
    mysql_select_db("eeechatn_eeechatdb") or die("<EeeResponse>Could not select database.</EeeResponse>");
    mysql_query("SET NAMES latin2");

    $myPasswordHash = $_POST["myPasswordHash"];
	$myUserID = $_POST["myUserID"];
    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

    if (!($user = mysql_fetch_object($result)))
    {
        mysql_free_result($result);
        mysql_close($link);
        die("<EeeResponse>Invalid password or user ID.</EeeResponse>");
    }
?>