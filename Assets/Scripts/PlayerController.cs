using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Animation Details")]
    public Sprite DefaultSprite;
    public List<NoteActionDetails> NoteActions;//FEEDBACK All Variable Names should be lower case
    [SerializeField] private float spriteAnimationTime = 0.4f;
    [SerializeField] private bool matchAnimationTimerToTimingWindow = false;
    [SerializeField] private float spriteAnimationSlideDistance;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Vector2 baseScale;//FEEDBACK This variable is never used
    private Vector2 basePosition;

    private float spriteTimer = 0f;
    private bool spriteTimerActive = false;
    private Vector2 slideDirection;

    public static event Action<int> OnPlayerAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        baseScale = transform.localScale;
        basePosition = transform.localPosition;

        EnemyEncounter.SendEncounterDetails += GetSongDetails;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        spriteTimer -= delta;

        if (spriteTimer > 0f)
        {
            float distancePercentage = spriteTimer / spriteAnimationTime;
            transform.localPosition = basePosition + slideDirection * (Mathf.Sin(distancePercentage * Mathf.PI) * spriteAnimationSlideDistance);
        }
        else if (spriteTimerActive && spriteTimer < 0) ResetSprite();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (spriteTimerActive) return;

        Note direction = Note.Blank;
        Vector2 inputDir = context.ReadValue<Vector2>().normalized;

        if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y))
        {
            //FEEDBACK :(
            if (inputDir.x > 0) direction = Note.Right;
            else direction = Note.Left;
            //FEEDBACK This can simplified to:
            // direction = inputDir.x > 0 ? Note.Right : Note.Left;
        }
        else 
        {
            if (inputDir.y > 0) direction = Note.Up;
            else direction = Note.Down;
            //FEEDBACK This can simplified to:
            // direction = inputDir.y > 0 ? Note.Up : Note.Down;
        }

        foreach (NoteActionDetails noteAction in NoteActions)
        {
            if (direction != noteAction.Note) continue;

            SetSlideDirection(noteAction.Note);
            if (noteAction.Sprite != null) SetSpriteWithTimer(noteAction.Sprite, spriteAnimationTime);
            if (noteAction.Sound != null) audioSource.PlayOneShot(noteAction.Sound);
        }

        OnPlayerAction?.Invoke((int)direction);
    }

    private void SetSpriteWithTimer(Sprite sprite, float time)
    {
        spriteRenderer.sprite = sprite;
        spriteTimer = time;
        spriteTimerActive = true;
    }

    private void SetSlideDirection(Note note)
    {
        switch (note)
        {
            case Note.Up: slideDirection = Vector2.up; break;
            case Note.Down: slideDirection = Vector2.down; break;
            case Note.Left: slideDirection = Vector2.left; break;
            case Note.Right: slideDirection = Vector2.right; break;
            case Note.Finish: slideDirection = Vector2.zero; break;
        }
    }//FEEDBACK Whitespace
    private void ResetSprite()
    {
        spriteRenderer.sprite = DefaultSprite;
        spriteTimerActive = false;
        transform.localPosition = basePosition;
    }

    private void GetSongDetails(Track track, float timingWindow) //FEEDBACK Avoid using "Get" in method names for void methods
    {
        if (matchAnimationTimerToTimingWindow) spriteAnimationTime = (timingWindow * 2) / 1000;
    }
}
