using MiniCasino.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons
{
    public interface CardPlayer : iPerson
    {
        void AddCards(Card c);
        void SetCardList(List<Card> cards);
        List<Card> ReturnCards();
        bool DestroyCards();
        double GetMoney();

    }
}
