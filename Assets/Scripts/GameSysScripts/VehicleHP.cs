using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VehicleHP : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    public Slider healthSlider;

    [Header("Audio Setting")]
    public AudioClip crashSound;
    private AudioSource audioSource;

    [Range(0.1f, 5f)]
    public float crashVolume = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP; //시작시 현재 체력을 최대체력값으로 fixed

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDmg(float amount)
    {
        currentHP -= amount;
        //Debug.Log("플레이어 충돌발생. 현재 내구도 : " + currentHP);

        if (crashSound != null && audioSource != null)
        {
           audioSource.PlayOneShot(crashSound, crashVolume);
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            TotalLoss();
        }
    }

    void TotalLoss()
    {
        GameManager.gManager.GameFailure(GameManager.FailureType.VehicleDestroyed);
        //초기형 - 콘솔 메시지만 제공
        Debug.Log("폐차되셨습니다.");

        //전손 엔딩 재생 구현은 여기에.

        gameObject.SetActive(false); //차량 비활성화
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //초기형 - 충돌 force에 따른 각기다른 데미지가 아닌, 당장은 고정 데미지로. - 고정 데미지로 계속 진행하기로 결정
            TakeDmg(25f);
        }
    }
}
