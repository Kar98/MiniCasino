using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.PlayingCards
{
    public class Card
    {
        public enum Suit { HEART, CLUB, SPADE, DIAMOND };
        public static char[] card = { '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K', 'A' };

        Suit suit;
        char number;
        bool picture;

        public Card(Suit s, char n)
        {
            suit = s;
            number = n;
            if (n == 'J' | n == 'Q' | n == 'K')
                picture = true;
            else
                picture = false;
        }

        public bool Picture()
        {
            return this.picture;
        }

        public char Number()
        {
            return this.number;
        }
        public Suit ReturnSuit()
        {
            return this.suit;
        }
    }
}
