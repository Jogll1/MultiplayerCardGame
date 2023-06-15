using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

//class to manage saving and loading JSON data
//static lets it be accessed without reference or instantiating objects
//https://www.youtube.com/watch?v=KZft1p8t2lQ
public static class SaveCardData
{
    //Saving object lists
    #region Saving
    //<T> is a generic object
    public static void SaveToJson<T>(List<T> toSave, string fileName)
    {
        Debug.Log(GetPath(fileName));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
    }

    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create); //create or override new file at path

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content); //im guessing this writes to the file
        }
    }
    #endregion

    #region Loading

    public static List<T> ReadFromJson<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            Debug.Log("No data");
            return new List<T>(); //if file is null or empty
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        return ""; //return empty if file doesn't exist
    }

    #endregion

    private static string GetPath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName; //points to AppData/LocalLow
    }
}

//dont mind this - helps out with Unity's built in json solution, JsonUtility
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
