<?php
    $link = mysql_connect("localhost", "eeechatn_kolik", "xxx") or die("Could not connect");

    mysql_select_db("eeechatn_eeechatdb") or die("Could not select database");

    $myUserID = $_GET["myUserID"];
    $myPasswordHash = $_GET["myPasswordHash"];
    $fileId = $_GET["fileId"];

    $query = "SELECT UserID FROM eee_User WHERE UserID=$myUserID AND Password = '$myPasswordHash' AND State>0";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($user = mysql_fetch_object($result))
    {
        $targetPath = "Upload/" . $fileId;

        if (is_uploaded_file($_FILES['file']['tmp_name']))
        {
            $uploadedFile = fopen($_FILES['file']['tmp_name'], "rb");
            
            if ($uploadedFile === FALSE)
                die("<EeeResponse>Could not open uploaded file.</EeeResponse>");

            $finalFile = fopen($targetPath, "ab");

            if ($finalFile === FALSE)
                die("<EeeResponse>Could not open final file.</EeeResponse>");

            while (!feof($uploadedFile))
            {
                if (fwrite($finalFile, fread($uploadedFile, 1024)) === FALSE)
                {
                    fclose($uploadedFile);
                    fclose($finalFile);
                    die("<EeeResponse>Error writing final file.</EeeResponse>");
                    return false;
                }
            }

            fclose($uploadedFile);
            fclose($finalFile);

            print "<EeeResponse>OK</EeeResponse>";

/*            if (move_uploaded_file($_FILES['file']['tmp_name'], $target_path))
            {
                print "<EeeResponse>OK</EeeResponse>";
            }
            else
            {
                print("<EeeResponse>NOT OK</EeeResponse>");
            }*/
        }
        else
        {
print_r($_FILES);            print("<EeeResponse>File not uploaded.</EeeResponse>");
        }
    }
    else
    {
        print "<EeeResponse>Invalid password or user ID.</EeeResponse>";
    }

    mysql_free_result($result);

    mysql_close($link);
?>
