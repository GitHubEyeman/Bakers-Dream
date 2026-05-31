using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Required for the new input system classes

public class DoughKneadingDirectInput : MonoBehaviour
{
    [Header("Target Mesh Components")]
    [SerializeField] private SkinnedMeshRenderer doughMesh;
    private Material targetMaterial;

    [Header("Minigame Settings")]
    [SerializeField] private int totalCyclesRequired = 5;
    [SerializeField] private float dragSensitivity = 3.0f;

    // BlendShape Index Mapping
    private const int INDEX_INCOMPLETE_DOUGH = 0;
    private const int INDEX_START_GRAB = 1;
    private const int INDEX_PULL = 2;
    private const int INDEX_PUSH = 3;

    // Game Variables
    private int completedCycles = 0;
    private float currentIncompleteValue = 100f;
    private bool isPullActive = false;
    private bool isPushActive = false;
    private bool isPullFullyCompleted = false;

    // Mouse Tracking Variables
    private Vector2 mouseStartPos;

    void Start()
    {
        if (doughMesh == null)
        {
            doughMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        targetMaterial = doughMesh.material;
        ResetDoughState();
    }

    void Update()
    {
        // Safety check to ensure a mouse device is plugged into the system
        if (Mouse.current == null) return;

        HandleDirectInput();
    }

    private void ResetDoughState()
    {
        StartTextureBlend(1.0f-currentIncompleteValue/100f , 0.1f);
        doughMesh.SetBlendShapeWeight(INDEX_INCOMPLETE_DOUGH, currentIncompleteValue);
        StartBlendShapeTween(INDEX_START_GRAB, 0f, 0.1f);
        //doughMesh.SetBlendShapeWeight(INDEX_PULL, 0f);
        StartBlendShapeTween(INDEX_PULL, 0f, 0.1f);
        //doughMesh.SetBlendShapeWeight(INDEX_PUSH, 0f);
        StartBlendShapeTween(INDEX_PUSH, 0f, 0.1f);
        
        isPullFullyCompleted = false;
        isPullActive = false;
        isPushActive = false;
    }

    private void HandleDirectInput()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue(); 

        // 1. Click Started This Frame
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(currentMousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("topDough") && !isPullFullyCompleted)
                {
                    isPullActive = true;
                    mouseStartPos = currentMousePos;
                    //doughMesh.SetBlendShapeWeight(INDEX_START_GRAB, 100f);
                    StartBlendShapeTween(INDEX_START_GRAB, 100f, 0.1f);
                }
                else if (hit.collider.CompareTag("botDough") && isPullFullyCompleted)
                {
                    isPushActive = true;
                    mouseStartPos = currentMousePos;
                }
            }
        }

        // 2. Click Held Down (Continuous Evaluation)
        if (Mouse.current.leftButton.isPressed)
        {
            if (isPullActive)
            {
                // Dragging Backwards (Downwards on Screen)
                float currentDragY = mouseStartPos.y - currentMousePos.y;
                float dragPercentage = Mathf.Clamp(currentDragY * dragSensitivity / Screen.height * 100f, 0f, 100f);

                doughMesh.SetBlendShapeWeight(INDEX_START_GRAB, 100f - dragPercentage);
                doughMesh.SetBlendShapeWeight(INDEX_PULL, dragPercentage);
            }
            else if (isPushActive)
            {
                // Dragging Forwards (Upwards on Screen)
                float currentDragY = currentMousePos.y - mouseStartPos.y;
                float dragPercentage = Mathf.Clamp(currentDragY * dragSensitivity / Screen.height * 100f, 0f, 100f);

                doughMesh.SetBlendShapeWeight(INDEX_PULL, 100f - dragPercentage);
                doughMesh.SetBlendShapeWeight(INDEX_PUSH, dragPercentage);
            }
        }

        // 3. Click Released This Frame
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isPullActive)
            {
                float finalPullWeight = doughMesh.GetBlendShapeWeight(INDEX_PULL);

                if (Mathf.Approximately(finalPullWeight, 100f))
                {
                    isPullFullyCompleted = true;
                }
                else
                {
                    //doughMesh.SetBlendShapeWeight(INDEX_START_GRAB, 0f);
                    StartBlendShapeTween(INDEX_START_GRAB, 0f, 0.1f);
                    //doughMesh.SetBlendShapeWeight(INDEX_PULL, 0f);
                    StartBlendShapeTween(INDEX_PULL, 0f, 0.1f);
                }
                isPullActive = false;
            }
            else if (isPushActive)
            {
                float finalPushWeight = doughMesh.GetBlendShapeWeight(INDEX_PUSH);

                if (Mathf.Approximately(finalPushWeight, 100f))
                {
                    CompleteKneadCycle();
                }
                else
                {
                    //doughMesh.SetBlendShapeWeight(INDEX_PUSH, 0f);
                    StartBlendShapeTween(INDEX_PUSH, 0f, 0.5f);
                }
                isPushActive = false;
            }
        }
    }

    private void CompleteKneadCycle()
    {
        completedCycles++;
        Debug.Log($"Knead Cycle Complete! Total: {completedCycles}/{totalCyclesRequired}");

        float cycleReduction = 100f / totalCyclesRequired;
        currentIncompleteValue = Mathf.Clamp(currentIncompleteValue - cycleReduction, 0f, 100f);

        if (currentIncompleteValue <= 0)
        {
            OnKneadingFinished();
        }
        else
        {
            ResetDoughState();
        }
    }

    private void OnKneadingFinished()
    {
        doughMesh.SetBlendShapeWeight(INDEX_INCOMPLETE_DOUGH, 0f);
        doughMesh.SetBlendShapeWeight(INDEX_START_GRAB, 0f);
        doughMesh.SetBlendShapeWeight(INDEX_PULL, 0f);
        doughMesh.SetBlendShapeWeight(INDEX_PUSH, 0f);

        StartTextureBlend( 1f, 0.1f);
        
        Debug.Log("Dough is perfectly kneaded!");
        this.enabled = false;
    }


    //Tweening with Lerp
    public void StartBlendShapeTween(int blendShapeIndex, float endValue, float duration)
    {
        // Stop any conflicting loops and start a fresh smooth transition
        StopAllCoroutines();
        StartCoroutine(TweenRoutine(blendShapeIndex, endValue, duration));
    }

    private IEnumerator TweenRoutine(int index, float targetValue, float duration)
    {
        float timeElapsed = 0f;
        float startValue = doughMesh.GetBlendShapeWeight(index);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / duration;
            
            // SmootherStep formula for an organic, ease-in-ease-out look
            float smoothProgress = progress * progress * (3f - 2f * progress);

            float currentValue = Mathf.Lerp(startValue, targetValue, smoothProgress);
            doughMesh.SetBlendShapeWeight(index, currentValue);

            yield return null; // Wait for the next frame
        }

        // Snap precisely to the final value at the end
        doughMesh.SetBlendShapeWeight(index, targetValue);
    }


    public void StartTextureBlend(float targetBlend, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(BlendRoutine(targetBlend, duration));
    }

    private IEnumerator BlendRoutine(float targetValue, float duration)
    {
        float timeElapsed = 0f;
        float startValue = targetMaterial.GetFloat("_BlendAmount");

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            // Calculates an exponential ease curve for natural fading
            float currentBlend = Mathf.Lerp(startValue, targetValue, t);
            
            // Sends the current progress value directly to the Shader Graph
            targetMaterial.SetFloat("_BlendAmount", currentBlend);

            yield return null;
        }

        targetMaterial.SetFloat("_BlendAmount", targetValue);
    }
}
