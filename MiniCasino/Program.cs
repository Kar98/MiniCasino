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

/*
 * TODO: Have a DB where the rooms/people can be stored so no hard coding needed.
 * Have SQL scripts to auto setup the DB if needed.
 * TODO: Add the ability to play blackjack from the console.
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

        public static void Main(string[] args)
        {
            GenerateRooms();
            r = new Random();
            

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
                    case "a":
                         tasks.Add(AddPlayerToGameAsync(0)); 
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
                    case "self":
                        tasks.Add(AddSelfToGameAsync(0));
                        PlayerMode(games[0]);
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
            Console.WriteLine($"Number of games running: {r.GetBlackjackGames().Count}");
            r.GetBlackjackGames().ForEach(a => { Console.WriteLine(a.ID); });
        }

        private static async Task AddPlayerToGameAsync(int Gameindex)
        {
            var game = GetRunningGame();
            if (game != null)
                await Task.Factory.StartNew(() => games[Gameindex].AddDefaultPlayer());
        }

        private static async Task AddSelfToGameAsync(int gameIndex)
        {
            var game = GetRunningGame();
            if(game != null )
                await Task.Factory.StartNew(() => games[gameIndex].AddSelf(true));
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

        private static CardGame GetRunningGame()
        {
            foreach(var g in games)
            {
                if (g.IsRunning())
                    return g;
            }
            return null;
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

        public static BlackjackGame NewBlackjackGame()
        {
            gameID++;
            var newgame = rooms[0].AddBlackJackTable(new BlackjackDealer("123 Fake St", new DateTime(1980, 2, 3), 'M'), 10);
            games.Add(newgame);
            return newgame;
        }

        public static HoldemGame NewHoldenGame()
        {
            gameID++;
            var newgame = rooms[0].AddHoldemTable(new BlackjackDealer("123 Fake St", new DateTime(1980, 2, 3), 'M'), 10);
            games.Add(newgame);
            return newgame;
        }

        private static void GenerateRooms()
        {
            rooms.Add(new Room("101"));
        }
        
    }
}
