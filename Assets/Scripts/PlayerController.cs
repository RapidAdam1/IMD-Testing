using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//https://essssam.itch.io/rocky-roads

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    PlayerInput m_PlayerInput;
    Rigidbody2D m_rb;
    CornerCorrection CC;

    [SerializeField] float mf_moveSpeed = 10.0f;
    [SerializeField] float mf_jumpForce = 10.0f;

    public bool bisMoving;
    public bool MovementLocked = false;

    bool isRising; public bool IsPlayerRising() { return isRising; }

    float mf_axis;
    float mf_Vert;


    public bool KeyHeld = true;

    Coroutine mcr_Move;
    Coroutine mcr_Fall;

    CoyoteTimeScript CoyoteTimeComp;
    JumpBufferScript JumpBufferComp;
    DashScript DashComp;
    TimeSlowScript TimeSlowComp;


    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
        CC=GetComponentInChildren<CornerCorrection>();

        CoyoteTimeComp = GetComponent<CoyoteTimeScript>();
        JumpBufferComp = GetComponent<JumpBufferScript>();
        DashComp = GetComponent<DashScript>();
        TimeSlowComp = GetComponent<TimeSlowScript>();
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

        m_PlayerInput.actions.FindAction("SlowTime").performed += Handle_SlowTimePerformed;

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

        m_PlayerInput.actions.FindAction("SlowTime").performed -= Handle_SlowTimePerformed;
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
        if(JumpBufferComp.GetJumpBufferActivated())
            PlayerJump();

        if (mcr_Fall != null)
        {
            StopCoroutine(IE_AirChecks());
            mcr_Fall = null;
            if(DashComp)
                DashComp.ResetDashes();
        }

        if (JumpBufferComp)
            JumpBufferComp.StopJumpBuffer();

    }



    public void StartFall()
    {
        if (mcr_Fall == null)
        {
            mcr_Fall = StartCoroutine(IE_AirChecks());
        }
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
        if (CC.isGrounded || CoyoteTimeComp.GetCTEnabled())
        {
            PlayerJump();
            if (mcr_Fall == null) 
                mcr_Fall = StartCoroutine(IE_AirChecks()); 
        }
        else
        {
            if (JumpBufferComp)
                JumpBufferComp.StartJumpBuffer(CC.CanJumpBuffer());
  
        }
    }
    public void PlayerJump()
    {
        m_rb.gravityScale = 1;
        m_rb.velocity = new Vector2(m_rb.velocity.x, 0);
        m_rb.AddForce(Vector2.up * mf_jumpForce, ForceMode2D.Impulse);
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

    IEnumerator IE_AirChecks()
    {
        CoyoteTimeComp.StartCoyoteTime(true);
        while (!CC.isGrounded)
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
            mcr_Move = StartCoroutine(IE_MoveUpdate());
    }
     
    private void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        mf_axis = 0;
        bisMoving = false;
        if (mcr_Move == null)
            return;

        m_rb.velocity = new Vector2(0, m_rb.velocity.y);
        if (mcr_Move != null)
        {
            StopCoroutine(mcr_Move);
            mcr_Move = null;
        }
     
    }
    void VerticalRead(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mf_Vert = context.ReadValue<float>();
        }
        else if (context.canceled) { mf_Vert = 0; }
    }

    void Handle_SlowTimePerformed(InputAction.CallbackContext context)
    {
        if (TimeSlowComp)
            TimeSlowComp.SlowTime();
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (DashComp)
            DashComp.Dash(new Vector2(mf_axis,mf_Vert),m_rb);
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


    #endregion


    #region Debug Tools
    private void OnDrawGizmos()
    {
        //Line To Ground (Unused)
        //Gizmos.DrawLine(m_CastPosition.position, m_CastPosition.position - Vector3.up);


/*        if (CC.isGrounded) { Gizmos.color = Color.red; }
        else { Gizmos.color = Color.green; }
        Gizmos.DrawCube(CC.m_CastPosition.position, new Vector3(.9f, 0.2f, 1));
        Gizmos.DrawCube(CC.m_CastPosition.position + Vector3.up, new Vector3(0.9f, 0.2f, 1));
*/
        /*
        //Coyote Time Cast
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(m_CastPosition.position + new Vector3(0,1f), new Vector2(1.5f, 1.3f));
        */
    }
    #endregion
};