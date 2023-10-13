using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using UnityEngine;

namespace Network
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance;
        
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
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );
                
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
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );
            }
            catch (RelayServiceException exception)
            {
                Debug.LogError(exception);
            }
        }
    }
}
