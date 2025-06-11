using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParryListener : MonoBehaviour
{
    public KeyCode dfk = KeyCode.LeftShift;
    public float parryWindow = 0.5f;
    public float maxParryRange = 3f;
    public GameObject parryIndicatorUI;
    public Animator onii;

    private bool isdefending = false;
    private Coroutine parryCoroutine;
    // testing purpose
    private bool isTesting = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isTesting = !isTesting;
            Debug.Log("is testing - " + isTesting.ToString());
        }
        if (Input.GetKey(dfk))
        {
            SetDefending(true);
        }
        else
        {
            SetDefending(false);
        }

        onii.SetBool("isDefending", isdefending);
       
    }

    private void OnEnable()
    {
        EventBroadcaster.Attacked += HandleIncomingAttack;
    }

    private void OnDisable()
    {
        EventBroadcaster.Attacked -= HandleIncomingAttack;
    }

    private void HandleIncomingAttack(GameObject attacker) 
    {
        float distance = Vector3.Distance(attacker.transform.position, transform.position);
        Vector3 toDefender = (transform.position - attacker.transform.position).normalized;
        float anglecheck = Vector3.Dot(attacker.transform.forward, toDefender);

        if (!isTesting && (distance > maxParryRange || anglecheck < 0.5f)) 
        {
            Debug.Log("ignore the attack");
            return;
        }

        if (isdefending) 
        {
            Debug.Log("Defended normally");
            return;
        }

        if (parryCoroutine != null) 
        {
            StopCoroutine(parryCoroutine);// stops any coroutine that is already running, dont go scratching your head in the futule
        }

        parryCoroutine = StartCoroutine(ParryWindow());

    }

    private IEnumerator ParryWindow() 
    {
        float timer = 0f;
        parryIndicatorUI?.SetActive(true);// for now this imitates the parry shader(this fully came out of misconception but it has advantage)
        Debug.Log("Parry window opened!");

        while (timer < parryWindow) 
        {
            if (Input.GetKeyDown(dfk)) 
            {
                ParrySuccess();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        parryIndicatorUI?.SetActive(false);
        Debug.Log("Parry Failed, YOU ARE F*****");
        TakeDamage();
    }

    private void ParrySuccess() 
    {
        Debug.Log("Parry Successful");
        parryIndicatorUI?.SetActive(false);
        //StartCoroutine(ParryEffect());  uncomment for slowmo parry
    }

    private void TakeDamage() 
    {
        Debug.Log("Player took damage");
    }

    //private IEnumerator ParryEffect() 
    //{
    //    Time.timeScale = 0.2f;
    //    yield return new WaitForSecondsRealtime(0.2f);
    //    Time.timeScale = 1f;
    //}

    private void SetDefending(bool value) 
    {
        isdefending = value;
        //Debug.Log(isdefending.ToString())
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxParryRange);
    }
}

