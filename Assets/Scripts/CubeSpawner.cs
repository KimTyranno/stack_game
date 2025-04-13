using UnityEngine;


public enum MoveAxis { x = 0, z }

public class CubeSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] cubeSpawnPoints; // 큐브 생성ㄷ위치 (x,y)

    [SerializeField]
    private Transform movingCubePrefab; // 이동큐브 프리팹
    [SerializeField]
    private PerfectController perfectController;

    [field: SerializeField]
    public Transform LastCube { set; get; } // 마지막에 생성한 큐브정보
    public MovingCube CurrentCube { set; get; } = null; // 현재 이동중인 큐브

    [SerializeField]
    private float colorWeight = 15.0f; // 색상의 비슷한정도 (값이 작을수록 더 비슷함)

    private int currentColorNumberOfTime = 5;
    private int maxColorNumberOfTime = 5;

    private MoveAxis moveAxis = MoveAxis.x; // 현재 이동축, cubeSpawnPoints 배열의 현재 인덱스

    public void SpawnCube()
    {
        // 이동큐브 생성
        Transform clone = Instantiate(movingCubePrefab);

        // 생성한 이동큐브의 위치
        // 첫 이동큐브 생성시
        if (LastCube == null || LastCube.name.Equals("StartCubeTop"))
        {
            // cubeSpawnerPoints의 위치를 그대로 사용함
            clone.position = cubeSpawnPoints[(int)moveAxis].position;
        }
        else
        {
            // xz축은 이동하는 방향과 동일한 축, 위치는 cubeSpawnerPoints의 위치를 사용하고 다른축은 LastCube의 위치를 사용
            // float x = cubeSpawnPoints[(int)moveAxis].position.x;
            // float z = cubeSpawnPoints[(int)moveAxis].position.z;
            float x = moveAxis == MoveAxis.x ? cubeSpawnPoints[(int)moveAxis].position.x : LastCube.position.x;
            float z = moveAxis == MoveAxis.z ? cubeSpawnPoints[(int)moveAxis].position.z : LastCube.position.z;

            // y축은 LastCube의 위치 + 프리팹의 y크기로 설정하여 마지막에 생성한 큐브보다 프리팹의 y크기 (0.1)만큼 더 높게 설정함
            float y = LastCube.position.y + movingCubePrefab.localScale.y;

            clone.position = new Vector3(x, y, z);
        }
        // 이동큐브의 크기를 설정
        clone.localScale = new Vector3(LastCube.localScale.x, movingCubePrefab.localScale.y, LastCube.localScale.z);

        // 이동큐브의 색상을 설정
        clone.GetComponent<MeshRenderer>().material.color = GetRandomColor();

        // 이동큐브의 이동방향을 전달
        clone.GetComponent<MovingCube>().Setup(this, perfectController, moveAxis);

        // cubeSpawnerPoints 배열의 인덱스를 변경함 (x는 0, z는 1)
        moveAxis = (MoveAxis)(((int)moveAxis + 1) % cubeSpawnPoints.Length);

        // LastCube = clone;
        // 이동큐브를 현재큐브로 설정
        CurrentCube = clone.GetComponent<MovingCube>();
    }

    // 생성위치를 보기위한 디버깅 역할 (삭제해도OK)
    private void onDrawGizmos()
    {
        for (int i = 0; i < cubeSpawnPoints.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(cubeSpawnPoints[i].transform.position, movingCubePrefab.localScale);
        }
    }
    private Color GetRandomColor()
    {
        Color color = Color.white;

        if (0 < currentColorNumberOfTime)
        {
            // 현재 색상에서 비슷한색상으로 변경
            float colorAmount = (1.0f / 255.0f) * colorWeight;
            color = LastCube.GetComponent<MeshRenderer>().material.color;
            color = new Color(color.r - colorAmount, color.g - colorAmount, color.b - colorAmount);

            currentColorNumberOfTime--;
        }
        else
        {
            // 완전 새로운 색상으로 변경
            color = new Color(Random.value, Random.value, Random.value);
            currentColorNumberOfTime = maxColorNumberOfTime;
        }
        return color;
    }
}
