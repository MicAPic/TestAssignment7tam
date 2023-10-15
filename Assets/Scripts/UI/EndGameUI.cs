using Unity.Netcode;

namespace UI
{
    public class EndGameUI : UI
    {
        public override void LoadScene()
        {
            NetworkManager.Singleton.Shutdown();
            base.LoadScene();
        }
    }
}
