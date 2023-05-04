using System;
using UnityEngine;

[Serializable]
public struct CharacterAttribute
{
    public CharacterPartAnimator CharacterPart; // "Arms"

    public PartVariantColor PartVariantColor;   // "None" (not using this)

    public PartVariantType PartVariantType;     // "Carry"

    public CharacterAttribute(CharacterPartAnimator characterPart, PartVariantColor partVariantColor = PartVariantColor.None, PartVariantType partVariantType = PartVariantType.None)
    {
        this.CharacterPart = characterPart;
        this.PartVariantColor = partVariantColor;
        this.PartVariantType = partVariantType;
    }
}
