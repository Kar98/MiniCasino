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
        protected List<Card> pocketCards;
        //public List<Card> TotalCards { get; protected set; }
        protected int cardsValue;
        public long Chips { get; set; }
        public bool Active { get;set;}
        public bool BetsPlaced { get; set; }
        public long BetValue { get; protected set; }

        

        public PokerPlayer(string address, DateTime bday, char sex, double startingMoney = 10.0) : base(address, bday, sex)
        {
            Money = startingMoney;
            pocketCards = new List<Card>();
            //TotalCards = new List<Card>();
            Chips = 10000;
        }

        public PokerPlayer(CardPlayer player, double startingMoney = 10.0) : base(player.GetAddress().QualifiedAddress, player.GetBirthday(), player.GetSex())
        {
            Money = startingMoney;
            pocketCards = new List<Card>();
            Chips = 10000;
        }

        public PokerPlayer(Patron p) : base(p.GetAddress().QualifiedAddress, p.GetBirthday(), p.GetSex())
        {
            Money = p.Money;
            base.PlayerControlled = p.PlayerControlled;
            base.Address = p.Address;
            base.Age = p.Age;
            base.Birthday = p.Birthday;
            base.Firstname = p.Firstname;
            base.Lastname = p.Lastname;
            pocketCards = new List<Card>();
            Chips = 10000;
        }

        private static PokerPlayer DefaultPatron()
        {
            return new PokerPlayer("321 Default St", new DateTime(1984, 1, 1), 'M');
        }

        public void AddCards(Card c)
        {
            pocketCards.Add(c);
        }

        /*
        public void SetAvailableCards(List<Card> cards)
        {
            List<Card> tmp = new List<Card>();
            tmp.AddRange(pocketCards);
            if(cards != null)
                tmp.AddRange(cards);
            TotalCards = tmp;
        }
        */


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
                pocketCards.RemoveRange(0, pocketCards.Count);
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
            return pocketCards;
        }

        public void SetCardList(List<Card> cards)
        {
            pocketCards = cards;
        }

        public void PrintCards()
        {
            pocketCards.ForEach(a => { Console.Write(a.ToString() + " "); });
            Console.WriteLine();
        }

        public bool PlayerControlled()
        {
            return base.PlayerControlled;
        }
    }
}
