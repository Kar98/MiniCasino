using System;
using MiniCasino.PlayingCards;
using MiniCasino.Blackjack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons.Staff
{
    public class BlackjackDealer : Person, iCardDealer
    {
        protected List<Card> cards = new List<Card>();
        protected int cardsValue;

        public int CardsValue { get => cardsValue; set => cardsValue = value; }


        public BlackjackDealer(string address, DateTime bday, char sex) : base(address, bday, sex)
        {
        }

        public void AddCards(Card c)
        {
            cards.Add(c);
        }
        public void SetCardList(List<Card> cards)
        {
            this.cards = cards;
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
