using System;
using System.Threading;
using MiniCasino.PlayingCards;
using MiniCasino.Patrons;
using MiniCasino.Rooms;
using System.Collections;
using System.Collections.Generic;
using MiniCasino.Patrons.Staff;
using System.Collections.Concurrent;
using MiniCasino.Logging;

namespace MiniCasino.Blackjack
{
    public class BlackjackGame : CardGame
    {
        private int hands = 0;
        private Dictionary<BlackjackPlayer, double> betPool = new Dictionary<BlackjackPlayer, double>();
        public string logID;
        public int ID;
        new BlackjackDealer dealer;

        BlackjackPlayer player1 = DefaultPatron();
        BlackjackPlayer player2 = DefaultPatron();
        BlackjackPlayer player3 = DefaultPatron();
        BlackjackPlayer player4 = DefaultPatron();

        public BlackjackGame(Tables AssignedTable, iCardDealer dealer, double minbet = 2.0)
        {
            this.table = AssignedTable;
            this.dealer = new BlackjackDealer(dealer.GetAddress().QualifiedAddress,dealer.GetBirthday(),dealer.GetSex());
            logID = Guid.NewGuid().ToString();
            Console.WriteLine(logID);
            ID = table.TableID();
            pendingPlayers = new BlockingCollection<CardPlayer>();

            Logs.RegisterNewTrace(logID, null, "bj");

            Logs.LogTrace("log GUID : " + logID, logID);
            Logs.LogTrace($"T: {Thread.CurrentThread.ManagedThreadId} New blackjack game started", logID);

            st = new Stack(new Deck().ReturnDeck());
            minBet = minbet;
            playerGroup = new List<CardPlayer>
            {
                player1,
                player2,
                player3,
                player4
            };
        }

        public override void StartGame()
        {
            run = true;
            Hands = 0;
            table.players = this.CastToPerson(playerGroup);

            Console.WriteLine($"Game started - {ID}");

            while (run)
            {
                Logs.LogTrace("count of players: " + playerGroup.Count, logID);
                Hands++;

                Logs.LogTrace($"*Start {hands}*", logID);
                while(pendingPlayers.Count > 0)
                {
                    playerGroup.Add(pendingPlayers.Take());
                }

                CheckForValidPlayers();
                Bets();
                Deal();

                foreach (BlackjackPlayer bp in playerGroup)
                {
                    PlayerAI(bp);
                }

                DealerAI();

                Logs.LogTrace("Dealer : " + dealer.CardsValue, logID);

                foreach (BlackjackPlayer bp in playerGroup)
                {
                    Logs.LogTrace("Player: " + bp.CardsValue, logID);
                    if (DecideOutcome(bp))
                    {
                        Logs.LogTrace("Winner");
                        var bet = betPool[bp];
                        bp.Money += bet*2;
                    }
                    else
                        Logs.LogTrace("Loser", logID);
                }
                ShuffleCardsBackIn();
                PrintPlayersMoney();
                End();

                if(Hands > 30)
                    run = false;
                if (playerGroup.Count < 1)
                    run = false;

                Logs.LogTrace($"*End {hands}*", logID);
                Thread.Sleep(500);
            }
            Console.WriteLine($"Game has ended {logID}");
        }

        private void Hit(CardPlayer bp)
        {
            bp.AddCards((Card)st.Pop());
        }
        private void Hit(iCardDealer bp)
        {
           bp.AddCards((Card)st.Pop());
        }
        private void Hit(List<Card> cards)
        {
            cards.Add((Card)st.Pop());
        }

        private void Bets()
        {
            double betValue;
            foreach(BlackjackPlayer bp in playerGroup)
            {
                betValue = minBet;
                betPool.Add(bp, betValue);
                bp.Money -= minBet;
            }
        }

        private void CheckForValidPlayers()
        {
            var tempGroup = new List<CardPlayer>();

            foreach (var player in playerGroup)
            {
                Console.Write(((Person)player).Firstname);
                if (player.GetMoney() >= minBet)
                    tempGroup.Add(player);
                else
                    Logs.LogTrace($"{player.GetLastname()} {player.GetFirstname()} has been removed from {logID}", logID);
            }
            playerGroup = tempGroup;
        }

