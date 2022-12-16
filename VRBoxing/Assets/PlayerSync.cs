using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerSync : MonoBehaviourPun
{
    #region Player Body Parts

    [Header("Body Parts")]
    public Transform bodyTransform;

    public Transform leftHandTransform;

    public Transform rightHandTransform;

    private Vector3 bodyPosition;

    private Quaternion bodyRotation;

    private Vector3 leftHandPosition;

    private Quaternion leftHandRotation;

    private Vector3 rightHandPosition;

    private Quaternion rightHandRotation;

    #endregion

    #region Animators

    [Header("Animators")]
    public Animator leftHandAnimator;

    public Animator rightHandAnimator;

    public string[] leftHandAnimatorParameterNames;

    public string[] rightHandAnimatorParameterNames;
    #endregion

    void FixedUpdate()
    {
        // Dont update the players data if we are the master client
        if (!photonView.IsMine)
        {
            return;
        }

        // Get the current position and rotation of the player's body and hands
        bodyPosition = bodyTransform.position;
        bodyRotation = bodyTransform.rotation;
        leftHandPosition = leftHandTransform.position;
        leftHandRotation = leftHandTransform.rotation;
        rightHandPosition = rightHandTransform.position;
        rightHandRotation = rightHandTransform.rotation;

        // Set the position and rotation of the player's body and hands on the Photon server
        photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, bodyTransform, bodyPosition, bodyRotation);
        photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, leftHandTransform, leftHandPosition, leftHandRotation);
        photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, rightHandTransform, rightHandPosition, rightHandRotation);

        //Get the current Parameters of the players hand animators

        AnimatorParameters[] leftHandParameters = new AnimatorParameters[]
        {
            new AnimatorParameters
            {
                paramName = leftHandAnimatorParameterNames[0],
                value = leftHandAnimator.GetBool(leftHandAnimatorParameterNames[0]),
                valueType = AnimatorValueType.Bool
            }
        };

        AnimatorParameters[] rightHandParameters = new AnimatorParameters[]
        {
            new AnimatorParameters
            {
                paramName = rightHandAnimatorParameterNames[0],
                value = rightHandAnimator.GetBool(rightHandAnimatorParameterNames[0]),
                valueType = AnimatorValueType.Bool
            }
        };

        //Set the animators parameters on the photon server

        photonView.RPC(nameof(SyncAnimators), RpcTarget.Others, leftHandAnimator, leftHandParameters);
        photonView.RPC(nameof(SyncAnimators), RpcTarget.Others, rightHandAnimator, rightHandParameters);
    }

    public enum AnimatorValueType
    {
        Int,
        Float,
        Bool,
        Trigger,
    }
    public struct AnimatorParameters
    {
        public string paramName;
        public object value;
        public AnimatorValueType valueType;
    }

    #region RPCMethods;
    //RPC method to synchronize objects across the photon network
    [PunRPC]
    public void SyncBodyPart(Transform bodyPart, Vector3 pos, Quaternion rot)
    {
        if (!Application.isPlaying) return;

        bodyPart.position = pos;
        bodyPart.rotation = rot;
    }

    //RPC Method to synchronize animators across the photon network
    [PunRPC]
    public void SyncAnimators(Animator animator, params AnimatorParameters[] parameters )
    {
        if (!Application.isPlaying) return;


        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            switch (parameter.valueType)
            {
                case AnimatorValueType.Int:
                    animator.SetInteger(parameter.paramName, (int)parameter.value);
                    break;
                case AnimatorValueType.Float:
                    animator.SetFloat(parameter.paramName, (float)parameter.value);
                    break;
                case AnimatorValueType.Bool:
                    animator.SetBool(parameter.paramName, (bool)parameter.value);
                    break;
                case AnimatorValueType.Trigger:
                    animator.SetTrigger(parameter.paramName);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
}