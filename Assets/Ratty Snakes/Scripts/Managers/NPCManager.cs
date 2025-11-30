using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;

    [Header("Scene References")]
    public GameObject currentNPC;

    [Header("UI Settings")]
    public NPCUIController uiController;

    [Header("Floating Dialogue")]
    public GameObject floatingDialoguePrefab;

    [System.Serializable]
    public class NPCData
    {
        public string npcName;
        public string causeOfDeath;
        public string bio;
        public string goodActs;
        public string badActs;

        [TextArea] public string positiveReaction;
        [TextArea] public string negativeReaction;
    }

    public NPCData[] npcDemoData;
    private int currentIndex = 0;

    public void SpawnNextNPC()
    {
        if (currentNPC != null)
        {
            Debug.LogWarning("NPCManager: Ya hay un NPC activo.");
            return;
        }

        if (npcDemoData == null || npcDemoData.Length == 0)
        {
            Debug.LogError("NPCManager: npcDemoData está vacío o es null.");
            return;
        }

        if (currentIndex >= npcDemoData.Length)
        {
            Debug.LogWarning("NPCManager: No quedan NPCs en npcDemoData.");
            return;
        }

        int indexForThis = currentIndex;

        // Instanciar NPC
        currentNPC = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);

        // Obtener NPCMovement
        NPCMovement movement = currentNPC.GetComponent<NPCMovement>();
        if (movement == null)
        {
            Debug.LogError("NPCManager: NPCMovement no encontrado en el prefab.");
            Destroy(currentNPC);
            currentNPC = null;
            return;
        }

        // Instanciar diálogo flotante
        NPCFloatingDialogue dialogue = null;
        if (floatingDialoguePrefab != null)
        {
            GameObject floatingUI = Instantiate(floatingDialoguePrefab);
            dialogue = floatingUI.GetComponent<NPCFloatingDialogue>();
            if (dialogue != null)
            {
                Transform headAnchor = currentNPC.transform.Find("HeadAnchor");
                if (headAnchor != null)
                    dialogue.targetToFollow = headAnchor;
                else
                    Debug.LogError("NPCManager: HeadAnchor no encontrado en el prefab NPC.");

                // Asignar TextMeshProUGUI
                var textComponent = floatingUI.transform.Find("Canvas/DialogueText")?
                    .GetComponent<TMPro.TextMeshProUGUI>();
                if (textComponent != null)
                    dialogue.dialogueText = textComponent;
                else
                    Debug.LogWarning("NPCManager: DialogueText no encontrado en Canvas.");
            }
            else
            {
                Debug.LogError("NPCManager: NPCFloatingDialogue no encontrado en el prefab floatingDialoguePrefab.");
            }
        }

        // Inicializar ReactionController
        NPCReactionController reaction = currentNPC.GetComponent<NPCReactionController>();
        if (reaction == null)
            reaction = currentNPC.AddComponent<NPCReactionController>();

        reaction.Initialize(this, npcDemoData[indexForThis], dialogue);

        // Asignar target del movimiento
        movement.SetTarget(targetPoint);

        // Evento al llegar al target
        movement.OnReachedTarget = () =>
        {
            NPCData data = npcDemoData[indexForThis];
            if (uiController != null)
            {
                uiController.ShowUI(
                    data.npcName,
                    data.causeOfDeath,
                    data.bio,
                    data.goodActs,
                    data.badActs
                );
            }
            movement.OnReachedTarget = null;
        };

        currentIndex++;
    }

    public void ClearCurrentNPC()
    {
        currentNPC = null;
    }

    // Métodos para gestos
    public void OnThumbsUp() => TriggerPositive();
    public void OnThumbsDown() => TriggerNegative();
    public void OnMiddleFinger() => TriggerNegative();
    public void OnNegationGesture() => TriggerNegative();

    private void TriggerPositive()
    {
        if (currentNPC == null) return;
        var reaction = currentNPC.GetComponent<NPCReactionController>();
        reaction?.ReactPositive();
    }

    private void TriggerNegative()
    {
        if (currentNPC == null) return;
        var reaction = currentNPC.GetComponent<NPCReactionController>();
        reaction?.ReactNegative();
    }
}
