using MiniCasino.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MiniCasino.Poker
{
    public class PokerEvaluator
    {
        public Dictionary<PokerPlayer, Hand> rankings;
        private List<PokerPlayer> players;
        private List<Card> tableCards;

        /*
         * TODO: poker evaluation
         * Assign a winner when there is a tie
         * Test the logic for assigning 2 pairs
         * Fix two pair assigning the same pairs 
         
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
            var highestHand = rankings.Values.MaxObject(item => (int)item.HandType);
            var totalWinners = rankings.Values.Count(a => a.HandType == highestHand.HandType);

            if(totalWinners > 1)
            {
                List<PokerPlayer> tiePlayers = new List<PokerPlayer>();
                foreach(var p in rankings.Keys)
                {
                    if (rankings[p] == highestHand)
                        tiePlayers.Add(p);
                }
                Console.WriteLine("Tie breaker hit");
                return TieBreaker(tiePlayers);
            }
            else
            {
                foreach (var p in rankings.Keys)
                {
                    if (rankings[p] == highestHand)
                    {
                        Console.WriteLine("Winning hand : " + rankings[p].HandType.ToString());
                        return p;
                    }
                        
                }
            }

            return null;
        }

        private PokerPlayer TieBreaker(List<PokerPlayer> players)
        {
            return null;
        }

        public void Run()
        {
            foreach (var p in players)
            {
                rankings[p] = Evaluate(p.ReturnCards(), tableCards);
                
            }
        }

        public void SetPlayerList(List<PokerPlayer> _players)
        {
            players = _players;
        }

        private Hand Evaluate(List<Card> playerCards, List<Card> tableCards)
        {
            bool flush = false;
            bool straight = false;
            int highCardOrder = -1;
            List<Hand> potentialHands = new List<Hand>();

            var catCards = new List<Card>();
            catCards.AddRange(playerCards);
            catCards.AddRange(tableCards);

            catCards.ForEach(a => Console.Write($"{a.Suit}{a.Number} "));
            Console.WriteLine();

            //Get card values:
            highCardOrder = HighCard(catCards).Order;
            potentialHands.AddRange(FindPairsOrTriples(catCards));
            
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
            if(straight && flush && highCardOrder == Card.order.Last())
            {
                return new Hand(PokerHands.RFLUSH);
            }else if(straight && flush)
            {
                return new Hand(PokerHands.SFLUSH);
            }

            potentialHands.ForEach(a => Console.Write(a.HandType.ToString() + " "));
            

            if (potentialHands.Count > 0)
            {
                var maxHand = potentialHands.OrderByDescending(a => (int)a.HandType).First();
                Console.WriteLine(" * Best hand : " + maxHand.HandType.ToString());
                return maxHand;
            }else if(potentialHands.Count == 0)
            {
                //If nothing found use the High card value.
                Console.WriteLine($"High card: {highCardOrder}");
                return new Hand((PokerHands)highCardOrder);
            }

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
            //If a 2 pair is found, add a new TWOPAIR Hand object.
            if (currentHands.Count(a => a.HandType == PokerHands.PAIR) >= 2)
            {
                var pairs = currentHands.Where(a => a.HandType == PokerHands.PAIR).ToList();
                pairs.Sort(new HandComparerDesc());
                
                currentHands.Add(new Hand(PokerHands.TWOPAIR, pairs[0].HighCard, pairs[1].HighCard));
            }

            return currentHands;
        }

        private Card HighCard(List<Card> cards)
        {
            cards.Sort(new CardComparerByOrderDesc());
            return cards[0];
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
        public int HighCard2 { get; protected set; }
        public PokerEvaluator.PokerHands HandType { get; protected set; }

        public Hand(PokerEvaluator.PokerHands presetHand,int highCardValue = -1)
        {
            HighCard = highCardValue;
            HandType = presetHand;
            if(presetHand <= PokerEvaluator.PokerHands.AHIGH)
            {
                highCardValue = (int)presetHand;
            }
        }

        public Hand(PokerEvaluator.PokerHands presetHand, int highCardValue, int highCardValue2)
        {
            HighCard = highCardValue;
            HighCard2 = highCardValue;
            HandType = presetHand;
            if (presetHand <= PokerEvaluator.PokerHands.AHIGH)
            {
                highCardValue = (int)presetHand;
            }
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

    internal class CardComparerByOrderDesc : Comparer<Card>
    {
        public override int Compare(Card x, Card y)
        {
            //Descending
            if (x.Order > y.Order)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    internal class HandComparer : Comparer<Hand>
    {
        public override int Compare(Hand x, Hand y)
        {
            if (x.HighCard > y.HighCard)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
    internal class HandComparerDesc : Comparer<Hand>
    {
        public override int Compare(Hand x, Hand y)
        {
            if (x.HighCard > y.HighCard)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    static class EnumerableExtensions
    {
        public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
          where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T maxObj = default(T);
            U maxKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxKey = currentKey;
                        maxObj = item;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return maxObj;
        }
    }

}
