using Network;

namespace UI
{
    public class LobbyUI : UI
    {
        void OnEnable()
        {
            LobbyManager.Instance.OnLobbyConnectionEstablished += LoadScene;
        }
        
        void OnDisable()
        {
            LobbyManager.Instance.OnLobbyConnectionEstablished -= LoadScene;
        }
    }
}
