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
        private Dictionary<PokerPlayer, long> betPool = new Dictionary<PokerPlayer, long>();
        private Dictionary<int, PokerPlayer> positions = new Dictionary<int, PokerPlayer>();
        private List<PokerPlayer> availablePlayers = new List<PokerPlayer>();

        Random r;
        long bb; //big blind
        long sb; //small blind
        public string logID;
        public int ID;
        
        private long pot;
        private long raiseSize; //Amount that is needed for a player to match the bets placed.
        int button; //dealer position
        List<Card> tableCards;

        enum BetStage { INITIAL, FLOP, TURN, RIVER };

        public HoldemGame(Tables AssignedTable, double buyin = 2.0)
        {
            this.table = AssignedTable;
            this.dealer = null;
            this.minBet = buyin;
            r = new Random();
            bb = 20;
            sb = bb / 2;
            Hands = 1;
            st = new Queue(new Deck(4).ReturnDeck());
            playerGroup = new List<CardPlayer>();
            logID = Guid.NewGuid().ToString();
            for (int i = 0; i < this.table.PlayerLimit(); i++)
            {
                var newPlayer = new PokerPlayer("123 fake st", new DateTime(1980, 1, 1), 'M');
                playerGroup.Add(newPlayer);
                positions.Add(i, newPlayer);
                betPool.Add(newPlayer,0);
            }
            button = 0;
        }


        private void Main()
        {
            /* TODO: POKER GAME
             * Determine winner
             * Pot logic - sides pots
             * Enable actual play from console
             * 
             */
            IncrementBlindCheck();

            //Initial
            Blinds();
            SetPlayersToActive();
            Deal();
            BettingRound(BetStage.INITIAL);
            Flop();
            BettingRound(BetStage.FLOP);
            Turn();
            BettingRound(BetStage.TURN);
            River();
            BettingRound(BetStage.RIVER);
            Showdown();

            
            End();
        }

        public override void StartGame()
        {
            this.run = true;
            
            Console.WriteLine($"Game started - {ID}");

            while (run)
            {
                Hands++;
                Main();
            }

            Console.WriteLine($"game ended: {logID}");

        }

        protected override void Deal()
        {
            foreach (PokerPlayer player in playerGroup)
            {
                if (player.Active == true)
                {
                    player.BetsPlaced = false;
                    player.AddCards((Card)st.Dequeue());
                    player.AddCards((Card)st.Dequeue());
                }
            }
        }

        private void Flop()
        {
            ResetPlayerContext();
            tableCards = new List<Card>
            {
                (Card)st.Dequeue(),
                (Card)st.Dequeue(),
                (Card)st.Dequeue()
            };
        }

        private void Turn()
        {
            ResetPlayerContext();
            tableCards.Add((Card)st.Dequeue());
        }

        private void River()
        {
            ResetPlayerContext();
            tableCards.Add((Card)st.Dequeue());
        }

        private void Showdown()
        {
            PokerEvaluator pe = new PokerEvaluator(availablePlayers, tableCards);
            pe.Winner();
            
            // if no winner, then run side pot, else give all to winner.



        }

        private void Blinds()
        {
            AddToPot(positions[button+1], sb);
            positions[button + 1].SetBlinds(sb);
            AddToPot(positions[button+2], bb);
            positions[button + 2].SetBlinds(bb);
            raiseSize = bb;
        }

        private void IncrementBlindCheck()
        {
            if(Hands != 0 && Hands % 20 == 0)
            {
                bb *= 2;
                sb *= 2;
            }
        }

        private void AddToPot(PokerPlayer player,long chips)
        {
            var tempChips = betPool[player];
            player.Chips -= chips;
            pot += chips;
            betPool[player] = tempChips + chips;
        }

        private PlayerAction Raise(PokerPlayer player, long chips)
        {
            long totalChips;
            if (player.Chips < chips)
            {
                raiseSize += player.Chips;
                AddToPot(player, raiseSize);
            }
            else
            {
                totalChips = raiseSize - player.BetValue + chips;
                raiseSize += chips;
                AddToPot(player, totalChips);
            }
            return player.Raise(raiseSize);
        }

        private PlayerAction Call(PokerPlayer player)
        {
            long totalChips;
            if (player.Chips < raiseSize)
            {
                totalChips = player.Chips;
                AddToPot(player, totalChips);
            }
            else
            {
                totalChips = raiseSize - player.BetValue;
                AddToPot(player, totalChips);
            }
            return player.Call(totalChips);
        }

        private PlayerAction Fold(PokerPlayer player)
        {
            return player.Fold();
        }

        private PlayerAction PlayerAI(PokerPlayer player, BetStage bs)
        {
            switch (bs)
            {
                case BetStage.INITIAL:
                    return Call(player);
                case BetStage.FLOP:
                    if (r.Next(20) == 1)
                        return Raise(player, 20);
                    else
                        return Call(player);
                case BetStage.TURN:
                    return Call(player);
                case BetStage.RIVER:
                    return Call(player);
                default:
                    Console.WriteLine("Error in player AI");
                    return PlayerAction.NONE;
            }
        }

        private void BettingRound(BetStage bs)
        {
            var players = ValidPlayers();
            var startingPoint = GetPosition();
            var raisePoint = startingPoint; // This is the spot where the person doing the raising is located. 
            players = ReformPlayerList(players, startingPoint);
            bool betsDone = false;

            /*foreach (var p in players)
            {
                p.SetAvailableCards(tableCards);
            }*/

            if (bs != BetStage.INITIAL)
            {
                raiseSize = 0;
                
            }

            while (betsDone == false)
            {
                for(int i = 0;i < players.Count; i++)
                {
                    if (players[i].BetsPlaced == false)
                    {
                        var action = PlayerAI(players[i], bs);
                        if (action == PlayerAction.RAISE)
                        {
                            raisePoint = i;
                            SetPlayersBetsToFalseExceptPlayer(players[i]);
                            i = players.Count; //Exit the for and reform the players list.
                        }
                    }
                }
                players = ReformPlayerList(players, raisePoint);
                betsDone = BettingChecks(players);
            }

            availablePlayers = players;

        }

        private List<PokerPlayer> ValidPlayers()
        {
            List<PokerPlayer> valids = new List<PokerPlayer>();
            foreach(var p in positions)
            {
                if (p.Value.Active == true)
                    valids.Add(p.Value);
            }
            return valids;
        }

        private List<PokerPlayer> ReformPlayerList(List<PokerPlayer> players, int startingPoint)
        {
            //reform the list into a series of players from a particular point.
            //var playerArr = players.ToArray();
            var tempList = new PokerPlayer[players.Count];
            var tempListi = 0;
            for(int i = startingPoint;i < players.Count; i++, tempListi++)
            {
                if(players[i].Active)
                    tempList[tempListi] = players[i];
            }
            for(int i = 0;i < startingPoint; i++, tempListi++)
            {
                if (players[i].Active)
                    tempList[tempListi] = players[i];
            }
            return tempList.ToList();
        }

        private List<PokerPlayer> ReformPlayerListExcludeRaiser(List<PokerPlayer> players, int startingPoint)
        {
            //reform the list into a series of 
            var playerArr = players.ToArray();
            var tempList = new List<PokerPlayer>();
            for (int i = startingPoint + 1; i < playerArr.Length; i++)
            {
                if (positions[i].Active)
                    tempList.Add(positions[i]);
            }
            for (int i = 0; i < startingPoint; i++)
            {
                if (positions[i].Active)
                    tempList.Add(positions[i]);
            }
            return tempList;
        }

        private void SetPlayersToActive()
        {
            foreach(var p in positions)
            {
                p.Value.Active = true;
            }
        }

        private void ResetPlayerContext()
        {
            for(int i = 0;i < positions.Count; i++)
            {
                positions[i].ResetContext();
            }
        }

        private void SetPlayersBetsToFalseExceptPlayer(PokerPlayer player)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if(player != positions[i])
                    positions[i].BetsPlaced = false;
            }
        }

        private void SetBetPoolToZero()
        {
            for(int i = 0; i < playerGroup.Count; i++)
            {
                betPool[positions[i]] = 0;
            }
        }

        private int GetPosition()
        {
            if(button + 3 > 8)
            {
                var total = button + 3;
                return total - 8;
            }
            else
            {
                return button + 3;
            }
        }

        private bool BettingChecks(List<PokerPlayer> players)
        {
            //Check to see if all players have placed their bets. 
            foreach(var p in players)
            {
                if(p.BetsPlaced == false)
                {
                    return false;
                }
            }
            return true;
        }

        private int IncrementIndex(int target,int increment)
        {
            //Apply increment on target, used in relation to the table group of players.
            var max = playerGroup.Count - 1;
            if (target + increment > max)
            {
                return (increment + target) - max - 1;
            }
            else
            {
                return target + increment;
            }
        }

        private int IncrementIndex(int target, int increment, int limit)
        {
            //Apply increment on target, with limit set by parameter
            if (target + increment > limit)
            {
                return (increment + target) - limit - 1;
            }
            else
            {
                return target + increment;
            }
        }

        protected override void End()
        {
            Console.WriteLine($"Pot size: {pot}");
            
            ShuffleCardsBackIn(GetAllPlayerCards());
            ShuffleCardsBackIn(tableCards);
            Console.WriteLine(st.Count);
            ResetPlayerContext();
            SetBetPoolToZero();
            tableCards = null;
            betPool.Values.ToList().ForEach(a => a = 0);
            playerGroup.ForEach(a => a.DestroyCards());
            pot = 0;
            raiseSize = 0;
            availablePlayers = null;
            IncrementIndex(button, 1);
            
        }




    }
}
