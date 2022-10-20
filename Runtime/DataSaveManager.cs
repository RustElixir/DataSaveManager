using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public class DataSaveManager
{
    static string GetKey<T>(string key, T obj) where T : class, new()
    {
        string res = $"{key}:{typeof(T)}";

        return res;
    }

    [System.Serializable]
    public class DataHandler
    {
        string p;
        Dictionary<string, string> saveDictionary;

        internal DataHandler(string path, bool readFlag = true)
        {
            p = GetDataPath(path); ;
            Load(out saveDictionary);
        }

        internal DataHandler(DataHandler data)
        {
            p = data.p;
            saveDictionary = new Dictionary<string, string>(data.saveDictionary);
        }


        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(p, false, Encoding.GetEncoding("utf-8")))
            {
                var serialDict = new Serialization<string, string>(saveDictionary);
                serialDict.OnBeforeSerialize();
                string dictJsonString = JsonUtility.ToJson(serialDict);
                writer.WriteLine(dictJsonString);
            }
        }


        void Load(out Dictionary<string, string> dictionary)
        {

            dictionary = new Dictionary<string, string>();


            if (File.Exists(p))
            {
                using (StreamReader sr = new StreamReader(p, Encoding.GetEncoding("utf-8")))
                {
                    var jsonString = sr.ReadToEnd();
                    var sDict = JsonUtility.FromJson<Serialization<string, string>>(jsonString);
                    sDict.OnAfterDeserialize();
                    dictionary = sDict.ToDictionary();
                }
            }
            else
            {
                FileInfo fileInfo = new FileInfo(p);
                FileStream fileStream = fileInfo.Create();
                fileStream.Dispose();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns>Wheather you overwrote data or not.</returns>
        public bool SetData<T>(string key, T obj) where T : class, new()
        {
            var k = GetKey<T>(key, obj);

            var res = saveDictionary.ContainsKey(k);
            saveDictionary[k] = JsonUtility.ToJson(obj);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns>Wheather you can get valid data or not.</returns>

        public T GetData<T>(string key) where T : class, new()
        {
            T data = new T();
            var k = GetKey<T>(key, data);

            if (saveDictionary.ContainsKey(k))
            {
                string json = saveDictionary[k];
                return JsonUtility.FromJson<T>(json);
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static DataHandler Load(string filename)
    {
        return new DataHandler(filename);
    }

    public static void Delete(string filename)
    {
        File.Delete(GetDataPath(filename));
    }

    [Serializable]
    class Serialization<TKey, TValue>
    {
        public List<TKey> keys;
        public List<TValue> values;
        private Dictionary<TKey, TValue> dict;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return dict;
        }

        public Serialization(Dictionary<TKey, TValue> dict)
        {
            this.dict = dict;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(dict.Keys);
            values = new List<TValue>(dict.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            dict = new Dictionary<TKey, TValue>(count);
            Enumerable.Range(0, count).ToList().ForEach(i => dict.Add(keys[i], values[i]));
        }
    }

    static string GetDataPath(string path)
    {
        return $"{Application.persistentDataPath}/{path}";
    }

}
