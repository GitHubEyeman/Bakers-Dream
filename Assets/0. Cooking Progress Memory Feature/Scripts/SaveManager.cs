using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [TextArea(3, 10)] public string designerNotes = "Insert Note Here";
    public static SaveManager Instance;

    private string savePath;

    public SaveData CurrentSave = new SaveData();

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "/save.json";

        LoadGame();
    }

    // =====================================================
    // SAVE
    // =====================================================

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentSave, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved!");
        Debug.Log(savePath);
    }

    // =====================================================
    // LOAD
    // =====================================================

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            CurrentSave = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.Log("No save file found. Creating new save.");

            CurrentSave = new SaveData();
        }
    }

    // =====================================================
    // DELETE SAVE
    // =====================================================

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        CurrentSave = new SaveData();

        Debug.Log("Save Deleted");
    }


    // =====================================================
    // SAVE on QUIT
    // =====================================================
    private void OnApplicationQuit()
    {
        SaveGame();
    }

}