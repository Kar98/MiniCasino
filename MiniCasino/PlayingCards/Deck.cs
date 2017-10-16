using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.PlayingCards
{
    public class Deck
    {
        List<Card> cards = new List<Card>();
        
        public Deck()
        {
            CreateNewDeck();
            Shuffle();
            //PrintCards();
        }

        public List<Card> ReturnDeck()
        {
            return cards;
        }

        private void CreateNewDeck()
        {
            //List<Card> newDeck = new List<Card>();

            foreach (var c in Card.card)
            {
                cards.Add(new Card(Card.Suit.CLUB, c));
            }
            foreach (var c in Card.card)
            {
                cards.Add(new Card(Card.Suit.DIAMOND, c));
            }
            foreach (var c in Card.card)
            {
                cards.Add(new Card(Card.Suit.HEART, c));
            }
            foreach (var c in Card.card)
            {
                cards.Add(new Card(Card.Suit.SPADE, c));
            }

        }

        public void Shuffle()
        {
            Random r = new Random();
            int currentInt;
            int totalCards = this.cards.Count;

            currentInt = this.cards.Count;

            List<Card> currentDeck = new List<Card>(cards);
            List<Card> tempDeck = new List<Card>(52);

            for(int idx = 0;idx < totalCards; idx++)
            {
                int rand = r.Next(currentInt);
                tempDeck.Add(currentDeck.ElementAt(rand));
                currentDeck.RemoveAt(rand);
                currentInt--;
            }
            cards = tempDeck;
        }

        public void PrintCards()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\\tmp\\data.txt"))
            {
                foreach (Card c in cards)
                {
                    file.WriteLine(c.ReturnSuit().ToString() + " " + c.Number());
                }
            }
        }

    }
}
