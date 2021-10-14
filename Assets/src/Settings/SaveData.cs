using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class SaveData : MonoBehaviour
{
    private static SaveData Instance;

    private void Awake()
    {
        var isFirst = Instance == null;

        if (!isFirst) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    public bool ResetSaveData = false;


    public static void Save(LevelLocation location, LevelScoreData data)
    {
        _saveData(location.ToString(), data);
    }

    public static LevelScoreData Load(LevelLocation location)
    {
        return Instance.ResetSaveData ? new LevelScoreData() : _loadData(location.ToString());
    }
    
    public static void Persist()
    {
        PlayerPrefs.Save();
    }

    private static LevelScoreData _loadData(string address)
    {
        var data = new LevelScoreData();
        var fields = typeof(LevelScoreData).GetFields();

        foreach (var field in fields)
        {
            var fieldAddress = address + "." + field.Name;

            if (field.FieldType == typeof(float))
            {
                field.SetValue(data, PlayerPrefs.GetFloat(fieldAddress, (float)field.GetValue(data)));
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(data, PlayerPrefs.GetString(fieldAddress, (string)field.GetValue(data)));
            }
            else if (field.FieldType == typeof(int))
            {
                field.SetValue(data, PlayerPrefs.GetInt(fieldAddress, (int)field.GetValue(data)));
            }
            else if (field.FieldType == typeof(bool))
            {
                field.SetValue(data, Extensions.GetBool(fieldAddress, (bool)field.GetValue(data)));
            }
        }
        return data;
    }
    private static void _saveData(string address, LevelScoreData data)
    {
        var fields = typeof(LevelScoreData).GetFields();

        foreach (var field in fields)
        {
            var fieldAddress = address + "." + field.Name;

            if (field.FieldType == typeof(float))
            {
                PlayerPrefs.SetFloat(fieldAddress, (float)field.GetValue(data));
            }
            else if (field.FieldType == typeof(string))
            {
                PlayerPrefs.SetString(fieldAddress, (string)field.GetValue(data));
            }
            else if (field.FieldType == typeof(int))
            {
                PlayerPrefs.SetInt(fieldAddress, (int)field.GetValue(data));
            }
            else if (field.FieldType == typeof(bool))
            {
                Extensions.SetBool(fieldAddress, (bool)field.GetValue(data));
            }
        }
    }
}
