using MiniCasino.Patrons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCasino.PlayingCards;

namespace MiniCasino.Poker
{
    public enum PlayerAction { NONE, CALL, RAISE, FOLD };

    public class PokerPlayer : Patron, CardPlayer
    {
        protected List<Card> cards;
        protected int cardsValue;
        public long Chips { get; set; }
        public bool Active { get;set;}
        public bool BetsPlaced { get; set; }
        public long BetValue { get; protected set; }

        

        public PokerPlayer(string address, DateTime bday, char sex, double startingMoney = 10.0) : base(address, bday, sex)
        {
            Money = startingMoney;
            cards = new List<Card>();
            Chips = 10000;
        }

        public PokerPlayer(CardPlayer player, double startingMoney = 10.0) : base(player.GetAddress().QualifiedAddress, player.GetBirthday(), player.GetSex())
        {
            Money = startingMoney;
            cards = new List<Card>();
            Chips = 10000;
        }

        private static PokerPlayer DefaultPatron()
        {
            return new PokerPlayer("321 Default St", new DateTime(1984, 1, 1), 'M');
        }

        public void AddCards(Card c)
        {
            cards.Add(c);
        }

        public void SetBlinds(long l)
        {
            BetValue = l;
        }

        public PlayerAction Raise(long value)
        {
            BetsPlaced = true;
            BetValue += value;
            return PlayerAction.RAISE;
        }

        public PlayerAction Call(long value)
        {
            BetsPlaced = true;
            BetValue = value;
            return PlayerAction.CALL;
        }

        public PlayerAction Fold()
        {
            Active = false;
            return PlayerAction.FOLD;
        }

        public void ResetContext()
        {
            BetValue = 0;
            BetsPlaced = false;
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
