using UnityEngine;
using System.Collections.Generic; //List 사용을 위해 불러옴

public class ChunkController : MonoBehaviour
{
    [Header("Player Setting")]
    public Transform playerTransform;

    [Header("Chunk List")]
    public List<GameObject> allRoadChunks;

    [Header("Fog Range Setting")]
    [Tooltip("Activate Chunks within the Player's Front Range")]
    public float visibleRangeForward = 500f;
    [Tooltip("Activate Chunks within the Player's Behind Range")]
    public float visibleRangeBehind = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        //시작 시, 청크 상태를 한번 업데이트
        UpdateChunkRange();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateChunkRange();
    }
    
    void UpdateChunkRange()
    {
        if (playerTransform == null) return;

        float playerZ = playerTransform.position.z;
        
        //리스트의 모든 청크를 순회
        foreach (GameObject chunk in allRoadChunks)
        {
            //각 청크의 z축 중심 위치 가져오기
            //이렇게 하면 청크 004부터 문제가 발생합니다. 현재는 전역공간의 z축좌표를 기준으로 잡기에
            //004의 청크는 전역공간 기준 z축이 0이기에 참조값에 문제가 발생합니다.
            //float chunkZ = chunk.transform.position.z;

            //따라서 부모 오브젝트의 좌표값을 가져오는 것이 아닌 자식 메시의 실제 월드 좌표상 중심 z값을 가져오면 해결될 것같습니다.
            float chunkZ = chunk.GetComponentInChildren<Renderer>().bounds.center.z;

            // 양수 : 청크가 플레이어 앞 위치, 음수 : 청크가 플레이어 뒤 위치)
            float relativeDistanceZ = chunkZ - playerZ;

            //1. 청크가 후방 가시범위 보다 뒤에 있거나
            //2. 청크가 전방 가시범위 보다 앞에 있나?
            if (relativeDistanceZ < -visibleRangeBehind || relativeDistanceZ > visibleRangeForward)
            {
                //가시범위를 벗어났기에 비활성화
                if (chunk.activeSelf)
                {
                    chunk.SetActive(false);
                }
            }
            else
            {
                //가시범위 안에 있기에 활성화
                if (!chunk.activeSelf)
                {
                    chunk.SetActive(true);
                }
            }
        }
    }
}
