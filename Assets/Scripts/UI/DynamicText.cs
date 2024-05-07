using UnityEngine;
using TMPro;

public class DynamicText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float speed = 1f;
    public float amplitude = 10f;

    private float originalY;
    private float timeCounter = 0f;

    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }
        originalY = textMeshPro.rectTransform.localPosition.y;
    }

    void Update()
    {
        timeCounter += Time.deltaTime * speed;
        float newY = originalY + Mathf.Sin(timeCounter) * amplitude;
        textMeshPro.rectTransform.localPosition = new Vector3(textMeshPro.rectTransform.localPosition.x, newY, textMeshPro.rectTransform.localPosition.z);
    }
}
