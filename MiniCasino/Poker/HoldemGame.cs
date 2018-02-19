using MiniCasino.Patrons;
using MiniCasino.PlayingCards;
using MiniCasino.Rooms;
using MiniCasino.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;


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
        
        private long pot;
        private long raiseSize; //Amount that is needed for a player to match the bets placed.
        int button; //dealer position
        List<Card> tableCards;

        enum BetStage { INITIAL, FLOP, TURN, RIVER };

        public HoldemGame(Tables AssignedTable, double buyin = 2.0, Patron p = null)
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
            this.pendingPlayers = new BlockingCollection<CardPlayer>();
            logID = Guid.NewGuid().ToString();
            Logs.RegisterNewTrace(logID, null, "poker");
            Type = CardGameType.POKER;
            int totalPlayersToAdd = this.table.PlayerLimit();
            commands = new ConcurrentQueue<string>();

            Logs.LogTrace("log GUID : " + logID, logID);
            Logs.LogTrace($"T: {Thread.CurrentThread.ManagedThreadId} New blackjack game started", logID);

            int forInt = 0;

            if (p != null)
            {
                var newPlayer = new PokerPlayer(p);
                playerGroup.Add(newPlayer);
                positions.Add(0, newPlayer);
                betPool.Add(newPlayer, 0);
                outputToConsole = true;
                forInt++;
            }
                
            for (int i = forInt; i < totalPlayersToAdd; i++)
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
             * Pot logic - sides pots
             * Basic Poker AI. Need to evaluate potential cards for upcoming hands and to factor in the AIs position on the table during bets
             * Low value cards need to be folded and medium level need to be call. High value will raise. 
             * In the future potentially have some players considered aggressive or passive play.
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
            Thread.Sleep(1000);
        }

        public override void StartGame()
        {
            this.run = true;
            
            Console.WriteLine($"Game started - {ID}");

            while (pendingPlayers.Count > 0)
            {
                var newPlayer = new PokerPlayer(pendingPlayers.Take());
                playerGroup.Add(newPlayer);
                positions.Add(positions.Count-1, newPlayer);
                betPool.Add(newPlayer, 0);
            }

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

                    if (player.PlayerControlled())
                        player.PrintCards();
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
            if (outputToConsole)
                PrintTableCards();
        }

        private void Turn()
        {
            ResetPlayerContext();
            tableCards.Add((Card)st.Dequeue());
            if (outputToConsole)
                PrintTableCards();
        }

        private void River()
        {
            ResetPlayerContext();
            tableCards.Add((Card)st.Dequeue());
            if (outputToConsole)
                PrintTableCards();
        }

        private void Showdown()
        {
            PokerEvaluator pe = new PokerEvaluator(availablePlayers, tableCards, outputToConsole);
            var winner = pe.Winner();
            
            // if no winner, then run side pot, else give all to winner.
            if(winner.Count == 1)
            {
                winner[0].Chips += pot;
            }
            else
            {
                LogLine("Multi win");
                var splitPot = this.pot / winner.Count;
                LogLine("Split pot = " + splitPot);
                foreach (var w in winner)
                {
                    w.Chips += splitPot;
                }
            }
        }

        private void Blinds()
        {
            if(this.Hands % 20 == 0)
            {
                bb *= 2;
                sb = bb / 2;
            }
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
            long totalChips = 0;

            if (player.Chips < chips)
            {
                raiseSize += player.Chips;
                AddToPot(player, raiseSize);
                LogLine($"Not enough chips to raise {chips}, raising up to {raiseSize} instead");
            }
            else
            {
                totalChips = raiseSize - player.BetValue + chips;
                raiseSize += chips;
                AddToPot(player, totalChips);
            }
            
            LogLine($"Raised to {raiseSize}");
            return player.Raise(raiseSize);
        }

        private PlayerAction Call(PokerPlayer player)
        {
            long totalChips;
            if (player.Chips < raiseSize)
            {
                totalChips = player.Chips;
                AddToPot(player, totalChips);
                LogLine($"Not enough money, calling to {player.Chips} instead");
            }
            else
            {
                totalChips = raiseSize - player.BetValue;
                AddToPot(player, totalChips);
            }
            LogLine(player.ToString() + " called");
            return player.Call(totalChips);
        }

        private PlayerAction Fold(PokerPlayer player)
        {
            LogLine(player.ToString() + " folded");
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

        private PlayerAction HumanPlay(PokerPlayer player, BetStage bs)
        {
            bool awaitResp = true;
            Console.WriteLine($"Current betting stage : {bs.ToString()}");
            Console.WriteLine("Enter 'raise #', 'call', 'hand' or 'fold' for player actions");
            Console.WriteLine("Enter 'hand' to view current hand");

            while (awaitResp)
            {
                var action = WaitForCommand();
                action = action.ToLower();

                var splitAct = action.Split(' ');

                if (splitAct.Length == 1)
                {
                    switch (splitAct[0])
                    {
                        case "call":
                            var a = Call(player);
                            if(a != PlayerAction.NONE)
                                return Call(player);
                            break;
                        case "money":
                            LogLine("Chips: " + player.Chips);
                            break;
                        case "fold":
                            return Fold(player);
                        case "hand":
                            PrintTableCards();
                            player.PrintCards();
                            Console.WriteLine();
                            break;
                    }
                }
                else if (splitAct.Length > 1 && splitAct[0] == "raise")
                {
                    if (int.TryParse(splitAct[1], out int res))
                    {
                        var a = Raise(player, res);
                        if(a != PlayerAction.NONE)
                            return Raise(player, res);
                    }
                }
                else
                {
                    LogLine("Enter a valid value");
                }
            }

            throw new Exception("No player action performed!");
        }

        private void BettingRound(BetStage bs)
        {
            var players = ValidPlayers();
            var startingPoint = GetStartingPosition();
            var raisePoint = startingPoint; // This is the spot where the person doing the raising is located. 
            players = ReformPlayerList(players, startingPoint);
            bool betsDone = false;

            if (bs != BetStage.INITIAL)
                raiseSize = 0;
            

            while (betsDone == false)
            {
                for(int i = 0;i < players.Count; i++)
                {
                    if (players[i].BetsPlaced == false)
                    {
                        PlayerAction action;
                        if (players[i].PlayerControlled())
                            action = HumanPlay(players[i], bs);
                        else
                            action = PlayerAI(players[i], bs);

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
            var tempList = new List<PokerPlayer>();
            var tempListi = 0;
            for(int i = startingPoint;i < players.Count; i++, tempListi++)
            {
                if(players[i].Active)
                    tempList.Add(players[i]);
            }
            for(int i = 0;i < startingPoint; i++, tempListi++)
            {
                if (players[i].Active)
                    tempList.Add(players[i]);
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
            for(int i = 0; i < betPool.Count; i++)
            {
                betPool[positions[i]] = 0;
            }
        }

        private int GetStartingPosition()
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

        private void PrintTableCards()
        {
            Console.Write("Table cards: ");
            if (tableCards.Count > 1)
                tableCards.ForEach(a => Console.Write(a.ToString() + " "));
            else
                Log("No table cards");
            Console.WriteLine();
        }

        protected override void End()
        {
            Console.WriteLine($"Pot size: {pot}");
            
            ShuffleCardsBackIn(GetAllPlayerCards());
            ShuffleCardsBackIn(tableCards);
            Console.WriteLine(st.Count);
            ResetPlayerContext();
            SetBetPoolToZero();
            tableCards.Clear();
            betPool.Values.ToList().ForEach(a => a = 0);
            playerGroup.ForEach(a => a.DestroyCards());
            pot = 0;
            raiseSize = 0;
            availablePlayers = null;
            IncrementIndex(button, 1);
            
        }




    }
}
