using UnityEngine;

/// <summary>
/// Controla las reacciones de un NPC (frase positiva / negativa).
/// Se inicializa desde NPCManager con la referencia al manager y los datos del NPC.
/// </summary>
public class NPCReactionController : MonoBehaviour
{
    private NPCManager npcManager;
    private NPCManager.NPCData npcData;

    /// <summary>
    /// Inicializa el controller con referencia al manager y los datos del NPC.
    /// Llamar justo después de instanciar el NPC.
    /// </summary>
    public void Initialize(NPCManager manager, NPCManager.NPCData data)
    {
        npcManager = manager;
        npcData = data;
    }

    /// <summary>
    /// Reacción positiva: dice la frase positiva (aquí con Debug.Log; más tarde puedes reproducir audio).
    /// </summary>
    public void ReactPositive()
    {
        if (npcData != null)
        {
            Debug.Log("NPCReact Positive: " + npcData.positiveReaction);
            // Aquí puedes reproducir un audio clip, TTS o animación.
        }
        else
        {
            Debug.LogWarning("NPCReactionController.ReactPositive: npcData es null.");
        }
    }

    /// <summary>
    /// Reacción negativa: dice la frase negativa y elimina el NPC y su UI.
    /// </summary>
    public void ReactNegative()
    {
        if (npcData != null)
        {
            Debug.Log("NPCReact Negative: " + npcData.negativeReaction);
        }
        else
        {
            Debug.LogWarning("NPCReactionController.ReactNegative: npcData es null.");
        }

        // Ocultar UI si existe
        if (npcManager != null && npcManager.uiController != null)
        {
            npcManager.uiController.HideUI();
        }

        // Limpiar referencia en el manager y destruir este NPC
        if (npcManager != null)
        {
            npcManager.ClearCurrentNPC();
        }

        Destroy(gameObject);
    }
}
