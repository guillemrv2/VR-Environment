using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;

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

        if (uiController == null)
        {
            Debug.LogWarning("NPCManager: uiController no está asignado en el Inspector.");
        }

        // Guardamos el índice *para esta instancia* para evitar problemas por closure
        int indexForThis = currentIndex;

        // Instancia del NPC
        currentNPC = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("NPCManager: NPC instanciado -> " + currentNPC.name + " (indexForThis = " + indexForThis + ")");

        NPCMovement movement = currentNPC.GetComponent<NPCMovement>();

        if (movement == null)
        {
            Debug.LogError("NPCManager: NPCMovement no encontrado en el prefab.");
            currentNPC = null;
            return;
        }

        movement.SetTarget(targetPoint);

        // Suscribimos al evento usando la copia indexForThis
        System.Action onReachedHandler = null;
        onReachedHandler = () =>
        {
            Debug.Log("NPCManager: Evento recibido: NPC llegó al target. indexForThis = " + indexForThis);

            // Tomamos los datos del NPC correspondiente
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
                    // Ahora la UI se genera en el punto fijo definido en NPCUIController
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

            // Limpiamos la suscripción para evitar dobles llamadas o referencias colgantes
            movement.OnReachedTarget = null;
        };

        movement.OnReachedTarget = onReachedHandler;

        // Incrementamos el índice para el siguiente NPC
        currentIndex++;
    }

    public void ClearCurrentNPC()
    {
        Debug.Log("NPCManager: NPC eliminado del sistema.");
        currentNPC = null;
    }
}
