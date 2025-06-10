using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParryManager : MonoBehaviour
{
    public enum Stance {Parry, Defended, Undefended};

    private Stance currentStance;

    private bool canParry = false;
    [SerializeField] private float parryTime = 0.5f;

    public static Action OpenParryWindow;
    public static Action CloseParryWindow;

    [SerializeField] private Animator anim;

    private void Awake() {
        OpenParryWindow += ParryWindowOpened;
        CloseParryWindow += ParryWindowClosed;
        currentStance = Stance.Undefended;
    }
    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            if (canParry) {
                if (currentStance == Stance.Undefended) currentStance = Stance.Parry;
            }
            else {
                currentStance = Stance.Defended;
            }
        }
        if (!canParry && currentStance == Stance.Parry) {
            currentStance = Input.GetMouseButton(1) ? Stance.Defended : Stance.Undefended;
        }
        if (Input.GetMouseButtonUp(1)) {
            currentStance = Stance.Undefended;
        }
        //Debug.Log(canParry);
        Debug.Log(currentStance);
        anim.SetBool("defended",currentStance != Stance.Undefended);
    }
    public Stance GetCurrentStance() {
        return currentStance;
    }
    private void ParryWindowOpened() {
        canParry = true;
    }
    private void ParryWindowClosed() {
        canParry = false;
    }
}
