<?php
    /* Connecting, selecting database */
    $link = mysql_connect("localhost", "eeechatn_kolik", "kuratko")
        or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

	$login = $_POST["login"];
	$color = $_POST["color"];
	$passwordHash = $_POST["passwordHash"];
	$salt = $_POST["salt"];

    $query = "SELECT UserID FROM eee_User WHERE Login='$login'";
    $result = mysql_query($query) or die("Query failed");

    if ($user = mysql_fetch_object($result))
    {
	    print "<EeeResponse>Užívate¾ s takýmto loginom už existuje.</EeeResponse>";
    }
    else
    {
	    $query = "INSERT INTO eee_User (Login, Password, Salt, Color) VALUES ('$login', '$passwordHash', '$salt', $color)";
	    $result = mysql_query($query) or die("Query failed");

	    print "<EeeResponse>OK</EeeResponse>";
    }


    /* Free resultset */
    mysql_free_result($result);

    /* Closing connection */
    mysql_close($link);
?>
