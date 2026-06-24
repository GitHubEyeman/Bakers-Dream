using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlourSprinkle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private ParticleSystem flourParticleSystem;
    [SerializeField] private Renderer tableFlourRenderer; // The flat plane overlaying the table

    [Header("Flour Settings")]
    [SerializeField] private float maxFlourCapacity = 100f;
    [SerializeField] private float accumulationRate = 20f;
    [SerializeField] private float particleEmissionRate = 50f;

    [SerializeField] private float currentFlourAmount = 0f;
    
    private bool isSprinkling = false;
    private ParticleSystem.EmissionModule emissionModule;
    private Material flourMaterial;


    [Header("Flour Bonus Settings")]
    [SerializeField] private float flourConsumptionPerCycle = 10f; // how much flour is used per knead
    [SerializeField] private float lowFlourThreshold = 20f;       // below this, no bonus
    [SerializeField] private float mediumFlourThreshold = 40f;    // above this, better bonus
    [SerializeField] private float lowBonusMultiplier = 0.15f;    // bonus as fraction of base reduction
    [SerializeField] private float highBonusMultiplier = 0.30f;




    void Start()
    {
        // if (ParticleObject != null) startPosition = ParticleObject.transform.position;

        if (flourParticleSystem != null)
        {
            emissionModule = flourParticleSystem.emission;
        }

        // Get the material instance so we can change its transparency safely at runtime
        if (tableFlourRenderer != null)
        {
            flourMaterial = tableFlourRenderer.material;
            UpdateVisualPile();
        }
    }

    void Update()
    {
        // if (enableSprinkleAnimation) PlaySprinkleAnim();
        if (isSprinkling)
        {
            if (currentFlourAmount < maxFlourCapacity)
            {
                currentFlourAmount += accumulationRate * Time.deltaTime;
                currentFlourAmount = Mathf.Min(currentFlourAmount, maxFlourCapacity);
                UpdateVisualPile();
            }
            else
            {
                // Auto-stop if the table is completely full of flour
                StopSprinkling();
            }
        }
    }

    // Called smoothly during update to fade the flour texture in
    private void UpdateVisualPile()
    {
        if (flourMaterial != null)
        {
            float percentage = currentFlourAmount / maxFlourCapacity;
            
            // Gets the current color, modifies the Alpha (A), and re-assigns it
            Color color = flourMaterial.color;
            color.a = Mathf.Clamp(percentage, 0, 0.7f); 
            flourMaterial.color = color;
        }
    }

    // UI BUTTON FUNCTIONS (Public so the Event Trigger component can see them)
    public void StartSprinkling()
    {
        if (debugMode) Debug.Log("Ran StartSprinkling()!");
        if (currentFlourAmount >= maxFlourCapacity) return;

        isSprinkling = true;
        emissionModule.rateOverTime = particleEmissionRate;
        
        if (!flourParticleSystem.isPlaying)
        {
            flourParticleSystem.Play();
        }
    }

    public void StopSprinkling()
    {
        if (debugMode) Debug.Log("Ran StopSprinkling()!");
        isSprinkling = false;
        emissionModule.rateOverTime = 0f;
    }

    public float GetCurrentFlourAmount()
    {
        return currentFlourAmount;
    }
    public void SetCurrentFlourAmount(float value)
    {
        currentFlourAmount = value;
    }
    public void AddCurrentFlourAmount(float value)
    {
        currentFlourAmount = Mathf.Clamp(currentFlourAmount + value, 0f, maxFlourCapacity);
        UpdateVisualPile(); // ensure visual updates after changes
    }

    public float GetFlourBonusAndConsume(float baseReduction)
    {
        if (currentFlourAmount < lowFlourThreshold)
            return 0f;

        float bonusMultiplier;
        if (currentFlourAmount >= mediumFlourThreshold)
            bonusMultiplier = highBonusMultiplier;
        else
            bonusMultiplier = lowBonusMultiplier;

        float bonus = baseReduction * bonusMultiplier;

        // Consume flour (but not below zero)
        currentFlourAmount = Mathf.Max(currentFlourAmount - flourConsumptionPerCycle, 0f);
        UpdateVisualPile();

        return bonus;
    }
}
