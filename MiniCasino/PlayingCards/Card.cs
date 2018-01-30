using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.PlayingCards
{
    public class Card
    {
        public enum Suits { HEART, CLUB, SPADE, DIAMOND };
        public static char[] order = { '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K', 'A' };

        public Suits Suit { get; protected set; }
        public char Number { get; protected set; }
        public bool Picture { get; protected set; }
        public int Order { get; protected set; }

        public Card(Suits s, char n)
        {
            Suit = s;
            Number = n;
            if (n == 'J' | n == 'Q' | n == 'K')
                Picture = true;
            else
                Picture = false;

            Order = FindOrder(n);
        }

        public int FindOrder(char c)
        {
            for(int i = 0;i < order.Length; i++)
            {
                if (c == order[i])
                    return i + 1;
            }
            return -1;
        }

    }
}
