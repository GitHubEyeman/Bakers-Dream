using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Target Scene")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private TransitionType transitionType = TransitionType.GoToScene;
    
    public enum TransitionType
    {
        GoToScene,
        GoToNextScene,
        GoToPreviousScene,
        GoToMainMenu,
        ResetGame
    }
    
    public void TriggerTransition()
    {
        switch (transitionType)
        {
            case TransitionType.GoToScene:
                if (!string.IsNullOrEmpty(targetSceneName))
                {
                    if (GameManager.Instance != null)
                        GameManager.Instance.GoToScene(targetSceneName);
                    else
                        SceneManager.LoadScene(targetSceneName);
                }
                break;
                
            case TransitionType.GoToNextScene:
                if (GameManager.Instance != null)
                    GameManager.Instance.GoToNextScene();
                else
                    Debug.LogWarning("GameManager not found!");
                break;
                
            case TransitionType.GoToPreviousScene:
                if (GameManager.Instance != null)
                    GameManager.Instance.GoToPreviousScene();
                else
                    Debug.LogWarning("GameManager not found!");
                break;
                
            case TransitionType.GoToMainMenu:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ResetGame();
                    GameManager.Instance.GoToScene("MainMenu");
                }
                else
                {
                    SceneManager.LoadScene("MainMenu");
                }
                break;
                
            case TransitionType.ResetGame:
                if (GameManager.Instance != null)
                    GameManager.Instance.ResetGame();
                break;
        }
    }
}