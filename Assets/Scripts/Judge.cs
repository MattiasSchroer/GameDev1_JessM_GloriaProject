using JetBrains.Annotations;
using System;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private int NextCorrectInput = 0;
    [SerializeField] private int LastReceivedInput = 0;
    private bool InputReceived = false;
    public bool RanValidation = false;

    public static event Action OnInputReceived;
    public static event Action<bool> OnValidateInput;

    public void OnEnable()
    {
        Choreographer.OnBeatListen += SetNextInput;
        PlayerController.OnPlayerAction += ReceiveInput;
        Metronome.OnBeatWindowClosed += ValidateLastInput;
    }

    public void SetNextInput(int input) 
    {
        if (input != 0) Debug.Log($"Judge waiting for input: {(Note)input}");

        RanValidation = false;
        InputReceived = false;
        LastReceivedInput = 0;
        NextCorrectInput = input;
    }

    public void ReceiveInput(int input)
    {
        if (!InputReceived) 
        {
            Debug.Log($"Judge received player input: {(Note)input}");

            LastReceivedInput = input;
            InputReceived = true;
            OnInputReceived?.Invoke();

            ValidateLastInput();
        }
    }

    public void ValidateLastInput() 
    {
        if (RanValidation) return;
        if (NextCorrectInput == 0 || NextCorrectInput == 7) return;

        RanValidation = true;

        bool valid = false;

        if (LastReceivedInput == NextCorrectInput) valid = true;

        OnValidateInput?.Invoke(valid);

        if (NextCorrectInput != 0) Debug.Log($"Player succeeded note: {valid} (Expected {NextCorrectInput}, Received {LastReceivedInput})");
    }
}
