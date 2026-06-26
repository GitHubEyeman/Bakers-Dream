using System.Collections;
using UnityEngine;

public class OvenDoor : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private float openAngle = 90f;
    
    [Header("Opening Durations")]
    [SerializeField] private float dropDuration = 0.3f;
    [SerializeField] private float bounceDuration = 0.7f;

    [Header("Closing Settings")]
    [SerializeField] private float closeDuration = 0.5f;

    [Header("Bounce Settings (Opening Only)")]
    [SerializeField] private int bounceCount = 3;
    [SerializeField] private float bounceAmplitude = 12f;

    public bool isOpen = false;
    private Coroutine animationCoroutine;
    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    [ContextMenu("Toggle Door")]
    public void ToggleDoor()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        isOpen = !isOpen;
        animationCoroutine = StartCoroutine(isOpen ? OpenDoorSequence() : CloseDoorSequence());
    }

    private IEnumerator OpenDoorSequence()
    {
        // PHASE 1: Fast Drop to Open Angle
        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dropDuration);
            
            // Fast drop curve (Ease-in)
            float curveT = t * t; 
            float currentAngle = Mathf.Lerp(closedAngle, openAngle, curveT);
            
            SetXRotation(currentAngle);
            yield return null;
        }

        // PHASE 2: Decay Bounce at Open Angle
        elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bounceDuration);

            float bounceDecay = Mathf.Sin(t * Mathf.PI * (bounceCount * 2)) * bounceAmplitude * (1f - t);
            float currentAngle = openAngle + bounceDecay;

            SetXRotation(currentAngle);
            yield return null;
        }

        SetXRotation(openAngle);
        animationCoroutine = null;
    }

    private IEnumerator CloseDoorSequence()
    {
        float elapsed = 0f;

        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / closeDuration);
            
            // Math for Ease-Out Quadratic (starts fast, slows down at the end)
            float easeOutT = 1f - (1f - t) * (1f - t);
            float currentAngle = Mathf.Lerp(openAngle, closedAngle, easeOutT);

            SetXRotation(currentAngle);
            yield return null;
        }

        SetXRotation(closedAngle);
        animationCoroutine = null;
    }

    private void SetXRotation(float angle)
    {
        transform.localRotation = initialRotation * Quaternion.Euler(angle, 0f, 0f);
    }
}
