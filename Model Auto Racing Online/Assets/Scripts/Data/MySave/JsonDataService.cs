using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
public class JsonDataService : IDataService
{

    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;


        try
        {

            if (File.Exists(path))
            {
                Debug.Log("FILE EXISTS OVERWRITNG PREVIOUS DATA");
                File.Delete(path); 
            }
            Debug.Log("CREATING FILE");
            using FileStream fileStream = File.Create(path);
                fileStream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Data));
                return true;
            }
            catch (Exception e) 
            {
                Debug.LogWarning(e);
                return false;
            }
        
    }

    public T LoadData<T>(string RelativePath, bool Encrypted)
    {
        throw new System.NotImplementedException();
    }
}
   
