using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Manager
{
    public class BanManager
    {
        private string path = "";
        private List<string> bannedList = new List<string>();
        public List<string> BannedList { get { return bannedList; } }

        public BanManager(string path)
        {
            this.path = path;
            bannedList = DeserializeObject<List<String>>();
            if(bannedList == null)
            {
                bannedList = new List<string>();
            }
        }

        public void Ban(string name)
        {
            if (!bannedList.Contains(name))
            {
                bannedList.Add(name);
                SerializeObject<List<String>>(bannedList);
            }
        }

        public void Unban(string name)
        {
            if (bannedList.Contains(name))
            {
                bannedList.Remove(name);
                SerializeObject<List<String>>(bannedList);
            }
        }
        private void SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (var writer = XmlWriter.Create(path))
            {
                xmlSerializer.Serialize(writer, bannedList);
            }
        }

        private List<String> DeserializeObject<T>()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            // Declare an object variable of the type to be deserialized.
            List<String> ls = null;

            try
            {
                using (var reader = XmlReader.Create(path))
                {
                    ls = (List<String>)xmlSerializer.Deserialize(reader);
                }
            }catch (Exception ex) { }

            return ls;
        }
    }
}
