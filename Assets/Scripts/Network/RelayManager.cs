using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using UnityEngine;

namespace Network
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance;
        private RelayServerData _relayServerData;
        
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
        // async void Start()
        // {
        //     await UnityServices.InitializeAsync();
        // }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }

        public async Task<string> CreateRelay()
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.MaxPlayers - 1); // host doesn't count
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                _relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
                NetworkManager.Singleton.StartHost();
                

                return joinCode;
            }
            catch (RelayServiceException exception)
            {
                Debug.LogError(exception);
                return null;
            }
        }
        
        public async void JoinRelay(string joinCode)
        {
            try
            {
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                
                _relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException exception)
            {
                Debug.LogError(exception);
            }
        }
    }
}
