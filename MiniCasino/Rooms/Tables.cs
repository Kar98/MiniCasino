using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCasino.Patrons;
using MiniCasino.Patrons.Staff;

namespace MiniCasino.Rooms
{
    class Tables
    {
        public string Code;
        public BlackjackDealer dealer;
        public List<Person> players;
        private int playerLimit = 8;

        Tables()
        {
            dealer = new BlackjackDealer("123",new DateTime(1990,4,3),'F');
            players = new List<Person>();
        }

        public bool AddPlayer(Person p)
        {
            if (players.Count > playerLimit)
            {
                return false;
            }
            else
            {
                players.Add(p);
                return true;
            }
        }
        

    }
}
