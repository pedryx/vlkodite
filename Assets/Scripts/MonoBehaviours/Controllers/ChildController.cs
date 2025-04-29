using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class ChildController : Singleton<ChildController>
{
    [SerializeField]
    private GameObject childSprite;

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

    private void OnEnable() => childSprite.SetActive(true);

    private void OnDisable() => childSprite.SetActive(false);

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