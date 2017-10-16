using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Rooms
{
    public class Room
    {
        string RoomCode;
        List<Tables> tables;

        public Room(string RoomCode)
        {
            this.RoomCode = RoomCode;
            this.tables = new List<Tables>();
        }


    }
}