        protected override void Deal()
        {
            foreach (BlackjackPlayer player in playerGroup)
            {
                player.AddCards((Card)st.Pop());
                player.AddCards((Card)st.Pop());
            }
            dealer.AddCards((Card)st.Pop());
            dealer.AddCards((Card)st.Pop());
        }

        protected override void End()
        {
            betPool.Clear();
            Console.WriteLine("Hand finished");
        }

        public override void AddDefaultPlayer()
        {
            pendingPlayers.Add(DefaultPatron());
        }

        private void PlayerAI(BlackjackPlayer bp)
        {
            int cardValue = 0;
            if (CanSplit(bp))
            {
                Logs.LogTrace("Found split", logID);
                List<Card> deck1 = new List<Card> { bp.ReturnCards()[0] };
                List<Card> deck2 = new List<Card> { bp.ReturnCards()[1] };

                var deck1Val = BasicPlayerAI(deck1);
                var deck2Val = BasicPlayerAI(deck2);

                if(deck1Val <= 21 && deck1Val < deck2Val)
                {
                    bp.CardsValue = deck1Val;
                }
                else
                {
                    bp.CardsValue = deck2Val;
                }
                List<Card> returnCardList = new List<Card>();
                returnCardList.AddRange(deck1);
                returnCardList.AddRange(deck2);
                bp.SetCardList(returnCardList);
            }
            else {
                cardValue = CalcCards(bp.ReturnCards());
                while (cardValue <= 17)
                {
                    Hit(bp);
                    cardValue = CalcCards(bp.ReturnCards());
                }
                bp.CardsValue = cardValue;
            }
            
        }

        private int BasicPlayerAI(List<Card> cards)
        {
            int cardValue = CalcCards(cards);
            
            while (cardValue <= 17)
            {
                Hit(cards);
                cardValue = CalcCards(cards);
            }
            if (FindAce(cards) && cardValue > 21)
            {
                cardValue = CalcCards(cards, true);
                while (cardValue <= 17)
                {
                    Hit(cards);
                    cardValue = CalcCards(cards, true);
                }
            }
                
            return cardValue;
        }

        private void DealerAI()
        {
            int cardValue = 0;
            cardValue = CalcCards(dealer.ReturnCards());
            while (cardValue <= 17)
            {
                Hit(dealer);
                cardValue = CalcCards(dealer.ReturnCards());
            }
            dealer.CardsValue = cardValue;
        }

        
        private void ShuffleCardsBackIn()
        {
            List<Card> dealtCards = new List<Card>();
            dealtCards.AddRange(GetAllPlayerCards());
            DestroyPlayerCards();
            dealtCards.AddRange(dealer.ReturnCards());
            dealer.DestroyCards();
            ShuffleCardsBackIn(dealtCards);
        }

        private void PrintPlayersMoney()
        {
            foreach(var player in playerGroup)
            {
                Logs.LogTrace($"${player.GetMoney()}");
            }
            
        }

        private int CalcCards(List<Card> cards, bool aceIsOne = false)
        {
            int cardValues = 0;
            foreach (Card c in cards)
            {
                if (c.Picture())
                {
                    cardValues += 10;
                }
                else
                {
                    if(c.Number() == '0')
                    {
                        cardValues += 10;
                    }
                    else if(c.Number() != 'A')
                    {
                        cardValues += int.Parse(c.Number().ToString());
                    }
                    else if(aceIsOne == false)
                    {
                        cardValues += 11;
                    }
                    else if (aceIsOne == true)
                    {
                        cardValues += 1;
                    }
                }
            }
            return cardValues;
        }

        private bool DecideOutcome(BlackjackPlayer bp)
        {
            if (bp.CardsValue > 21)
                return false;
            if (dealer.CardsValue > 21)
                return true;

            if (bp.CardsValue > dealer.CardsValue)
                return true;
            else
                return false;
        }

        private bool CanSplit(BlackjackPlayer bp)
        {
            var cardArr = bp.ReturnCards().ToArray();
            if (cardArr[0].Number() == cardArr[1].Number())
                return true;
            else
                return false;
        }

        private static BlackjackPlayer DefaultPatron()
        {
            return new BlackjackPlayer("321 Default St", new DateTime(1984, 1, 1), 'M');
        }

        private bool FindAce(List<Card> cards)
        {
            foreach (Card c in cards)
            {
                if (char.Equals(c.Number(),'A'))
                    return true;
            }
            return false;
                
        }

    }
}
