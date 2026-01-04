using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator characterAnimator;
    public float timeBonus = 2.5f;
    private bool activated = false;

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;
            characterAnimator.SetBool("Activated", true); //cambiar color de las poles y banderas
            GameManager.Instance.AddTime(timeBonus);
            Object.FindFirstObjectByType<Timer>().FlashGreen(2f);//llamar coroutina
            Debug.Log("+ Tiempo");
        }
    }
}

