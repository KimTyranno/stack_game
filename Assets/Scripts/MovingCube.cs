using UnityEngine;

public class MovingCube : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.5f;
    private Vector3 moveDirection;

    private CubeSpawner cubeSpawner;
    private MoveAxis moveAxis;
    private PerfectController perfectController;

    public void Setup(CubeSpawner cubeSpawner, PerfectController perfectController, MoveAxis moveAxis)
    {
        this.cubeSpawner = cubeSpawner;
        this.perfectController = perfectController;
        this.moveAxis = moveAxis;

        if (moveAxis == MoveAxis.x) moveDirection = Vector3.left;
        else if (moveAxis == MoveAxis.z) moveDirection = Vector3.back;
    }

    private void Update()
    {
        // -1.5 ~ 1.5 위치를 왕복으로 움직임
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveAxis == MoveAxis.x)
        {
            if (transform.position.x <= -1.5f) moveDirection = Vector3.right;
            else if (transform.position.x >= 1.5f) moveDirection = Vector3.left;
        }
        else if (moveAxis == MoveAxis.z)
        {
            if (transform.position.z <= -1.5f) moveDirection = Vector3.forward;
            else if (transform.position.z >= 1.5f) moveDirection = Vector3.back;
        }
    }

    // 두개의 큐브(CurrentCube, LastCube)의 겹치지않는 크기를 검사
    // 게임오버, 퍼펙트도 검사함
    public bool Arrangement()
    {
        // 이동중인 큐브를 정지시킴
        moveSpeed = 0;

        float hangOver = GetHangOver();

        if (IsGameOver(hangOver)) return true;

        // 퍼펙트검사
        bool isPerfect = perfectController.IsPerfect(hangOver);

        // 퍼펙트가 아니면 조각큐브를 생성
        if (!isPerfect)
        {
            // x축 이동일때 hangOver가 양수면 오른쪽에 겹치지 않는부분이 있다는뜻
            // 음수면 왼쪽에 겹치지 않는부분이 있다는뜻
            float direction = hangOver >= 0 ? 1 : -1;

            if (moveAxis == MoveAxis.x)
            {
                SplitCubeOnX(hangOver, direction);
            }
            else if (moveAxis == MoveAxis.z)
            {
                SplitCubeOnZ(hangOver, direction);
            }
        }

        // 가장 상단의 큐브로 저장
        cubeSpawner.LastCube = this.transform;

        return false;
    }

    // 겹치지 않는부분이 있는지 검사하여 그 값을 반환
    private float GetHangOver()
    {
        float amount = 0;

        if (moveAxis == MoveAxis.x)
        {
            amount = transform.position.x - cubeSpawner.LastCube.transform.position.x;
        }
        else if (moveAxis == MoveAxis.z)
        {
            amount = transform.position.z - cubeSpawner.LastCube.transform.position.z;
        }

        return amount;
    }

    private void SplitCubeOnX(float hangOver, float direction)
    {
        // 이동큐브의 새로운 위치와 크기
        // (hangOver / 2)인 이유는 반대편도 큐브가 부족하게 갔기때문에 /2를 해줘야 위치가 중앙으로 가기때문인듯
        float newXPosition = transform.position.x - (hangOver / 2);
        float newXSize = transform.localScale.x - Mathf.Abs(hangOver);

        // 이동큐브의 위치와 크기를 설정
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);

        // 이동큐브와 조각큐브의 경계위치를 구함
        // TODO: transform.localScale.x / 2에 대해서는 따로 박스 생성해서 원리를 확인 해봐야할듯
        float cubeEdge = transform.position.x + (transform.localScale.x / 2 * direction);
        // 조각큐브의 위치와 크기
        float fallingBlockSize = Mathf.Abs(hangOver);
        float fallingBlockPosition = cubeEdge + fallingBlockSize / 2 * direction;

        // 조각큐브를 생성
        SpawnDropCube(fallingBlockPosition, fallingBlockSize);
    }
    private void SplitCubeOnZ(float hangOver, float direction)
    {
        float newZPosition = transform.position.z - (hangOver / 2);
        float newZSize = transform.localScale.z - Mathf.Abs(hangOver);

        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);

        float cubeEdge = transform.position.z + (transform.localScale.z / 2 * direction);
        float fallingBlockSize = Mathf.Abs(hangOver);
        float fallingBlockPosition = cubeEdge + fallingBlockSize / 2 * direction;

        SpawnDropCube(fallingBlockPosition, fallingBlockSize);
    }

    // 조각큐브를 생성하는 메소드
    private void SpawnDropCube(float fallingBlockPosition, float fallingBlockSize)
    {
        GameObject clone = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (moveAxis == MoveAxis.x)
        {
            clone.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z);
            clone.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z);
        }
        else if (moveAxis == MoveAxis.z)
        {
            clone.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);
            clone.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize);
        }

        // 동일한 색상으로 설정
        clone.GetComponent<MeshRenderer>().material.color = GetComponent<MeshRenderer>().material.color;

        // 중력을 받아 아래로 떨어지도록 설정
        clone.AddComponent<Rigidbody>();

        // 2초뒤 삭제시킴
        Destroy(clone, 2);
    }

    private bool IsGameOver(float hangOver)
    {
        // hangOver가 LastCube보다 큰 경우 아예 겹치지 않는다(?)는 것이므로 게임오버로 처리함.
        float max = moveAxis == MoveAxis.x ? cubeSpawner.LastCube.transform.localScale.x : cubeSpawner.LastCube.transform.localScale.z;

        if (Mathf.Abs(hangOver) > max) return true;

        return false;
    }

    public void RecoveryCube()
    {
        float revoerySize = 0.1f;

        if (moveAxis == MoveAxis.x)
        {
            float newXPosition = transform.position.x + revoerySize * 0.5f;
            float newXSize = transform.localScale.x + revoerySize;

            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            float newZPosition = transform.position.z + revoerySize * 0.5f;
            float newZSize = transform.localScale.z + revoerySize;

            transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        }
    }
}
