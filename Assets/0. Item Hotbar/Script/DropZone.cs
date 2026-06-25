using System;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    public Transform centerPoint; // Assign a Transform marking the exact center floor placement
    [SerializeField] private ParticleSystem impactParticlePrefab;
    [SerializeField] private HotbarManager hotbarManager;
    
    [Header("Settings")]
    [SerializeField] private bool enableCustomAnimationSettings = true;
    [SerializeField] private float startHeightOffset = 1.0f;
    [SerializeField] private float scaleDuration = 0.4f;
    [SerializeField] private float dropDuration = 0.5f;
    [SerializeField] private float bounceHeight = 0.4f;
    [SerializeField] private float bounceDuration = 0.3f;

    public ParticleSystem ImpactParticlePrefab => impactParticlePrefab;
    public bool EnableCustomAnimationSettings => enableCustomAnimationSettings;


    public virtual void SpawnIngredient(GameObject prefab)
    {
        
        if (prefab == null) return;
        
        // Target spot is centerPoint position; if unassigned, use this object's center
        Vector3 spawnTarget = centerPoint != null ? centerPoint.position : transform.position;

        GameObject spawnedItem = Instantiate(prefab, spawnTarget, Quaternion.identity, transform);
        
        // Inject the animation behavior script dynamically
        ItemDropAnimator animator = spawnedItem.AddComponent<ItemDropAnimator>();
        animator.impactParticlePrefab = impactParticlePrefab;
        SetAnimatorSettings(animator,enableCustomAnimationSettings);
        animator.StartAnimation(spawnTarget);
        
        
    }
    public void RemoveItemFromHotbar(String itemName)
    {
        hotbarManager.RemoveItemFromHotbar(itemName);
    }

    public void SetAnimatorSettings(ItemDropAnimator animator, bool enable)
    {
        if (enable)
        {
            animator.startHeightOffset = startHeightOffset;
            animator.scaleDuration = scaleDuration;
            animator.dropDuration = dropDuration;
            animator.bounceHeight = bounceHeight;
            animator.bounceDuration = bounceDuration;
        }
    }

    
}
