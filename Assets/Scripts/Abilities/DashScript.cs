using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashScript : MonoBehaviour
{
    
    PlayerController PlayerCon;
    TrailRenderer TrailRenderer;
    [SerializeField] float mf_DashForce = 250.0f;
    [SerializeField] int m_DashCount = 1;
    [SerializeField] [Range(0,1.5f)] float VerticalSpeedMultiplier = 1;
    [SerializeField] [Range(0.5f, 2)] float HorizontalSpeedMultiplier = 1;
    AudioSource m_AudioSource;
    [SerializeField] AudioClip DashAudio;
    int m_Dashes = 1;

    private void Awake()
    {
        PlayerCon = GetComponent<PlayerController>();
        m_AudioSource = GetComponent<AudioSource>();
        TrailRenderer = GetComponent<TrailRenderer>();
    }

    public void Dash(Vector2 Direction, Rigidbody2D OwningRigidBody)
    {
        Direction *= new Vector2(HorizontalSpeedMultiplier, VerticalSpeedMultiplier);
        if (m_Dashes == 0 || PlayerCon.MovementLocked)
        {
            return;
        }
        if (Direction.x != 0 || Direction.y != 0)
        {
            m_AudioSource.PlayOneShot(DashAudio, 1);
            OwningRigidBody.velocity = new Vector2(0, 0);
            OwningRigidBody.AddForce(Direction * mf_DashForce, ForceMode2D.Impulse);
            StartCoroutine(IE_Dash(OwningRigidBody));
            m_Dashes -= 1;
        }
    }

    public void ResetDashes() { m_Dashes = m_DashCount; }

    IEnumerator IE_Dash(Rigidbody2D OwningRigidBody)
    {
        PlayerCon.MovementLocked = true;
        TrailRenderer.emitting = true;
        OwningRigidBody.gravityScale = 0;
        yield return new WaitForSeconds(0.15f);
        TrailRenderer.emitting = false;
        OwningRigidBody.velocity = new Vector2(0, OwningRigidBody.velocity.y);
        PlayerCon.MovementLocked = false;
        OwningRigidBody.gravityScale = 1;

        yield break;
    }
}
