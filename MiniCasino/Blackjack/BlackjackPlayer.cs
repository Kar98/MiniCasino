using MiniCasino.Patrons;
using System;
using MiniCasino.PlayingCards;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Blackjack
{
    public class BlackjackPlayer : Patron, CardPlayer
    {
        protected List<Card> cards;
        protected int cardsValue;

        public BlackjackPlayer(string address, DateTime bday, char sex, bool playerControlled = false, double startingMoney = 100.0) : base(address, bday, sex)
        {
            Money = startingMoney;
            cards = new List<Card>();
            this.PlayerControlled = playerControlled;
        }

        public int CardsValue { get => cardsValue; set => cardsValue = value; }

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

        public void PrintCards()
        {
            cards.ForEach(a => { Console.WriteLine(a.ToString()); });
        }

        bool CardPlayer.PlayerControlled()
        {
            return base.PlayerControlled;
        }
    }
}
