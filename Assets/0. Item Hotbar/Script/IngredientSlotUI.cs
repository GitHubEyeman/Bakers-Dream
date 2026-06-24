using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image iconImage;
    [HideInInspector] public IngredientsData data;

    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Setup(IngredientsData ingredientData)
    {
        data = ingredientData;
        iconImage.sprite = ingredientData.itemSprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // Move to canvas root so it renders on top of other UI elements during drag
        transform.SetParent(parentCanvas.transform, true); 
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Allows drop zone to detect mouse
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow mouse pointer precisely
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            DropZone dropZone = hit.collider.GetComponent<DropZone>();
            if (dropZone != null)
            {
                dropZone.RemoveItemFromHotbar(data.ingredientName);
                dropZone.SpawnIngredient(data.itemPrefab);
                Destroy(gameObject);   // Remove the dragged slot
                return;                // Skip the return-to-panel code
            }
        }

        // If not dropped on a valid zone, return it to its original position
        transform.SetParent(originalParent, false);
        rectTransform.anchoredPosition = originalPosition;
    }
}
