using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public LayerMask layerMask; // 체크할 레이어
    public GameObject curInteractGameObject;
    public float maxCheckDistance = 4f; // 최대 체크 거리
    public float checkRate = 0.05f; // 체크 주기
    private float lastCheckTime; // 마지막 체크 시간

    public IInteractable curInteractable;
    public Transform interactTransform;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = new Ray(interactTransform.position, interactTransform.forward);
            RaycastHit hit;

            // 인터렉트 오브젝트 갱신
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    promptText.gameObject.SetActive(true);
                    promptText.text = curInteractable.GetInteractPrompt();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    // 상호작용 키 입력 처리
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.Interact();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
