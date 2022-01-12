using System;
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
        public static void ShowMistakeCmd() 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неверная команда.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DbCollectionUpdate(List<Game> gameList, string name) 
        {
            using (var contex = new BoardGameContex())
            {
                foreach (var game in gameList)
                {
                    contex.DbGames.Add(new DbGame() { Name = game.Name, UserName = name });
                }
                contex.SaveChanges();
                Console.WriteLine("Объекты сохранены в БД.");
            }
        }

        public static void DbGameDataUpdate(List<GameData> gameList) 
        {
            using (var contex = new BoardGameContex())
            {
                foreach (var game in gameList)
                {
                    foreach (var item in contex.DbGames)
                    {
                        if (item.Name == game.Name)
                        {
                            contex.DbGamesDates.Add(new DbGameDate() { Date = game.Date, DbGameId = item.Id });
                        }
                    }
                }
                contex.SaveChanges();
                Console.WriteLine("Объекты сохранены в БД."); 
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Введите никнейм пользователя сайта boardgamgeek.com.");
            string userName = Console.ReadLine();
            string collectionPath = userName + "_local_collection.xml";
            string dataPath = userName + "_local_data.xml";
            
            UserController newUser = new UserController(userName);
            newUser.ApiRead += NewUser_ApiRead;
            newUser.Added += NewUser_Added;
            
            FileService fileCollectionService = new FileService(collectionPath);
            FileService fileDataService = new FileService(dataPath);

            newUser.GetUserCollection();

            if (File.Exists(collectionPath))
            {
                fileCollectionService.LoadCollection(newUser);
                List<Game> updateCollectionList = new List<Game>();
                foreach (var game in newUser.gameCollection)
                {
                    for (int i = 0; i < newUser.tempGameCollection.Count; i++)
                    {
                        if (game.Name == newUser.tempGameCollection[i].Name)
                        {
                            newUser.tempGameCollection.RemoveAt(i);
                        }
                    }
                }
                newUser.gameCollection.AddRange(newUser.tempGameCollection);
                if (newUser.tempGameCollection.Count > 0)
                {
                    DbCollectionUpdate(newUser.tempGameCollection, userName);
                }

            }
            else
            {
                newUser.gameCollection = newUser.tempGameCollection;
                DbCollectionUpdate(newUser.gameCollection, userName);
            }
            fileCollectionService.SaveCollection(newUser.gameCollection);


            newUser.GetUserHistory();

            if (File.Exists(dataPath))
            {
                fileDataService.LoadData(newUser);
                foreach (var game in newUser.gameData)
                {
                    for (int i = 0; i < newUser.tempGameData.Count; i++)
                    {
                        if (game.Name == newUser.tempGameData[i].Name)
                        {
                            if (game.Date == newUser.tempGameData[i].Date)
                            {
                                newUser.tempGameData.RemoveAt(i);
                            }
                        }
                    }
                }
                newUser.gameData.AddRange(newUser.tempGameData);
                if (newUser.tempGameData.Count > 0)
                {
                    DbGameDataUpdate(newUser.tempGameData);
                }

            }
            else
            {
                newUser.gameData = newUser.tempGameData;
                DbGameDataUpdate(newUser.gameData);
            }
            fileDataService.SaveData(newUser.gameData);

            bool alive = true;
            while (alive)
            {
                Console.WriteLine("Выбери действие:");
                Console.WriteLine();
                Console.WriteLine("1. Добавить партию.   2. Показать количество партий во все игры.   3. Показать историю партий одной игры.   4. Выход.");
                int decision;
                while (true)
                {
                    if (Int32.TryParse(Console.ReadLine(), out decision) & decision > 0 & decision < 5)
                    {
                        break;
                    }
                    else
                    {
                        ShowMistakeCmd();
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
                                    ShowMistakeCmd();
                                }
                            }
                            newUser.AddPlayedGame(newUser.gameCollection[decision1-1].Name);
                            fileDataService.SaveData(newUser.gameData);
                            List<GameData> currentGame = new List<GameData>();
                            currentGame.Add(newUser.gameData.Last());
                            DbGameDataUpdate(currentGame);

                            break;
                        }
                    case 2:
                        {
                            newUser.ShowPlayedGames();
                            break;
                        }
                    case 3:
                        {
                            newUser.ShowCollection();
                            Console.WriteLine("Выбери игру, историю которой показать. Введи номер.");
                            int decision2;
                            while (true)
                            {
                                if (Int32.TryParse(Console.ReadLine(), out decision2) & decision2 > 0 & decision2 <= newUser.gameCollection.Count)
                                {
                                    break;
                                }
                                else
                                {
                                    ShowMistakeCmd();
                                }
                            }
                            newUser.ShowCurrentGameHistory(newUser.gameCollection[decision2 - 1].Name);
                            break;
                        }
                    case 4:
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
        private static void NewUser_ApiRead(object sender, UserEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
        

        

        
