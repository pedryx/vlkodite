using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class TextSequencer : MonoBehaviour
{
    [System.Serializable]
    public class TextBlock
    {
        public GameObject textObject;
        public float duration = 2f;
    }

    [Header("Text Blocks")]
    [SerializeField] private List<TextBlock> sequence = new List<TextBlock>();

    [Header("FMOD Sound")]
    [SerializeField] private EventReference stepSound;

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = false;

    [Header("Scene Transition")]
    [SerializeField] private bool loadNextScene = false;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float delayBeforeSceneLoad = 2f;

    private Coroutine sequenceRoutine;
    private bool skipRequested = false;

    private void Start()
    {
        if (playOnStart)
            PlaySequence();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            skipRequested = true;
        }
    }

    public void PlaySequence()
    {
        if (sequenceRoutine != null)
            StopCoroutine(sequenceRoutine);

        sequenceRoutine = StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        do
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                var block = sequence[i];

                // Play FMOD blip
                if (!stepSound.IsNull)
                    RuntimeManager.PlayOneShot(stepSound, transform.position);

                block.textObject.SetActive(true);

                float elapsed = 0f;
                skipRequested = false;

                while (elapsed < block.duration && !skipRequested)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                block.textObject.SetActive(false);
            }

            if (loadNextScene && !loop)
            {
                yield return new WaitForSeconds(delayBeforeSceneLoad);
                SceneManager.LoadScene(nextSceneName);
            }

        } while (loop);
    }

    public void StopSequence()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        foreach (var block in sequence)
        {
            block.textObject.SetActive(false);
        }
    }
}
