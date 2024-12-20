using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 orignalPosition;

    private void Awake()
    {
        orignalPosition = transform.localPosition;
    }

    private IEnumerator ShakeEffect(float durating, float magnitude)
    {
        float elapsed = 0.0f;

        while(elapsed < durating)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(orignalPosition.x + offsetX, orignalPosition.y +offsetY, orignalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = orignalPosition;
    }

    public void Shake(float durating, float magnitude)
    {
        StartCoroutine(ShakeEffect(durating, magnitude));
    }

    public void CameraTransUpdate()
    {
        orignalPosition = transform.localPosition;
    }
}
