using System;
using UnityEngine;

public class NpcCheekyController : MonoBehaviour
{
    /*
        얌체운전? 보복운전?은 보통 내 차 앞에 갑작스레 끼어들고 급정거를 진행하는 상황을 뜻합니다.
        이에 enum형으로 Chasing, Postioning, CuttingIn, Braking으로 
        총 4가지의 상황을 기준점으로 삼았습니다.
    */
    private enum NpcState
    {
        Chasing, //1. 플레이어를 추격중
        Postioning, //2. 끼어들기 위해 플레이어 옆/앞으로 위치 잡기
        CuttingIn, //3. 앞으로 끼어드는중
        Braking //4. 급정거하는중
    };

    [Header("Npc Target")]
    public Transform playerTarget;

    [Header("Wheels")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Npc Sys")]
    public float motorForce = 1000f;
    //public float maxSteerAngle = 20f;
    public float brakeForce = 5000f; //급정거를 위해 추가
    public float maxSpeed = 100f; //최고 속도 변수 추가 (속도 감응형 시스템 구축)

    public float boostMotorForce = 4000f;

    [Header("Npc Turning")]
    [Range(0.1f, 1f)] //정규화시켜 좌우 회전을 판단.
    public float steerSensitivity = 0.5f;

    //플레이어에 있던 변수 가져옴
    [Header("Steering Sensitivity")]
    public float lowSpeedSteerAngle = 12f; //저속 최대 조향 각도
    public float highSpeedSteerAngle = 5f; //고속 최대 조향 각도
    public float speedThreshold = 50f; //고속 판단 기준

    //Cheeky 전용 변수
    [Header("Cheeky Npc Stats")]
    public float cutInDistance = 20f; //이 거리 안에 들어오면 끼어들기 시도
    //상하좌우 거리 조절을 위한 변수 세개 추가
    public float positioningOffset = 10f; //플레이어보도 10f만큼 앞으로 가는게 목표
    public float cutInTriggerDistance = 5f; //5f만큼 가까워지면 끼어들기 시작
    public float overtakeLaneWidth = 2f; //옆으로 벌릴 거리, TODO : 실제 맵디자인 후 맵 가로 길이에 따라 조정해야함.

    private NpcState currentState;
    private Rigidbody rb;
    private float overtakeSide = 0f; //추월 방향 확인을 위한 변수 추가


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = NpcState.Chasing; //초기 상태는 추적으로 지정

        //타겟이 설정 안됐을 경우
        if (playerTarget == null)
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTarget == null) return;

        //현재 State에 따라 다른 행동을 수행
        switch (currentState)
        {
            case NpcState.Chasing:
                StateChasing();
                break;
            case NpcState.Postioning:
                StatePositioning();
                break;
            case NpcState.CuttingIn:
                StateCuttingIn();
                break;
            case NpcState.Braking:
                StateBraking();
                break;
        }

