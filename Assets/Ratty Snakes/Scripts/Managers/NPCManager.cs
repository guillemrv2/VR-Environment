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

    [System.Serializable]
    public class NPCData
    {
        public string npcName;
        public string causeOfDeath;
        public string bio;
        public string goodActs;
        public string badActs;

        // Frases personalizables
        [TextArea] public string positiveReaction;   // ejemplo: "Gracias, que Dios te lo pague."
        [TextArea] public string negativeReaction;   // ejemplo: "¡Oiga! Eso no es justo..."
    }

    public NPCData[] npcDemoData;
    private int currentIndex = 0;

    public void SpawnNextNPC()
    {
        Debug.Log("NPCManager: SpawnNextNPC() llamado.");

        if (currentNPC != null)
        {
            Debug.LogWarning("NPCManager: Ya hay un NPC activo.");
            return;
        }

        if (npcDemoData == null || npcDemoData.Length == 0)
        {
            Debug.LogError("NPCManager: npcDemoData está vacío o es null. Rellena los datos en el Inspector.");
            return;
        }

        if (currentIndex >= npcDemoData.Length)
        {
            Debug.LogWarning("NPCManager: No quedan NPCs en npcDemoData.");
            return;
        }

        // Guardamos el índice para esta instancia (evita problemas con closures)
        int indexForThis = currentIndex;

        // Instancia del NPC en el spawn point
        currentNPC = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("NPCManager: NPC instanciado -> " + currentNPC.name + " (indexForThis = " + indexForThis + ")");

        // Obtener movement
        NPCMovement movement = currentNPC.GetComponent<NPCMovement>();
        if (movement == null)
        {
            Debug.LogError("NPCManager: NPCMovement no encontrado en el prefab.");
            currentNPC = null;
            return;
        }

        // Inicializar reaction controller: SI el prefab tiene el componente, obténlo; si no, añádelo.
        NPCReactionController reaction = currentNPC.GetComponent<NPCReactionController>();
        if (reaction == null)
        {
            reaction = currentNPC.AddComponent<NPCReactionController>();
        }

        // Inicializar con manager y datos
        reaction.Initialize(this, npcDemoData[indexForThis]);

        // Asignar target
        movement.SetTarget(targetPoint);

        // Evento de llegada (usa copia indexForThis)
        System.Action onReachedHandler = null;
        onReachedHandler = () =>
        {
            Debug.Log("NPCManager: Evento recibido: NPC llegó al target. indexForThis = " + indexForThis);

            if (indexForThis < 0 || indexForThis >= npcDemoData.Length)
            {
                Debug.LogError("NPCManager: indexForThis fuera de rango: " + indexForThis);
            }
            else
            {
                NPCData data = npcDemoData[indexForThis];

                if (uiController != null)
                {
                    Debug.Log("NPCManager: Mostrando UI del NPC index " + indexForThis);
                    // ShowUI ahora no pide posición (usa el uiSpawnPoint del UI controller)
                    uiController.ShowUI(
                        data.npcName,
                        data.causeOfDeath,
                        data.bio,
                        data.goodActs,
                        data.badActs
                    );
                }
                else
                {
                    Debug.LogWarning("NPCManager: uiController es NULL, no se puede mostrar la UI.");
                }
            }

            // Limpiamos la suscripción
            movement.OnReachedTarget = null;
        };

        movement.OnReachedTarget = onReachedHandler;

        // Incrementamos el índice para el siguiente NPC
        currentIndex++;
    }

    /// <summary>
    /// Limpia la referencia al NPC actual (llamar cuando el NPC desaparece).
    /// </summary>
    public void ClearCurrentNPC()
    {
        Debug.Log("NPCManager: ClearCurrentNPC() llamado.");
        currentNPC = null;
    }

    // -------------------------
    // Métodos públicos para conectar a los StaticHandGesture (Gesture Performed)
    // -------------------------
    public void OnThumbsUp()
    {
        if (currentNPC == null)
        {
            Debug.Log("OnThumbsUp: no hay NPC activo.");
            return;
        }

        Debug.Log("OnThumbsUp: gesto detectado.");

        NPCReactionController reaction = currentNPC.GetComponent<NPCReactionController>();
        if (reaction != null)
        {
            reaction.ReactPositive();
        }
        else
        {
            Debug.LogWarning("OnThumbsUp: NPCReactionController no encontrado en currentNPC.");
        }
    }

    public void OnThumbsDown()
    {
        HandleNegativeGesture("ThumbsDown");
    }

    public void OnMiddleFinger()
    {
        HandleNegativeGesture("MiddleFinger");
    }

    public void OnNegationGesture()
    {
        HandleNegativeGesture("NegationIndex");
    }

    // Helper para gestos negativos
    private void HandleNegativeGesture(string gestureName)
    {
        if (currentNPC == null)
        {
            Debug.Log("HandleNegativeGesture: no hay NPC activo.");
            return;
        }

        Debug.Log("HandleNegativeGesture: gesto negativo detectado -> " + gestureName);

        NPCReactionController reaction = currentNPC.GetComponent<NPCReactionController>();
        if (reaction != null)
        {
            reaction.ReactNegative();
        }
        else
        {
            Debug.LogWarning("HandleNegativeGesture: NPCReactionController no encontrado en currentNPC.");
        }
    }
}
