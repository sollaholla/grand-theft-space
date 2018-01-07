using GrandTheftSpace.CoreGame.Debugging;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GrandTheftSpace.CoreGame.Serialization
{
    internal static class Serializer
    {
        public static T Deserialize<T>(string path) where T : class
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Open);

                var serializer = new XmlSerializer(typeof(T));

                var obj = serializer.Deserialize(stream);

                stream.Close();

                return (T)obj;
            }
            catch (Exception e)
            {
                Logger.Log(e);

                if (stream != null)
                {
                    stream.Close();
                }

                return null;
            }
        }

        public static void Serialize<T>(T obj, string filePath)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(filePath, FileMode.OpenOrCreate);

                var serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(stream, obj);

                stream.Close();
            }
            catch (Exception e)
            {
                Logger.Log(e);

                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }
}
