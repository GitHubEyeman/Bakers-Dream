using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance;
    [Header("How To Use")]
    [TextArea(3, 10)] public string designerNotes = "Insert Note Here";

    [Header("UI Reference Array")]
    [Tooltip("Add your transition panels here. Element 0 is the base layer, subsequent layers will move progressively faster.")]
    [SerializeField] private RectTransform[] faderRectTransforms;

    [Header("Slide Easing Settings")]
    [Tooltip("The base speed multiplier of the slide animation.")]
    [SerializeField] private float baseSlideSpeed = 1.5f;
    [Tooltip("How much faster each consecutive element in the array moves (e.g., 0.2 means Layer 1 is 20% faster than Layer 0).")]
    [SerializeField] private float speedMultiplierPerLayer = 0.3f;
    [Tooltip("Where the screens are completely covered (usually 0).")]
    [SerializeField] private float coveredXCoordinate = 0f;
    [Tooltip("Where the panels hide on the right side before moving in.")]
    [SerializeField] private float offscreenRightX = 60f;
    [Tooltip("Where the panels exit to the left side.")]
    [SerializeField] private float offscreenLeftX = -60f;

    [Header("In-Between Settings")]
    [SerializeField] private float timeSpentInBetween = 2f;
    [SerializeField] private bool skipInBetweenScene = true;

    public bool SkipInBetweenScene {get => skipInBetweenScene; set => skipInBetweenScene = value;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Start all panels safely tucked away to the right side
            SetAllXPositions(offscreenRightX);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerTransition(string targetSceneName)
    {
        StartCoroutine(TransitionSequence(targetSceneName));
    }

    private IEnumerator TransitionSequence(string targetSceneName)
    {
        // 1. Slide all layers in from the right to cover the screen
        yield return StartCoroutine(SlideAllToX(offscreenRightX, coveredXCoordinate));

        if (!skipInBetweenScene)
        {
            // 2. Load the In-Between scene behind the curtain
            yield return SceneManager.LoadSceneAsync("InBetweenScene");

            // 3. Slide all layers out to the left to reveal the In-Between scene
            yield return StartCoroutine(SlideAllToXInversed(coveredXCoordinate, offscreenLeftX));

            // 4. Wait so the user can interact or look at the intermediate scene
            yield return new WaitForSeconds(timeSpentInBetween);

            // 5. Reset all panels to the right side instantly, then slide in to cover screen again
            SetAllXPositions(offscreenRightX);
            yield return StartCoroutine(SlideAllToX(offscreenRightX, coveredXCoordinate));
    
        }
        
        // 6. Load the final destination scene
        yield return SceneManager.LoadSceneAsync(targetSceneName);

        // 7. Slide all layers away to the left to reveal the final scene
        yield return StartCoroutine(SlideAllToXInversed(coveredXCoordinate, offscreenLeftX));
        skipInBetweenScene = true;
    }

    // Coroutine that runs all individual panel slide calculations at the exact same time
    private IEnumerator SlideAllToX(float startX, float targetX)
    {
        // If no panels are assigned, skip the animation to avoid errors
        if (faderRectTransforms == null || faderRectTransforms.Length == 0) yield break;

        int totalLayers = faderRectTransforms.Length;
        float[] progressArray = new float[totalLayers]; // Track independent progress for each panel
        bool allDone = false;

        while (!allDone)
        {
            allDone = true; // Assume true until proven otherwise below

            for (int i = 0; i < totalLayers; i++)
            {
                // Each index element calculations get progressively faster based on the step offset multiplier
                float layerSpeed = baseSlideSpeed + (i * speedMultiplierPerLayer);
                
                if (progressArray[i] < 1f)
                {
                    progressArray[i] += Time.deltaTime * layerSpeed;
                    if (progressArray[i] > 1f) progressArray[i] = 1f;

                    // Smoothstep math curve calculations
                    float easedT = progressArray[i] * progressArray[i] * (3f - 2f * progressArray[i]);
                    float currentX = Mathf.Lerp(startX, targetX, easedT);
                    
                    SetSingleXPosition(faderRectTransforms[i], currentX);
                    allDone = false; // A layer is still moving, keep the loop running
                }
            }

            yield return null;
        }

        // Hard snap everything directly to the destination target position at the absolute end
        SetAllXPositions(targetX);
    }

    private IEnumerator SlideAllToXInversed(float startX, float targetX)
    {
        if (faderRectTransforms == null || faderRectTransforms.Length == 0) yield break;

        int totalLayers = faderRectTransforms.Length;
        float[] progressArray = new float[totalLayers]; 
        bool allDone = false;

        while (!allDone)
        {
            allDone = true; 

            for (int i = 0; i < totalLayers; i++)
            {
                // Inverted calculation: Element 0 gets the highest speed boost, last element gets 0 boost
                int speedTier = (totalLayers - 1) - i;
                float layerSpeed = baseSlideSpeed + (speedTier * speedMultiplierPerLayer);
                
                if (progressArray[i] < 1f)
                {
                    progressArray[i] += Time.deltaTime * layerSpeed;
                    if (progressArray[i] > 1f) progressArray[i] = 1f;

                    float easedT = progressArray[i] * progressArray[i] * (3f - 2f * progressArray[i]);
                    float currentX = Mathf.Lerp(startX, targetX, easedT);
                    
                    SetSingleXPosition(faderRectTransforms[i], currentX);
                    allDone = false; 
                }
            }

            yield return null;
        }

        SetAllXPositions(targetX);
    }

    // Helper logic to instantly reset all elements in the array
    private void SetAllXPositions(float xPos)
    {
        if (faderRectTransforms == null) return;
        
        foreach (RectTransform rect in faderRectTransforms)
        {
            if (rect != null) SetSingleXPosition(rect, xPos);
        }
    }

    // Helper logic to safely adjust a single specific RectTransform
    private void SetSingleXPosition(RectTransform rect, float xPos)
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x = xPos;
        rect.anchoredPosition = pos;
    }
}
