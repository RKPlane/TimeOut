using UnityEngine;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;

    private GameManager gm;
    private Color originalColor;
    private Coroutine colorCoroutine;

    void Awake()
    {
        //validacion del texto
        if (timerText == null)
            timerText = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        gm = GameManager.Instance;
        originalColor = Color.white;
    }

    void Update()
    {
        if (gm == null || timerText == null) return; //validacion por si es null o el tiempo se bugea

        timerText.text = gm.currentTime.ToString("F1");
    }

    public void FlashGreen(float duration = 3f) //llama a la coroutina
    {
        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);

        colorCoroutine = StartCoroutine(GreenRoutine(duration));
    }

    private IEnumerator GreenRoutine(float duration)
    {
        if (timerText == null)
        {
            Debug.LogError("Cannot flash green: timerText is null!");
            yield break;
        }

        timerText.color = Color.green;
        yield return new WaitForSeconds(duration);
        timerText.color = originalColor;
    }

}

