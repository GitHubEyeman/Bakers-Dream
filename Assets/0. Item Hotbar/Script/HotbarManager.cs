using UnityEngine;
using System.Collections.Generic;
using System;

public class HotbarManager : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject slotPrefab;
    
    // A master array/list of all possible ingredients assets in your game project
    public List<IngredientsData> allIngredientsDatabase; 

    private void Start()
    {
        SaveManager.Instance.CurrentSave.ClearInventory();
        // SaveManager.Instance.CurrentSave.AddToInventory("Dough"); //DEBUG DELETE WHEN DONE!
        // SaveManager.Instance.CurrentSave.AddToInventory("POOP"); //DEBUG DELETE WHEN DONE!
        SaveManager.Instance.CurrentSave.AddToInventory("GDough");
        SaveManager.Instance.CurrentSave.AddToInventory("GDough2");
        // SaveManager.Instance.CurrentSave.AddToInventory("CavedInDough2");
        // SaveManager.Instance.CurrentSave.AddToInventory("CavedInDough");
        PopulateHotbar();
    }

    public void PopulateHotbar()
    {
        
        // Clear old UI slots
        foreach (Transform child in contentPanel) Destroy(child.gameObject);

        // Fetch user data IDs
        List<string> savedInventory = SaveManager.Instance.CurrentSave.IngredientsInventory;
        if (savedInventory == null) return;
        
        foreach (string ingredientName in savedInventory)
        {
            
            IngredientsData data = allIngredientsDatabase.Find(x => x.ingredientName == ingredientName);
            if (data != null)
            {
                GameObject newSlot = Instantiate(slotPrefab, contentPanel);
                newSlot.GetComponent<IngredientSlotUI>().Setup(data);
            }
            else {Debug.Log("data is Null");}
        }
    }
    public void RemoveItemFromHotbar(string itemName)
    {
        // Remove one instance of the item (the first occurrence)
        SaveManager.Instance.CurrentSave.RemoveItem(itemName);
        // Rebuild the hotbar from the updated list
        PopulateHotbar();
        DisableHotbar();
    }

    public void DisableHotbar()
    {
        gameObject.SetActive(false);
    }
}
