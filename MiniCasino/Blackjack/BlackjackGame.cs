using System;
using System.Threading;
using MiniCasino.PlayingCards;
using MiniCasino.Patrons;
using MiniCasino.Rooms;
using System.Collections;
using System.Collections.Generic;
using MiniCasino.Patrons.Staff;

namespace MiniCasino.Blackjack
{
    public class BlackjackGame
    {
        Stack st;
        private Room room;
        private BlackjackDealer dealer;
        private List<BlackjackPlayer> playerGroup;
        private bool run;
        private int hands = 0;

        BlackjackPlayer player1 = DefaultPatron();
        BlackjackPlayer player2 = DefaultPatron();
        BlackjackPlayer player3 = DefaultPatron();
        BlackjackPlayer player4 = DefaultPatron();

        public int Hands { get => hands; set => hands = value; }

        public BlackjackGame(Room AssignedRoom, BlackjackDealer dealer)
        {
            Console.WriteLine("New blackjack game started");
            this.room = AssignedRoom;
            this.dealer = dealer;
            st = new Stack(new Deck().ReturnDeck());
            playerGroup = new List<BlackjackPlayer>
            {
                player1,
                player2,
                player3,
                player4
            };
            Main();
            Console.WriteLine("Game has ended");
        }

        private void Main()
        {
            run = true;
            Hands = 0;
            while (run)
            {
                Hands++;
                if (hands == 5)
                    playerGroup.Add(DefaultPatron());

                if (hands == 8)
                    playerGroup.Remove(player1);

                Console.WriteLine($"*Start {hands}*");
                Deal();

                foreach (BlackjackPlayer bp in playerGroup)
                {
                    PlayerAI(bp);
                }

                DealerAI();

                Console.WriteLine("Dealer : " + dealer.CardsValue);

                foreach (BlackjackPlayer bp in playerGroup)
                {
                    Console.WriteLine("Player: " + bp.CardsValue);
                    if (DecideOutcome(bp))
                        Console.WriteLine("Winner");
                    else
                        Console.WriteLine("Loser");
                }
                ShuffleCardsBackIn();
                if(Hands > 10)
                    run = false;
                Console.WriteLine($"*End {hands}*");
                Thread.Sleep(1000);
            }
            
        }

        private void Hit(BlackjackPlayer bp)
        {
            bp.AddCards((Card)st.Pop());
        }
        private void Hit(BlackjackDealer bp)
        {
            bp.AddCards((Card)st.Pop());
        }
        private void Hit(List<Card> cards)
        {
            cards.Add((Card)st.Pop());
        }

        private void Deal()
        {
            foreach (BlackjackPlayer player in playerGroup)
            {
                player.AddCards((Card)st.Pop());
                player.AddCards((Card)st.Pop());
            }
            dealer.AddCards((Card)st.Pop());
            dealer.AddCards((Card)st.Pop());
        }

        private void PlayerAI(BlackjackPlayer bp)
        {
            int cardValue = 0;
            if (CanSplit(bp))
            {
                Console.WriteLine("Found split");
                List<Card> deck1 = new List<Card> { bp.ReturnCards()[0] };
                List<Card> deck2 = new List<Card> { bp.ReturnCards()[1] };

                var deck1Val = BasicPlayerAI(deck1);
                var deck2Val = BasicPlayerAI(deck2);

                if(deck1Val <= 21 && deck1Val < deck2Val)
                {
                    bp.CardsValue = deck1Val;
                }
                else
                {
                    bp.CardsValue = deck2Val;
                }
                

            }
            else {
                cardValue = CalcCards(bp.ReturnCards());
                while (cardValue <= 17)
                {
                    Hit(bp);
                    cardValue = CalcCards(bp.ReturnCards());
                }
                bp.CardsValue = cardValue;
            }
        }

        private int BasicPlayerAI(List<Card> cards)
        {
            int cardValue = CalcCards(cards);
            
            while (cardValue <= 17)
            {
                Hit(cards);
                cardValue = CalcCards(cards);
            }
            if (FindAce(cards) && cardValue > 21)
            {
                cardValue = CalcCards(cards, true);
                while (cardValue <= 17)
                {
                    Hit(cards);
                    cardValue = CalcCards(cards, true);
                }
            }
                
            return cardValue;
        }
        private void DealerAI()
        {
            int cardValue = 0;
            cardValue = CalcCards(dealer.ReturnCards());
            while (cardValue <= 17)
            {
                Hit(dealer);
                cardValue = CalcCards(dealer.ReturnCards());
            }
            dealer.CardsValue = cardValue;
        }

        private void ShuffleCardsBackIn()
        {
            List<Card> dealtCards = new List<Card>();
            foreach(BlackjackPlayer bp in playerGroup)
            {
                var playerCards = bp.ReturnCards();
                dealtCards.AddRange(playerCards);
                bp.DestroyCards();
            }
            dealtCards.AddRange(dealer.ReturnCards());
            dealer.DestroyCards();
            foreach(Card c in dealtCards)
            {
                st.Push(c);
            }
        }

        private int CalcCards(List<Card> cards, bool aceIsOne = false)
        {
            int cardValues = 0;
            if (FindAce(cards))
                cardValues = cardValues;
            foreach (Card c in cards)
            {
                if (c.Picture())
                {
                    cardValues += 10;
                }
                else
                {
                    if(c.Number() == '0')
                    {
                        cardValues += 10;
                    }
                    else if(c.Number() != 'A')
                    {
                        cardValues += int.Parse(c.Number().ToString());
                    }
                    else if(aceIsOne == false)
                    {
                        cardValues += 11;
                    }
                    else if (aceIsOne == true)
                    {
                        cardValues += 1;
                    }
                }
            }
            return cardValues;
        }

        private bool DecideOutcome(BlackjackPlayer bp)
        {
            if (bp.CardsValue > 21)
                return false;
            if (dealer.CardsValue > 21)
                return true;
            

            if (bp.CardsValue > dealer.CardsValue)
                return true;
            else
                return false;
        }

        private bool CanSplit(BlackjackPlayer bp)
        {
            var cardArr = bp.ReturnCards().ToArray();
            if (cardArr[0].Number() == cardArr[1].Number())
                return true;
            else
                return false;
        }

        private static BlackjackPlayer DefaultPatron()
        {
            return new BlackjackPlayer("321 Default St", new DateTime(1984, 1, 1), 'M');
        }

        private bool FindAce(List<Card> cards)
        {
            foreach (Card c in cards)
            {
                if (char.Equals(c.Number(),'A'))
                    return true;
            }
            return false;
                
        }

    }
}
