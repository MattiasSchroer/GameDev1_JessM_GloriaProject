using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //FEEDBACK All Variable Names should be lower case
    public static GameManager Instance { get; private set; }

    public MusicController Music;
    public Metronome Metronome;
    public Choreographer Choreographer;
    public Judge Judge;
    public LightingManager LightingManager;
    public UIManager UIManager;

    public static event Action<bool> OnToggleMusic;
    public static event Action ResetGameState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        //FEEDBACK Clean up whitespace
    }

    private void Start()
    {
        EnemyEncounter.SendEncounterDetails += GetEnemyDetails;
        EnemyEncounter.OnEnemyHealthEmpty += WinGame;
        PlayerStatus.OnPlayerHealthEmpty += LoseGame;

        UIManager.ToggleMainMenu(true);
    }

    public void StartGameplay() 
    {
        EnemyEncounter enemy = GameObject.FindAnyObjectByType<EnemyEncounter>();//FEEDBACK FindAnyObjectByType is ineficient and should be avoided whenever possible
        enemy.BeginEncounter();

        Music.PlaySong();
        Metronome.ResetValues();
        Metronome.ToggleGameplay(true);
        UIManager.ToggleGameplayUI(true);
        UIManager.ToggleMainMenu(false);
    }

    public void StartButEvil() //FEEDBACK Vague method name
    {
        EnemyEncounter enemy = GameObject.FindAnyObjectByType<EnemyEncounter>();
        Track evilTrack = Instantiate(Resources.Load<Track>("Tracks/TitleSong"));
        evilTrack.BeatsInBar = 8;

        GetEnemyDetails(evilTrack, enemy.TimingWindow);

        Music.PlaySong();
        Metronome.ResetValues();
        Metronome.ToggleGameplay(true);
        UIManager.ToggleGameplayUI(true);
        UIManager.ToggleMainMenu(false);
    }

    public void GetEnemyDetails(Track track, float timingWindow) 
    {
        Debug.Log("Ran get enemy details...");//FEEDBACK Clean up debug statements

        Music.audioSource.clip = track.Song;
        Metronome.ResetValues(track);
        Choreographer.ResetValues(track.GameplayStartBeat);
        Metronome.TimingWindowMargin = timingWindow;
        LightingManager.SetStartBattleBeat(track.GameplayStartBeat);
    }

    public void LoseGame() 
    {
        OnToggleMusic?.Invoke(false);
        Metronome.ToggleGameplay(false);

        UIManager.ShowLossPanel();
    }

    public void WinGame() 
    {
        OnToggleMusic?.Invoke(false);
        Metronome.ToggleGameplay(false);

        UIManager.ShowWinPanel();
    }

    public void PauseGameplay() 
    {
        OnToggleMusic?.Invoke(false);
        Metronome.ToggleGameplay(false);
    }

    public void ResumeGameplay() 
    {
        OnToggleMusic?.Invoke(true);
        Metronome.ToggleGameplay(true);
    }

    //FEEDBACK Avoid repetitive code, the following lines are repeated in each method above:
    /*  
        OnToggleMusic?.Invoke(false);
        Metronome.ToggleGameplay(false);

        They should go into their own method such as:

        void ToggleValues(bool toggleMusic, bool toggleGameplay){
            OnToggleMusic?.Invoke(toggleMusic);
            Metronome.ToggleGameplay(toggleGameplay);
        }

        that way you are not repeating lines
    */

    public void RestartGame() 
    {
        ResetGameState?.Invoke();

        StartGameplay();
    }

    public void ReturnToMenu() 
    {
        ResetGameState?.Invoke();
        UIManager.ToggleGameplayUI(false);
        UIManager.ToggleMainMenu(true);
    }

    public void QuitGame() 
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
