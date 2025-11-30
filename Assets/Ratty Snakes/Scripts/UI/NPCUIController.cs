using TMPro;
using UnityEngine;

public class NPCUIController : MonoBehaviour
{
    public GameObject canvasPrefab;
    public Transform uiSpawnPoint;       // NUEVO: punto fijo donde aparecerá la UI

    private GameObject instance;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI causeOfDeathText;
    private TextMeshProUGUI bioText;
    private TextMeshProUGUI goodActsText;
    private TextMeshProUGUI badActsText;

    public void ShowUI(string npcName, string causeOfDeath, string bio, string goodActs, string badActs)
    {
        if (instance != null)
            Destroy(instance);

        // Instancia en un punto fijo de la escena
        instance = Instantiate(canvasPrefab, uiSpawnPoint.position, uiSpawnPoint.rotation);


        // Referencias a los campos de texto usando tu jerarquía actual
        nameText = instance.transform.Find("NPC_NamePanel/NPC_NameText").GetComponent<TextMeshProUGUI>();
        causeOfDeathText = instance.transform.Find("NPC_CauseOfDeathPanel/NPC_CauseOfDeathText").GetComponent<TextMeshProUGUI>();
        bioText = instance.transform.Find("NPC_BioPanel/NPC_BioText").GetComponent<TextMeshProUGUI>();
        goodActsText = instance.transform.Find("NPC_GoodActsPanel/NPC_GoodActsText").GetComponent<TextMeshProUGUI>();
        badActsText = instance.transform.Find("NPC_BadActsPanel/NPC_BadActsText").GetComponent<TextMeshProUGUI>();

        // Rellenar
        nameText.text = npcName;
        causeOfDeathText.text = causeOfDeath;
        bioText.text = bio;
        goodActsText.text = goodActs;
        badActsText.text = badActs;
    }

    public void HideUI()
    {
        if (instance != null)
            Destroy(instance);
    }
}
