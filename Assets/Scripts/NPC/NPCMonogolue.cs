using UnityEngine;
using TMPro;
using System.Collections;

public class NPCMonologue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textBubble;
    [SerializeField] private TextAsset dialogueFile;
    [SerializeField] private TextAsset stopChasingFile;

    private MonologueJson monologueData;
    private int currentLineIndex = 0;
    private Coroutine monologueRoutine;

    [Header("The stop chasing data")]
    private MonologueJson stopChasingData;
    private int currentLineIndexStopChasing = 0;
    private bool stopChasingTriggered = false;
    private float resetTimer = 0f;
    private float resetDelay = 20f; // 20 seconds to reset after last line

    void Awake()
    {
        if (dialogueFile != null)
        {
            monologueData = JsonUtility.FromJson<MonologueJson>(dialogueFile.text);
        }
        if (stopChasingFile != null)
        {
            stopChasingData = JsonUtility.FromJson<MonologueJson>(stopChasingFile.text);
        }
    }

    /// <summary>
    /// Call this to start the NPC’s monologue loop.
    /// </summary>
    public void StartMonologue()
    {
        if (monologueData == null || monologueData.lines.Length == 0)
        {
            Debug.LogWarning("No dialogue data found for " + gameObject.name);
            return;
        }

        // If one is already running, stop it first
        if (monologueRoutine != null)
        {
            StopCoroutine(monologueRoutine);
        }
        currentLineIndex = 0; // reset each time we start
        monologueRoutine = StartCoroutine(MonologueLoop());
    }

    /// <summary>
    /// Stops the NPC’s monologue.
    /// </summary>
    public void StopMonologue()
    {
        if (monologueRoutine != null)
        {
            StopCoroutine(monologueRoutine);
            monologueRoutine = null;
        }
        textBubble.text = ""; // clear text
    }

    private IEnumerator MonologueLoop()
    {
        while (currentLineIndex < monologueData.lines.Length)
        {
            textBubble.text = monologueData.lines[currentLineIndex];
            currentLineIndex++;
            yield return new WaitForSeconds(monologueData.interval);
        }

        // cleanup when done
        StopMonologue();
    }

    /// <summary>
    /// Stops the NPC’s monologue and responds with next iteration of stopChasing response.
    /// </summary>
    public void StopChasingResponse()
    {
        StopMonologue();
        if (stopChasingData == null || stopChasingData.lines == null || stopChasingData.lines.Length == 0)
            return;

        // Show the current line
        textBubble.text = stopChasingData.lines[currentLineIndexStopChasing];

        // Check if this is the last line
        if (currentLineIndexStopChasing == stopChasingData.lines.Length - 1)
        {
            stopChasingTriggered = true;
            resetTimer = resetDelay; // start countdown to reset
        }

        // Increment index for next call
        currentLineIndexStopChasing = Mathf.Min(currentLineIndexStopChasing + 1, stopChasingData.lines.Length - 1);
    }

    void Update()
    {
        // Handle 20-second reset on the stop chasing
        if (stopChasingTriggered)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0f)
            {
                currentLineIndexStopChasing = 0;
                stopChasingTriggered = false;
            }
        }
    }
}
