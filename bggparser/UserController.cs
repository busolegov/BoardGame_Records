using System;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

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

        const string BEGINURL = "https://www.boardgamegeek.com/xmlapi/collection/";
        const string ENDURL = "?own=1.xml";

        internal event UserStateHandler ApiRead;
        internal event UserStateHandler Added;
        internal event UserStateHandler Showed;

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
        protected void OnShowed(UserEventArgs e)
        {
            CallEvent(e, Added);
        }

        public string AddressConstructor()
        {
            string address;
            address = BEGINURL + UserName + ENDURL;
            return address;
        }

        public void ApiReader()
        {
            WebClient client = new WebClient();
            using (client)
            {
                client.DownloadFile(AddressConstructor(), "apiuser.xml");
            }

            XDocument xmldoc = XDocument.Load("apiuser.xml");
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
                System.Environment.Exit(0);
            }
            OnAdded(new UserEventArgs($"С сайта boardgamegeek.com закружена коллекция игрока {UserName}."));
        }

        public void AddPlayedGame(string name)
        {
            gameDataCollection.Add(new GameData { Name = name, Date = DateTime.Now });
            Console.WriteLine();
            OnShowed(new UserEventArgs($"Добавлена партия в игру {name}, Дата {DateTime.Now}."));
        }

        public void ShowPlayedGames()
        {
            OnShowed(new UserEventArgs($"История партий игрока {UserName}: "));
            Console.WriteLine();
            foreach (var game in gameDataCollection)
            {
                Console.WriteLine($"{game.Name} - {game.Date}");
            }
        }

        public void ShowCollection() 
        {
            OnShowed(new UserEventArgs($"Коллекция игрока {UserName}: "));
            Console.WriteLine();
            foreach (var game in gameCollection)
            {
                Console.WriteLine((gameCollection.IndexOf(game)+1) + ". " + game.Name);
            }
        }
    }
}
