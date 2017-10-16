using MiniCasino.PlayingCards;
using MiniCasino.Rooms;
using System.Collections.Generic;
using MiniCasino.Patrons.Staff;
using System.Threading;
using System;

/*
 * TODO: Threading
 * TODO: Have a DB where the rooms/people can be stored so no hard coding needed.
 */

namespace MiniCasino
{
    class Program
    {
        static List<Room> rooms = new List<Room>();

        public static void Main(string[] args)
        {
            GenerateRooms();

            ThreadStart childref = new ThreadStart(NewBlackjackGame);
            Console.WriteLine("In Main: Creating the Child thread");
            Thread childThread = new Thread(childref);
            childThread.Start();

            Console.ReadKey();

            //var temp = new Blackjack.BlackjackGame(rooms[0],new BlackjackDealer("",new System.DateTime(1980,1,1),'M'));
        }

        public static void NewBlackjackGame()
        {
            var temp = new Blackjack.BlackjackGame(rooms[0], new BlackjackDealer("", new System.DateTime(1980, 1, 1), 'M'));
            //var temp2 = new Blackjack.BlackjackGame(rooms[0], new BlackjackDealer("", new System.DateTime(1980, 1, 1), 'M'));
        }

        public static void CallToChildThread()
        {
            try
            {
                Console.WriteLine("Child thread starts");

                // do some work, like counting to 10
                for (int counter = 0; counter <= 10; counter++)
                {
                    Thread.Sleep(500);
                    Console.WriteLine(counter);
                }

                Console.WriteLine("Child Thread Completed");
            }

            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread Abort Exception");
            }
            finally
            {
                Console.WriteLine("Couldn't catch the Thread Exception");
            }
        }

        private static void GenerateRooms()
        {
            rooms.Add(new Room("101"));
        }

        
    }
}
