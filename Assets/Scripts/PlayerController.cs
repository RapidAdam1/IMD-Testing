using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//https://essssam.itch.io/rocky-roads

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    PlayerInput m_PlayerInput;
    public Rigidbody2D m_rb;
    CornerCorrection CC;

    [SerializeField] float mf_moveSpeed = 10.0f;
    [SerializeField] float mf_jumpForce = 10.0f;
    [SerializeField] float mf_DashForce = 10.0f;
    [SerializeField] int m_DashCount = 1;
    int m_Dashes = 1;

    [SerializeField] Transform m_CastPosition;
    [SerializeField] LayerMask m_LayerMask;

    public bool bisMoving;
    bool MovementLocked = false;

    bool bJumpBuffer;
    bool bCoyoteTime;
    bool isGrounded;
    bool isRising; public bool IsPlayerRising() { return isRising; }

    float mf_axis;
    float mf_Vert;
    float mf_coyoteTime = 0.2f;
    float mf_JumpBufferTime = 0.25f;

    public bool KeyHeld = true;

    Coroutine mcr_Move;
    Coroutine mcr_JumpBuff;
    Coroutine mcr_Fall;

    bool IsMoving;
    bool JumpBuff;


    private void Update()
    {
        JumpBuff = mcr_JumpBuff != null;
        IsMoving = mcr_Move != null;

    }

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
        CC=GetComponentInChildren<CornerCorrection>();
    }

    #region Bindings
    private void OnEnable()
    {
        m_PlayerInput.actions.FindAction("Jump").performed += Jump;
        m_PlayerInput.actions.FindAction("Jump").canceled += Jump;

        m_PlayerInput.actions.FindAction("Dash").performed += Dash;
        m_PlayerInput.actions.FindAction("Vertical").performed += VerticalRead;
        m_PlayerInput.actions.FindAction("Vertical").canceled += VerticalRead;


        m_PlayerInput.actions.FindAction("Move").performed += Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled += Handle_MoveCancelled;

    }

    private void OnDisable()
    {
        m_PlayerInput.actions.FindAction("Jump").performed -= Jump;
        m_PlayerInput.actions.FindAction("Jump").canceled -= Jump;

        m_PlayerInput.actions.FindAction("Dash").performed -= Dash;
        m_PlayerInput.actions.FindAction("Vertical").performed -= VerticalRead;
        m_PlayerInput.actions.FindAction("Vertical").canceled -= VerticalRead;



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

 

    public void OnGrounded()
    {
        m_Dashes = m_DashCount;
        if(bJumpBuffer)
            DoJumpBuffer();

        if (mcr_Fall != null)
        {
            StopCoroutine(IE_AirChecks());
            mcr_Fall = null;
        }
        if (mcr_JumpBuff != null)
        {
            StopCoroutine(IE_JumpBuffer());
            mcr_JumpBuff = null;
        }

        CC.EnableMainCollider(true);

    }

    void DoJumpBuffer()
    {
        m_rb.gravityScale = 1;
        m_rb.velocity = new Vector2(m_rb.velocity.x, 0);
        m_rb.AddForce(Vector2.up * mf_jumpForce, ForceMode2D.Impulse);
        CC.EnableMainCollider(false);
    }
    public void StartFall()
    {
        if (mcr_Fall == null)
        {
            mcr_Fall = StartCoroutine(IE_AirChecks());
        }
    }

    bool CoyoteCollisionCheck()
    {
        return !Physics2D.BoxCast(m_CastPosition.position + new Vector3(0, 1f), new Vector2(1.5f, 1.3f), 0, Vector2.zero, 0, m_LayerMask);
    }
    public bool GroundCheck()
    {
        isGrounded = Physics2D.BoxCast(m_CastPosition.position, new Vector2(.9f, 0.2f), 0, Vector2.zero, 0, m_LayerMask);
        if(isGrounded)
        {
            OnGrounded();
        }
        return isGrounded;
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
        if (GroundCheck() || bCoyoteTime)
        {
            m_rb.gravityScale = 1;
            m_rb.velocity = new Vector2(m_rb.velocity.x, 0);
            m_rb.AddForce(Vector2.up * mf_jumpForce, ForceMode2D.Impulse);
            CC.EnableMainCollider(false);

            if (mcr_Fall == null) 
                mcr_Fall = StartCoroutine(IE_AirChecks()); 

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
        StartCoroutine(IE_CoyoteTime());
        while (!GroundCheck())
         {
            //SpeedApex
            m_rb.velocity = new Vector2(m_rb.velocity.x, Mathf.Clamp(m_rb.velocity.y, -6.2f, 100));
            isRising = m_rb.velocity.y > 0;
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
            if (mcr_Move != null)
            {
                StopCoroutine(mcr_Move);
                mcr_Move = null;
            }
        }
    }
    IEnumerator IE_MoveUpdate()
    {
        while (mf_axis != 0)
        {
            if(!MovementLocked)
            {
                m_rb.velocity = new Vector2(mf_axis * mf_moveSpeed, m_rb.velocity.y);
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    void VerticalRead(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mf_Vert = context.ReadValue<float>();
        }
        else if(context.canceled){ mf_Vert = 0; }
    }

    void Dash(InputAction.CallbackContext context)
    {
        if(m_Dashes == 0 || MovementLocked)
        {
            return;
        }
        if (mf_axis != 0 || mf_Vert != 0 )
        {
            m_rb.velocity = new Vector2(0, 0);
            m_rb.AddForce(new Vector2(mf_axis, mf_Vert/2) * mf_DashForce, ForceMode2D.Impulse);
            StartCoroutine(IE_Dash());
            m_Dashes -= 1;
        }
    }
    IEnumerator IE_Dash()
    {
        MovementLocked = true;
        m_rb.gravityScale = 0;
        yield return new WaitForSeconds(0.15f);
        m_rb.velocity = new Vector2(0, m_rb.velocity.y);
        MovementLocked = false;
        m_rb.gravityScale = 1;

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
      //  Gizmos.DrawCube(m_CastPosition.position,new Vector3(.9f,0.2f,1));
      //  Gizmos.DrawCube(m_CastPosition.position + Vector3.up, new Vector3(0.9f, 0.2f,1));

        /*
        //Coyote Time Cast
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(m_CastPosition.position + new Vector3(0,1f), new Vector2(1.5f, 1.3f));
        */
    }
    #endregion
};