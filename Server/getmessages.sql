SELECT M.MessageID, M.FromUserID, M.ToUserID, M.Message, M.Time, M.Seen, U.Login, U.Color
FROM eee_Message as M, eee_User as U, eee_User as Ja
WHERE Ja.UserID=$myUserID AND M.FromUserID=U.UserID
AND	( (M.ToUserID=$myUserID AND M.Seen=0) OR (M.Time>=Ja.LogintTime AND (M.ToUserID IS NULL OR M.ToUserID=0 OR M.FromUserID=$myUserID)) )
ORDER BY M.Time

