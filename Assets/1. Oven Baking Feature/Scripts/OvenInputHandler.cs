using UnityEngine;
using UnityEngine.InputSystem;

public class OvenInputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OvenController ovenController;
    [SerializeField] private OvenUIController ovenUI;
    [SerializeField] private Camera mainCamera;
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private GameObject hoverHighlight;
    
    private bool isHovering = false;
    
    private void Start()
    {
        if (ovenController == null)
            ovenController = GetComponent<OvenController>();
        
        if (ovenUI == null)
            ovenUI = FindFirstObjectByType<OvenUIController>(); // Updated to FindFirstObjectByType
        
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (hoverHighlight != null)
            hoverHighlight.SetActive(false);
    }
    
    private void Update()
    {
        // Check for hover
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            
            bool isHit = Physics.Raycast(ray, out hit, interactionRange, interactionLayer);
            bool hitThis = isHit && hit.collider.gameObject == gameObject;
            
            if (hitThis && !isHovering)
            {
                isHovering = true;
                if (hoverHighlight != null)
                    hoverHighlight.SetActive(true);
            }
            else if (!hitThis && isHovering)
            {
                isHovering = false;
                if (hoverHighlight != null)
                    hoverHighlight.SetActive(false);
            }
            
            // Click to interact
            if (Mouse.current.leftButton.wasPressedThisFrame && hitThis)
            {
                OnOvenClick();
            }
        }
    }
    
    private void OnOvenClick()
    {
        if (ovenUI != null)
        {
            ovenUI.ToggleOvenPanel();
        }
        else
        {
            // Fallback: toggle baking directly
            if (ovenController != null)
            {
                ovenController.ToggleBaking();
            }
        }
    }
}