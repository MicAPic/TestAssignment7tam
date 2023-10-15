using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class UIManager : NetworkBehaviour
    {
        public static UIManager Instance;

        [SerializeField]
        private GameObject controlsUI;
        [SerializeField]
        private GameObject endGameUI;
        
        [SerializeField]
        private TMP_Text winnerText;
        
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

        [ClientRpc]
        public void RegisterPlayerInfoClientRpc(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            _playerInfos = new Dictionary<ulong, PlayerInfo>();
            // we use an inefficient FindObjectsOfType in case a player joins after somebody dies
            foreach (var playerInfo in FindObjectsOfType<PlayerInfo>())
            {
                _playerInfos[playerInfo.OwnerClientId] = playerInfo;
            }
            UpdatePlayerInfosClientRpc();
        }

        [ClientRpc]
        public void RemovePlayerInfoClientRpc(ulong id)
        {
            Debug.Log("huh");
            _playerInfos.Remove(id);
            Debug.Log(_playerInfos.Count);
            if (_playerInfos.Count < 2)
            {
                SetEndGameScreen();
            }
        }

        private void SetEndGameScreen()
        {
            controlsUI.SetActive(false);
            endGameUI.SetActive(true);

            var winnerPlayerInfo = _playerInfos.First().Value;
            winnerText.text = $"{winnerPlayerInfo.playerName.Value.ToString()} Wins!\tCoins:{winnerPlayerInfo.coinPouch.coins.Value}";
        }

        [ClientRpc]
        private void UpdatePlayerInfosClientRpc()
        {
            foreach (var playerInfo in _playerInfos.Values)
            {
                playerInfo.UpdatePlayerNameText();
                playerInfo.UpdateHealthBar();
                playerInfo.UpdateCoinCounter();
            }
        }
    }
}
