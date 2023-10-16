using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput m_PlayerInput;

    [SerializeField] float mf_moveSpeed = 10.0f;

    [SerializeField] float mf_jumpForce = 50.0f;
    //[SerializeField] float mf_jumpBufferTime = 0.2f;
    [SerializeField] float mf_coyoteTime = 0.25f;

    [SerializeField] float mf_CastRadius = 0.1f;
    [SerializeField] Transform m_CastPosition;
    [SerializeField] LayerMask m_LayerMask;

    Rigidbody2D m_rb;
    bool isGrounded;
    public bool isMoving;
    float mf_axis;

    [SerializeField] public bool KeyHeld = true;

    Coroutine mcr_MoveCoroutine;

    bool CoyoteTime = false;
    bool JumpBuffer = false;

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        m_PlayerInput.actions.FindAction("Jump").performed += Jump;
        m_PlayerInput.actions.FindAction("Move").performed += Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled += Handle_MoveCancelled;
    }

    private void OnDisable()
    {
        m_PlayerInput.actions.FindAction("Jump").performed -= Jump;
        m_PlayerInput.actions.FindAction("Move").performed -= Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled -= Handle_MoveCancelled;
    }

    // Update is called once per frame (Very Expensive)
    void FixedUpdate()
    {
        isGrounded = Physics2D.CircleCast(m_CastPosition.position, mf_CastRadius, Vector2.zero, 0, m_LayerMask);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable Interface = collision.GetComponent<IInteractable>();
        if (Interface != null)
        {
            Interface.OnInteract(gameObject);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded || CoyoteTime || JumpBuffer)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x, 0);
            m_rb.AddForce(Vector2.up * mf_jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            if (m_rb.velocity.y < 0)
            {
                CoyoteTime = true;
                StartCoroutine(IE_CoyoteTime());
            }
            else { CoyoteTime = false;}
        }
    }

    IEnumerator IE_CoyoteTime()
    {
        yield return new WaitForSeconds(mf_coyoteTime);
        CoyoteTime = false;
    }

    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        mf_axis = context.ReadValue<float>();
        isMoving = true;
        if (mcr_MoveCoroutine == null)
        {
            mcr_MoveCoroutine = StartCoroutine(IE_MoveUpdate());
        }
    }

    private void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        mf_axis = 0;
        isMoving = false;
        if (mcr_MoveCoroutine != null)
        {
            StopCoroutine(mcr_MoveCoroutine);
            mcr_MoveCoroutine = null;
        }
    }

    IEnumerator IE_MoveUpdate()
    {
        while (mf_axis != 0)
        {
            m_rb.velocity = new Vector2(mf_axis * mf_moveSpeed, m_rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_CastPosition.position, mf_CastRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_CastPosition.position, mf_CastRadius);
            Gizmos.color = Color.blue;
            //Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y,0), new Vector3(transform.position.x, transform.position.y - mf_jumpBufferDist, 0));
        }
    }
}
