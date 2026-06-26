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
    [SerializeField] private GameObject titleGroup;  // ← ADD THIS
    
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button creditsButton;
    
    [Header("Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;
    
    [Header("Title Animation")]
    [SerializeField] private float titleBounceSpeed = 1.5f;
    [SerializeField] private float titleBounceHeight = 15f;
    
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "IngredientsScene";
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    
    private Vector3 titleStartPos;
    private bool isOptionsOpen = false;
    private bool isTutorialOpen = false;
    
    private void Start()
    {
        Debug.Log("=== MAIN MENU STARTED ===");
        
        // ============================================
        // STORE TITLE GROUP START POSITION
        // ============================================
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
        // SETUP BUTTONS
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
        // CHECK PANELS
        // ============================================
        
        if (optionsPanel != null)
        {
            Debug.Log($"Options Panel found: {optionsPanel.name}");
        }
        else
        {
            Debug.LogError("Options Panel is NOT assigned! Drag it in the Inspector!");
        }
        
        if (mainMenuPanel != null)
        {
            Debug.Log($"Main Menu Panel found: {mainMenuPanel.name}");
        }
        else
        {
            Debug.LogWarning("Main Menu Panel is not assigned!");
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
    }
    
    private void Update()
    {
        // ============================================
        // ANIMATE TITLE GROUP (Image + Text together)
        // ============================================
        if (titleGroup != null)
        {
            float bounce = Mathf.Sin(Time.time * titleBounceSpeed) * titleBounceHeight;
            titleGroup.transform.position = titleStartPos + new Vector3(0, bounce, 0);
        }
    }
    
    // ============================================
    // BUTTON FUNCTIONS
    // ============================================
    
    public void StartGame()
    {
        PlayClickSound();
        Debug.Log("START GAME button pressed!");
        
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"Loading scene: {gameSceneName}");
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("Game scene name is not set in the Inspector!");
        }
    }
    
    public void OpenTutorial()
    {
        PlayClickSound();
        Debug.Log("TUTORIAL button pressed!");
        
        isTutorialOpen = !isTutorialOpen;
        Debug.Log($"Tutorial open: {isTutorialOpen}");
        
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(isTutorialOpen);
            Debug.Log($"TutorialPanel active: {tutorialPanel.activeSelf}");
        }
        else
        {
            Debug.LogError("Tutorial Panel is not assigned!");
        }
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(!isTutorialOpen);
        }
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