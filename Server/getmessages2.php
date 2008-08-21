<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "kuratko") or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

    mysql_query("SET NAMES latin2");

    $myUserID = $_POST["myUserID"];
    $myPasswordHash = $_POST["myPasswordHash"];
    $fromID = $_POST["fromID"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
        $query = <<<QUERY
            SELECT
                M.MessageID,
                M.FromUserID,
                R.Name as Room,
                M.ToUserID,
                M.Message,
                M.Time,
                U.Login,
                U.Color
            FROM
                eee_Message as M
                INNER JOIN eee_User as U ON (U.UserID = M.FromUserID)
                INNER JOIN eee_User as ME ON (ME.UserID = $myUserID)
                LEFT JOIN eee_Room as R ON (R.RoomID = M.RoomID)
            WHERE
                (M.ToUserID = $myUserID AND M.Seen=0)
                OR (M.MessageID >= $fromID
                    AND M.Time >= ME.LoginTime
                    AND (M.ToUserID IS NULL OR M.ToUserID = 0 OR M.FromUserID = $myUserID)
                    )                
            ORDER BY
                M.Time
QUERY;

        $result = mysql_query($query) or die("<EeeResponse>Query failed.</EeeResponse>");

        print '<EeeDataSet xmlns="http://tempuri.org/EeeDataSet.xsd">';

        while ($message = mysql_fetch_object($result))
        {
            $time = gmdate('Y-m-d\TH\:i\:s', strtotime($message->Time)) . '.0000000-00:00';
            print '<Message>';
            print "<MessageID>$message->MessageID</MessageID>";
            print "<FromUserID>$message->FromUserID</FromUserID>";
            print "<ToUserID>$message->ToUserID</ToUserID>";
	    	if (isset($message->Room))
                print "<Room>$message->Room</Room>";
            else
                print "<Room></Room>";
            print "<Text>" . htmlspecialchars($message->Message) . "</Text>";
            print "<Time>$time</Time>";
            print "<Login>$message->Login</Login>";
            print "<Color>#". dechex($message->Color) ."</Color>";
            print '</Message>';
        }

        print '</EeeDataSet>';

        $query = "UPDATE eee_Message SET Seen=1 WHERE ToUserID=$myUserID AND Seen=0";
        mysql_query($query);

        $query = "UPDATE eee_User SET Access = Now() WHERE UserID=$myUserID";
        mysql_query($query);
    }
    else
    {
        print "<EeeResponse>Invalid password or user ID.</EeeResponse>";
    }
    mysql_free_result($result);

    mysql_close($link);
?>

