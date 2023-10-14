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
    float DesiredOrthoSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        playerController = PlayerObject.GetComponent<PlayerController>();
        m_PlayerRb = playerController.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        DefaultOrthoSize = cam.orthographicSize;
    }

    private void Update()
    {
        m_CameraOffset = m_PlayerRb.velocity * new Vector2(LookAheadXDistance,LookAheadYDistance);
        if(m_PlayerRb.velocity.y > -4)
        {
            m_CameraOffset.y += VerticalOffset;
            DesiredOrthoSize = DefaultOrthoSize;
        }
        else { DesiredOrthoSize = OrthoSizeMaxZoom; }

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_CameraOffset, LookAheadSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,DesiredOrthoSize,2*Time.deltaTime);
    }

    IEnumerator UpdateCameraOffset()
    {
        while (true)
        {
            LerpValue(1, 2, 4);
        transform.localPosition = m_CameraOffset;
        }
        yield return null;
    }

    void LerpValue(float StartValue, float EndValue, float InTime)
    {
        float TempTime = 0;
        while (TempTime < InTime)
        {
            Mathf.Lerp(StartValue, EndValue, TempTime / InTime);
            TempTime += Time.smoothDeltaTime;
        }
    }
}
