using MiniCasino.Rooms;
using System.Collections.Generic;
using MiniCasino.Patrons.Staff;
using System.Threading;
using System.Threading.Tasks;
using System;
using MiniCasino.Blackjack;
using System.Data.SqlClient;
using MiniCasino.Poker;
using System.IO;
using MiniCasino.Patrons;

/*
 * TODO: Have a DB where the rooms/people can be stored so no hard coding needed.
 * Have SQL scripts to auto setup the DB if needed.
 * Setup local DB to store values in
 *
 */

namespace MiniCasino
{
    class Program
    {
        static Random r;
        static int gameID = 0;
        static List<CardGame> games = new List<CardGame>();
        static List<Room> rooms = new List<Room>();
        static List<Task> tasks = new List<Task>();
        static Patron self = new Patron(new DateTime(1991, 4, 2),'M',true,"Rory","Crickmore");

        public static void Main(string[] args)
        {
            GenerateRooms();
            r = new Random();

            self.PlayerControlled = true;


            for (int i = 0; i < 1; i++)
            {
                //NewHoldenGame();
                NewBlackjackGame();
            }

            games.ForEach(a => {
                tasks.Add(Task.Factory.StartNew(() => { a.StartGame(); }));
                    });

            HandleCommands();
            
            /*var connstr = Db.GetDbString().ConnectionString;
            var cmd = $"INSERT INTO Patron (Address,Firstname,Lastname,Age,Sex,Verified) " +
                "VALUES(null, 'Tom', 'jones', 21, 'M', 1); ";

            Db.GetPatronFromDB(2);
            Console.ReadLine();
            */
            
        }

        private static void HandleCommands()
        {
            var tasks = new List<Task>();
            bool stop = false;
            while (!stop)
            {
                var stuff = Console.ReadLine();

                switch (stuff)
                {
                    case "add bj":
                         tasks.Add(AddPlayerToBlackjack(0)); 
                        break;
                    case "add poker":
                        tasks.Add(AddPlayerToPoker(0));
                        break;
                    case "check":
                        CheckForValidGames();
                        break;
                    case "bj":
                        tasks.Add(Task.Factory.StartNew(() => { NewBlackjackGame().StartGame(); }));
                        break;
                    case "poker":
                        tasks.Add(Task.Factory.StartNew(() => { NewHoldenGame().StartGame(); }));
                        break;
                    case "stop":
                        stop = true;
                        tasks.Clear();
                        break;
                    case "self bj":
                        tasks.Add(AddSelfToBlackjack());
                        PlayerMode(games.FindLast(a => a.Type == CardGame.CardGameType.BJ));
                        break;
                    case "self poker":
                        tasks.Add(AddSelfToPoker());
                        /*foreach(var g in games)
                        {
                            if(g.Type == CardGame.CardGameType.POKER)
                            {
                                PlayerMode(games.FindLast(a => a.Type == CardGame.CardGameType.POKER));
                                break;
                            }
                        }*/
                        PlayerMode(games.FindLast(a => a.Type == CardGame.CardGameType.POKER));
                        break;
                    case "games":
                        PrintGames(rooms[0]);
                        break;
                    case "menu":
                        Console.WriteLine("Main menu");
                        break;
                    case "clean":
                        CleanupGames();
                        break;
                    default:
                        break;
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void PlayerMode(CardGame game)
        {
            bool run = true;
            while (run)
            {
                var input = Console.ReadLine();
                game.AddPlayerCommand(input);
                if (input == "exit")
                {
                    run = false;
                    Console.WriteLine("Run is now false for PlayerMode");
                }
                if(input == "menu")
                {
                    Console.WriteLine("Player mode menu");
                }
            }
        }

        public static void CleanupGames()
        {
            foreach(var t in tasks)
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    t.Dispose();
                }
            }
        }

        public static void PrintGames(Room r)
        {
            List<CardGame> runningGames = new List<CardGame>();
            foreach(var g in games)
            {
                if (g.IsRunning())
                    runningGames.Add(g);
            }
            Console.WriteLine($"Number of games running: {runningGames.Count}");
        }

        private static async Task AddPlayerToBlackjack(int Gameindex)
        {
            var game = GetBlackjackGames();
            if (game.Count != 0)
                await Task.Factory.StartNew(() => game[0].AddDefaultPlayer());
        }

        private static async Task AddPlayerToPoker(int Gameindex)
        {
            var game = GetPokerGames();
            if (game.Count != 0)
                await Task.Factory.StartNew(() => game[0].AddDefaultPlayer());
        }

        private async static Task AddSelfToPoker()
        {
            Console.Clear();
            var pokerGame = NewHoldenGame(self);
            await Task.Factory.StartNew(() => pokerGame.StartGame());
        }

        private async static Task AddSelfToBlackjack()
        {
            Console.Clear();
            var game = GetBlackjackGames();
            if (game != null)
                await Task.Factory.StartNew(() => game[0].AddSelf(self));
        }

        private static void CheckForValidGames()
        {
                for (int i = 0; i < games.Count; i++)
                {
                    var g = games[i];
                    if (g.IsRunning() == false)
                    {
                        games.Remove(g);
                        i = -1;
                        Console.WriteLine("Game removed");
                    }
                }
            Console.WriteLine($"Total games: {games.Count}");
        }

        private static List<CardGame> GetRunningGames()
        {
            var retList = new List<CardGame>();
            foreach (var g in games)
            {
                if (g.IsRunning())
                    retList.Add(g);
            }
            return retList;
        }

        private static List<CardGame> GetPokerGames()
        {
            var retList = new List<CardGame>();
            foreach(var g in games)
            {
                if (g.Type == CardGame.CardGameType.POKER)
                    retList.Add(g);
            }
            return retList;
        }

        private static List<CardGame> GetBlackjackGames()
        {
            var retList = new List<CardGame>();
            foreach (var g in games)
            {
                if (g.Type == CardGame.CardGameType.BJ)
                    retList.Add(g);
            }
            return retList;
        }

        private static string LogString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[r.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public static BlackjackGame NewBlackjackGame(Patron self = null)
        {
            gameID++;
            var newgame = rooms[0].AddBlackJackTable(new BlackjackDealer("123 Fake St", new DateTime(1980, 2, 3), 'M'), 10);
            if (self != null)
                newgame.AddSelf(self);
            games.Add(newgame);
            return newgame;
        }

        public static HoldemGame NewHoldenGame(Patron self = null)
        {
            gameID++;
            var newgame = rooms[0].AddHoldemTable(10, self);
            games.Add(newgame);
            return newgame;
        }

        private static void GenerateRooms()
        {
            rooms.Add(new Room("101"));
        }
        
    }
}
