using MiniCasino.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons.Staff
{
    public interface iCardDealer : iPerson
    {

        void AddCards(Card c);
        void SetCardList(List<Card> cards);
        List<Card> ReturnCards();
        bool DestroyCards();
        void PrintCards();


    }
}
