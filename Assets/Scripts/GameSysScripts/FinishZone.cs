using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //FindObjectOfType<GameManager>().GameSuccess(); -더이상 사용되지 않는다는 경고 발생
            //FindAnyObjectByType<GameManager>().GameSuccess(); 안쓸거임
            GameManager.gManager.GameSuccess();
        }
    }
}
