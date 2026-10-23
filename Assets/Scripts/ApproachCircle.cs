using UnityEngine;

public class ApproachCircle : MonoBehaviour
{
    [SerializeField] private NoteController noteController;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Scale Boundaries")]
    [SerializeField] private float startingScale = 4.0f;
    [SerializeField] private float targetScale = 1.0f;

    [Header("Fade Boundaries")]
    [SerializeField] private float maxOpacity = 0.5f;
    [SerializeField] private float fadeDurationProgress = 0.2f;

    void Update()
    {
        float currentProgress = noteController.GetProgress();

        Color currentColor = spriteRenderer.color;

        if (currentProgress <= fadeDurationProgress)
        {
            float fadeProgress = currentProgress / fadeDurationProgress;
            currentColor.a = Mathf.Lerp(0f, maxOpacity, fadeProgress);
        }
        else
        {
            currentColor.a = maxOpacity;
        }

        spriteRenderer.color = currentColor;

        float currentScale = Mathf.Lerp(startingScale, targetScale, currentProgress);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
}
