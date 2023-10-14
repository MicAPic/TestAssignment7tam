using DG.Tweening;
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
        [Space]
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private HealthController healthController;

        private NetworkVariable<FixedString32Bytes> _playerName = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        void Start()
        {
            if (!IsOwner) return;
            _playerName.Value = AuthenticationService.Instance.Profile;
        }

        void OnEnable()
        {
            _playerName.OnValueChanged += UIManager.Instance.RegisterPlayerInfoClientRpc;
            healthController.healthPoints.OnValueChanged += UpdateHealthBarClientRpc;
        }
        
        void OnDisable()
        {
            _playerName.OnValueChanged -= UIManager.Instance.RegisterPlayerInfoClientRpc;
            healthController.healthPoints.OnValueChanged -= UpdateHealthBarClientRpc;
        }

        public void UpdatePlayerNameText()
        {
            playerNameText.text = _playerName.Value.ToString();
        }
        
        [ClientRpc]
        private void UpdateHealthBarClientRpc(float previousValue, float newValue)
        {
            healthBar.DOFillAmount(
                newValue / healthController.maxHealth.Value, 
                0.3f
            );
        }
    }
}
