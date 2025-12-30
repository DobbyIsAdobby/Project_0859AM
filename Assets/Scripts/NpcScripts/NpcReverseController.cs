using UnityEngine;

public class NpcReverseController : MonoBehaviour
{
    [Header("NPC Target")]
    public Transform playerTarget;

    [Header("Wheels")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Npc Sys")]
    public float motorForce = 2000f;
    public float maxSteerAngle = 45f;

    [Header("NPC Turning")]
    [Range(0.1f, 1f)] //인스펙터에서 슬라이더로 조정할 수 있게 지정
    public float steerSensitivity = 0.5f; //조향 민감도 설정

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //프리팹으로 지정하고 generator.cs로 만들어서 하면 플레이어를 인식하지 못한다..이유를 곰곰히 생각해보면
        //플레이어는 현재 씬에만 존재한다. npc는 프로젝트 폴더의 프리팹에 있다. 유니티는 씬에만 존재하는 게임오브젝트를 미리 저장해둘 수 없다는 절대적인 규칙이있다..?
        //따라서 시작할때는 이 프리팹이 씬상에 올라가있으니 이 규칙의 제약에서 풀려(?)나게 된다. 따라서 이때 태그가 플레이어인 오브젝트를 찾으라하면 되지 않을까? -> 이게 정답이였다.
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTarget == null) return; // 목표가 없으면 아무것도 하지마셈

        FollowTarget();
        HandleSteering(playerTarget.position);
    }

    private void FollowTarget()
    {
        frontLeftWheel.motorTorque = motorForce;
        frontRightWheel.motorTorque = motorForce;
        rearLeftWheel.motorTorque = motorForce;
        rearRightWheel.motorTorque = motorForce;
    }

    private void HandleSteering(Vector3 targetPosition)
    {
        // 적대NPC와 목표물 사이의 방향 벡터 계산
        Vector3 directionToTarget = targetPosition - transform.position;
        // 적대NPC의 전방 벡터와 목표 방향 벡터 사이의 각도 계산
        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);

        // 계산된 각도를 조향각으로 변환, Mathf.Clamp로 최대 각도를 넘지 않도록 제한시키기
        //float steerAmount = Mathf.Clamp(angle, maxSteerAngle, -maxSteerAngle);

        //민감도를 곱하여 조향량을 조절하기
        float steerAmount = angle * steerSensitivity;

        steerAmount *= -1; //방향 조정.. 플레이어와 반대로 날아가는 이유가 이거였음.

        //조향량이 최대 각도를 넘어가지 않도록 제한
        steerAmount = Mathf.Clamp(steerAmount, -maxSteerAngle, maxSteerAngle);

        frontLeftWheel.steerAngle = steerAmount;
        frontRightWheel.steerAngle = steerAmount;
    }
}
