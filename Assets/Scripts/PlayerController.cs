using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//https://essssam.itch.io/rocky-roads

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    PlayerInput m_PlayerInput;
    Rigidbody2D m_rb;
    Collisions CC;

    [SerializeField] float mf_moveSpeed = 10.0f;
    [SerializeField] float mf_jumpForce = 10.0f;
    [SerializeField] public Transform KeyFollowPoint;
    bool bisMoving; public bool GetPlayerMoving() { return bisMoving; }
   
    public bool MovementLocked = false;

    bool isRising; public bool IsPlayerRising() { return isRising; }

    float mf_axis;
    float mf_Vert;


    Coroutine mcr_Move;
    Coroutine mcr_Fall;
    Coroutine mcr_Lerp;

    [SerializeField] CinemachineVirtualCamera Cam;
    [SerializeField] float CamZoomTime;



    AudioSource m_audioSource;
    [SerializeField] AudioClip JumpAudio;
    [SerializeField] AudioClip HurtAudio;
    [SerializeField] AudioClip HealAudio;




    Color SpriteOriginalColour ;

    
    HealthComponent HealthComp;
    CoyoteTimeScript CoyoteTimeComp;
    JumpBufferScript JumpBufferComp;
    DashScript DashComp;
    TimeSlowScript TimeSlowComp;
    ItemStorageScript ItemStorageComp;

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
        CC=GetComponentInChildren<Collisions>();

        m_audioSource = GetComponent<AudioSource>();
        SpriteOriginalColour = GetComponent<SpriteRenderer>().color;

        CoyoteTimeComp = GetComponent<CoyoteTimeScript>();
        JumpBufferComp = GetComponent<JumpBufferScript>();
        DashComp = GetComponent<DashScript>();

        HealthComp = GetComponent<HealthComponent>();
        if (HealthComp)
        {
            HealthComp.OnDeath += PlayerDead;
            HealthComp.OnHealthChangeVFX += OnHealthChange;
        }

        TimeSlowComp = GetComponent<TimeSlowScript>();
        if(TimeSlowComp)
            TimeSlowComp.OnTimeSlowed += ZoomCamera;
        ItemStorageComp = GetComponent<ItemStorageScript>();
    }

    private void OnHealthChange(bool IsHealing)
    {
        if (IsHealing)
        {
            StartCoroutine(SpriteFlash(Color.green));
            m_audioSource.PlayOneShot(HealAudio, .5f);
        }
        else
        {
            m_rb.AddForce(Vector2.up * 100f,ForceMode2D.Impulse);
            StartCoroutine(SpriteFlash(Color.white));
            m_audioSource.PlayOneShot(HurtAudio, 1);

        }
    }
    
    IEnumerator SpriteFlash(Color Colour)
    {
        SpriteRenderer Sprite = GetComponent<SpriteRenderer>();
        Sprite.color = Colour;
        yield return new WaitForSecondsRealtime(0.1f);
        Sprite.color = SpriteOriginalColour;
        yield break;
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

    #region Ground & Fall

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
        m_audioSource.PlayOneShot(JumpAudio, 0.3f);
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
        KeyFollowPoint.localPosition = new Vector2 (mf_axis*-1,0);
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

    IEnumerator IE_MoveUpdate()
    {
        while (mf_axis != 0)
        {
            if (!MovementLocked)
            {
                m_rb.velocity = new Vector2(mf_axis * mf_moveSpeed, m_rb.velocity.y);
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    #endregion

    //Abilities
    void Handle_SlowTimePerformed(InputAction.CallbackContext context)
    {
        if (TimeSlowComp)
            TimeSlowComp.SlowTime();
    }

    void ZoomCamera(float Time , bool ZoomIn)
    {
        if(mcr_Lerp != null)
        {
            StopCoroutine (mcr_Lerp);
            mcr_Lerp = null;
        }
        if(ZoomIn)
            mcr_Lerp = StartCoroutine(LerpCamView(3, 2));
        else
            mcr_Lerp = StartCoroutine(LerpCamView(5, 0.5f));

    }

    IEnumerator LerpCamView(float ZoomSize,float ZoomTime)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < ZoomTime)
        {
            Cam.m_Lens.OrthographicSize = Mathf.Lerp(Cam.m_Lens.OrthographicSize, ZoomSize, ElapsedTime/ZoomTime);
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        Cam.m_Lens.OrthographicSize = ZoomSize;
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (DashComp)
        {
            DashComp.Dash(new Vector2(mf_axis,mf_Vert),m_rb);
        }

    }

    void PlayerDead(bool IsPlayer)
    {
        if (IsPlayer)
        {
            
        }
    }
}