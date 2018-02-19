using MiniCasino.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Poker
{
    public class AIPotentialCardValues
    {
        /*DESIGN
         * Pass in person and cards (table and pocket cards)
         * Return a percentage of winchance based on the cards available.
         
         */
        public List<Card> cards = new List<Card>();
        public PokerPlayer player;

        public AIPotentialCardValues(List<Card> cards, PokerPlayer player)
        {
            this.cards = cards;
            this.player = player;
            Evaluate();
        }

        public void Evaluate()
        {

        }

    }
}
