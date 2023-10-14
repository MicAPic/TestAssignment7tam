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

namespace Network
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
    
        [Header("Settings")] 
        public static readonly int MaxPlayers = 4;
        private const string StartKey = "STARTED";

        public event Action OnLobbyConnectionEstablished;

        private string _lobbyToHost;
        private string _lobbyToJoin;
        private Lobby _currentLobby;
        
        [Header("Timers")]
        private const float HeartbeatTimerInterval = 15.0f; // seconds, default timeout is 30 seconds
        private const float PollTimerInterval = 1.8f; // seconds, min interval is 1 second
        private float _heartbeatTimer;
        private float _lobbyPollTimer;


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
            HandleLobbyPolling();
        }

        public async void CreateLobby()
        {
            try
            {
                SceneLoadData.CanLoadScene = false;
                
                var options = new CreateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { StartKey, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                    }
                };
                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyToHost, MaxPlayers, options);
                
                Debug.Log($"Created lobby named: {_currentLobby.Name}");
                OnLobbyConnectionEstablished?.Invoke();
                
                StartGameAsHost();
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
                SceneLoadData.CanLoadScene = false;
                
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
                OnLobbyConnectionEstablished?.Invoke();
            }
            catch (Exception exception)
            {
                if (exception is IndexOutOfRangeException)
                {
                    // no lobby found
                    // TODO: show error message to the user
                    SceneLoadData.CanLoadScene = true;
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

        public bool IsLobbyHost()
        {
            return _currentLobby != null && _currentLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private async void Authenticate()
        {
            // Initialize UnityServices and our player
            var initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(_currentPlayerName);
        
            await UnityServices.InitializeAsync(initializationOptions);
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in!");
            
                nicknameText.text = $"Имя игрока: {_currentPlayerName}";
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (!IsLobbyHost()) return;
            // pinging lobby, so that it stays accessible for potential players
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0.0f)
            {
                _heartbeatTimer = HeartbeatTimerInterval;
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                // Debug.Log("❤");
            }
        }

        private async void HandleLobbyPolling()
        {
            if (_currentLobby == null) return;
            // pinging lobby, so that it stays accessible for potential players
            _lobbyPollTimer -= Time.deltaTime;
            if (_lobbyPollTimer < 0.0f)
            {
                _lobbyPollTimer = PollTimerInterval;
                _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);

                if (_currentLobby.Data[StartKey].Value != "0")
                {
                    StartGameAsClient();
                } 
            }
        }

        private void StartGameAsClient()
        {
            if (IsLobbyHost()) return;
            SceneLoadData.CanLoadScene = true;
            RelayManager.Instance.JoinRelay(_currentLobby.Data[StartKey].Value);
        }

        private async void StartGameAsHost()
        {
            if (!IsLobbyHost()) return;
            try
            {
                Debug.Log("Starting game...");

                SceneLoadData.CanLoadScene = true;
                // await GameManager.IsLoadedTask.Task;
                
                var relayCode = await RelayManager.Instance.CreateRelay();

                var lobby = await Lobbies.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { StartKey, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });
                _currentLobby = lobby;
            }
            catch (LobbyServiceException exception)
            {
                Debug.LogError(exception);
            }
        }

        private static string GenerateRandomName()
        {
            return "7TAM_" + Random.Range(0, 100);
        }
    }
}
