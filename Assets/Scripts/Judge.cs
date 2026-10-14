using System;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private int NextCorrectInput = 0;
    [SerializeField] private int LastReceivedInput = 0;
    private bool InputReceived = false;
    public bool RanValidation = false;
    private bool SequenceEnded = false;
    private bool SuccessfulSequence = true;

    public static event Action OnInputReceived;
    public static event Action<bool> OnValidateInput;
    public static event Action<bool> OnSequenceEnd;

    [SerializeField] private bool DebugMessages = false;

    public void OnEnable()
    {
        Choreographer.OnBeatListen += SetNextInput;
        PlayerController.OnPlayerAction += ReceiveInput;
        Metronome.OnBeatWindowClosed += ValidateLastInput;
        Choreographer.OnEndListenPhase += EndSequenceCheck;
        GameManager.ResetGameState += ResetState;
    }

    public void SetNextInput(int input) 
    {
        if (input != 0 && DebugMessages) Debug.Log($"Judge waiting for input: {(Note)input}");

        RanValidation = false;
        InputReceived = false;
        LastReceivedInput = 0;
        NextCorrectInput = input;
    }

    public void ReceiveInput(int input)
    {
        if (!InputReceived) 
        {
            if (DebugMessages) Debug.Log($"Judge received player input: {(Note)input}");

            LastReceivedInput = input;
            InputReceived = true;
            OnInputReceived?.Invoke();

            ValidateLastInput();
        }
    }

    private void ValidateLastInput() 
    {
        if (RanValidation) return;
        if (NextCorrectInput == 0 || NextCorrectInput == 7) return;

        RanValidation = true;

        bool valid = false;

        if (LastReceivedInput == NextCorrectInput) valid = true;
        else SuccessfulSequence = false;

        OnValidateInput?.Invoke(valid);

        if (SequenceEnded) 
        {
            OnSequenceEnd?.Invoke(SuccessfulSequence);
            SuccessfulSequence = true;
            SequenceEnded = false;
        }

        if (NextCorrectInput != 0 && DebugMessages) Debug.Log($"Player succeeded note: {valid} (Expected {NextCorrectInput}, Received {LastReceivedInput})");
    }

    private void EndSequenceCheck() 
    {
        SequenceEnded = true;
    }

    private void ResetState() 
    {
        SuccessfulSequence = true;
        LastReceivedInput = 0;
        NextCorrectInput = 0;
        InputReceived = false;
        RanValidation = false;
    }
}
