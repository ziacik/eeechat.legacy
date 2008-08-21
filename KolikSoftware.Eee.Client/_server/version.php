<?php
    $link = mysql_connect("localhost", "skeee", "kvetinka") or die("Could not connect");

    mysql_select_db("skeee") or die("Could not select database");

    $query = "SELECT Max(Version) as Version FROM eee_Setting";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($setting = mysql_fetch_array($result))
    {
	    print "<EeeResponse>" . $setting["Version"] . "</EeeResponse>";
	}
	else
	{
		print "<EeeResponse>(unknown)</EeeResponse>";
	}

    mysql_free_result($result);

    mysql_close($link);
?>
