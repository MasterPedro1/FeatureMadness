using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Handler : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask raycastLayer;
    public GameObject hand;
    public GameObject square;
    public float raycastDistance = 10f; // Longitud máxima del raycast
    public float scalingTime = 1f; // Tiempo en segundos para reducir la escala a 0

    private Coroutine scalingCoroutine;

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        bool isHit = Physics.Raycast(ray, out hit, raycastDistance, raycastLayer);

        hand.SetActive(isHit && (hit.collider.CompareTag("Grab") || hit.collider.CompareTag("Toc")));
        square.SetActive(!isHit || (!hit.collider.CompareTag("Grab") && !hit.collider.CompareTag("Toc")));

        if (isHit && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Grab");

            if (hit.collider.CompareTag("Toc"))
            {
                // Detiene la corutina anterior si existe
                if (scalingCoroutine != null)
                {
                    StopCoroutine(scalingCoroutine);
                }

                // Inicia la corutina para reducir gradualmente la escala a 0
                scalingCoroutine = StartCoroutine(ScaleObjectToZero(hit.collider.transform));
            }
        }
    }

    private IEnumerator ScaleObjectToZero(Transform objectTransform)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = objectTransform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (elapsedTime < scalingTime)
        {
            objectTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scalingTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegura que la escala sea exactamente cero al final de la corutina
        objectTransform.localScale = targetScale;
    }
}
