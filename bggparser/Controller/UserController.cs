using System;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Xml;

namespace bggparser
{
    public class UserController
    {
        public string UserName { get; set; }
        public UserController()
        {

        }
        public UserController(string name)
        {
            UserName = name;
        }

        public List<GameData> gameData = new List<GameData>();
        public List<GameData> tempGameData = new List<GameData>();
        public List<Game> gameCollection = new List<Game>();
        public List<Game> tempGameCollection = new List<Game>();


        const string BEGINURL_COLLECTION = "https://www.boardgamegeek.com/xmlapi/collection/";
        const string ENDURL_COLLECTION = "?own=1.xml";

        const string BEGINURL_HISTORY = "https://boardgamegeek.com/xmlapi2/plays?username=";
        const string ENDURL_HISTORY = "&type=thing.xml";

        internal event UserStateHandler ApiRead;
        internal event UserStateHandler Added;

        private void CallEvent(UserEventArgs e, UserStateHandler handler)
        {
            if (e != null)
            {
                handler.Invoke(this, e);
            }
        }

        protected void OnApiRead(UserEventArgs e) 
        {
            CallEvent(e, ApiRead);
        }
        protected void OnAdded(UserEventArgs e) 
        {
            CallEvent(e, Added);
        }

        public string CollectionPathConstructor()
        {
            string path = BEGINURL_COLLECTION + UserName + ENDURL_COLLECTION;
            return path;
        }
        public string HistoryPathConstructor()
        {
            string path = BEGINURL_HISTORY + UserName + ENDURL_HISTORY;
            return path;
        }

        public string CollectionFilePathConstructor() 
        {
            string path = UserName + "_Collection.xml";
            return path;
        }
        public string HistoryFilePathConstructor()
        {
            string path = UserName + "_History.xml";
            return path;
        }

        public void GetUserCollection()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(CollectionPathConstructor(), CollectionFilePathConstructor());
            }

            XDocument xmldoc = XDocument.Load(CollectionFilePathConstructor());
            try
            {
                var games = from ex in xmldoc.Element("items").Elements("item")
                            select new Game
                            {
                                Name = ex.Element("name").Value,
                            };
                foreach (var game in games)
                {
                    tempGameCollection.Add(game);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Возникло исключение: {ex.Message}");
                Console.ReadLine();
                Environment.Exit(0);
            }
            OnAdded(new UserEventArgs($"С сайта boardgamegeek.com закружена коллекция игрока {UserName} в файл {CollectionFilePathConstructor()}."));
        }

        public void GetUserHistory() 
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(HistoryPathConstructor(), HistoryFilePathConstructor());
            }
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(HistoryFilePathConstructor());
            XmlElement xRoot = xmldoc.DocumentElement;
            XmlNodeList childnodesPlay = xRoot.SelectNodes("play");
            foreach (XmlNode playEx in childnodesPlay)
            {
                foreach (XmlNode itemEx in playEx.SelectNodes("*"))
                {
                    string name = itemEx.SelectSingleNode("@name").Value;
                    DateTime date = Convert.ToDateTime(playEx.SelectSingleNode("@date").Value);
                    int count = Convert.ToInt32(playEx.SelectSingleNode("@quantity").Value);

                    tempGameData.Add(new GameData
                    {
                        Date = date,
                        Count = count,
                        Name = name
                    });


                }
            }
            OnAdded(new UserEventArgs($"С сайта boardgamegeek.com закружена история игрока {UserName} в файл {HistoryFilePathConstructor()}."));
        }

        public void AddPlayedGame(string name)
        {
            gameData.Add(new GameData { Name = name, Date = DateTime.Now, Count = 1});
            Console.WriteLine();
            Console.WriteLine($"Добавлена 1 партия в игру {name}, Дата {DateTime.Now.ToShortDateString()}.");
        }

        public void ShowCurrentGameHistory(string name)
        {
            Console.WriteLine($"История партий в игру {name}:");
            foreach (var game in gameData)
            {
                if (game.Name == name)
                {
                    Console.WriteLine($"\t{game.Date.ToShortDateString()}");
                }
            }
        }

        public void ShowPlayedGames()
        {
            Console.WriteLine($"Количество партий в игры из коллекции игрока {UserName}: ");
            gameData.Sort((x, y) => y.Date.CompareTo(x.Date));
            Console.WriteLine();
            foreach (var game in gameData)
            {
                Console.WriteLine($"{game.Name} - {game.Count} партий");
            }
        }

        public void ShowCollection() 
        {
            Console.WriteLine($"Коллекция игрока {UserName}: ");
            Console.WriteLine();
            foreach (var game in gameCollection)
            {
                Console.WriteLine((gameCollection.IndexOf(game)+1) + ". " + game.Name);
            }
        }

        public void GetPlayHistory() 
        {
            Console.WriteLine($"История игрока {UserName} с сайта bgg загружена успешно.");
        }

        public void GetNewCollection()
        {
            foreach (var game in gameCollection)
            {
                foreach (var item in gameCollection)
                {
                    if (game.Name == item.Name)
                    {
                        break;
                    }
                    else
                    {
                        gameCollection.Add(game);
                    }
                }
            }
        }

        public void GetNewLoggedGames()
        {
            foreach (var gameData in gameData)
            {
                foreach (var item in this.gameData)
                {
                    if (gameData.Name == item.Name & gameData.Date == item.Date)
                    {
                        break;
                    }
                    else
                    {
                        this.gameData.Add(new GameData
                        {
                            Date = gameData.Date,
                            Count = gameData.Count,
                            Name = gameData.Name
                        });
                    }
                }
            }
        }
    }
}
