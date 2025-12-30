using UnityEngine;
using System.Collections; //코루틴 사용에 필요한 IEnumerator를 위해 추가


//원래는 운전하며 따라오게하려했으나 추적을 잘못하는 문제 발생..
//아예 플레이어 뒤에 자식컴포넌트로 붙여놓고 랜덤한 시간에 생성시켜 눈뽕시키는건 어떨까?
//start랑 update 삭제 및 인스펙터 정리 필요
public class NpcHighLightController : MonoBehaviour
{
    [Header("HighLight Setting")]
    public Light highLight; //인스펙터에서 SpotLight 연결
    public float flashInterval = 0.5f; //깜빡이는 주기 (0.5초)

    private Rigidbody rb;

    //OnEnable, OnDisable 사용. SpotLight 오브젝트가 SetActive(true || false)될때 자동 호출됨.

    void OnEnable()
    {
        //상향등 깜빡임 코루틴 시작
        if (highLight != null)
        {
            StartCoroutine(FlashHighLight());
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        //상향등 깜빡임 코루틴 시작
        if (highLight != null)
        {
            highLight.enabled = false;
        }
    }

    IEnumerator FlashHighLight()
    {
        //게임이 실행되는 동안 무한 루프
        while (true)
        {
                highLight.enabled = true;
                yield return new WaitForSeconds(flashInterval);
                highLight.enabled = false;
                yield return new WaitForSeconds(flashInterval);
        }
    }
}
