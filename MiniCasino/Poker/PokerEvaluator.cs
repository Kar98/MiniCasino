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
        public Dictionary<PokerPlayer, Hand> rankings;
        private List<PokerPlayer> players;
        private List<Card> tableCards;

        /*
         * TODO: poker evaluation
         * Get high card value
         * Logic for pairs and triples
         * 
         
             */

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
            rankings = new Dictionary<PokerPlayer, Hand>();
            tableCards = _tableCards;
            players = _players;
            foreach (var p in _players)
            {
                rankings.Add(p, new Hand(PokerHands.UNRANKED));
            }
            Run();
        }

        public PokerEvaluator(PokerPlayer _player, List<Card> _tableCards)
        {
            rankings = new Dictionary<PokerPlayer, Hand>();
            tableCards = _tableCards;

            players.Add(_player);
            rankings.Add(_player, new Hand(PokerHands.UNRANKED));

            Run();
        }


        public PokerPlayer Winner()
        {
            return null;
        }

        public PokerPlayer Run()
        {
            foreach (var p in players)
            {
                rankings[p] = Evaluate(p.ReturnCards(), tableCards);
            }
            return Winner();
        }

        public void SetPlayerList(List<PokerPlayer> _players)
        {
            players = _players;
        }

        private Hand Evaluate(List<Card> playerCards, List<Card> tableCards)
        {
            bool flush = false;
            bool straight = false;
            int highCard = -1;
            List<Hand> potentialHands = new List<Hand>();
            /*
             * Eval pairs
             */
            var catCards = new List<Card>();
            catCards.AddRange(playerCards);
            catCards.AddRange(tableCards);

            highCard = HighCard(catCards);
            FindPairsOrTriples(catCards).AddRange(potentialHands);

            //Check to see if it's a 2 pair.
            if(potentialHands.Count(a => a.HandType == PokerHands.PAIR) >= 2)
            {

            }

            //Get card values:
            if (IsFlush(catCards))
            {
                potentialHands.Add(new Hand(PokerHands.FLUSH));
                flush = true;
            }

            if (IsStraight(catCards))
            {
                potentialHands.Add(new Hand(PokerHands.STRAIGHT));
                straight = true;
            }

            //Check for high value hands and return straight away.
            if(straight && flush && highCard == Card.order.Last())
            {
                return new Hand(PokerHands.RFLUSH);
            }else if(straight && flush)
            {
                return new Hand(PokerHands.SFLUSH);
            }

            potentialHands.Max(a => (int)a.HandType);

            
                




            return new Hand(PokerHands.UNRANKED);
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
            var cardsInt = ConvertCardsToIntValues(cards);

            cardsInt.Distinct(); //clear dupes. Only needed for straight.
            //Sort in ascending.
            CardComparerAsc cca = new CardComparerAsc();
            cardsInt.Sort(cca);


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
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Straight exception caught");
                    return false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Straight exception caught");
                    return false;
                }
            }
            return false;
        }

        private bool IsFullHouse(List<Hand> hands)
        {
            bool triple = false;
            bool pair = false;
            foreach (var h in hands)
            {
                if (h.HandType == PokerHands.TRIPLE)
                    triple = true;
                if (h.HandType == PokerHands.PAIR)
                    pair = true;
            }
            if (triple && pair)
                return true;
            else
                return false;
        }

        private List<Hand> FindPairsOrTriples(List<Card> cards)
        {
            var ints = ConvertCardsToIntValues(cards);
            List<Hand> currentHands = new List<Hand>();

            var query = from x in ints
                        group x by x into g
                        let count = g.Count()
                        orderby count descending
                        select new { Value = g.Key, Count = count };

            bool tripleFound = false;
            foreach (var q in query)
            {
                if(q.Count == 4)
                {
                    currentHands.Add(new Hand(PokerHands.FOURKIND, (int)PokerHands.FOURKIND));
                    return currentHands;
                }
                if (q.Count == 3)
                {
                    //If a triple has already been assigned, get the highest triple value card.
                    if (tripleFound == false)
                    {
                        tripleFound = true;
                        currentHands.Add(new Hand(PokerHands.TRIPLE, q.Value));
                    }
                    else
                    {
                        var newHand = new Hand(PokerHands.TRIPLE, q.Value);
                        if (newHand.HighCard > currentHands.First(a => a.HandType == PokerHands.TRIPLE).HighCard)
                        {
                            currentHands.Remove(currentHands.First(a => a.HandType == PokerHands.TRIPLE));
                            currentHands.Add(newHand);
                        }       
                    }
                }
                if(q.Count == 2)
                {
                    currentHands.Add(new Hand(PokerHands.PAIR, q.Value));
                }
            }


            return currentHands;
        }

        private int HighCard(List<Card> cards)
        {
            var ints = ConvertCardsToIntValues(cards);
            ints.Sort(new CardComparerDesc());
            return ints[0];
        }

        private List<int> ConvertCardsToIntValues(List<Card> cards)
        {
            List<int> cardsInt = new List<int>();
            foreach (var c in cards)
            {
                cardsInt.Add(c.FindOrder(c.Number));
            }
            return cardsInt;
        }

        private void PrintStuff(List<Card> cards)
        {
            foreach (var c in cards)
            {
                Console.Write(c.Number + ",");
            }
            Console.WriteLine("");
        }
        private void PrintStuff(List<int> cards)
        {
            foreach (var c in cards)
            {
                Console.Write(c + ",");
            }
            Console.WriteLine("");
        }

    }

    public class Hand
    {
        public int HighCard { get; protected set; }
        public PokerEvaluator.PokerHands HandType { get; protected set; }

        public Hand(List<Card> cards)
        {
            if(cards.Count == 3)
            {
                HandType = PokerEvaluator.PokerHands.TRIPLE;

            }
            else if (cards.Count == 2)
            {
                HandType = PokerEvaluator.PokerHands.PAIR;
            }
        }

        public Hand(PokerEvaluator.PokerHands presetHand,int highCardValue = -1)
        {
            HighCard = highCardValue;
            HandType = presetHand;
        }
    }


    internal class CardComparerAsc : IComparer<int>
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

    internal class CardComparerDesc : IComparer<int>
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

    internal class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand x, Hand y)
        {
            throw new Exception();
        }
    }



}
