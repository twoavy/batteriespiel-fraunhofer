using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string m_Uuid;
    public string m_Name;
    public int m_GlobalScore;
    public string m_Language;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    
    public void LoadFromJson(string a_Json)
    {
       JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData a_SaveData);
    void LoadFromSaveData(SaveData a_SaveData);
}
