using UnityEngine;

public class NoteController : MonoBehaviour
{
    public float TargetHitTime { get; private set; }
    [SerializeField] private float PreBeatsVisualTime;

    [SerializeField] private float MsTilDestruction = 300;

    public void Initialize(float hitTime, Note note)
    {
        TargetHitTime = hitTime;
        transform.position = GameManager.Instance.IndicatorManager.GetIndicatorPosition(note);
    }

    public float GetProgress()
    {
        float timeRemaining = TargetHitTime - GameManager.Instance.Music.SongTimeInMilliseconds();

        float progress = 1f - (timeRemaining / PreBeatsVisualTime);

        Debug.Log("Time remaining: " + timeRemaining + " | Progress: " + progress);

        return Mathf.Clamp01(progress);
    }

    private void Update()
    {
        float timeRemaining = TargetHitTime - GameManager.Instance.Music.SongTimeInMilliseconds();

        if (timeRemaining <= -MsTilDestruction) Destroy(gameObject);
    }
}
