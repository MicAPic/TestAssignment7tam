using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class UIManager : NetworkBehaviour
    {
        public static UIManager Instance;
        private PlayerInfo[] _playerInfos;

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
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        [ClientRpc]
        public void RegisterPlayerInfoClientRpc(FixedString32Bytes oldValue, FixedString32Bytes newValue)
        {
            _playerInfos = FindObjectsOfType<PlayerInfo>();
            UpdatePlayerNamesClientRpc();
        }

        [ClientRpc]
        private void UpdatePlayerNamesClientRpc()
        {
            Debug.Log("Update loop");
            foreach (var playerInfo in _playerInfos)
            {
                playerInfo.UpdatePlayerNameText();
            }
        }
    }
}
