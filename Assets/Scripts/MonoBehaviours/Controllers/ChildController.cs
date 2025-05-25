using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class ChildController : Singleton<ChildController>
{
    [SerializeField]
    private GameObject humanForm;

    /// <summary>
    /// Spawn position of the child.
    /// </summary>
    private Vector3 spawnPosition;

    protected override void Awake()
    {
        base.Awake();

        spawnPosition = transform.position;

        GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(GameManager_OnNightBegin);
    }

    private void OnEnable() => humanForm.SetActive(true);

    private void OnDisable() => humanForm.SetActive(false);

    private void GameManager_OnDayBegin()
    {
        transform.position = spawnPosition;
        enabled = true;
    }

    private void GameManager_OnNightBegin()
    {
        enabled = false;
    }
}