using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    [Header("Bouncy Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float animationSpeed = 15f;

    private Vector3 targetScale;
    private Vector3 originalScale;

    [SerializeField] private CursorManager cursorManager;
    [SerializeField] private bool enableAutoAdd = true;
    private Button button;

    void Awake()
    {
        
    }

    private void Start()
    {
        cursorManager = UnityEngine.Object.FindFirstObjectByType<CursorManager>();
        // Store the starting scale of the button
        originalScale = transform.localScale;
        targetScale = originalScale;

        if (!enableAutoAdd) return;
        
        button = GetComponent<Button>();
        
        // 1. Force remove it first (prevents duplicates if Awake/Start runs twice)
        button.onClick.RemoveListener(OnClickPlaySound);
        
        // 2. Safely add it exactly once
        button.onClick.AddListener(OnClickPlaySound);
    }

    private void Update()
    {
        // Smoothly lerp to the target scale every frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up slightly when cursor hovers
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Return to normal scale when cursor leaves
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Squash down when clicked
        targetScale = originalScale * pressScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to hover scale upon release if still hovering
        targetScale = eventData.pointerCurrentRaycast.gameObject == gameObject 
            ? originalScale * hoverScale 
            : originalScale;
    }
    
    public void OnClickPlaySound()
    {
        AudioManager.Instance.PlaySFX("Click");
    }

    public void SetHoverText(String text)
    {
        cursorManager.SetPopupBox(text);
    }

    public void EnablePopupBox(bool enabled){ cursorManager.EnablePopupBox(enabled);}
    public void SetPopupBoxDirection(String direction) {cursorManager.SetPopupBoxPosition(direction);}

    public void GoToScene (String scene) { SceneTransitioner.Instance.TriggerTransition(scene); }


}
