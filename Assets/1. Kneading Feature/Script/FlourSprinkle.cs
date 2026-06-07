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




    // [Header("Sprinkle Movement Animation Settings")]
    
    // public bool enableSprinkleAnimation = true;

    // [Tooltip("The GameObject to be moved in 8 shape.")]
    // [SerializeField] GameObject ParticleObject;

    // [Tooltip("Speed of the movement.")]
    // public float speed = 2.0f;

    // [Tooltip("Width of the figure-8 loop (X-axis).")]
    // public float width = 5.0f;

    // [Tooltip("Height of the figure-8 loop (Z-axis). Use 'height' on Y-axis for 2D.")]
    // public float height = 3.0f;

    // [Header("Axis Orientation")]
    // [Tooltip("True for a horizontal 3D plane (X and Z). False for a 2D plane (X and Y).")]
    // public bool moveOnXZPlane = true;

    // private Vector3 startPosition;
    // private float timer = 0.0f;




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
        currentFlourAmount = Mathf.Clamp(currentFlourAmount+value,0,1);
    }

    //Sprinkle Movement Stuff
    // private void PlaySprinkleAnim()
    // {
    //     // Advance time independently of the frame rate
    //     timer += Time.deltaTime * speed;

    //     // Parametric equations for a figure-8 curve
    //     float x = Mathf.Sin(timer) * width;
    //     float orthogonalAxis = Mathf.Sin(timer) * Mathf.Cos(timer) * height;

    //     // Apply calculated values based on desired orientation
    //     if (moveOnXZPlane)
    //     {
    //         // 3D Plane: Horizontal movement across X and Z
    //         ParticleObject.transform.position = startPosition + new Vector3(x, 0.0f, orthogonalAxis);
    //     }
    //     else
    //     {
    //         // 2D Plane: Vertical movement across X and Y
    //         ParticleObject.transform.position = startPosition + new Vector3(x, orthogonalAxis, 0.0f);
    //     }
    // }
}
