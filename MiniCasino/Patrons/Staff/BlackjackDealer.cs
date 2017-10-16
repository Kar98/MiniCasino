using System;
using MiniCasino.PlayingCards;
using MiniCasino.Blackjack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons.Staff
{
    public class BlackjackDealer : BlackjackPlayer
    {
        
        public BlackjackDealer(string address, DateTime bday, char sex) : base(address, bday, sex)
        { 
        }
        
    }
}
