<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "xxx") or die("Could not connect");
 
    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");
 
    mysql_query("SET NAMES latin2");    

    if (isset($_GET["d"]))
    {
        $today = $_GET["d"];
    }
    else
    {
        $today = date('Y-m-d');
    }
 
    $query = <<<QUERY
        SELECT
            M.MessageID,
            U.Login as FromLogin,
            T.Login as ToLogin,
            M.Message,
            M.Time,
	    M.Seen
        FROM
            eee_Message as M
            LEFT JOIN eee_User as U ON (U.UserID = M.FromUserID)
            LEFT JOIN eee_User as T ON (T.UserID = M.ToUserID)
        WHERE
            M.Time >= '$today'
            AND M.Message NOT LIKE '[%'
        ORDER BY
            M.Time
QUERY;

    print "Today $today<br /><br />\n";

    $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

    while ($message = mysql_fetch_object($result))
    {
        $time = gmdate('H\:i\:s', strtotime($message->Time));
        print "$time : $message->FromLogin => $message->ToLogin  ($message->Seen)<br />\n";
        print "<em>" . htmlspecialchars($message->Message) . "</em><br /><br />\n";
    }

    mysql_free_result($result);
 
    mysql_close($link);
?>