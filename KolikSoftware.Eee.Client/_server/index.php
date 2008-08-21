<html>
<head>
</head>
<body>
<?php
    $link = mysql_connect("localhost", "skeee", "kvetinka") or die("Could not connect");

    mysql_select_db("skeee") or die("Could not select database");

    $query = "SELECT Max(Version) as Version FROM eee_Setting";
    $result = mysql_query($query) or die("<EeeResponse>Query failed</EeeResponse>");

    if ($setting = mysql_fetch_array($result))
    {
	    print "<a href='EeeClient" . str_replace(".", "_", $setting["Version"]) . ".zip'>Eee Client " . $setting["Version"] . "</a>";
	}
	else
	{
		print "<EeeResponse>(unknown)</EeeResponse>";
	}

    mysql_free_result($result);

    mysql_close($link);
?>

<br/>
<br/>

Changes & BUGFixes:<br/>
**** 2.5 ****<br/>
- nove bublinky sa nezatvaraju, ak sa nad nimi podrzi mys<br/>
- opat funguje ukladanie nastavenia roomov<br/>
- klient je mozne otvorit kombinaciou klavesov Ctrl+` (t.j. klaves nalavo od 1ky)<br/>
- obrazky smilikov: :), :-), ;), ;-)<br/>
- odstranene bugy so zobrazovanim<br/>
**** 2.4 ****<br/>
- minimalizacia na ESC<br/>
- zobrazenie okna naspat na povodne<br/>
- odstranenie problemov so spustanim klienta<br/>
**** 2.3 ****<br/>
- pri afk je ina ikona v trayi<br/>
- alt+tab nezobrazuje ikonu, ak nie je zobrazeny v taskbare<br/>
- afk mesidz sa uz nezobrazuje, je to v tooltipe<br/>
- autologin & autostart<br/>
- klikanie na link v historii<br/>
- klik na link spusti default browser, nie explorer<br/>
- autoupdate<br/>
- ukladanie pozicie splitterov<br/>
*************<br/>
</body>
<HEAD>

<META HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">
</HEAD>
</html>