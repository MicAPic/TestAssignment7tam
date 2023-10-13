using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    
    [Header("Heartbeat")]
    private const float HeartbeatTimerInterval = 15.0f; // seconds
    private float _heartbeatTimer;

    [Header("Lobby Settings")]
    private const int MaxPlayers = 4;
    
    private string _lobbyToHost;
    private string _lobbyToJoin;
    private Lobby _currentLobby;

    [Header("Player Info")]
    private string _currentPlayerName;
    
    [Header("Visuals")]
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button joinButton;
    [SerializeField]
    private TMP_Text nicknameText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentPlayerName = GenerateRandomName();
        Authenticate();
    }

    // Update is called once per frame
    void Update()
    {
        HandleLobbyHeartbeat();
    }

    public async void CreateLobby()
    {
        try
        {
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyToHost, MaxPlayers);
            Debug.Log($"Created lobby named: {_currentLobby.Name}");
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogError(exception);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            // since joining by lobby name is not supported, we query available lobbies by name and join the 1st one
            var filter = new QueryLobbiesOptions
            {
                Count = 1,
                Filters = new List<QueryFilter>
                {
                    new(QueryFilter.FieldOptions.Name, _lobbyToJoin, QueryFilter.OpOptions.EQ)
                }
            };
            var queryResponse = await Lobbies.Instance.QueryLobbiesAsync(filter);
            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            Debug.Log($"Joined lobby named: {_currentLobby.Name}");
        }
        catch (Exception exception)
        {
            if (exception is IndexOutOfRangeException)
            {
                // no lobby found
                // TODO: show error message to the user
                return;
            }
            Debug.LogError(exception);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogError(exception);
        }
    }

    public void SetHostParameters(string newLobbyToHost)
    {
        hostButton.interactable = newLobbyToHost != string.Empty;
        _lobbyToHost = newLobbyToHost;
    }
    
    public void SetJoinParameters(string newLobbyToJoin)
    {
        joinButton.interactable = newLobbyToJoin != string.Empty;
        _lobbyToJoin = newLobbyToJoin;
    }

    private async void Authenticate()
    {
        // Initialize UnityServices and our player
        var initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(_currentPlayerName);
        
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in!");
            
            nicknameText.text = $"Имя игрока: {_currentPlayerName}";
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (_currentLobby == null) return;
        // pinging lobby, so that it stays accessible for potential players
        _heartbeatTimer -= Time.unscaledDeltaTime;
        if (_heartbeatTimer < 0.0f)
        {
            _heartbeatTimer = HeartbeatTimerInterval;
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
        }
    }

    private static string GenerateRandomName()
    {
        return "7TAM_" + Random.Range(0, 100);
    }
}
