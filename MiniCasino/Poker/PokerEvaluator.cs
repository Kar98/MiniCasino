using MiniCasino.Patrons;
using MiniCasino.Patrons.Staff;
using MiniCasino.PlayingCards;
using MiniCasino.Rooms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniCasino.Poker
{
    public class PokerEvaluator
    {
        public Dictionary<PokerPlayer,int> rankings;
        private List<PokerPlayer> players;
        private List<Card> tableCards;

        /*
         * TODO: poker evaluation
         * Get high card value
         * Logic for straight
         * Logic for flush
         * Logic for pairs and triples
         * 
         
             */

            public enum Testing {  ONE, TWO, THREE };

        

        public enum PokerHands {
            UNRANKED = 0,
            TWOHIGH = 1,
            THREEHIGH = 2,
            FOURHIGH = 3,
            FIVEHIGH = 4,
            SIXHIGH = 5,
            SEVENHIGH = 6,
            EIGHTHIGH = 7,
            NINEHIGH = 8,
            TENHIGH = 9,
            JHIGH = 10,
            QHIGH = 11,
            KHIGH = 12,
            AHIGH = 13,
            PAIR = 14,
            TWOPAIR = 15,
            TRIPLE = 16,
            STRAIGHT = 17,
            FLUSH = 18,
            FULLHOUSE = 19,
            FOURKIND = 20,
            SFLUSH = 21,
            RFLUSH = 22
        };



        public PokerEvaluator(List<PokerPlayer> _players, List<Card> _tableCards)
        {
            rankings = new Dictionary<PokerPlayer, int>();
            tableCards = _tableCards;
            players = _players;
            foreach(var p in _players)
            {
                rankings.Add(p, 0);
            }
            Run();
        }

        public PokerEvaluator(PokerPlayer _player, List<Card> _tableCards)
        {
            rankings = new Dictionary<PokerPlayer, int>();
            tableCards = _tableCards;

            players.Add(_player);
            rankings.Add(_player, 0);
            
            Run();
        }


        public PokerPlayer Winner()
        {
            return null;
        }

        public void Run()
        {
            foreach(var p in players)
            {
                rankings[p] = (int)Evaluate(p.ReturnCards(), tableCards);
            }
        }

        public void SetPlayerList(List<PokerPlayer> _players)
        {
            players = _players;
        }

        private PokerHands Evaluate(List<Card> playerCards,List<Card> tableCards)
        {
            bool flush;
            bool straight;
            char highCard;
            /*
             * Flush
             * Straight
             * High card.
             * Eval pairs
             */
            var catCards = new List<Card>();
            catCards.AddRange(playerCards);
            catCards.AddRange(tableCards);

            flush = IsFlush(catCards);

            straight = IsStraight(catCards);

            if (straight == true)
                Console.WriteLine("STRAIGHT");

            if (flush == true)
                Console.WriteLine("FLUSH");



            return PokerHands.UNRANKED;
        }

        private bool IsFlush(List<Card> cards)
        {
            int heart = 0, diamond = 0, club = 0, spade = 0;

            foreach (var c in cards)
            {
                switch (c.Suit)
                {
                    case Card.Suits.CLUB:
                        club++;
                        break;
                    case Card.Suits.SPADE:
                        spade++;
                        break;
                    case Card.Suits.DIAMOND:
                        diamond++;
                        break;
                    case Card.Suits.HEART:
                        heart++;
                        break;
                    default:
                        return false;
                }

            }

            if (heart == 5 || diamond == 5 || club == 5 || spade == 5)
                return true;

            return false;

        }

        private bool IsStraight(List<Card> cards)
        {
            int straightCount = 1; // number of cards found that are in sequence. 
            int failures = 0; // current failures.
            int maxFailures = 10 - cards.Count; // 7 cards = 3, 6 = 2, 5 = 1. Any lower and it's impossible to have a straight.

            //Convert the Card objecet to a easily sortable int. The higher then int the higher the card value.
            List<int> cardsInt = new List<int>();
            foreach(var c in cards)
            {
                cardsInt.Add(c.FindOrder(c.Number));
            }

            cardsInt.Distinct(); //clear dupes. Only needed for straight.
            //Sort in ascending.
            CardComparerAsc cca = new CardComparerAsc();
            cardsInt.Sort(cca);
            PrintStuff(cardsInt);

            for (int i = 0; i < cardsInt.Count; i++)
            {
                if (straightCount == 5)
                    return true;

                if (failures == maxFailures)
                    return false;

                try
                {
                    if (cardsInt[i] + 1 != cardsInt[i + 1])
                    {
                        failures++;
                        straightCount = 1;
                    }
                    else
                        straightCount++;
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }


            return false;
        }

        private bool HighCard(List<Card> cards)
        {
            return false;
        }

        private void PrintStuff(List<Card> cards)
        {
            foreach(var c in cards)
            {
                Console.Write(c.Number + ",");
            }
            Console.WriteLine("");
        }
        private void PrintStuff(List<int> cards)
        {
            foreach (var c in cards)
            {
                Console.Write(c+",");
            }
            Console.WriteLine("");
        }

    }

    public class CardComparerAsc : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if(x > y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    public class CardComparerDesc : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x < y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }


}
