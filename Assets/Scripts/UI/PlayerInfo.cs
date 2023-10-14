using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfo : NetworkBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("playerName")] 
        private TMP_Text playerNameText;
        [SerializeField]
        [FormerlySerializedAs("coinCounter")] 
        private TMP_Text coinCounterText;
        [SerializeField]
        private Image healthBar;

        private NetworkVariable<FixedString32Bytes> _playerName = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        void Start()
        {
            if (!IsOwner) return;
            _playerName.Value = AuthenticationService.Instance.Profile;
        }

        void OnEnable()
        {
            _playerName.OnValueChanged += UIManager.Instance.RegisterPlayerInfoClientRpc;
        }
        
        void OnDisable()
        {
            _playerName.OnValueChanged -= UIManager.Instance.RegisterPlayerInfoClientRpc;
        }

        public void UpdatePlayerNameText()
        {
            playerNameText.text = _playerName.Value.ToString();
        }

        // [ServerRpc]
        // private void SetNameServerRpc()
        // {
        //     Debug.Log("Im set 1");
        //     SetNameClientRpc();
        // }

        // [ClientRpc]
        // private void SetNameClientRpc()
        // {
        //     Debug.Log("Im set 2");
        //     playerNameText.text = _playerName.Value.ToString();
        // }

    }
}
