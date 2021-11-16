using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace bggparser
{
    class FileService
    {
        private readonly string PATH;
        public FileService()
        {

        }
        public FileService(string path)
        {
            PATH = path;
        }

        public List<Game> LoadCollection(UserController u) 
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Game>));
            using (FileStream stream = new FileStream(PATH, FileMode.OpenOrCreate))
            {
                u.gameCollection = (List<Game>)formatter.Deserialize(stream);
                return u.gameCollection;
            }
        }

        public List<GameData> LoadDataCollection(UserController u)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<GameData>));
            using (FileStream stream = new FileStream(PATH, FileMode.OpenOrCreate))
            {
                u.gameDataCollection = (List<GameData>)formatter.Deserialize(stream);
                return u.gameDataCollection;
            }
        }

        public void SaveCollection(List<Game> o) 
        {
             XmlSerializer formatter = new XmlSerializer(typeof(List<Game>));
             using (FileStream stream = new FileStream(PATH, FileMode.OpenOrCreate))
             {
                 formatter.Serialize(stream, o);
             }
        }

        public void SaveDataCollection(List<GameData> o)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<GameData>));
            using (FileStream stream = new FileStream(PATH, FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, o);
            }
        }
    }
}
