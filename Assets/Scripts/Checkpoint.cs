using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator characterAnimator;
    public float timeBonus = 2.5f;
    private bool activated = false;
    public Timer timer; //validacion texto

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
            if (timer != null) //validacion texto
            {
                timer.FlashGreen();
            }

            Debug.Log("+ Tiempo");
        }
    }
}

