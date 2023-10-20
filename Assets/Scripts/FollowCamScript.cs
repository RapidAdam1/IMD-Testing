using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamScript : MonoBehaviour
{
    [SerializeField] GameObject PlayerObject;
    PlayerController playerController;
    Rigidbody2D m_PlayerRb;
    Camera cam;

    [SerializeField] float LookAheadXDistance = 1.3f;
    [SerializeField] float LookAheadYDistance = 0.5f;
    [SerializeField] float LookAheadSpeed = 2.0f;
    const float VerticalOffset = 2.5f;
    Vector3 m_CameraOffset;

    [SerializeField] float OrthoSizeMaxZoom;
    float DefaultOrthoSize = 5;
    float DesiredOrthoSize = 5;

    Coroutine CamUpdate;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        playerController = PlayerObject.GetComponent<PlayerController>();
        m_PlayerRb = playerController.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        DefaultOrthoSize = cam.orthographicSize;
        CameraHandle(true);
    }

    void CameraHandle(bool Update)
    {
        if (CamUpdate == null && Update)
        {
            CamUpdate = StartCoroutine(UpdateCamera());
        }
        else if (!Update)
        {
            StopCoroutine(UpdateCamera());
            CamUpdate = null;
        }
    }

    IEnumerator UpdateCamera()
    {
        while (true)
        {
            if (playerController.bisMoving) { m_CameraOffset.x = Mathf.Clamp(m_PlayerRb.velocity.x * LookAheadXDistance, -LookAheadXDistance, LookAheadXDistance); }
            else m_CameraOffset.x = 0;

            if (m_PlayerRb.velocity.y < -4)
            {
                m_CameraOffset.y = Mathf.Clamp(m_PlayerRb.velocity.y * LookAheadYDistance, -LookAheadYDistance, VerticalOffset);
                DesiredOrthoSize = OrthoSizeMaxZoom;
            }
            else
            {
                m_CameraOffset.y = VerticalOffset;
                DesiredOrthoSize = DefaultOrthoSize;
            }
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, DesiredOrthoSize, 0.5f * Time.deltaTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_CameraOffset, LookAheadSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    
}
