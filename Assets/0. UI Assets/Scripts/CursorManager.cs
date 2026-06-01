using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    private RectTransform cursorRectTransform;
    private Image cursorImage;
    [SerializeField] private GameObject CursorImageObject;

    [Header("Size Settings")]
    // Set your desired width and height values here (e.g., 32x32 or 64x64)
    [SerializeField] private Vector2 normalSize = new Vector2(32f, 32f);
    [SerializeField] private Vector2 leftClickSize = new Vector2(32f, 32f); // Slightly smaller when squeezing dough

    [Header("Sprites")]
    [SerializeField] private int spriteSetNo = 0;
    [SerializeField] private Sprite[] defaultSprite;
    [SerializeField] private Sprite[] leftClickSprite;

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
        cursorRectTransform.position = Mouse.current.position.ReadValue();


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
}
