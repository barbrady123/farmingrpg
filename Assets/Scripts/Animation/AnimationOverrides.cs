using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField]
    private GameObject _character = null;

    [SerializeField]
    private SO_AnimationType[] _soAnimationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> _animationTypeDictionaryByAnimation;

    private Dictionary<string, SO_AnimationType> _animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        // Initialize animation type dictonary keyed by animation clip
        _animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (var item in _soAnimationTypeArray)
        {
            _animationTypeDictionaryByAnimation.Add(item.AnimationClip, item);
        }

        // Initialize aniamtion type dictionary keyed by string
        _animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (var item in _soAnimationTypeArray)
        {
            _animationTypeDictionaryByCompositeAttributeKey.Add(item.ToString(), item);
        }
    }

    /// <summary>
    /// For now we'll only be using a single CharacterAttribute (Arms, None, Carry)
    /// </summary>
    /// <param name="characterAttributeList"></param>
    public void ApplyCharacterCustomizationParameters(CharacterAttribute[] characterAttributeList)
    {
        // Loop through all character attributes and set the animation override controller for each
        // For now this will only be a single one, since we're only swapping out "Arms"
        foreach (var characterAttribute in characterAttributeList)
        {
            // Final original AnimationClip -> swapped AnimationClip mapping...
            var animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // Get the name of the part we're looking for ("Arms")
            string animatorSOAssetName = characterAttribute.CharacterPart.ToString().ToLower();

            // Look at all the animators under the player and find the one we want (the one under the object named "Arms")
            var currentAnimator = _character.GetComponentsInChildren<Animator>().First(x => x.name == animatorSOAssetName);

            // Setup new AnimatorOverrideController against the current animator controller
            var aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);

            // Go through all the current animation clips in the controller...
            foreach (var animationClip in aoc.animationClips)
            {
                // find animation in dictionary (based on our SO data we used to populate it above)
                if (!_animationTypeDictionaryByAnimation.TryGetValue(animationClip, out var so_AnimationType))
                    continue;

                string key = $"{characterAttribute.CharacterPart}{characterAttribute.PartVariantColor}{characterAttribute.PartVariantType}{so_AnimationType.AnimationName}";

                if (!_animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out var swapSO_AnimationType))
                    continue;

                animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapSO_AnimationType.AnimationClip));
            }

            // Apply animation updates to animation override controller and then update animator with the new controller
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
