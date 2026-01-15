using UnityEngine;
using UnityEngine.UI;

public class RowPixel : MonoBehaviour
{
    [SerializeField] private Image[] bitrow;

    private KeyCode[] keys =
{
        KeyCode.W,
        KeyCode.A,
        KeyCode.UpArrow,
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
        KeyCode.Space
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            bitrow[i].color = Input.GetKey(keys[i])
                ? Color.black
                : Color.white;
        }
    }
}
