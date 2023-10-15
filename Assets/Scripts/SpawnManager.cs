using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("Parameters")]
    [Tooltip("Amount of coins to spawn in the scene")]
    [SerializeField]
    private int coinsToSpawn;
    [Tooltip("Radius of a circle in the center where nothing should spawn")]
    [SerializeField]
    private float deadZoneRadius = 2.0f;

    private Vector3 _scale;
    private Vector3 _spawnPosition;
    private const int MaxTries = 5;
    
    [Header("Prefabs")]
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private GameObject borderPrefab;

    private float _halfWidth;
    private float[] _screenBoundsX = new float[2];
    private float _halfHeight;
    private float[] _screenBoundsY = new float[2];

    void Awake()
    {
        _halfHeight = Camera.main!.orthographicSize;
        _screenBoundsY[0] = -_halfHeight;
        _screenBoundsY[1] = _halfHeight;

        _halfWidth = Screen.width / (float)Screen.height * _halfHeight;
        _screenBoundsX[0] = -_halfWidth;
        _screenBoundsX[1] = _halfWidth;
    }
    
    void OnEnable()
    {
        PlayerNetworkController.OnPlayerLoaded += InitializeGameField;
    }

    private void InitializeGameField()
    {
        if (!IsHost) return;
        
        // Spawn borders
        _scale = new Vector3(1.0f, 2.0f * _halfHeight, 1.0f);
        foreach (var x in _screenBoundsX)
        {
            SpawnBorderServerRpc(new Vector3(x + 0.5f * Mathf.Sign(x), 0.0f, 0.0f));
        }

        _scale = new Vector3(2.0f * _halfWidth, 1.0f, 1.0f);
        foreach (var y in _screenBoundsY)
        {
            SpawnBorderServerRpc(new Vector3(0.0f, y + 0.5f * Mathf.Sign(y), 0.0f));
        }
        
        // Spawn coins
        for (var _ = 0; _ < coinsToSpawn; _++)
        {
            var tries = 0;
            do
            {
                _spawnPosition = new Vector3(
                    Random.Range(_screenBoundsX[0], _screenBoundsX[1]),
                    Random.Range(_screenBoundsY[0], _screenBoundsY[1]),
                    0.0f
                );
                tries++;
            } while (Vector3.Distance(Vector3.zero, _spawnPosition) > deadZoneRadius && tries < MaxTries);
            SpawnCoinServerRpc(_spawnPosition);
        }
        
        PlayerNetworkController.OnPlayerLoaded -= InitializeGameField;
    }
    
    [ServerRpc]
    private void SpawnBorderServerRpc(Vector3 at)
    {
        var result = Instantiate(borderPrefab, at, Quaternion.identity);
        result.transform.localScale = _scale;
        result.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    private void SpawnCoinServerRpc(Vector3 at)
    {
        var result = Instantiate(coinPrefab, at, Quaternion.identity);
        result.GetComponent<NetworkObject>().Spawn(true);
    }
}
