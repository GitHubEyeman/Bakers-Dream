using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject titleGroup;
    
    [Header("Tutorial Sub-Panels")]
    [SerializeField] private GameObject howToPanel;
    [SerializeField] private GameObject mixingPanel;
    [SerializeField] private GameObject kneadingPanel;
    [SerializeField] private GameObject bakingPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button creditsButton;
    
    [Header("Tutorial Buttons")]
    [SerializeField] private Button howToButton;
    [SerializeField] private Button mixingButton;
    [SerializeField] private Button kneadingButton;
    [SerializeField] private Button bakingButton;
    [SerializeField] private Button tutorialBackButton;
    
    [Header("Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;
    
    [Header("Title Animation")]
    [SerializeField] private float titleBounceSpeed = 1.5f;
    [SerializeField] private float titleBounceHeight = 15f;
    
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "1. Mixing";
    [SerializeField] private int gameSceneIndex = 5;
    [SerializeField] private bool useSceneIndex = true;  // SET TO TRUE BY DEFAULT
    
    private Vector3 titleStartPos;
    private bool isOptionsOpen = false;
    private bool isTutorialOpen = false;
    
    private void Start()
    {
        Debug.Log("=== MAIN MENU STARTED ===");
        
        // Store title start position
        if (titleGroup != null)
        {
            titleStartPos = titleGroup.transform.position;
            Debug.Log("TitleGroup position stored");
        }
        else
        {
            Debug.LogWarning("TitleGroup is not assigned!");
        }
        
        // ============================================
        // SETUP MAIN BUTTONS
        // ============================================
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
            Debug.Log("Start button connected");
        }
        else
        {
            Debug.LogWarning("Start Button is not assigned!");
        }
        
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(OpenTutorial);
            Debug.Log("Tutorial button connected");
        }
        else
        {
            Debug.LogWarning("Tutorial Button is not assigned!");
        }
        
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(ToggleOptions);
            Debug.Log("Options button connected");
        }
        else
        {
            Debug.LogWarning("Options Button is not assigned!");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            Debug.Log("Exit button connected");
        }
        else
        {
            Debug.LogWarning("Exit Button is not assigned!");
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseAllPanels);
            Debug.Log("Back button connected");
        }
        else
        {
            Debug.LogWarning("Back Button is not assigned!");
        }
        
        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OpenCredits);
            Debug.Log("Credits button connected");
        }
        else
        {
            Debug.LogWarning("Credits Button is not assigned!");
        }
        
        // ============================================
        // SETUP TUTORIAL BUTTONS
        // ============================================
        
        if (howToButton != null)
        {
            howToButton.onClick.AddListener(OpenHowTo);
            Debug.Log("HowTo button connected");
        }
        
        if (mixingButton != null)
        {
            mixingButton.onClick.AddListener(OpenMixing);
            Debug.Log("Mixing button connected");
        }
        
        if (kneadingButton != null)
        {
            kneadingButton.onClick.AddListener(OpenKneading);
            Debug.Log("Kneading button connected");
        }
        
        if (bakingButton != null)
        {
            bakingButton.onClick.AddListener(OpenBaking);
            Debug.Log("Baking button connected");
        }
        
        if (tutorialBackButton != null)
        {
            tutorialBackButton.onClick.AddListener(CloseTutorial);
            Debug.Log("Tutorial Back button connected");
        }
        
        // ============================================
        // SETUP SLIDERS
        // ============================================
        
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            Debug.Log($"Music slider set to: {musicSlider.value}");
        }
        else
        {
            Debug.LogWarning("Music Slider is not assigned!");
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            Debug.Log($"SFX slider set to: {sfxSlider.value}");
        }
        else
        {
            Debug.LogWarning("SFX Slider is not assigned!");
        }
        
        // ============================================
        // CLOSE ALL PANELS INITIALLY
        // ============================================
        
        CloseAllPanels();
        
        // ============================================
        // PLAY BACKGROUND MUSIC
        // ============================================
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("MainMenuMusic");
            Debug.Log("Background music started");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
        
        // ============================================
        // UPDATE VOLUME DISPLAYS
        // ============================================
        
        UpdateVolumeDisplay();
        
        Debug.Log("=== MAIN MENU INITIALIZATION COMPLETE ===");
        Debug.Log($"Game scene will load: {gameSceneName} (Index: {gameSceneIndex})");
        Debug.Log($"Using scene index: {useSceneIndex}");
    }
    
    private void Update()
    {
        // Animate title group (image + text together)
        if (titleGroup != null)
        {
            float bounce = Mathf.Sin(Time.time * titleBounceSpeed) * titleBounceHeight;
            titleGroup.transform.position = titleStartPos + new Vector3(0, bounce, 0);
        }
    }
    
    // ============================================
    // MAIN BUTTON FUNCTIONS
    // ============================================
    
    public void StartGame()
    {
        PlayClickSound();
        Debug.Log("=== START GAME BUTTON PRESSED ===");
        
        // ============================================
        // METHOD 1: Load by Scene Index (Recommended)
        // ============================================
        if (useSceneIndex)
        {
            try
            {
                Debug.Log($"Loading scene by index: {gameSceneIndex}");
                // SceneManager.LoadScene(gameSceneIndex);
                SceneTransitioner.Instance.TriggerTransition("1. Mixing");
                Debug.Log($"Successfully loaded scene index: {gameSceneIndex}");
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load by index: {e.Message}");
            }
        }
        
        // ============================================
        // METHOD 2: Load by Scene Name (Fallback)
        // ============================================
        try
        {
            Debug.Log($"Loading scene by name: {gameSceneName}");
            // SceneManager.LoadScene(gameSceneName);
            SceneTransitioner.Instance.TriggerTransition("1. Mixing");
            Debug.Log($"Successfully loaded scene: {gameSceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load by name: {e.Message}");
            Debug.LogError("Make sure the scene is added to Build Settings!");
            Debug.LogError($"Scene name: '{gameSceneName}'");
            Debug.LogError($"Scene index: {gameSceneIndex}");
        }
        
        Debug.Log("=== START GAME COMPLETE ===");
    }
    
    public void OpenTutorial()
    {
        PlayClickSound();
        Debug.Log("TUTORIAL button pressed!");
        
        isTutorialOpen = true;
        
        // Show tutorial panel, hide main menu
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Debug.Log("TutorialPanel shown");
        }
        else
        {
            Debug.LogError("Tutorial Panel is not assigned!");
        }
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        
        // Show HowTo panel by default
        OpenHowTo();
    }
    
    public void CloseTutorial()
    {
        PlayClickSound();
        Debug.Log("Closing Tutorial...");
        
        // Hide all tutorial sub-panels
        if (howToPanel != null)
            howToPanel.SetActive(false);
        if (mixingPanel != null)
            mixingPanel.SetActive(false);
        if (kneadingPanel != null)
            kneadingPanel.SetActive(false);
        if (bakingPanel != null)
            bakingPanel.SetActive(false);
        
        // Hide tutorial panel
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
        
        // Show main menu
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        
        isTutorialOpen = false;
        Debug.Log("Tutorial closed");
    }
    
    public void ToggleOptions()
    {
        PlayClickSound();
        Debug.Log("=== OPTIONS BUTTON PRESSED ===");
        
        isOptionsOpen = !isOptionsOpen;
        Debug.Log($"Options open: {isOptionsOpen}");
        
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(isOptionsOpen);
            Debug.Log($"OptionsPanel active: {optionsPanel.activeSelf}");
        }
        else
        {
            Debug.LogError("Options Panel is NULL! Assign it in the Inspector!");
            return;
        }
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(!isOptionsOpen);
            Debug.Log($"MainMenuPanel active: {mainMenuPanel.activeSelf}");
        }
        
        Debug.Log("=== OPTIONS TOGGLED ===");
    }
    
    public void OpenCredits()
    {
        PlayClickSound();
        Debug.Log("CREDITS button pressed!");
        
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            Debug.Log($"CreditsPanel active: {creditsPanel.activeSelf}");
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Credits Panel is not assigned!");
        }
    }
    
    public void CloseAllPanels()
    {
        PlayClickSound();
        Debug.Log("=== CLOSING ALL PANELS ===");
        
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            Debug.Log("OptionsPanel closed");
        }
        
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            Debug.Log("TutorialPanel closed");
        }
        
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
            Debug.Log("CreditsPanel closed");
        }
        
        // Hide all tutorial sub-panels
        if (howToPanel != null)
            howToPanel.SetActive(false);
        if (mixingPanel != null)
            mixingPanel.SetActive(false);
        if (kneadingPanel != null)
            kneadingPanel.SetActive(false);
        if (bakingPanel != null)
            bakingPanel.SetActive(false);
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Debug.Log("MainMenuPanel shown");
        }
        
        isOptionsOpen = false;
        isTutorialOpen = false;
        
        Debug.Log("=== ALL PANELS CLOSED ===");
    }
    
    public void ExitGame()
    {
        PlayClickSound();
        Debug.Log("EXIT GAME button pressed!");
        
        #if UNITY_EDITOR
            Debug.Log("Stopping play mode in Editor...");
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Debug.Log("Quitting application...");
            Application.Quit();
        #endif
    }
    
    // ============================================
    // TUTORIAL SUB-PANEL FUNCTIONS
    // ============================================
    
    public void OpenHowTo()
    {
        PlayClickSound();
        Debug.Log("Opening HowTo panel...");
        
        // Hide all other panels
        if (mixingPanel != null)
            mixingPanel.SetActive(false);
        if (kneadingPanel != null)
            kneadingPanel.SetActive(false);
        if (bakingPanel != null)
            bakingPanel.SetActive(false);
        
        // Show HowTo panel
        if (howToPanel != null)
        {
            howToPanel.SetActive(true);
            Debug.Log("HowToPanel shown");
        }
    }
    
    public void OpenMixing()
    {
        PlayClickSound();
        Debug.Log("Opening Mixing tutorial...");
        
        // Hide all other panels
        if (howToPanel != null)
            howToPanel.SetActive(false);
        if (kneadingPanel != null)
            kneadingPanel.SetActive(false);
        if (bakingPanel != null)
            bakingPanel.SetActive(false);
        
        // Show Mixing panel
        if (mixingPanel != null)
        {
            mixingPanel.SetActive(true);
            Debug.Log("MixingPanel shown");
        }
    }
    
    public void OpenKneading()
    {
        PlayClickSound();
        Debug.Log("Opening Kneading tutorial...");
        
        // Hide all other panels
        if (howToPanel != null)
            howToPanel.SetActive(false);
        if (mixingPanel != null)
            mixingPanel.SetActive(false);
        if (bakingPanel != null)
            bakingPanel.SetActive(false);
        
        // Show Kneading panel
        if (kneadingPanel != null)
        {
            kneadingPanel.SetActive(true);
            Debug.Log("KneadingPanel shown");
        }
    }
    
    public void OpenBaking()
    {
        PlayClickSound();
        Debug.Log("Opening Baking tutorial...");
        
        // Hide all other panels
        if (howToPanel != null)
            howToPanel.SetActive(false);
        if (mixingPanel != null)
            mixingPanel.SetActive(false);
        if (kneadingPanel != null)
            kneadingPanel.SetActive(false);
        
        // Show Baking panel
        if (bakingPanel != null)
        {
            bakingPanel.SetActive(true);
            Debug.Log("BakingPanel shown");
        }
    }
    
    // ============================================
    // VOLUME SETTINGS
    // ============================================
    
    private void OnMusicVolumeChanged(float value)
    {
        Debug.Log($"Music volume changed to: {value}");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
        
        PlayerPrefs.SetFloat("MusicVolume", value);
        UpdateVolumeDisplay();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        Debug.Log($"SFX volume changed to: {value}");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
        
        PlayerPrefs.SetFloat("SFXVolume", value);
        UpdateVolumeDisplay();
    }
    
    private void UpdateVolumeDisplay()
    {
        if (musicValueText != null && musicSlider != null)
        {
            musicValueText.text = $"{Mathf.RoundToInt(musicSlider.value * 100)}%";
        }
        
        if (sfxValueText != null && sfxSlider != null)
        {
            sfxValueText.text = $"{Mathf.RoundToInt(sfxSlider.value * 100)}%";
        }
    }
    
    // ============================================
    // AUDIO HELPERS
    // ============================================
    
    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick", 0.5f);
        }
    }
}