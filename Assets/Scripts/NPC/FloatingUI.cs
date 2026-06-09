using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public Transform targetNPC;         // NPC to follow
    private Vector3 offset = new Vector3(0f, 1.5f, 0f); // above NPC
    public float smoothSpeed = 5f;
    private Camera mainCam;


    void Awake()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (targetNPC == null || mainCam == null) return;

        // Smoothly follow NPC
        Vector3 desiredPos = targetNPC.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // Make canvas face the camera directly
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }
}
