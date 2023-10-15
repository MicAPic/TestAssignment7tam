using UnityEngine;

namespace UI
{
    public class ControlsUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] buttons;

        void OnEnable()
        {
            PlayerNetworkController.OnPlayerLoaded += ToggleButtons;
        }

        private void ToggleButtons()
        {
            foreach (var button in buttons)
            {
                button.SetActive(true);
            }
            PlayerNetworkController.OnPlayerLoaded -= ToggleButtons;
        }
    }
}
