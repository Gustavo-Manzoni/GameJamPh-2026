using UnityEngine;
using TMPro;
public class TMP_DropdownNoBlocker : TMP_Dropdown
{
    protected override GameObject CreateBlocker(Canvas rootCanvas)
    {
        return null;
    }
}
