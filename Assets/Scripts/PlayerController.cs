using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    PlayerInput m_PlayerInput;
    public Rigidbody2D m_rb;

    Collider2D m_collider;
    CornerCorrection CollTrigger;

    [SerializeField] float mf_moveSpeed = 10.0f;

    [SerializeField] float mf_jumpForce = 10.0f;

    [SerializeField] Transform m_CastPosition;
    [SerializeField] LayerMask m_LayerMask;

    public bool bisMoving;
    [SerializeField] public bool bGoingUp;
    [SerializeField] bool bJumpBuffer;
    [SerializeField] bool bCoyoteTime = false;
    float mf_axis;

    bool isGrounded;
    float mf_coyoteTime = 0.2f;
    float mf_JumpBufferTime = 0.25f;

    [SerializeField] public bool KeyHeld = true;

    Coroutine mcr_Move;
    Coroutine mcr_JumpBuff;
    Coroutine mcr_Fall;
    Coroutine mcr_SlowPlayer;

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
        m_collider = m_rb.GetComponent<Collider2D>();
        CollTrigger = GetComponentInChildren<CornerCorrection>();
    }

    #region Bindings
    private void OnEnable()
    {
        m_PlayerInput.actions.FindAction("Jump").performed += Jump;
        m_PlayerInput.actions.FindAction("Jump").canceled += Jump;


        m_PlayerInput.actions.FindAction("Move").performed += Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled += Handle_MoveCancelled;

    }

    private void OnDisable()
    {
        m_PlayerInput.actions.FindAction("Jump").performed -= Jump;
        m_PlayerInput.actions.FindAction("Jump").canceled -= Jump;


        m_PlayerInput.actions.FindAction("Move").performed -= Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled -= Handle_MoveCancelled;
        StopAllCoroutines();
    }
    #endregion

    #region Interafaces
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable Interface = collision.GetComponent<IInteractable>();
        if (Interface != null)
        {
            Interface.OnInteract(gameObject);
        }
    }
    #endregion


    #region Colliders

    public void EnabledCollider(bool enabled) { m_collider.enabled = enabled; }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            if (GroundCheck()) 
            {
                m_collider.enabled = true;
                StopCoroutine(IE_AirChecks());
                mcr_Fall = null;
                if (bJumpBuffer)
                {
                    InitialJump();
                }
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            if(mcr_Fall == null && this.isActiveAndEnabled)
            {
                mcr_Fall = StartCoroutine(IE_AirChecks());
            }
        }
    }

    bool CoyoteCollisionCheck()
    {
        return !Physics2D.BoxCast(m_CastPosition.position + new Vector3(0, 1f), new Vector2(1.5f, 1.3f), 0, Vector2.zero, 0, m_LayerMask);
    }
    bool GroundCheck()
    {
        return Physics2D.BoxCast(m_CastPosition.position, new Vector2(.9f, 0.2f), 0, Vector2.zero, 0, m_LayerMask);
    }
    bool CeilingCollision()
    {
        return Physics2D.BoxCast(m_CastPosition.position + Vector3.up, new Vector2(0.9f, 0.2f), 0, Vector2.zero, 0, m_LayerMask);
    }

    #endregion

    #region Jumping
    void Jump(InputAction.CallbackContext context)
    {
        if (context.performed) { InitialJump();}
        else if (m_rb.velocity.y > 1) { StartCoroutine(IE_CancelJump()); }
    }
    void InitialJump()
    {
        if (isGrounded = GroundCheck() || bCoyoteTime)
        {
            m_rb.gravityScale = 1;
            m_rb.velocity = new Vector2(m_rb.velocity.x, 0);
            m_rb.AddForce(Vector2.up * mf_jumpForce, ForceMode2D.Impulse);
            m_collider.enabled = false;
        }
        else
        {
            mcr_JumpBuff = StartCoroutine(IE_JumpBuffer());
        }
    }

    IEnumerator IE_CoyoteTime()
    {
        bCoyoteTime = CoyoteCollisionCheck();
        yield return new WaitForSeconds(mf_coyoteTime);
        bCoyoteTime = false;
        yield break;
    }
    IEnumerator IE_CancelJump()
    {
        while (m_rb.velocity.y > 1)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x,m_rb.velocity.y - 1);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator IE_JumpBuffer()
    {
        if(Physics2D.Linecast(m_CastPosition.position,m_CastPosition.position - Vector3.up,m_LayerMask))
        {
        bJumpBuffer = true;
        yield return new WaitForSeconds(mf_JumpBufferTime);
        }
        bJumpBuffer = false;
        yield break;
    }

    IEnumerator IE_AirChecks()
    {
        bGoingUp = m_rb.velocity.y >= 1f;
        StartCoroutine(IE_CoyoteTime());
         while(!isGrounded)
         {
            m_rb.velocity = new Vector2(m_rb.velocity.x, Mathf.Clamp(m_rb.velocity.y, -6f, 100));
            if (bGoingUp = m_rb.velocity.y >= 2f)
            {
                m_collider.enabled = false;
            }
            else
            {
                m_collider.enabled = true;
            }

            yield return new WaitForEndOfFrame();
         }
        yield break;
    }
    #endregion

    #region Movement Handle

    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        mf_axis = context.ReadValue<float>();
        bisMoving = true;
        if (mcr_Move == null)
        {
            mcr_Move = StartCoroutine(IE_MoveUpdate());
        }
    }
     
    private void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        mf_axis = 0;
        bisMoving = false;
        if (mcr_Move != null)
        {
            m_rb.velocity = new Vector2(0, m_rb.velocity.y);
            StopCoroutine(mcr_Move);
            mcr_Move = null;
        }
    }

    IEnumerator IE_MoveUpdate()
    {
        while (mf_axis != 0)
        {
            m_rb.velocity = new Vector2(mf_axis * mf_moveSpeed, m_rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    #endregion

    #region Debug Tools
    private void OnDrawGizmos()
    {
        //Line To Ground (Unused)
        //Gizmos.DrawLine(m_CastPosition.position, m_CastPosition.position - Vector3.up);


        if (isGrounded) { Gizmos.color = Color.red; }
        else { Gizmos.color = Color.green; }
        Gizmos.DrawCube(m_CastPosition.position,new Vector3(.9f,0.2f,1));
        Gizmos.DrawCube(m_CastPosition.position + Vector3.up, new Vector3(0.9f, 0.2f,1));

        /*
        //Coyote Time Cast
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(m_CastPosition.position + new Vector3(0,1f), new Vector2(1.5f, 1.3f));
        */
    }
    #endregion
};