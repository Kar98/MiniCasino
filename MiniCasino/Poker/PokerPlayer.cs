using MiniCasino.Patrons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCasino.PlayingCards;

namespace MiniCasino.Poker
{
    public class PokerPlayer : Patron, CardPlayer
    {
        protected List<Card> cards;
        protected int cardsValue;
        protected uint chips;

        public PokerPlayer(string address, DateTime bday, char sex, double startingMoney = 10.0) : base(address, bday, sex)
        {
            Money = startingMoney;
            cards = new List<Card>();
            chips = 10000;
        }

        public PokerPlayer(CardPlayer player, double startingMoney = 10.0) : base(player.GetAddress().QualifiedAddress, player.GetBirthday(), player.GetSex())
        {
            Money = startingMoney;
            cards = new List<Card>();
        }

        private static PokerPlayer DefaultPatron()
        {
            return new PokerPlayer("321 Default St", new DateTime(1984, 1, 1), 'M');
        }

        public void AddCards(Card c)
        {
            cards.Add(c);
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

        public List<Card> ReturnCards()
        {
            return cards;
        }

        public void SetCardList(List<Card> cards)
        {
            this.cards = cards;
        }
    }
}
