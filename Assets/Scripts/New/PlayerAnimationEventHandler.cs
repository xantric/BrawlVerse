using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    public void Jump()
    {
        characterMovement.Jump();
    }
}
