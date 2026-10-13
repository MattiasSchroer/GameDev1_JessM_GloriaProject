using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI proximityText;
    [SerializeField] private float proximityTextDisplayTime = 0.5f;
    private float proxTextTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        proximityText.color = new Color(0, 0, 0, 0);

        Judge.OnValidateInput += SetProximityText;
    }

    // Update is called once per frame
    void Update()
    {
        proxTextTimer -= Time.deltaTime;

        Color proxTextColor = proximityText.color;

        proxTextColor.a = Mathf.Clamp01(proxTextTimer / (proximityTextDisplayTime / 2));

        proximityText.color = proxTextColor;
    }

    private void SetProximityText(bool success) 
    {
        string text = "ERROR";
        Color color = Color.white;

        if (!success)
        {
            text = "Ouch!";
            color = Color.red;
        }
        else 
        {
            float proximity = GameManager.Instance.Metronome.GetProximityToNearestBeat();

            if (Mathf.Abs(proximity) < 10)
            {
                text = "Perfect!";
                color = Color.yellow;
            }
            else if (Mathf.Abs(proximity) < 45)
            {
                text = "Nice";
                color = Color.green;
            }
            else 
            {
                text = "Okay...";
                color = Color.blue;
            }
        }

        proximityText.text = text;
        proximityText.color = color;
        proxTextTimer = proximityTextDisplayTime;
    }
}
