using UnityEngine;

[CreateAssetMenu(fileName = "SO_AnimationType", menuName = "Scriptable Objects/Animation/Animation Type")]
public class SO_AnimationType : ScriptableObject
{
    public AnimationClip AnimationClip;

    public AnimationName AnimationName;

    public CharacterPartAnimator CharacterPart;

    public PartVariantColor PartVariableColor;

    public PartVariantType PartVariantType;

    public override string ToString()
    {
        return $"{this.CharacterPart}{this.PartVariableColor}{this.PartVariantType}{this.AnimationName}";
    }
}
