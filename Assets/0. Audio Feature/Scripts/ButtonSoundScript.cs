using UnityEngine;

public class ButtonSoundScript : MonoBehaviour
{
    public void OnClickPlaySound()
    {
        AudioManager.Instance.PlaySFX("Click");
    }
}
