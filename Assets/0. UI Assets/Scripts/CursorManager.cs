using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class CursorManager : MonoBehaviour
{
    private RectTransform cursorRectTransform;
    private Image cursorImage;
    [SerializeField] private GameObject CursorImageObject;

    [Header("Size Settings")]
    [Tooltip("Adjusts the cursor image position relative to the actual mouse hotspot. (e.g., (-16, 16) to center a 32x32 sprite)")]
    [SerializeField] private Vector2 cursorOffset = new Vector2(15f, -35f);
    // Set your desired width and height values here (e.g., 32x32 or 64x64)
    [SerializeField] private Vector2 normalSize = new Vector2(32f, 32f);
    [SerializeField] private Vector2 leftClickSize = new Vector2(32f, 32f); // Slightly smaller when squeezing dough

    [Header("Sprites")]
    [SerializeField] private int spriteSetNo = 0;
    [SerializeField] private Sprite[] defaultSprite;
    [SerializeField] private Sprite[] leftClickSprite;

    [Header("Popup Box Settings")]
    [SerializeField] private GameObject contentBox;
    [SerializeField] private TextMeshProUGUI popupText;
    

    public int SpriteSetNo {get => spriteSetNo; set => spriteSetNo = value;}

    void Start()
    {
        CursorImageObject.SetActive(true);
        cursorRectTransform = CursorImageObject.GetComponent<RectTransform>();
        cursorImage = CursorImageObject.GetComponent<Image>();

        // Set the initial scale via code
        cursorRectTransform.sizeDelta = normalSize;
        Cursor.visible = false; 
    }

    void Update()
    {
        if (Mouse.current == null) return;
        
        // Match mouse coordinates smoothly
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        cursorRectTransform.position = mousePosition + cursorOffset;

        if (Mouse.current.leftButton.isPressed) SetGrabState(true);
        else SetGrabState(false);
        
    }

    public void SetGrabState(bool isDragging)
    {
        Cursor.visible = false;

        cursorImage.sprite = isDragging ? leftClickSprite[spriteSetNo] : defaultSprite[spriteSetNo];
        
        // Instantly switch sizes depending on the state
        cursorRectTransform.sizeDelta = isDragging ? leftClickSize : normalSize;
    }

    void OnDisable()
    {
        Cursor.visible = true; 
    }

    public void SetPopupBox(String text)
    {
        popupText.SetText(text);
    }

    public void EnablePopupBox(bool isEnabled)
    {
        // Debug.Log("Enabled: "+ isEnabled);
        contentBox.SetActive(isEnabled);
        SetPopupBoxPosition("none");
    }
    public void EnablePopupBox(bool isEnabled, String direction)
    {
        contentBox.SetActive(isEnabled);
        SetPopupBoxPosition(direction);
    }

    public void SetPopupBoxPosition (String direction)
    {
        Vector3 newPosition = contentBox.transform.localPosition;
        newPosition.x = 119f;
        newPosition.y = -15f;

        if (string.Equals(direction, "left", StringComparison.OrdinalIgnoreCase))
        {
            newPosition.x = -119f;
        }
        if (string.Equals(direction, "up", StringComparison.OrdinalIgnoreCase))
        {
            newPosition.y = 140f;
        }
        if (string.Equals(direction, "upleft", StringComparison.OrdinalIgnoreCase))
        {
            newPosition.x = -119f;
            newPosition.y = 140f;
        }

        contentBox.transform.localPosition = newPosition;
    }
}
