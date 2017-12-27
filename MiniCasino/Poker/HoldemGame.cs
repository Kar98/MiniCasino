using MiniCasino.Patrons;
using MiniCasino.Patrons.Staff;
using MiniCasino.PlayingCards;
using MiniCasino.Rooms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Poker
{
    public class HoldemGame : CardGame
    {
        private Dictionary<PokerPlayer, double> betPool = new Dictionary<PokerPlayer, double>();
        private Dictionary<int, PokerPlayer> positions = new Dictionary<int, PokerPlayer>();
        int bb; //big blind
        int sb; //small blind
        public string logID;
        public int ID;
        public int Hands { get; set; }
        private long pot;
        private long raiseSize; //Amount that is needed for a player to match the bets placed.
        int button;
        List<Card> tableCards;
        

        public HoldemGame(Tables AssignedTable, double minbet = 2.0)
        {
            this.table = AssignedTable;
            this.dealer = null;
            this.minBet = minbet;
            bb = 20;
            sb = bb / 2;
            Hands = 1;
            st = new Stack(new Deck(4).ReturnDeck());
            playerGroup = new List<CardPlayer>();
            logID = Guid.NewGuid().ToString();
            for (int i = 0; i < this.table.PlayerLimit(); i++)
            {
                var newPlayer = new PokerPlayer("123 fake st", new DateTime(1980, 1, 1), 'M');
                playerGroup.Add(newPlayer);
                positions.Add(i, newPlayer);
            }
        }


        private void main()
        {
            /* TODO: POKER GAME
             * 
             * Logic for betting rounds. Blinds increments, 3 stages of betting
             * Pot logic
             */

            //Initial
            button = 0;
            Blinds();
            Deal();
            Deal();
            //do bets
            Flop();
            //bets
            Turn();
            //bets
            River();
            //bets

            //showdown.
            

        }

        public override void StartGame()
        {
            this.run = true;
            
            Console.WriteLine($"Game started - {ID}");

            while (run)
            {
                Hands++;
                main();
            }

            Console.WriteLine($"game ended: {logID}");
        }

        protected override void Deal()
        {
            foreach (CardPlayer player in playerGroup)
            {
                player.AddCards((Card)st.Pop());
            }
        }

        private void Flop()
        {
            tableCards = new List<Card>();
            tableCards.Add((Card)st.Pop());
            tableCards.Add((Card)st.Pop());
            tableCards.Add((Card)st.Pop());
        }

        private void Turn()
        {
            tableCards.Add((Card)st.Pop());
        }

        private void River()
        {
            tableCards.Add((Card)st.Pop());
        }

        private void Blinds()
        {
            AddToPot(positions[button+1], sb);
            AddToPot(positions[button+2], sb);
        }

        private void AddToPot(PokerPlayer player,long chips)
        {
            player.Money -= chips;
            pot += chips;
        }

        private void PlayerAI(PokerPlayer player)
        {
            //contribution.
        }

        protected override void End()
        {
            
        }




    }
}
