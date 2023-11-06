using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoyoteTimeScript : MonoBehaviour
{
    float CoyoteTimeLength=0.2f;
    bool bCoyoteTime; public bool GetCTEnabled() {return bCoyoteTime; }

    Coroutine CoyoteTime;
    public void StartCoyoteTime(bool CanCoyote)
    {
        if (!CanCoyote)
            return;
        CoyoteTime = StartCoroutine(IE_CoyoteTime());
    }

    IEnumerator IE_CoyoteTime()
    {
        bCoyoteTime = true;
        yield return new WaitForSeconds(CoyoteTimeLength);
        bCoyoteTime = false;
        yield break;
    }
}
