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
        [Space]
        [SerializeField]
        private CoinPouch coinPouch;

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
            coinPouch.coins.OnValueChanged += UpdateCoinCounterClientRpc;
        }
        
        void OnDisable()
        {
            _playerName.OnValueChanged -= UIManager.Instance.RegisterPlayerInfoClientRpc;
            healthController.healthPoints.OnValueChanged -= UpdateHealthBarClientRpc;
            coinPouch.coins.OnValueChanged -= UpdateCoinCounterClientRpc;
        }

        public void UpdatePlayerNameText()
        {
            playerNameText.text = _playerName.Value.ToString();
        }

        public void UpdateHealthBar()
        {
            healthBar.fillAmount = healthController.healthPoints.Value / healthController.maxHealth.Value;
        }
        
        public void UpdateCoinCounter()
        {
            coinCounterText.text = $"¢{coinPouch.coins.Value}";
        }

        [ClientRpc]
        private void UpdateHealthBarClientRpc(float previousValue, float newValue)
        {
            healthBar.DOFillAmount(
                newValue / healthController.maxHealth.Value, 
                0.3f
            );
        }

        [ClientRpc]
        private void UpdateCoinCounterClientRpc(int previousValue, int newValue)
        {
            coinCounterText.text = $"¢{newValue}";
        }
    }
}
