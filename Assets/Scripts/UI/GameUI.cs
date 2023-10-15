using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UI
{
    public class GameUI : UI
    {
        [SerializeField]
        private Volume globalVolume;
        [SerializeField]
        private float volumeAppearanceDuration = 1.25f;
        private DepthOfField _depthOfField;

        private void Awake()
        {
            globalVolume.profile.TryGet(out _depthOfField);
        }

        void OnEnable()
        {
            DOTween.To(
                () => _depthOfField.gaussianMaxRadius.value,
                x => _depthOfField.gaussianMaxRadius.value = x,
                1.5f,
                volumeAppearanceDuration
                );
        }

        private void OnDisable()
        {
            _depthOfField.gaussianMaxRadius.value = 0.0f;
        }

        public override void LoadScene()
        {
            NetworkManager.Singleton.Shutdown();
            base.LoadScene();
        }
    }
}