        //뒤에서 계속 플레이어를 박는 문제의 원인을 분석하기 위한 디버깅 - 완료하면 주석처리하기.
        /*if (playerTarget != null)
        {
            float currentSpeed = rb.linearVelocity.magnitude * 3.6f;
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            Debug.Log($"상태: {currentState} | 현재속도: {currentSpeed:F1} | 거리: {dist:F1} | 조향각(Steer): {frontLeftWheel.steerAngle} | 목표지점방향: {overtakeSide}");
        }*/
    }

    //State func

    //1. Chasing
    private void StateChasing()
    {
        //Debug.Log("추적 중..");
        ApplyBrakes(0);
        FollowTarget(boostMotorForce);
        //HandleSteering();
        HandleSteering(playerTarget.position);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer < cutInDistance)
        {
            //추월 방향 결정을 위한 relativePos Vector3값 추가
            Vector3 relativePos = playerTarget.InverseTransformPoint(transform.position);

            //만약 플레이어와 좌우 2m이내의 차이를 가진다면, 방향을 랜덤으로 결정해 추월할 수 있게 설계해야함.
            if (Mathf.Abs(relativePos.x) < 2f)
            {
                overtakeSide = (UnityEngine.Random.value > 0.5f) ? 1f : -1f; //갑자기 왜 참조 모호성이 발생함?? 일단 명시적 선언으로 해결합니다.
            }
            //그 외의 경우, 현재 Npc가 있는 쪽으로 방향 결정
            else
            {
                overtakeSide = Mathf.Sign(relativePos.x);
            }
            
            currentState = NpcState.Postioning;
            //Debug.Log("플레이어 근접, 끼어들기 시작..");
        }
    }

    private void StatePositioning()
    {
        //Debug.Log("끼어들 위치 잡기");
        ApplyBrakes(0);
        FollowTarget(motorForce);

        Vector3 relativePos = playerTarget.InverseTransformPoint(transform.position);

        //이제 목표 지점을 전방과 측면을 모두 계산해야함. 복합적 좌표계산 필요. 하지만 현재 계산은 직진구간에만 사용가능.
        //이게 문제가 아님. 현재 이구조론 offset이 길어지면 뒤에서 계속 차로 들이박는 문제가 생김.
        // 측면 거리 계산 playerTarget.right * Direction * Width
        Vector3 sideOffset = playerTarget.right * overtakeSide * overtakeLaneWidth;

        // 전방 거리 계산
        Vector3 frontOffset = playerTarget.forward * positioningOffset;

        //최종 목표 지점 = 플레이어 위치 + 전방 거리 + 측면 거리
        //이경우를 하나로 두는 것이 아닌 두개로 나눠야할 것같음
        //Vector3 targetPoint = playerTarget.position + frontOffset + sideOffset;
        Vector3 targetPoint;
        
        float steeringBlend = 0.5f; //기본 값도로 방향과 목표 방향을 반으로 섞음

        //만약 플레이어보다 뒤에 있거나 z축 좌표가 동일하다면? -> 확실하게 옆으로 빠졌는지로 평가를 보강해야할 필요성을 느낌
        if (Mathf.Abs(relativePos.x) < overtakeLaneWidth * 0.8f)
        {
            targetPoint = playerTarget.position + sideOffset;   
            steeringBlend = 0.0f; //급격한 핸들링 유도 - 앞으로 가는 벡터를 제거했음
        }
        else
        {
            targetPoint = playerTarget.position + frontOffset + sideOffset;
            steeringBlend = 0.0f;
        }

        //**목표 지점을 플레이어 10미터 앞으로 계산 - 더 이상 안씀.**
        //Vector3 targetPoint = playerTarget.position + (playerTarget.forward * 20f);

        //HandleSteering에 계산된 목표 지점 전달
        HandleSteering(targetPoint, steeringBlend);

        //TODO : 만약 targetPoint에 충분히 가까워지면, CuttingIn state로 변경
        //float distanceToTargetPoint = Vector3.Distance(transform.position, targetPoint);

        //위에처럼 진행하면 영원히 추월하고 쭉 지나감. 이건 원하는 목표가 아님.
        //FIXED TODO : Npc가 플레이어 앞으로갔다는 것을 검사, 조건이 참이 되면 즉시 상태 변경

        //Vector3 relativePos = playerTarget.InverseTransformPoint(transform.position);

        if (relativePos.z > 8f)
        {
            //status를 CuttingIn으로 변경
            currentState = NpcState.CuttingIn;
            //Debug.Log("목표 위치 도달, 끼어들기 실행..");

            //끼어들기 직전에 추월 방향을 0으로 리셋하여 NPC가 옆으로 가지 않게 방지함
            overtakeSide = 0f;
        }
    }

    private void StateCuttingIn()
    {
        //Debug.Log("끼어들기 실행 중!");
        ApplyBrakes(0);
        FollowTarget(motorForce);

        Vector3 targetPoint = playerTarget.position + (playerTarget.forward * 4f);

        HandleSteering(targetPoint, 0.3f);

        Vector3 relativePos = playerTarget.InverseTransformPoint(transform.position);

        if (relativePos.z > 8f && Mathf.Abs(relativePos.x) < 3f)
        {
            currentState = NpcState.Braking;
            //Debug.Log("끼어들기 완료, 급정거 시작..");
        }
    }
    
    private void StateBraking()
    {
        //Debug.Log("급정거 중!");

        frontLeftWheel.motorTorque = 0;
        frontRightWheel.motorTorque = 0;
        rearLeftWheel.motorTorque = 0;
        rearRightWheel.motorTorque = 0;

        ApplyBrakes(brakeForce);
    }

    //Npc func
    private void FollowTarget(float currentMotorForce)
    {
        float currentSpeed = rb.linearVelocity.magnitude * 3.6f;

        if (currentSpeed < maxSpeed)
        {
            frontLeftWheel.motorTorque = currentMotorForce;
            frontRightWheel.motorTorque = currentMotorForce;
            rearLeftWheel.motorTorque = currentMotorForce;
            rearRightWheel.motorTorque = currentMotorForce;
        }
        else
        {
            frontLeftWheel.motorTorque = 0;
            frontRightWheel.motorTorque = 0;
            rearLeftWheel.motorTorque = 0;
            rearRightWheel.motorTorque = 0;  
        }
    }

    //플레이어와 동일한 속도 감응형 시스템으로 바꿔야 추적을 더 잘할 수있을거같음.
    private void HandleSteering(Vector3 targetPosition, float blendFactor = 0.3f)
    {
        float currentSpeed = rb.linearVelocity.magnitude * 3.6f; //현재 차량 속도를 km/h 표준식으로 계산
        float maxAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, currentSpeed / speedThreshold);
        
        //Vector3 directionToTarget = playerTarget.position - transform.position;
        //균형 잡힌 핸들링을 위해서는 normalized를 해서 오직 방향 정보만을 제공하게끔 해야한다는 제미나이의 조언을 적용했습니다.
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        Vector3 roadForward = (playerTarget != null) ? playerTarget.forward : transform.forward;

        //선형 보간법 사용. time.deltaTime * speed 처럼 값이 급격하게 변하는 것이 아닌 부드럽게 변경되기를 원해서 사용했습니다.
        Vector3 finalDirection = Vector3.Lerp(directionToTarget, roadForward, blendFactor);

        float angle = Vector3.SignedAngle(transform.forward, finalDirection, Vector3.up);

        float steerAmount = angle * steerSensitivity;

        steerAmount *= -1; 

        steerAmount = Mathf.Clamp(steerAmount, -maxAngle, maxAngle);

        frontLeftWheel.steerAngle = steerAmount;
        frontRightWheel.steerAngle = steerAmount;
    }
    
    private void ApplyBrakes(float force)
    {
        frontLeftWheel.brakeTorque = force;
        frontRightWheel.brakeTorque = force;
        rearLeftWheel.brakeTorque = force;
        rearRightWheel.brakeTorque = force;
    }
}
