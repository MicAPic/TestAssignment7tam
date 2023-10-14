using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace UI
{
    public class UIManager : NetworkBehaviour
    {
        public static UIManager Instance;
        private Dictionary<ulong, PlayerInfo> _playerInfos;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // Start is called before the first frame update
        // void Start()
        // {
        //
        // }
        
        // Update is called once per frame
        // void Update()
        // {
        //
        // }

        [ClientRpc]
        public void RegisterPlayerInfoClientRpc(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            _playerInfos = new Dictionary<ulong, PlayerInfo>();
            // we use an inefficient FindObjectsOfType in case a player joins after somebody dies
            foreach (var playerInfo in FindObjectsOfType<PlayerInfo>())
            {
                _playerInfos[playerInfo.OwnerClientId] = playerInfo;
            }
            UpdatePlayerNamesClientRpc();
        }

        [ClientRpc]
        private void UpdatePlayerNamesClientRpc()
        {
            foreach (var playerInfo in _playerInfos.Values)
            {
                playerInfo.UpdatePlayerNameText();
            }
        }
    }
}
