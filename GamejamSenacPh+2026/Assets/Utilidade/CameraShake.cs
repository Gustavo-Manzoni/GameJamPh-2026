using UnityEngine;
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float maxOffset = 0.4f;
    [SerializeField] private float maxAngle = 4f;
    [SerializeField] private float traumaDecay = 1.2f;
    [SerializeField] private float frequency = 18f;

    private float trauma;
    private float seed;
    private Vector3 basePos;

    private void Awake()
    {
        Instance = this;
        basePos = transform.localPosition;
        seed = Random.value * 100f;
    }

    private void Update()
    {
        if (trauma <= 0f) return;

        trauma = Mathf.Clamp01(trauma - traumaDecay * Time.deltaTime);
        float shake = trauma * trauma;

        float t = Time.time * frequency;
        float offsetX = (Mathf.PerlinNoise(seed, t) * 2f - 1f) * maxOffset * shake;
        float offsetY = (Mathf.PerlinNoise(seed + 10f, t) * 2f - 1f) * maxOffset * shake;
        float angle = (Mathf.PerlinNoise(seed + 20f, t) * 2f - 1f) * maxAngle * shake;

        transform.localPosition = basePos + new Vector3(offsetX, offsetY, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (trauma <= 0f)
        {
            transform.localPosition = basePos;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void AddTrauma(float amount)
    {
        trauma = Mathf.Clamp01(trauma + amount);
    }
}
