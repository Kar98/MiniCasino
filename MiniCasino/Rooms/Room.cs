using MiniCasino.Blackjack;
using MiniCasino.Patrons.Staff;
using MiniCasino.Poker;
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
        List<BlackjackGame> bjgames;
        List<HoldemGame> pokergames;
        int nextID = 1;

        public Room(string RoomCode)
        {
            this.RoomCode = RoomCode;
            this.bjgames = new List<BlackjackGame>();
            this.pokergames = new List<HoldemGame>();
        }

        public BlackjackGame AddBlackJackTable(Patrons.Staff.BlackjackDealer dealer, int minbet)
        {
            bjgames.Add(new BlackjackGame(new Tables(nextID),dealer,minbet));
            nextID++;
            return bjgames.Last();
        }

        public List<BlackjackGame> GetBlackjackGames()
        {
            return bjgames;
        }

        public HoldemGame AddHoldemTable(iCardDealer dealer, int minbet)
        {
            pokergames.Add(new HoldemGame(new Tables(nextID), minbet));
            nextID++;
            return pokergames.Last();
        }

        public List<HoldemGame> GetHoldemGames()
        {
            return pokergames;
        }


    }
}
