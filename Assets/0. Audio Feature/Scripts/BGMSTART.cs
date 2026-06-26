using UnityEngine;

public class BGMSTART : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic("BGM");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
