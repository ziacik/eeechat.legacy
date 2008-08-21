<?php
    $link = mysql_connect("localhost", "skeee", "kvetinka") or die("Could not connect");

    mysql_select_db("skeee") or die("Could not select database");

	$login = $_GET["login"];

    $query = "SELECT UserID, Salt, Color FROM eee_User WHERE Login='$login'";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

    while ($user = mysql_fetch_object($result))
    {
    	print '<LoginUser>';
    	print "<UserID>$user->UserID</UserID>";
    	print "<Salt>$user->Salt</Salt>";
    	print "<Color>$user->Color</Color>";
    	print '</LoginUser>';
    }

    print '</EeeDataSet>';

    mysql_free_result($result);

    mysql_close($link);
?>
