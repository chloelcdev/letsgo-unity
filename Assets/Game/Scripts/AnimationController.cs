using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimancerComponent))]
public class AnimationController : MonoBehaviour
{
    [SerializeField] AnimancerComponent animancer;

    public AnimancerState animationState;
    [SerializeField] List<NamedAnimationClip> animations = new List<NamedAnimationClip>();


    Dictionary<string, AnimationClip> animationsDict = null;
    Dictionary<string, AnimationClip> Animations
    {
        get
        {
            if (animationsDict == null)
                UpdateAnimationsDictionary();

            return animationsDict;
        }

        set
        {
            animationsDict = value;
        }
    }

    public void UpdateAnimationsDictionary()
    {
        animationsDict = new Dictionary<string, AnimationClip>();

        foreach (NamedAnimationClip anim in animations)
            Animations.Add(anim.Name, anim.Clip);

    }

    public AnimationClip GetClip(string clipName)
    {
        if (Animations.ContainsKey(clipName))
            return Animations[clipName];

        return null;
    }

    public void PlayAnimation(string clipName)
    {
        var clip = GetClip(clipName);

        if (clip != null)
            animationState = animancer.Play(GetClip(clipName));
    }


    [Serializable]
    public class NamedAnimationClip
    {
        public string Name;
        public AnimationClip Clip;
    }
}
