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



        public PokerEvaluator(List<PokerPlayer> _players)
        {
            players = _players;
            foreach(var p in _players)
            {
                rankings.Add(p, 0);
            }
        }

        public PokerPlayer Winner()
        {
            return null;
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
            
            foreach(var p in players)
            {
                var catCards = p.ReturnCards();
                catCards.AddRange(tableCards);
                
                //Royal flush
                foreach(var c in catCards)
                {
                    var suit = c.ReturnSuit();

                    if(c.ReturnSuit() == suit && c.Number() == '0')
                    {

                    }
                }

            }


            return PokerHands.UNRANKED;
        }

        private bool IsFlush(List<Card> cards)
        {
            var suit = cards[0].ReturnSuit();
            foreach (var c in cards)
            {
                if(c.ReturnSuit() != suit)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsStraight(List<Card> cards)
        {

        }

        private bool HighCard(List<Card> cards)
        {

        }



    }
        
}
