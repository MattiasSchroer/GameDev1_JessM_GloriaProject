using UnityEngine;

public class NoteController : MonoBehaviour//THIS class should be controlling if you take damage or not
{
    public float TargetHitTime { get; private set; }
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Timing Variables")]
    [SerializeField] private float PreBeatsVisualTime = 500;//FEEBACK I swear to god with the capitalization 
    [SerializeField] private float MsTilDestruction = 300;

    [Header("Scale Boundaries")]
    [SerializeField] private float startingScaleRatio = 4.0f;
    [SerializeField] private float startingScale = 4.0f;
    [SerializeField] private float targetScale = 1.0f;

    [Header("Fade Boundaries")]
    [SerializeField] private float maxOpacity = 0.5f;
    [SerializeField] private float fadeDurationProgress = 0.2f;

    private void Start()
    {
        GameManager.EndBattle += Cancel;
        PlayerStatus.OnPlayerHealthEmpty += Cancel;
        EnemyEncounter.OnEnemyHealthEmpty += Cancel;
    }

    public void Initialize(float hitTime, Note note)
    {
        TargetHitTime = hitTime;
        transform.position = GameManager.Instance.IndicatorManager.GetIndicatorPosition(note);
        targetScale = GameManager.Instance.IndicatorManager.GetIndicatorScale(note).x;
        startingScale = targetScale * startingScaleRatio;
        Color color = Color.black;//spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
    }

    public float GetProgress()
    {
        float timeRemaining = TargetHitTime - GameManager.Instance.Music.SongTimeInMilliseconds();

        if (timeRemaining <= -MsTilDestruction) Destroy(gameObject);

        float progress = 1f - (timeRemaining / PreBeatsVisualTime);

        return Mathf.Clamp01(progress);
    }

    void Update()
    {
        float currentProgress = GetProgress();

        Color currentColor = spriteRenderer.color;

        if (currentProgress <= fadeDurationProgress)
        {
            float fadeProgress = currentProgress / fadeDurationProgress;
            currentColor.a = Mathf.Lerp(0f, maxOpacity, fadeProgress);
        }
        else if (TargetHitTime - GameManager.Instance.Music.SongTimeInMilliseconds() < GameManager.Instance.Metronome.TimingWindowMargin)
        {
            //FEEDBACK I added this to change the colour of the circle when the beat is in the timing window, both to help visualise where that window as a dev and player
            currentColor = Color.white;
            currentColor.a = maxOpacity;
        }

        spriteRenderer.color = currentColor;

        float currentScale = Mathf.Lerp(startingScale, targetScale, currentProgress);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }

    private void Cancel()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.EndBattle -= Cancel;
        PlayerStatus.OnPlayerHealthEmpty -= Cancel;
        EnemyEncounter.OnEnemyHealthEmpty -= Cancel;
    }
}
