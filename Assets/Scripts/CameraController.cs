using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [Header("카메라 이동 파라미터")]
    [SerializeField]
    private float moveDistance = 0.1f;

    [SerializeField]
    private float oneStepMoveTime = 0.25f;

    [Header("GameOver 파라미터")]
    [SerializeField]
    private float gameOverAnimationTime = 1.5f;
    [SerializeField]
    private float limitMinY = 4; // 애니메이션 재생시 LastCube의 최소Y위치
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void MoveOneStep()
    {
        // 현재 위치에서 이동큐브의 y크기(0.1)만큼 위로 이동시킴
        Vector3 start = transform.position;
        Vector3 end = transform.position + Vector3.up * moveDistance;

        StartCoroutine(OnMoveTo(start, end, oneStepMoveTime));
    }

    // 카메라를 time만큼 이동시키는 코루틴
    private IEnumerator OnMoveTo(Vector3 start, Vector3 end, float time)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }
    }

    // 카메라의 orthographicSize를 time만큼 서서히 변화시킴
    private IEnumerator OnOrthographicSizeTo(float start, float end, float time, UnityAction action)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            mainCamera.orthographicSize = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        if (action != null) action.Invoke();
    }

    public void gameOverAnimation(float lastCubeY, UnityAction action = null)
    {
        // 애니메이션을 재생하지 않음
        if (limitMinY > lastCubeY)
        {
            // action을 호출
            if (action != null) action.Invoke();

            return;
        }

        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, lastCubeY + 1, transform.position.z);
        // 카메라를 이동시킴
        StartCoroutine(OnMoveTo(startPosition, endPosition, gameOverAnimationTime));

        // 카메라 View 크기를 설정
        float startSize = mainCamera.orthographicSize;
        float endSize = lastCubeY - 1;
        // 카메라 View 크기변경
        StartCoroutine(OnOrthographicSizeTo(startSize, endSize, gameOverAnimationTime, action));
    }
}
