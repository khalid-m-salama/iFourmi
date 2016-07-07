using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.IO
{
   public static class Serializer
    {
       public static void SerializeClassifier(ClassifierInfo info)
       {
           if(!Directory.Exists("temp"))
               System.IO.Directory.CreateDirectory("temp");

           string path=@"temp\"+info.Desc;
           if (File.Exists(path)) 
               File.Delete(path);

           using(StreamWriter writer=new StreamWriter(@"temp\"+info.Desc))
           {
               
               BinaryFormatter formatter = new BinaryFormatter();              
              formatter.Serialize(writer.BaseStream, info);
               
           }
       }

       public static void SerializeEnsmeble(List<ClassifierInfo> list, string tag)
       {
           tag = tag.Replace(':', '-');
           if (!Directory.Exists("temp"))
               System.IO.Directory.CreateDirectory("temp");

           string path = @"temp\" + tag;
           if (File.Exists(path))
               File.Delete(path);

           using (StreamWriter writer = new StreamWriter(path))
           {
               BinaryFormatter formatter = new BinaryFormatter();
               formatter.Serialize(writer.BaseStream, list);

           }
           
       }

       public static ClassifierInfo DeserializeClassifier(string desc)
       {
           ClassifierInfo info = null;
           string path = @"temp\" + desc;

           using (StreamReader  reader = new StreamReader(path))
           {
               BinaryFormatter formatter = new BinaryFormatter();
               info = formatter.Deserialize(reader.BaseStream) as ClassifierInfo;
               reader.Close();

               File.Delete(path);
           }

           return info;

       }

       public static List<ClassifierInfo> DeserializeEnsemble(string tag)
       {
           tag = tag.Replace(':', '-');
           List<ClassifierInfo> list = null;
           string path = @"temp\" + tag;

           using (StreamReader reader = new StreamReader(path))
           {
               BinaryFormatter formatter = new BinaryFormatter();
               list = formatter.Deserialize(reader.BaseStream) as List<ClassifierInfo>;
               reader.Close();

               File.Delete(path);
           }

           return list;

       }
    }
}
