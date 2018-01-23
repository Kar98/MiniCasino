using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.PlayingCards
{
    public class DEStack : Stack
    {
        Card[] cardArr;
        /* TODO: Double ended stack needs to be implemented
         
             */

        public DEStack(List<Card> cards)
        {
            cardArr = cards.ToArray();
        }

        public virtual void PushLast(List<Card> objs)
        {
            
        }

    }
}
