using MiniCasino.Patrons;
using System;
using MiniCasino.PlayingCards;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Blackjack
{
    public class BlackjackPlayer : Person
    {
        List<Card> cards;
        int cardsValue;
        public BlackjackPlayer(string address, DateTime bday, char sex) : base(address, bday, sex)
        {
            cards = new List<Card>();
        }

        public int CardsValue { get => cardsValue; set => cardsValue = value; }

        public void AddCards(Card c)
        {
            cards.Add(c);
        }
        public List<Card> ReturnCards()
        {
            return cards;
        }
        public bool DestroyCards()
        {
            try
            {
                cards.RemoveRange(0, cards.Count);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }
        
    }
}
