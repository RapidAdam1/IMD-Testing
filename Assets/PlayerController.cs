using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] float mf_moveSpeed = 10.0f;
    [SerializeField] float mf_jumpForce = 50.0f;
    [SerializeField] float mf_CastRadius = 0.1f;
    [SerializeField] Transform m_CastPosition;
    [SerializeField] LayerMask m_LayerMask;

    Rigidbody2D m_rb;
    bool isGrounded;
    float mf_axis;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics2D.CircleCast(m_CastPosition.position, mf_CastRadius, Vector2.zero, 0, m_LayerMask);
        m_rb.velocity = new Vector2(mf_axis * mf_moveSpeed, m_rb.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            m_rb.AddForce(Vector2.up * mf_jumpForce);
        }
    }
    public void Move(InputAction.CallbackContext Context)
    {
        if (Context.performed)
        {
           mf_axis = Context.ReadValue<float>();
        }
        if (Context.canceled)
        {
            mf_axis = 0;
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
        }
    }
}
