using System.Collections;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.8f;
    private MeshRenderer target;

    private void Awake()
    {
        target = GetComponent<MeshRenderer>();
    }

    private IEnumerator Start()
    {
        float current = 0;
        float percent = 0;

        float start = target.material.color.a;
        float end = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / duration;

            // 알파값을 서서히 바꿈
            Color color = target.material.color;
            color.a = Mathf.Lerp(start, end, percent);
            target.material.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
