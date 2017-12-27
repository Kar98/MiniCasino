using MiniCasino.Blackjack;
using MiniCasino.Patrons;
using MiniCasino.Patrons.Staff;
using MiniCasino.PlayingCards;
using MiniCasino.Rooms;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino
{
    public abstract class CardGame
    {

        protected Tables table;
        public double minBet;
        protected iCardDealer dealer;
        protected List<CardPlayer> playerGroup;
        protected BlockingCollection<CardPlayer> pendingPlayers;
        protected Stack st;
        protected Dictionary<CardPlayer, double> betPool = new Dictionary<CardPlayer, double>();
        protected bool run;

        protected virtual void Deal()
        {
            foreach (CardPlayer player in playerGroup)
            {
                player.AddCards((Card)st.Pop());
            }
            dealer.AddCards((Card)st.Pop());
        }

        public virtual void StartGame()
        {
            throw new NotImplementedException();
        }

        public virtual void AddDefaultPlayer()
        { throw new NotImplementedException(); }

        protected List<Person> CastToPerson(List<CardPlayer> list)
        {
            List<Person> returnList = new List<Person>();
            foreach (var l in list)
            {
                returnList.Add((Person)l);
            }
            return returnList;
        }

        protected virtual void End()
        {
            throw new NotImplementedException();
        }

        public bool IsRunning()
        {
            return run;
        }

    }
}
