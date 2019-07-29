using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TittleManager : Singleton<TittleManager>
{
    public Animator rocketAnim;

    private void Start()
    {
        AudioManager.Instance.PlaySound("Music");
        StartCoroutine(StopTittleRocket());
    }



    public IEnumerator
    StopTittleRocket()
    {
        yield return new WaitForSeconds(5f);
        while(true)
        {
            rocketAnim.SetTrigger("TakeOff");
            yield return new WaitForSeconds(15f);
            rocketAnim.SetTrigger("Return");
            yield return new WaitForSeconds(15f);
        }
        
        
    }
}
