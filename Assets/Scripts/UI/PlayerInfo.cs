using DG.Tweening;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfo : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> playerName = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [SerializeField]
        private TMP_Text playerNameText;
        [SerializeField]
        private TMP_Text coinCounterText;
        [Space]
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private HealthController healthController;
        [Space]
        public CoinPouch coinPouch;
        
        void Start()
        {
            if (!IsOwner) return;
            playerName.Value = AuthenticationService.Instance.Profile;
        }

        void OnEnable()
        {
            playerName.OnValueChanged += UIManager.Instance.RegisterPlayerInfoClientRpc;
            healthController.healthPoints.OnValueChanged += UpdateHealthBarClientRpc;
            coinPouch.coins.OnValueChanged += UpdateCoinCounterClientRpc;
        }
        
        void OnDisable()
        {
            playerName.OnValueChanged -= UIManager.Instance.RegisterPlayerInfoClientRpc;
            healthController.healthPoints.OnValueChanged -= UpdateHealthBarClientRpc;
            coinPouch.coins.OnValueChanged -= UpdateCoinCounterClientRpc;
        }

        public void UpdatePlayerNameText()
        {
            playerNameText.text = playerName.Value.ToString();
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
