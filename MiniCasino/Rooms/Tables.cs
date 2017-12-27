using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCasino.Patrons;
using MiniCasino.Patrons.Staff;

namespace MiniCasino.Rooms
{
    public class Tables
    {
        protected int ID;
        public Person dealer;
        public List<Person> players;
        protected int playerLimit;

        public Tables(int id, int totalPlayers = 8)
        {
            ID = id;
            players = new List<Person>();
            playerLimit = totalPlayers;
        }

        public int TableID()
        {
            return ID;
        }

        public int PlayerLimit()
        {
            return playerLimit;
        }
        

    }
}
