using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBufferScript : MonoBehaviour
{

    [SerializeField] float mf_JumpBufferTime = 0.25f;
    public bool GetJumpBufferActivated() { return bJumpBuffer; }
    bool bJumpBuffer; 

    Coroutine mcr_JumpBuff;

    public void StartJumpBuffer(bool CanJumpCheck)
    {
        if (!CanJumpCheck)
            return;
        if(mcr_JumpBuff == null)    
            mcr_JumpBuff = StartCoroutine(IE_JumpBuffer());
    }
    public void StopJumpBuffer()
    {
        if(mcr_JumpBuff != null)
        {
            StopCoroutine(mcr_JumpBuff);
            mcr_JumpBuff = null;
            bJumpBuffer = false;
        }
    }

    IEnumerator IE_JumpBuffer()
    {
        bJumpBuffer = true;
        yield return new WaitForSeconds(mf_JumpBufferTime);
        bJumpBuffer = false;
        yield break;
    }
}
