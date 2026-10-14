using System;
using UnityEngine;

public class Choreographer : MonoBehaviour
{
    private int[] pattern;
    [SerializeField] private int NextDemoBeat;
    [SerializeField] private int NextListenBeat;
    [SerializeField] private int ActiveBeat;
    private int CurrentPatternBeat = 0;
    public ChoreographerState State = ChoreographerState.Waiting;
    public int BeatsBeforeSetting = 4;

    [SerializeField] private bool DisableBlanks = false;
    [SerializeField] private bool LastSequenceNoteIsGo = false;
    [SerializeField] private bool DebugMessages = false;

    public static event Action<int> OnBeatDemo;
    public static event Action<int> OnBeatListen;
    public static event Action OnSwitchToDemoState;
    public static event Action OnSwitchToListenState;
    public static event Action OnEndListenPhase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetValues();
    }

    public void ResetValues() 
    {
        State = ChoreographerState.Waiting;
        pattern = new int[GameManager.Instance.Metronome.BeatsInBar];
        ActiveBeat = 0;
        NextDemoBeat = GameManager.Instance.Metronome.BeatsInBar + 1;
        NextListenBeat = NextDemoBeat + GameManager.Instance.Metronome.BeatsInBar;
        CurrentPatternBeat = 0;
    }

    public void ResetValues(int firstBeat)
    {
        State = ChoreographerState.Waiting;
        pattern = new int[GameManager.Instance.Metronome.BeatsInBar];
        ActiveBeat = 0;
        NextDemoBeat = firstBeat + 1;
        NextListenBeat = NextDemoBeat + GameManager.Instance.Metronome.BeatsInBar;
        CurrentPatternBeat = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Metronome.GetActiveBeat() != -1) ActiveBeat = GameManager.Instance.Metronome.GetActiveBeat();

        if (State == ChoreographerState.Waiting) 
        {
            if (ActiveBeat >= NextDemoBeat - BeatsBeforeSetting) State = ChoreographerState.Setting;
        }
        else if (State == ChoreographerState.Setting)
        {
            SetNextPattern();
            State = ChoreographerState.Demonstrating;
            OnSwitchToDemoState?.Invoke();
        }
        else if (State == ChoreographerState.Demonstrating)
        {
            if (ActiveBeat == NextDemoBeat)
            {
                OnBeatListen.Invoke(0);

                // Demo the beat using pattern[CurrentPatternBeat]
                OnBeatDemo.Invoke(pattern[CurrentPatternBeat]);
                if (DebugMessages) Debug.Log($"Demo: {pattern[CurrentPatternBeat]} (Bar {CurrentPatternBeat + 1}/{pattern.Length}");

                NextDemoBeat++;
                CurrentPatternBeat++;
            }
            if (NextDemoBeat == NextListenBeat)
            {
                NextDemoBeat += GameManager.Instance.Metronome.BeatsInBar;
                CurrentPatternBeat = 0;
                State = ChoreographerState.Listening;
                OnSwitchToListenState?.Invoke();
            }
        }
        else if (State == ChoreographerState.Listening)
        {
            if (ActiveBeat == NextListenBeat)
            {
                OnBeatDemo.Invoke(-1);

                // Tell judge to listen for pattern[CurrentBeatPattern]
                OnBeatListen.Invoke(pattern[CurrentPatternBeat]);
                if (DebugMessages) Debug.Log($"Listening for: {pattern[CurrentPatternBeat]} (Bar {CurrentPatternBeat + 1}/{pattern.Length}");

                NextListenBeat++;
                CurrentPatternBeat++;
            }
            if (NextListenBeat == NextDemoBeat)
            {
                NextListenBeat += GameManager.Instance.Metronome.BeatsInBar;
                CurrentPatternBeat = 0;
                State = ChoreographerState.Setting;
                OnEndListenPhase?.Invoke();
            }
        }
    }

    public void SetNextPattern()
    {
        int minRange = 0;
        if (DisableBlanks) minRange = 1;

        for (int i = 0; i < pattern.Length; i++)
        {
            int note = UnityEngine.Random.Range(minRange, 5);
            pattern[i] = note;
        }

        pattern[0] = 2;

        if(LastSequenceNoteIsGo) pattern[pattern.Length - 1] = 7;

        if (DebugMessages) Debug.Log($"Next pattern: {string.Join(", ", pattern)}");
    }
}

public enum ChoreographerState 
{
    Setting,
    Waiting,
    Demonstrating,
    Listening
}
