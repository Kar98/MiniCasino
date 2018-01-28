using MiniCasino.Rooms;
using System.Collections.Generic;
using MiniCasino.Patrons.Staff;
using System.Threading;
using System.Threading.Tasks;
using System;
using MiniCasino.Blackjack;
using System.Data.SqlClient;
using MiniCasino.Poker;

/*
 * TODO: Have a DB where the rooms/people can be stored so no hard coding needed.
 */

namespace MiniCasino
{
    class Program
    {
        static Random r;
        static int gameID = 0;
        static List<CardGame> games = new List<CardGame>();
        static List<Room> rooms = new List<Room>();
        static Queue<int> queue = new Queue<int>();
        static Stack<int> stack = new Stack<int>();


        public static void Main(string[] args)
        {
            GenerateRooms();
            r = new Random();
            var tasks = new List<Task>();

            for (int i = 0; i < 1; i++)
            {
                NewHoldenGame();
                //NewBlackjackGame();
            }

            games.ForEach(a => {
                tasks.Add(Task.Factory.StartNew(() => { a.StartGame(); }));
                    });

            //Testing();

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
                    case "s":
                        stop = true;
                        tasks.Clear();
                        break;
                    case "games":
                        PrintGames(rooms[0]);
                        break;
                    case "p":
                        tasks.Add(Task.Factory.StartNew(() => { NewHoldenGame().StartGame(); }));
                        break;
                    default:
                        break;
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        public static void PrintGames(Room r)
        {
            Console.WriteLine($"Number of games running: {r.GetBlackjackGames().Count}");
            r.GetBlackjackGames().ForEach(a => { Console.WriteLine(a.ID); });
        }


        private static async Task AddPlayerToGameAsync(int Gameindex)
        {
            await Task.Factory.StartNew(() => games[Gameindex].AddDefaultPlayer());
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

        private static void Testing()
        {
            for(int i = 0;i < 10; i++)
            {
                queue.Enqueue(i);
                stack.Push(i);
            }
            var q1 = queue.Dequeue();
            var q2 = queue.Dequeue();
            var q3 = queue.Dequeue();
            var s1 = stack.Pop();
            var s2 = stack.Pop();
            var s3 = stack.Pop();

            queue.Enqueue(q1);
            queue.Enqueue(q2);
            queue.Enqueue(q3);
            stack.Push(s1);
            stack.Push(s2);
            stack.Push(s3);
        }
        
    }
}
