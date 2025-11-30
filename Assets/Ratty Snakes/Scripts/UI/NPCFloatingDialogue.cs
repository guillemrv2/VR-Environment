using System.Collections;
using TMPro;
using UnityEngine;

public class NPCFloatingDialogue : MonoBehaviour
{
    public Transform targetToFollow;
    public TextMeshProUGUI dialogueText;

    private Coroutine typingCoroutine;

    // Muestra texto con efecto máquina de escribir
    public void Show(string text, float typeSpeed = 0.05f)
    {
        // Cancelar cualquier texto que se esté escribiendo
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text, typeSpeed));
    }

    private IEnumerator TypeText(string fullText, float typeSpeed)
    {
        dialogueText.text = ""; // Limpiar texto inicial
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private void Update()
    {
        // Mantener el diálogo flotando sobre la cabeza
        if (targetToFollow != null)
        {
            transform.position = targetToFollow.position;
        }
    }
}
