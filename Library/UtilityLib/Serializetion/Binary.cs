using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UtilityLib.Serializetion
{
    public static class Binary<T>
    {
        public static void WriteObject(T obj, string path)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new BinaryFormatter();

            var mstream = new FileStream(path, FileMode.OpenOrCreate);
            serializer.Serialize(mstream, obj);
            mstream.Close();
        }

        public static T ReadObject(string path)
        {
            var mstream = new FileStream(path, FileMode.Open);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new BinaryFormatter();
            var obj = serializer.Deserialize(mstream);
            mstream.Close();
            return (T)obj;
        }
    }
}
