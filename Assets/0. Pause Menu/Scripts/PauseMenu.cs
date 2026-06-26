using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
    // Tracks whether the game is currently paused
    public static bool isPaused = false;

    // Reference to the Pause Menu UI Canvas or Panel GameObject
    [SerializeField] private GameObject _PauseMenuUI;
    [SerializeField] private GameObject _SettingsMenuUI;
    [SerializeField] private Slider _MusicVolume;
    [SerializeField] private Slider _SFXVolume;
    [SerializeField] private CursorManager _CursorManager;
    private int _prevCursorSetNo = 0;

    void Start()
    {
        _SettingsMenuUI.SetActive(false);
        _PauseMenuUI.SetActive(false);
        _MusicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _MusicVolume.onValueChanged.AddListener(OnMusicSliderValueChanged);
        _SFXVolume.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        _SFXVolume.onValueChanged.AddListener(OnSFXSliderValueChanged);
        
    }
    void Update()
    {
        // Toggle pause when the Escape key is pressed
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
                if (_CursorManager != null)
                {_prevCursorSetNo = _CursorManager.SpriteSetNo;
                _CursorManager.SpriteSetNo = 0;}
            }
        }
    }

    // Unfreezes time and hides the pause UI
    public void Resume()
    {
        _SettingsMenuUI.SetActive(false);
        _PauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Normal game speed
        isPaused = false;
        if (_CursorManager != null) _CursorManager.SpriteSetNo = _prevCursorSetNo;
    }

    // Freezes time and displays the pause UI
    public void Pause()
    {
        _SettingsMenuUI.SetActive(false);
        _PauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freezes gameplay and physics
        isPaused = true;
    }

    public void Settings()
    {
        _SettingsMenuUI.SetActive(true);
        _PauseMenuUI.SetActive(false);
        Time.timeScale = 0f; // Freezes gameplay and physics
        isPaused = true;
    }

    // Loads the Main Menu scene (Ensure it's added to Build Settings)
    public void LoadMainMenu(string sceneName)
    {
        Time.timeScale = 1f; // Always reset time scale before switching scenes
        isPaused = false;

        // CHANGE SCENE NAME TO MAIN MENU LATER!
        SceneManager.LoadScene(sceneName);
    }

    // Closes the game application
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }



    private void OnMusicSliderValueChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value); //Debug.Log(value);
        }
    }

    private void OnSFXSliderValueChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }
}
