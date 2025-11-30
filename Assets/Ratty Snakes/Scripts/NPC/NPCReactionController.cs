using UnityEngine;

/// <summary>
/// Controla las reacciones de un NPC (frase positiva / negativa) y el diálogo flotante.
/// Se inicializa desde NPCManager con la referencia al manager y los datos del NPC.
/// </summary>
public class NPCReactionController : MonoBehaviour
{
    private NPCManager npcManager;
    private NPCManager.NPCData npcData;
    private NPCFloatingDialogue activeDialogue;

    /// <summary>
    /// Inicializa el controller con manager, datos y diálogo flotante.
    /// </summary>
    public void Initialize(NPCManager manager, NPCManager.NPCData data, NPCFloatingDialogue dialogue)
    {
        npcManager = manager;
        npcData = data;
        activeDialogue = dialogue;
    }

    /// <summary>
    /// Muestra un texto en el diálogo flotante existente.
    /// </summary>
    private void ShowFloatingDialogue(string text)
    {
        if (activeDialogue != null)
        {
            activeDialogue.Show(text);
        }
        else
        {
            Debug.LogError("NPCReactionController: activeDialogue no asignado.");
        }
    }

    /// <summary>
    /// Reacción positiva: muestra la frase positiva.
    /// </summary>
    public void ReactPositive()
    {
        if (npcData != null)
            ShowFloatingDialogue(npcData.positiveReaction);
        else
            Debug.LogWarning("NPCReactionController.ReactPositive: npcData es null.");
    }

    /// <summary>
    /// Reacción negativa: muestra la frase negativa y elimina NPC + UI.
    /// </summary>
    public void ReactNegative()
    {
        if (npcData != null)
            ShowFloatingDialogue(npcData.negativeReaction);
        else
            Debug.LogWarning("NPCReactionController.ReactNegative: npcData es null.");

        // Ocultar UI principal
        if (npcManager != null && npcManager.uiController != null)
            npcManager.uiController.HideUI();

        // Limpiar referencia en manager
        if (npcManager != null)
            npcManager.ClearCurrentNPC();

        // Destruir diálogo flotante con un pequeño delay
        if (activeDialogue != null)
            Destroy(activeDialogue.gameObject, 0.1f);

        // Destruir NPC
        Destroy(gameObject, 0.1f);
    }
}
