using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CubeSpawner cubeSpawner;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private UIController uIController;

    private bool isGameStart = false;
    private int currentScore = 0;

    // 코루틴으로 정의함
    private IEnumerator Start()
    {
        while (true)
        {
            // 퍼펙트 확인용코드
            // if (Input.GetMouseButtonDown(1))
            // {
            //     if (cubeSpawner.CurrentCube != null)
            //     {
            //         cubeSpawner.CurrentCube.transform.position = cubeSpawner.LastCube.position + Vector3.up * 0.1f;
            //         cubeSpawner.CurrentCube.Arrangement();
            //         currentScore++;
            //         uIController.UpdateScore(currentScore);
            //     }
            //     cameraController.MoveOneStep();
            //     cubeSpawner.SpawnCube();
            // }
            // 마우스 좌클릭
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (isGameStart == false)
                {
                    isGameStart = true;
                    uIController.GameStart();
                }
                // 큐브의 이동을 중지시킴
                if (cubeSpawner.CurrentCube != null)
                {
                    bool isGameOver = cubeSpawner.CurrentCube.Arrangement();
                    if (isGameOver)
                    {
                        // OnGameOver();
                        cameraController.gameOverAnimation(cubeSpawner.LastCube.position.y, OnGameOver);
                        yield break;
                    }

                    // 점수증가
                    currentScore++;
                    uIController.UpdateScore(currentScore);
                }
                // 카메라의 y위치를 이동
                cameraController.MoveOneStep();

                // 이동큐브를 생성
                cubeSpawner.SpawnCube();
            }
            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ESC를 눌렀을때
            Application.Quit(); //게임/앱 종료.
        }
    }

    private void OnGameOver()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");

        if (highScore < currentScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            uIController.GameOver(true);
        }
        else
        {
            uIController.GameOver(false);
        }

        StartCoroutine("AfterGameOver");
    }

    private IEnumerator AfterGameOver()
    {
        yield return new WaitForEndOfFrame();

        // 마우스 버튼이 올라갈 때까지 대기 (PC에서 확인할때 초기화면으로 바로 넘어가는거 방지)
        while (Input.GetMouseButton(0))
        {
            yield return null;
        }

        while (true)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            yield return null;
        }
    }
}
