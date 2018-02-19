using MiniCasino.Logging;
using MiniCasino.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniCasino.Poker
{
    public class PokerEvaluator : InternalLog
    {
        public Dictionary<PokerPlayer, Hand> rankings;
        private List<PokerPlayer> players;
        private List<Card> tableCards;

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



        public PokerEvaluator(List<PokerPlayer> _players, List<Card> _tableCards, bool outputToConsole)
        {
            rankings = new Dictionary<PokerPlayer, Hand>();
            tableCards = _tableCards;
            players = _players;
            this.outputToConsole = outputToConsole;
            foreach (var p in _players)
            {
                rankings.Add(p, new Hand(PokerHands.UNRANKED,_tableCards));
            }
            Run();
        }

        public PokerEvaluator(PokerPlayer _player, List<Card> _tableCards, bool outputToConsole)
        {
            rankings = new Dictionary<PokerPlayer, Hand>();
            tableCards = _tableCards;
            this.outputToConsole = outputToConsole;
            players.Add(_player);
            rankings.Add(_player, new Hand(PokerHands.UNRANKED,_tableCards));

            Run();
        }

        public void Run()
        {
            foreach (var p in players)
            {
                rankings[p] = Evaluate(p.ReturnCards(), tableCards);
            }
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

            if (outputToConsole)
            {
                catCards.ForEach(a => Console.Write($"{a.ToString()} "));
                Console.WriteLine();
            }
            catCards.ForEach(a => Logs.LogTrace($"Card list: {a.ToString()}"));

            //Get card values:
            highCardOrder = HighCard(catCards).Order;
            potentialHands.AddRange(FindPairsOrTriples(catCards));
            var suit = IsFlush(catCards);

            if (suit != null)
            {
                potentialHands.Add(new Hand(PokerHands.FLUSH, catCards));
                flush = true;
            }

            if (IsStraight(catCards))
            {
                potentialHands.Add(new Hand(PokerHands.STRAIGHT, catCards));
                straight = true;
            }

            //Check for high value hands and return straight away.
            if (straight && flush && highCardOrder == Card.order.Last())
            {
                return new Hand(PokerHands.RFLUSH, catCards);
            }
            else if (straight && flush)
            {
                return new Hand(PokerHands.SFLUSH, catCards);
            }

            potentialHands.ForEach(a => Console.Write(a.HandType.ToString() + " "));


            if (potentialHands.Count > 0)
            {
                var maxHand = potentialHands.OrderByDescending(a => (int)a.HandType).First();
                if(outputToConsole)
                    Console.WriteLine(" * Best hand : " + maxHand.HandType.ToString());
                return maxHand;
            }
            else if (potentialHands.Count == 0)
            {
                //If nothing found use the High card value.
                if (outputToConsole)
                    Console.WriteLine($"High card: {highCardOrder}");
                return new Hand((PokerHands)highCardOrder, catCards);
            }

            return new Hand(PokerHands.UNRANKED, catCards);
        }

        public void SetPlayerList(List<PokerPlayer> _players)
        {
            players = _players;
        }

        public List<PokerPlayer> Winner()
        {
            var highestHand = rankings.Values.MaxObject(item => (int)item.HandType);
            var totalWinners = rankings.Values.Count(a => a.HandType == highestHand.HandType);

            if(totalWinners > 1)
            {
                List<PokerPlayer> tiePlayers = new List<PokerPlayer>();
                foreach(var p in rankings.Keys)
                {
                    if (rankings[p].HandType == highestHand.HandType)
                        tiePlayers.Add(p);
                }
                Console.WriteLine("Tie breaker hit");
                return TieBreaker(tiePlayers, highestHand.HandType);
            }
            else
            {
                foreach (var p in rankings.Keys)
                {
                    if (rankings[p] == highestHand)
                    {
                        Console.WriteLine("Winning hand : " + rankings[p].HandType.ToString() + " " + rankings[p].HighCard);
                        return new List<PokerPlayer>() { p };
                    }
                        
                }
            }

            return null;
        }

        private List<PokerPlayer> TieBreaker(List<PokerPlayer> players, PokerHands highestHand)
        {
            Dictionary<Hand, PokerPlayer> reverseDic = new Dictionary<Hand, PokerPlayer>();
            var winnerList = new List<PokerPlayer>();

            foreach (var p in players)
            {
                reverseDic.Add(rankings[p], p);
            }

            var tieList = reverseDic.Keys.ToList();

            if (highestHand == PokerHands.TWOPAIR || highestHand == PokerHands.FULLHOUSE)
                tieList.Sort(new HandComparerMultiPairDesc());
            else
                tieList.Sort(new HandComparerDesc());

            var winners = GetWinnersIfEqual(tieList);
            foreach (var w in winners)
            {
                winnerList.Add(reverseDic[w]);
            }

            return winnerList;
        }

        private List<Hand> GetWinnersIfEqual(List<Hand> listOfHands)
        {
            List<Hand> winnerList = new List<Hand>() { listOfHands[0] };

            for (int i = 0; i < listOfHands.Count - 1; i++)
            {
                if (listOfHands[0].Equals(listOfHands[i + 1]))
                {
                    winnerList.Add(listOfHands[i + 1]);
                }
            }
            return winnerList;
        }

        private Card.Suits? IsFlush(List<Card> cards)
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
                        return null;
                }

            }

            if (diamond >= 5)
                return Card.Suits.DIAMOND;

            if (heart >= 5)
                return Card.Suits.HEART;

            if (club >= 5)
                return Card.Suits.CLUB;

            if (spade >= 5)
                return Card.Suits.SPADE;

            return null;

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


            for (int i = 0; i < cardsInt.Count - 1; i++)
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
                    Console.WriteLine("Straight exception caught - Out of index arg");
                    return false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Straight exception caught - Out of range arg");
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
                    currentHands.Add(new Hand(PokerHands.FOURKIND, cards, (int)PokerHands.FOURKIND));
                    return currentHands;
                }
                if (q.Count == 3)
                {
                    //If a triple has already been assigned, get the highest triple value card.
                    if (tripleFound == false)
                    {
                        tripleFound = true;
                        currentHands.Add(new Hand(PokerHands.TRIPLE, cards, q.Value));
                    }
                    else
                    {
                        var newHand = new Hand(PokerHands.TRIPLE, cards, q.Value);
                        if (newHand.HighCard > currentHands.First(a => a.HandType == PokerHands.TRIPLE).HighCard)
                        {
                            currentHands.Remove(currentHands.First(a => a.HandType == PokerHands.TRIPLE));
                            currentHands.Add(newHand);
                        }       
                    }
                }
                if(q.Count == 2)
                {
                    currentHands.Add(new Hand(PokerHands.PAIR, cards, q.Value));
                }
            }
            //If a 2 pair is found, add a new TWOPAIR Hand object.
            if (currentHands.Count(a => a.HandType == PokerHands.PAIR) >= 2)
            {
                var pairs = currentHands.Where(a => a.HandType == PokerHands.PAIR).ToList();
                pairs.Sort(new HandComparerDesc());
                
                currentHands.Add(new Hand(PokerHands.TWOPAIR, cards, pairs[0].HighCard, pairs[1].HighCard));
            }
            //If a pair and triple is found, then add a new FULL HOUSE object.
            if(currentHands.Count(a => a.HandType == PokerHands.PAIR) >= 1 && currentHands.Count(a => a.HandType == PokerHands.TRIPLE) >= 1)
            {
                var pair = currentHands.Where(a => a.HandType == PokerHands.PAIR).First();
                var triple = currentHands.Where(a => a.HandType == PokerHands.TRIPLE).First();

                currentHands.Add(new Hand(PokerHands.FULLHOUSE, cards, triple.HighCard, pair.HighCard));
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

        private void PrintCards(List<Card> cards)
        {
            foreach (var c in cards)
            {
                Console.Write(c.Number + ",");
            }
            Console.WriteLine("");
        }
        private void PrintCards(List<int> cards)
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
        public int HighCard2 { get; protected set; } // Only used for a TWO PAIR and FULLHOUSE.
        public PokerEvaluator.PokerHands HandType { get; protected set; }
        public List<int> CardRankings { get; protected set; }
        public string Readable { get; protected set; }

        public Hand(PokerEvaluator.PokerHands presetHand,List<Card> cards, int highCardValue = -1)
        {
            HighCard = highCardValue;
            HandType = presetHand;
            CardRankings = new List<int>();
            cards.ForEach(a => CardRankings.Add(a.Order));
            CardRankings.Sort(new CardComparerDesc());
            if(presetHand <= PokerEvaluator.PokerHands.AHIGH)
            {
                highCardValue = (int)presetHand;
            }
            ConvertToReadable(cards);
            
        }


        public Hand(PokerEvaluator.PokerHands presetHand, List<Card> cards, int highCardValue, int highCardValue2)
        {
            HighCard = highCardValue;
            HighCard2 = highCardValue2; //Only for 2pair and full house.
            HandType = presetHand;
            CardRankings = new List<int>();
            cards.ForEach(a => CardRankings.Add(a.Order));
            CardRankings.Sort(new CardComparerDesc());
            if (presetHand <= PokerEvaluator.PokerHands.AHIGH)
            {
                highCardValue = (int)presetHand;
            }
            ConvertToReadable(cards);
        }

        public Hand(PokerEvaluator.PokerHands presetHand, List<Card> cards, Card.Suits _suit, int highCardValue)
        {
            //Used for the flushes. Much easier passing the suit value then reworking it out again.
            HighCard = highCardValue;
            HandType = presetHand;
            CardRankings = new List<int>();
            cards.ForEach(a => {
                if(a.Suit == _suit)
                    CardRankings.Add(a.Order); });
            if (CardRankings.Count < 5)
                throw new Exception("Flush was passed to Hand constructor, but there were less than 5 valid cards for a flush");
            CardRankings.Sort(new CardComparerDesc());
            if (presetHand <= PokerEvaluator.PokerHands.AHIGH)
            {
                highCardValue = (int)presetHand;
            }
            ConvertToReadable(cards);
        }

        private void ConvertToReadable(List<Card> cards)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var c in cards)
            {
                sb.Append($"{c.ToString()} ");
            }
            Readable = sb.ToString();
        }

        public bool Equals(Hand compareHand)
        {
            if(this.HandType == compareHand.HandType)
            {
                int numOfCards = CardRankings.Count;
                if (numOfCards > 5)
                    numOfCards = 5; //Only 5 cards will ever get evaluated when checking for equality. These are part of the rules.


                for (int i = 0;i < numOfCards; i++)
                {
                    if(this.CardRankings[i] != compareHand.CardRankings[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
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
            if (x.HighCard < y.HighCard)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
    internal class HandComparerMultiPairDesc : Comparer<Hand>
    {
        public override int Compare(Hand x, Hand y)
        {
            if(x.HighCard == y.HighCard)
            {
                if (x.HighCard2 < y.HighCard2)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            if(x.HighCard < y.HighCard)
            {
                return 1;
            }
            else
            {
                return -1;
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
