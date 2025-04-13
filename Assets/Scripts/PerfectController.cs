using System.Collections;
using UnityEngine;

public class PerfectController : MonoBehaviour
{
    [SerializeField]
    private CubeSpawner cubeSpawner;
    [SerializeField]
    private Transform perfectEffect;
    [SerializeField]
    private Transform perfectComboEffect;
    [SerializeField]
    private Transform perfectRecoveryEffect;

    private AudioSource audioSource;

    [SerializeField]
    private int recoveryCombo = 5; // 큐브의 크기를 증가시킬 수 있는 최소콤보
    private float perfectCorrection = 0.01f; // perfect로 인정하는 보정값
    private float addedSize = 0.1f; // 큐브의 크기를 증가시키는값
    private int perfectCombo = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public bool IsPerfect(float hangOver)
    {
        // perfect로 인정
        if (Mathf.Abs(hangOver) <= perfectCorrection)
        {
            // 이펙트 재생
            EffectProcess();
            // 사운드 재생
            SFXProcess();
            perfectCombo++;
            return true;
        }
        else
        {
            perfectCombo = 0;
            return false;
        }
    }

    private void EffectProcess()
    {
        // 이펙트 생성위치 설정
        Vector3 position = cubeSpawner.LastCube.position;
        position.y = cubeSpawner.CurrentCube.transform.position.y - cubeSpawner.CurrentCube.transform.localScale.y * 0.5f;

        // 이펙트 크기 설정
        Vector3 scale = cubeSpawner.CurrentCube.transform.localScale;
        scale = new Vector3(scale.x + addedSize, perfectEffect.localScale.y, scale.z + addedSize);

        // 퍼펙트 이펙트 생성
        OnPerfectEffect(position, scale);

        if (0 < perfectCombo && perfectCombo < recoveryCombo)
        {
            // 콤포이펙트 생성
            StartCoroutine(OnPerfectComboEffect(position, scale));
        }
        else if (recoveryCombo <= perfectCombo)
        {
            OnPerfectRecoveryEffect();
        }
    }

    private void OnPerfectEffect(Vector3 position, Vector3 scale)
    {
        // 이펙트 생성
        Transform effect = Instantiate(perfectEffect);
        effect.position = position;
        effect.localScale = scale;
    }

    private void SFXProcess()
    {
        int maxCombo = 5;
        float volumeMin = 0.3f;
        float volumeAdditive = 0.15f;
        float pitchMin = 0.7f;
        float pitchAdditive = 0.15f;

        if (perfectCombo < maxCombo)
        {
            // volume, pitch를 서서히 증가시킴
            audioSource.volume = volumeMin + perfectCombo * volumeAdditive;
            audioSource.pitch = pitchMin + perfectCombo * pitchAdditive;
        }

        audioSource.Play();
    }

    // 콤보횟수만큼 이펙트를 생성하는 코루틴
    private IEnumerator OnPerfectComboEffect(Vector3 position, Vector3 scale)
    {
        int currentCombo = 0;
        float beginTime = Time.time;
        float duration = 0.15f;

        while (currentCombo < perfectCombo)
        {
            float t = (Time.time - beginTime) / duration;

            if (t >= 1)
            {
                // 이펙트를 생성
                Transform effect = Instantiate(perfectComboEffect);
                effect.position = position;
                effect.localScale = scale;
                beginTime = Time.time;
                currentCombo++;
            }

            yield return null;
        }
    }

    private void OnPerfectRecoveryEffect()
    {
        // 이펙트 생성
        Transform effect = Instantiate(perfectRecoveryEffect);
        // 이펙트 생성위치
        effect.position = cubeSpawner.CurrentCube.transform.position;

        // 이펙트 생성반경 설정 (반지름, 두께?)
        var shape = effect.GetComponent<ParticleSystem>().shape;
        float radius = cubeSpawner.CurrentCube.transform.localScale.z < cubeSpawner.CurrentCube.transform.localScale.x ?
            cubeSpawner.CurrentCube.transform.localScale.x :
            cubeSpawner.CurrentCube.transform.localScale.z;

        shape.radius = radius;
        shape.radiusThickness = radius * 0.5f;

        // 이동큐브를 약간 재생시킴
        cubeSpawner.CurrentCube.RecoveryCube();
    }
}
