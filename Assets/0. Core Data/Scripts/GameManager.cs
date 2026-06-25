using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public string currentRecipeName = "";
    public Dictionary<string, int> ingredientsUsed = new Dictionary<string, int>();
    public GameObject preparedDough;
    public float doughQuality = 1f; // 0-1, from kneading
    public bool hasDoughReady = false;
    
    [Header("Baking Results")]
    public string bakingResult = ""; // "perfect", "burnt", "average"
    public float bakingScore = 0f;
    public GameObject bakedItem;
    
    [Header("Scene Management")]
    public string previousScene = "";
    public string currentScene = "";
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialized!");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameManager destroyed.");
        }
    }
    
    private void Start()
    {
        // Track current scene
        currentScene = SceneManager.GetActiveScene().name;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        previousScene = currentScene;
        currentScene = scene.name;
        Debug.Log($"Scene changed: {previousScene} → {currentScene}");
    }
    
    // ============================================
    // INGREDIENT DATA MANAGEMENT
    // ============================================
    
    public void SetIngredients(Dictionary<string, int> ingredients)
    {
        ingredientsUsed = new Dictionary<string, int>(ingredients);
        Debug.Log($"Ingredients saved: {ingredientsUsed.Count} items");
    }
    
    public void SetRecipe(string recipeName)
    {
        currentRecipeName = recipeName;
        Debug.Log($"Recipe set: {recipeName}");
    }
    
    public void ClearIngredients()
    {
        ingredientsUsed.Clear();
        currentRecipeName = "";
        Debug.Log("Ingredients cleared");
    }
    
    // ============================================
    // DOUGH MANAGEMENT
    // ============================================
    
    public void SetDough(GameObject dough, float quality = 1f)
    {
        preparedDough = dough;
        doughQuality = Mathf.Clamp01(quality);
        hasDoughReady = true;
        Debug.Log($"Dough set! Quality: {doughQuality:F2}");
    }
    
    public GameObject GetDough()
    {
        return preparedDough;
    }
    
    public void ClearDough()
    {
        if (preparedDough != null)
        {
            Destroy(preparedDough);
        }
        preparedDough = null;
        hasDoughReady = false;
        doughQuality = 0f;
        Debug.Log("Dough cleared");
    }
    
    // ============================================
    // BAKING MANAGEMENT
    // ============================================
    
    public void SetBakingResult(string result, float score, GameObject bakedItemObj = null)
    {
        bakingResult = result;
        bakingScore = Mathf.Clamp(score, 0f, 100f);
        bakedItem = bakedItemObj;
        Debug.Log($"Baking result: {result} (Score: {bakingScore:F0}%)");
    }
    
    public (string result, float score) GetBakingResult()
    {
        return (bakingResult, bakingScore);
    }
    
    // ============================================
    // SCENE TRANSITIONS
    // ============================================
    
    public void GoToScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
    
    public void GoToNextScene()
    {
        // Determine next scene based on current
        string nextScene = "";
        switch (currentScene)
        {
            case "IngredientsScene":
                nextScene = "KneadingScene";
                break;
            case "KneadingScene":
                nextScene = "BakingScene";
                break;
            case "BakingScene":
                nextScene = "DecorationScene";
                break;
            case "DecorationScene":
                nextScene = "ResultScene";
                break;
            default:
                nextScene = "MainMenu";
                break;
        }
        
        GoToScene(nextScene);
    }
    
    public void GoToPreviousScene()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            GoToScene(previousScene);
        }
        else
        {
            GoToScene("MainMenu");
        }
    }
    
    // ============================================
    // RESET GAME
    // ============================================
    
    public void ResetGame()
    {
        ClearIngredients();
        ClearDough();
        bakingResult = "";
        bakingScore = 0f;
        bakedItem = null;
        Debug.Log("Game reset!");
    }
}