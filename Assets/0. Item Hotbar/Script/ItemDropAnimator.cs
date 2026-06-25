using UnityEngine;

public class ItemDropAnimator : MonoBehaviour
{
    [Header("Settings")]
    public float startHeightOffset = 1.0f;
    public float scaleDuration = 0.4f;
    public float dropDuration = 0.5f;
    public float bounceHeight = 0.4f;
    public float bounceDuration = 0.3f;

    [Header("Effects")]
    [Tooltip("The particle system prefab to spawn when hitting the floor.")]
    public ParticleSystem impactParticlePrefab;
    [Tooltip("Destroy the spawned particle system after this many seconds to prevent memory leaks.")]
    public float particleDestroyDelay = 2.0f;

    private Vector3 targetFloorPos;

    public void StartAnimation(Vector3 targetFloor)
    {
        targetFloorPos = targetFloor;
        StartCoroutine(AnimateSequence());
    }

    private System.Collections.IEnumerator AnimateSequence()
    {
        // Phase 1: Initialize values above target point at scale 0
        transform.position = targetFloorPos + Vector3.up * startHeightOffset;
        transform.localScale = Vector3.zero;

        // Phase 2: Scale up with a bouncy effect (Overshoot)
        float elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            // Back/Overshoot curve calculation
            float scaleValue = AnimateOvershoot(t);
            transform.localScale = Vector3.one * scaleValue;
            yield return null;
        }
        transform.localScale = Vector3.one;

        // Phase 3: Drop down to the center point floor
        elapsed = 0;
        Vector3 startDropPos = transform.position;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropDuration;
            // Use acceleration ease-in for gravity feel
            transform.position = Vector3.Lerp(startDropPos, targetFloorPos, t * t);
            yield return null;
        }
        transform.position = targetFloorPos;

        // TRIGGER: Spawn impact particles exactly when hitting the ground
        SpawnImpactEffects();

        // Phase 4: Ground Impact Bounce Up, Twist, and Return to Zero Rotation
        elapsed = 0;
        Quaternion startRot = transform.rotation;
        
        // Target mid-bounce peak rotation twist
        Quaternion peakRot = startRot * Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(-45f, 45f), Random.Range(-15f, 15f));
        
        // Set ultimate end target rotation to exactly 0,0,0
        Quaternion endRot = Quaternion.identity; 

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            
            // Sine wave creates a clean parabolic bounce arch shape
            float height = Mathf.Sin(t * Mathf.PI) * bounceHeight;
            transform.position = targetFloorPos + Vector3.up * height;
            
            // Twists toward peakRot on the way up (t < 0.5), settles to flat endRot on the way down
            if (t < 0.5f)
            {
                transform.rotation = Quaternion.Slerp(startRot, peakRot, t * 2f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(peakRot, endRot, (t - 0.5f) * 2f);
            }
            
            yield return null;
        }
        
        // Finalize exact snap placement
        transform.position = targetFloorPos;
        transform.rotation = Quaternion.identity; // Hard snap absolute rotation to (0,0,0)
        Destroy(this); // Clean up the component helper when animation finishes
    }

    private void SpawnImpactEffects()
    {
        if (impactParticlePrefab != null)
        {
            // Spawn particle system flat on the ground at the impact coordinate
            ParticleSystem vfxInstance = Instantiate(impactParticlePrefab, targetFloorPos, Quaternion.identity, transform);
            vfxInstance.gameObject.transform.rotation = Quaternion.Euler(-90f,0,0);
            vfxInstance.gameObject.transform.localPosition = vfxInstance.gameObject.transform.localPosition + new Vector3(0,0.2f,0);
            vfxInstance.gameObject.SetActive(true);
            // Ensure the particles play immediately
            vfxInstance.Play();
            
            // Self-destruct the particle instance to clear memory
            Destroy(vfxInstance.gameObject, particleDestroyDelay);
        }
    }

    private float AnimateOvershoot(float t)
    {
        // Mathematical formula simulating a bouncy back-out interpolation curve
        float c1 = 1.70158f;
        float c3 = c1 + 1.0f;
        return 1.0f + c3 * Mathf.Pow(t - 1.0f, 3.0f) + c1 * Mathf.Pow(t - 1.0f, 2.0f);
    }
}
