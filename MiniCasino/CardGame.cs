using MiniCasino.Blackjack;
using MiniCasino.Patrons;
using MiniCasino.Patrons.Staff;
using MiniCasino.PlayingCards;
using MiniCasino.Rooms;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        public string logID;
        public int ID;
        protected ConcurrentQueue<string> commands;

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

        public virtual void AddSelf(bool playerControlled)
        {
            throw new NotImplementedException();
        }

        public void AddPlayerCommand(string cmd)
        {
            commands.Enqueue(cmd);
        }

        protected string GetCommand()
        {
            commands.TryDequeue(out string res);
            return res;
        }

        protected string WaitForCommand()
        {
            bool cmdFound = false;
            while (!cmdFound)
            {
                var s = GetCommand();
                if (string.IsNullOrEmpty(s))
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    cmdFound = true;
                    return s;
                }
            }
            return "";
        }

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

        public void Stop()
        {
            run = false;
        }

        public bool IsRunning()
        {
            return run;
        }
    }
}
