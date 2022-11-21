using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum AnimationCondition
{
    Bool,
    Int,
    Float,
    Trigger
}

[System.Serializable]
public class AnimatorData
{
    public Animator animator;
    public AnimationCondition condition;

    public string conditionName;

    public float delay;

    [Space(8)]
    public bool boolValue;
    public float floatValue;
    public int intValue;

    public async void Start()
    {
        await Task.Delay(ToMilliseconds(delay));

        if (animator == null) Debug.LogWarning("Animator" + animator + "is null!", animator);
        else
        {
            switch (condition)
            {
                case AnimationCondition.Bool:
                    animator.SetBool(conditionName, boolValue); 
                    break;
                case AnimationCondition.Int:
                    animator.SetInteger(conditionName, intValue);
                    break;
                case AnimationCondition.Float:
                    animator.SetFloat(conditionName, floatValue);
                    break;
                case AnimationCondition.Trigger:
                    animator.SetTrigger(conditionName);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Returns the amount of ticks in milliseconds
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ToMilliseconds(float value)
    {
        return Mathf.RoundToInt(value * 1000);
    }
}
