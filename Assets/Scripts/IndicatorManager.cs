using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    public GameObject LeftIndicator;
    public GameObject RightIndicator;
    public GameObject UpIndicator;
    public GameObject DownIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetIndicatorPosition(Note note) 
    {
        switch (note) 
        {
            case Note.Left:
                return LeftIndicator.transform.position;
            case Note.Right:
                return RightIndicator.transform.position;
            case Note.Up:
                return UpIndicator.transform.position;
            case Note.Down:
                return DownIndicator.transform.position;
            default:
                return transform.position;
        }
    }
}
