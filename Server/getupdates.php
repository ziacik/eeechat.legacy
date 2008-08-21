<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "kuratko") or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

    $myUserID = $_POST["myUserID"];
    $myPasswordHash = $_POST["myPasswordHash"];
    $client = $_POST["client"];
    $version = $_POST["version"];
    $fromID = $_POST["fromID"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
        $query = "SELECT * FROM eee_Updates WHERE UpdateID > $fromID AND Client = '$client' AND Version >= '$version' AND Cumulative = 1 ORDER BY UpdateID Desc LIMIT 1";
        $result1 = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");
        $query = "SELECT * FROM eee_Updates WHERE UpdateID > $fromID AND Client = '$client' AND Version = '$version'";
        $result2 = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

        print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';
        
        if ($update = mysql_fetch_object($result1))
        {
                print '<Update>';
                print "<UpdateID>$update->UpdateID</UpdateID>";
                print "<Name>$update->Name</Name>";
                print "<Description>$update->Description</Description>";
                print "<Link>$update->Link</Link>";
                print '</Update>';
        }
        else
        {
            while ($update = mysql_fetch_object($result2))
            {
                print '<Update>';
                print "<UpdateID>$update->UpdateID</UpdateID>";
                print "<Name>$update->Name</Name>";
                print "<Description>$update->Description</Description>";
                print "<Link>$update->Link</Link>";
                print '</Update>';
            }
        }

        print '</EeeDataSet>';
    }
    else
    {
        print "<EeeResponse>Invalid password or user ID.</EeeResponse>";
    }

    mysql_free_result($result);

    mysql_close($link);
?>
