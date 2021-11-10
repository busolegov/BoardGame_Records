﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace bggparser
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Введите никнейм пользователя сайта boardgamgeek.");
            string userName = Console.ReadLine();
            string collectionPath = userName + ".xml";
            string dataPath = userName + "_data" + ".xml";
            
            UserController newUser = new UserController(userName);
            newUser.ApiRead += NewUser_ApiRead;
            newUser.Showed += NewUser_Showed;
            newUser.Added += NewUser_Added;
            
            FileService fileCollectionService = new FileService(collectionPath);
            FileService fileDataService = new FileService(dataPath);

            if (File.Exists(collectionPath))
            {
                fileCollectionService.LoadCollection(newUser);
                if (File.Exists(dataPath))
                {
                    fileDataService.LoadDataCollection(newUser);
                }
            }
            else
            {
                newUser.GetUserCollection();
                newUser.GetUserHistory();
                fileCollectionService.SaveCollection(newUser.gameCollection);
            }
            bool alive = true;
            while (alive)
            {
                Console.WriteLine("Выбери действие:");
                Console.WriteLine();
                Console.WriteLine("1. Добавить партию.   2. Показать историю игр.   3. Выход.");
                int decision;
                while (true)
                {
                    if (Int32.TryParse(Console.ReadLine(), out decision) & decision > 0 & decision < 4)
                    {
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неверная команда.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                switch (decision)
                {
                    case 1:
                        {
                            newUser.ShowCollection();
                            Console.WriteLine("Выбери игру, в которую сыграл. Введи номер.");
                            int decision1;
                            while (true)
                            {
                                if (Int32.TryParse(Console.ReadLine(), out decision1) & decision1 > 0 & decision1 <= newUser.gameCollection.Count )
                                {
                                    break;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Неверная команда.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            }
                            newUser.AddPlayedGame(newUser.gameCollection[decision1-1].Name);
                            fileDataService.SaveDataCollection(newUser.gameDataCollection);
                            break;
                        }
                    case 2:
                        {
                            newUser.ShowPlayedGames();
                            break;
                        }
                    case 3:
                        {
                            alive = false;
                            break;
                        }
                }
            }
        }

        private static void NewUser_Added(object sender, UserEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void NewUser_Showed(object sender, UserEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void NewUser_ApiRead(object sender, UserEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
        

        

        
