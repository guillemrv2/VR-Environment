using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcPrefab;

    [Header("Scene References")]
    public GameObject currentNPC;

    [Header("UI Settings")]
    public NPCUIController uiController;

    [Header("Waypoints")]
    public Transform[] waypoints; // WP_0 a WP_5

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
    if (currentNPC != null) return;
    if (npcDemoData == null || npcDemoData.Length == 0) return;
    if (currentIndex >= npcDemoData.Length)
    {
        Debug.LogWarning("NPCManager: No quedan NPCs en npcDemoData.");
        return;
    }

    int indexForThis = currentIndex;

    // Instanciamos el NPC en WP_0
    currentNPC = Instantiate(npcPrefab, waypoints[0].position, Quaternion.identity);

    // Inicializamos diálogo flotante
    NPCFloatingDialogue dialogue = null;
    if (floatingDialoguePrefab != null)
    {
        GameObject floatingUI = Instantiate(floatingDialoguePrefab);
        dialogue = floatingUI.GetComponent<NPCFloatingDialogue>();
        if (dialogue != null)
        {
            Transform headAnchor = currentNPC.transform.Find("HeadAnchor");
            if (headAnchor != null)
            {
                dialogue.targetToFollow = headAnchor;
                var text = floatingUI.transform.Find("Canvas/DialogueText");
                if (text != null)
                    dialogue.dialogueText = text.GetComponent<TMPro.TextMeshProUGUI>();
            }
        }
    }

        // Inicializamos waypoint movement
        NPCWaypointMovement movement = currentNPC.GetComponent<NPCWaypointMovement>();
        if (!movement) movement = currentNPC.AddComponent<NPCWaypointMovement>();
        movement.waypoints = waypoints;

        // Callback cuando llega a WP_1 (UI)
        movement.onReachedWaypoint1 = () =>
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
            // Ahora el NPC espera la decisión
        };

        // Callback cuando llega al último waypoint (WP_5)
        movement.onReachedLastWaypoint = () =>
        {
            // Limpiar referencia del NPC
            ClearCurrentNPC();

            // Ocultar UI
            if (uiController != null)
                uiController.HideUI();
        };

        movement.StartWalking(); // WP_0 → WP_1 automático

    currentIndex++;
}


    public void ClearCurrentNPC()
    {
        currentNPC = null;
    }

    // Gestos
    public void OnThumbsUp() { TriggerPositive(); }
    public void OnThumbsDown() { TriggerNegative("ThumbsDown"); }
    public void OnMiddleFinger() { TriggerNegative("MiddleFinger"); }
    public void OnNegationGesture() { TriggerNegative("NegationIndex"); }

    private void TriggerPositive()
    {
        if (currentNPC == null) return;

        // Hacer reacción positiva
        NPCReactionController reaction = currentNPC.GetComponent<NPCReactionController>();
        reaction?.ReactPositive();

        // Continuar camino si estaba esperando decisión
        NPCWaypointMovement movement = currentNPC.GetComponent<NPCWaypointMovement>();
        if (movement != null && movement.isWaitingDecision)
        {
            movement.ContinueWalking(); // ahora sigue hacia WP_2 -> WP_5
        }
    }

    private void TriggerNegative(string gestureName)
    {
        if (currentNPC == null) return;

        // Reacción negativa
        NPCReactionController reaction = currentNPC.GetComponent<NPCReactionController>();
        reaction?.ReactNegative();

        // Si estaba esperando decisión, destruir NPC
        NPCWaypointMovement movement = currentNPC.GetComponent<NPCWaypointMovement>();
        if (movement != null && movement.isWaitingDecision)
        {
            movement.isWaitingDecision = false;
            // Se destruye el NPC desde ReactNegative
        }
    }

}
