using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int highScore;
    public int unlockedLevel;
    public float scoreLevel1;

    public float scoreLevel2;
    public float scoreLevel3;
    public float scoreLevel4;

    //Inventory
    // public List<IngredientsData> IngredientsInventory;
    public List<string> IngredientsInventory = new(); 

    // Clear Functions
    public void ClearInventory() { IngredientsInventory.Clear(); }
    public void ClearHighScore() { highScore = 0; }


    // Add Functions for Debug Only
    public void AddToInventory(String itemName) { IngredientsInventory.Add(itemName); }

    // Remove Functions
    public void RemoveItem(String itemName) { IngredientsInventory.Remove(itemName); }
}