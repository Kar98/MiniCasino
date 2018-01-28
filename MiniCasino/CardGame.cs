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
        protected Queue st;
        protected bool run;
        public int Hands { get; set; }

        protected virtual void Deal()
        {
            foreach (CardPlayer player in playerGroup)
            {
                player.AddCards((Card)st.Dequeue());
            }
            dealer.AddCards((Card)st.Dequeue());
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

        protected List<Card> GetAllPlayerCards()
        {
            List<Card> dealtCards = new List<Card>();
            foreach(var p in playerGroup)
            {
                dealtCards.AddRange(p.ReturnCards());
            }
            return dealtCards;
        }

        protected void DestroyPlayerCards()
        {
            foreach(var p in playerGroup)
            {
                p.DestroyCards();
            }
        }

        protected void ShuffleCardsBackIn(List<Card> cards)
        {
            foreach (Card c in cards)
            {
                st.Enqueue(c);
            }
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
