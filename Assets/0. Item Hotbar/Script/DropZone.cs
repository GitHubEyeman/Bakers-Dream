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
    private String currentDough;


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

    public void SetCurrentDough(String dough)
    {
        currentDough = dough;
        Debug.Log("CURRENT DOUGH: "+currentDough);
    }
    public void GoToScene(String scene)
    {
        
    }
    
    public void FinishedKneading()
    {
        Debug.Log("CURRENTDOUGH   "+currentDough.ToLowerInvariant());
        switch (currentDough.ToLowerInvariant())
                {
                    case "cavedindough":
                        Debug.Log("cavedinbread");
                        SaveManager.Instance.CurrentSave.AddToInventory("CavedInDough2");

                        break;
                        
                    case "flatbrickdough":
                        Debug.Log("flatbread");
                        SaveManager.Instance.CurrentSave.AddToInventory("FlatBrickDough2");
                        break;
                                                
                    case "gdough":
                        Debug.Log("goldenbread");
                        SaveManager.Instance.CurrentSave.AddToInventory("GDough2");
                        break;
                                                
                    case "overcookeddough":
                        Debug.Log("overcookedbread");
                        SaveManager.Instance.CurrentSave.AddToInventory("OverCookedDough2");
                        break;
                                                
                    case "undercookeddough":
                        Debug.Log("undercookedbread");
                        SaveManager.Instance.CurrentSave.AddToInventory("UnderCookedDough2");
                        break;
                        
                    default:
                        Debug.Log("Item not recognized.");
                        SaveManager.Instance.CurrentSave.AddToInventory("CavedInDough2");
                        break;
                }

        SceneTransitioner.Instance.TriggerTransition("3. Baking");
    }
}
