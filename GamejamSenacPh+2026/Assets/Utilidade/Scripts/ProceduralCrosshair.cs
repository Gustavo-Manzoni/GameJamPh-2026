using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
[DisallowMultipleComponent]
public class ProceduralCrosshair : MonoBehaviour
{
    public enum CrosshairShape { Cruz, Ponto, Circulo, CirculoComPonto, Diamante, Quadrado, T }
    public Image targetImage;
    public CrosshairShape shape = CrosshairShape.Cruz;
    [Range(32, 512)] public int textureSize = 128;
    [Tooltip("Pixels por unidade do sprite gerado.")]
    public float pixelsPerUnit = 100f;

    public bool topArm = true;
    public bool bottomArm = true;
    public bool leftArm = true;
    public bool rightArm = true;

    public float thickness = 4f;
    public float lineLength = 20f;
    public float gap = 8f;

    public float circleRadius = 30f;
    public float circleThickness = 2f;

    public bool centerDot = false;
    public float dotRadius = 2f;

    public Color mainColor = Color.white;
    public bool useOutline = true;
    public Color outlineColor = Color.black;
    public float outlineThickness = 1.5f;

    [Range(0.1f, 3f)] public float antiAliasWidth = 1f;


    [Range(0f, 360f)] public float rotationDegrees = 0f;


    public Sprite GeneratedSprite { get; private set; }

    private Texture2D _texture;

    void OnEnable() => Generate();
    void OnValidate() => Generate();

    [ContextMenu("Gerar Mira")]
    public void Generate()
    {
        if (textureSize < 8) textureSize = 8;
        EnsureTexture();

        var pixels = new Color[textureSize * textureSize];
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float rot = -rotationDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rot), sin = Mathf.Sin(rot);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f) - center;
                Vector2 pr = new Vector2(p.x * cos - p.y * sin, p.x * sin + p.y * cos);

                float dist = EvaluateShape(pr);
                pixels[y * textureSize + x] = ShadeFromDistance(dist);
            }
        }

        _texture.SetPixels(pixels);
        _texture.Apply(false);

        GeneratedSprite = Sprite.Create(
            _texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);

      
            targetImage.sprite = GeneratedSprite;
    }

    private void EnsureTexture()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            _texture.filterMode = FilterMode.Bilinear;
            _texture.wrapMode = TextureWrapMode.Clamp;
        }
        else if (_texture.width != textureSize)
        {
            _texture.Reinitialize(textureSize, textureSize);


        }
    }


    private float EvaluateShape(Vector2 p)
    {
        switch (shape)
        {
            case CrosshairShape.Ponto:
                return CircleSDF(p, dotRadius);

            case CrosshairShape.Circulo:
                return RingSDF(p, circleRadius, circleThickness);

            case CrosshairShape.CirculoComPonto:
                return Mathf.Min(RingSDF(p, circleRadius, circleThickness), CircleSDF(p, dotRadius));

            case CrosshairShape.Diamante:
            {
                Vector2 a = new Vector2(0, -lineLength - gap);
                Vector2 b = new Vector2(lineLength + gap, 0);
                Vector2 c = new Vector2(0, lineLength + gap);
                Vector2 d2 = new Vector2(-lineLength - gap, 0);
                float dd = SegmentSDF(p, a, b, thickness);
                dd = Mathf.Min(dd, SegmentSDF(p, b, c, thickness));
                dd = Mathf.Min(dd, SegmentSDF(p, c, d2, thickness));
                dd = Mathf.Min(dd, SegmentSDF(p, d2, a, thickness));
                return centerDot ? Mathf.Min(dd, CircleSDF(p, dotRadius)) : dd;
            }

            case CrosshairShape.Quadrado:
            {
                float half = lineLength + gap;
                Vector2 a = new Vector2(-half, -half);
                Vector2 b = new Vector2(half, -half);
                Vector2 c = new Vector2(half, half);
                Vector2 d2 = new Vector2(-half, half);
                float dd = SegmentSDF(p, a, b, thickness);
                dd = Mathf.Min(dd, SegmentSDF(p, b, c, thickness));
                dd = Mathf.Min(dd, SegmentSDF(p, c, d2, thickness));
                dd = Mathf.Min(dd, SegmentSDF(p, d2, a, thickness));
                return centerDot ? Mathf.Min(dd, CircleSDF(p, dotRadius)) : dd;
            }

            case CrosshairShape.T:
            {
                float half = lineLength * 0.5f;
                float barY = gap + lineLength;
                float dd = SegmentSDF(p, new Vector2(-half, barY), new Vector2(half, barY), thickness);
                dd = Mathf.Min(dd, SegmentSDF(p, new Vector2(0, gap), new Vector2(0, barY), thickness));
                return centerDot ? Mathf.Min(dd, CircleSDF(p, dotRadius)) : dd;
            }

            case CrosshairShape.Cruz:
            default:
            {
                float dd = float.MaxValue;
                if (topArm) dd = Mathf.Min(dd, SegmentSDF(p, new Vector2(0, gap), new Vector2(0, gap + lineLength), thickness));
                if (bottomArm) dd = Mathf.Min(dd, SegmentSDF(p, new Vector2(0, -gap), new Vector2(0, -gap - lineLength), thickness));
                if (leftArm) dd = Mathf.Min(dd, SegmentSDF(p, new Vector2(-gap, 0), new Vector2(-gap - lineLength, 0), thickness));
                if (rightArm) dd = Mathf.Min(dd, SegmentSDF(p, new Vector2(gap, 0), new Vector2(gap + lineLength, 0), thickness));
                if (centerDot) dd = Mathf.Min(dd, CircleSDF(p, dotRadius));
                return dd;
            }
        }
    }

    private Color ShadeFromDistance(float dist)
    {
        float fillCov = Coverage(dist);
        float outerCov = useOutline ? Coverage(dist - outlineThickness) : fillCov;

        Color col = Color.Lerp(outlineColor, mainColor, fillCov);
        col.a *= outerCov;
        return col;
    }

    private float Coverage(float d) => Mathf.Clamp01(0.5f - d / Mathf.Max(antiAliasWidth, 0.0001f));

    private static float CircleSDF(Vector2 p, float radius) => p.magnitude - radius;

    private static float RingSDF(Vector2 p, float radius, float ringThickness) =>
        Mathf.Abs(p.magnitude - radius) - ringThickness * 0.5f;

    private static float SegmentSDF(Vector2 p, Vector2 a, Vector2 b, float lineThickness)
    {
        Vector2 pa = p - a, ba = b - a;
        float h = Mathf.Clamp01(Vector2.Dot(pa, ba) / Mathf.Max(Vector2.Dot(ba, ba), 0.0001f));
        return (pa - ba * h).magnitude - lineThickness * 0.5f;
    }

    public void SetSpread(float extraGap)
    {
        gap = Mathf.Max(0, extraGap);
        Generate();
    }
}
