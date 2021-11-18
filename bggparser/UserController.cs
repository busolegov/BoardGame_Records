using System;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Xml;

namespace bggparser
{
    [Serializable]
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

        public List<GameData> gameDataCollection = new List<GameData>();
        public List<Game> gameCollection = new List<Game>();

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
            WebClient client = new WebClient();
            using (client)
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
                    gameCollection.Add(game);
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Возникло исключение: {ex.Message}");
                Console.ReadLine();
                Environment.Exit(0);
            }
            OnAdded(new UserEventArgs($"С сайта boardgamegeek.com закружена коллекция игрока {UserName} в файл {CollectionFilePathConstructor()}."));
        }

        public void GetUserHistory() 
        {
            WebClient client = new WebClient();
            using (client)
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
                    gameDataCollection.Add(new GameData
                    {
                        Date = Convert.ToDateTime(playEx.SelectSingleNode("@date").Value),
                        Count = Convert.ToInt32(playEx.SelectSingleNode("@quantity").Value),
                        Name = name
                    });
                }
            }
            OnAdded(new UserEventArgs($"С сайта boardgamegeek.com закружена история игрока {UserName} в файл {HistoryFilePathConstructor()}."));
        }

        public void AddPlayedGame(string name)
        {
            gameDataCollection.Add(new GameData { Name = name, Date = DateTime.Now, Count = 1});
            Console.WriteLine();
            Console.WriteLine($"Добавлена 1 партия в игру {name}, Дата {DateTime.Now.ToShortDateString()}.");
        }

        public void ShowCurrentGameHistory(string name)
        {
            Console.WriteLine($"История партий в игру {name}:");
            foreach (var game in gameDataCollection)
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
            gameDataCollection.Sort((x, y) => y.Date.CompareTo(x.Date));
            Console.WriteLine();
            foreach (var game in gameDataCollection)
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
    }
}
