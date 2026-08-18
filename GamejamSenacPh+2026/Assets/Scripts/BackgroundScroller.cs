using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
   [SerializeField] private RawImage backgroundImage;
  
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.04f, 0.02f);

    [SerializeField] private string texturePropertyName = "_MainTex";

    private Material _materialInstance;
    private Vector2 _offset;

    private void Awake()
    {
        if (backgroundImage == null) backgroundImage = GetComponent<RawImage>();
        if (backgroundImage == null || backgroundImage.material == null) return;

        _materialInstance = new Material(backgroundImage.material);
        backgroundImage.material = _materialInstance;
        _offset = _materialInstance.GetTextureOffset(texturePropertyName);
    }

    private void Update()
    {
        if (_materialInstance == null) return;

        _offset += scrollSpeed * Time.unscaledDeltaTime;
        _materialInstance.SetTextureOffset(texturePropertyName, _offset);
    }

    private void OnDestroy()
    {
        if (_materialInstance != null) Destroy(_materialInstance);
    }
}
