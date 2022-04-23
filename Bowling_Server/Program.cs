﻿using System;
using System.Collections.Generic;
using Bowling_Server.Classes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetLib;
using System.Text;

namespace Bowling_Server
{
    internal class Program
    {
        static List<Player> players = new List<Player>();
        private static Socket socket;
        public static Random random;

        static void Main(string[] args)
        {
            string ip_address = Console.ReadLine();
            int port = int.Parse(Console.ReadLine());
            Listen(ip_address, port);
            Thread matchmakingThread = new Thread(() =>
            {
                while (true)
                {
                    if (players.Count % 2 == 0)
                    {
                        List<Player> group = new List<Player>();
                        for (int i = 0; i < 2; i++)
                        {
                            group.Add(players[0]);
                            players.Remove(players[0]);
                        }
                        StartGame(group);
                    }
                    Thread.Sleep(1000);
                }
            });
            matchmakingThread.Start();

            while (true)
            {
                Socket client = socket.Accept();
                Thread thread = new Thread(() =>
                {
                    byte[] data = new byte[1024];
                    int recv = client.Receive(data);
                    string json = Encoding.ASCII.GetString(data, 0, recv);
                    Player player = new Player();
                    player.Deserialize(json);
                    player.socket = client;
                    players.Add(player);
                });
                thread.Start();
            }
        }

        static void Listen(string ip_address, int port)
        {
            IPAddress ip = IPAddress.Parse(ip_address);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipe);
            socket.Listen(2);
        }
        public static void Send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.Send(data);
        }

        static void StartGame(List<Player> group)
        {
            
        }
    }
}