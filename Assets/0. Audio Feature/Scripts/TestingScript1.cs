using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestingScript1 : MonoBehaviour
{
    
    public IngredientsData ingredients;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic("Donden");
        Debug.Log("" + ingredients.ingredientName);
    }



    // Update is called once per frame
    void Update()
    {
        
        
        
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame) {
            AudioManager.Instance.CrossfadeMusic("Donden");
        }else if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame) {
            AudioManager.Instance.CrossfadeMusic("PureAgain");

        }else if (Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame) {
            AudioManager.Instance.PlaySFX("sfx1");

        }else if (Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame) {
            AudioManager.Instance.PlaySFX("sfx2", 10);

        }else if (Keyboard.current != null && Keyboard.current.digit5Key.wasPressedThisFrame) {
            SaveManager.Instance.LoadGame();
 
        }else if (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame) {
            AudioManager.Instance.StopMusic();

        }
        

    }
}
