using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorBlindFilter : MonoBehaviour
{
    [SerializeField] private Material cbMat;

    private TMP_Dropdown dropdown;

    private LocalKeyword none;
    private LocalKeyword tritanopia;
    private LocalKeyword protonopia;
    private LocalKeyword deuteranopia;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        InitializeKeywords();
    }

    private void InitializeKeywords()
    {
        none = new LocalKeyword(cbMat.shader, "_COLORBLINDMODE_NONE");
        tritanopia = new LocalKeyword(cbMat.shader, "_COLORBLINDMODE_TRITANOPIA");
        protonopia = new LocalKeyword(cbMat.shader, "_COLORBLINDMODE_PROTONOPIA");
        deuteranopia = new LocalKeyword(cbMat.shader, "_COLORBLINDMODE_DEUTERANOPIA");
    }

    public void OnValueChanged(int index)
    {
        cbMat.SetKeyword(none, false);
        cbMat.SetKeyword(tritanopia, false);
        cbMat.SetKeyword(protonopia, false);
        cbMat.SetKeyword(deuteranopia, false);

        // None, Tritanopia, Protonopia, Deuteranopia
        switch (index)
        {
            case 0:
                cbMat.SetKeyword(none, true);
                break;
            case 1:
                cbMat.SetKeyword(tritanopia, true);
                break;
            case 2:
                cbMat.SetKeyword(protonopia, true);
                break;
            case 3:
                cbMat.SetKeyword(deuteranopia, true);
                break;
        }
    }
}
