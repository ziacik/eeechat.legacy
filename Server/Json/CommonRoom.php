<?
	class Room
	{
		public $Id;
		public $Name;
	}
	
	function GetRoom($roomName)
	{	
		$query = sprintf("SELECT RoomID, Name FROM eee_Room WHERE Name='%s'", Escape($roomName));
		$result = RunQuery($query, "Unable to fetch room data.");

		if ($result)
		{
			$room = mysql_fetch_object($result);
			Free($result);
			
			return CreateRoomFromRow($room);
		}
	}
	
	function CreateRoomFromRow($room)
	{	
		if ($room)
		{
			$data = new Room();	
			$data->Id = $room->RoomID;
			$data->Name = $room->Name;
			
			return $data;
		}
	}
	
?>