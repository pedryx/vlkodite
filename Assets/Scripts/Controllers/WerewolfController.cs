using System;

using UnityEngine;

public class WerewolfController : MonoBehaviour
{
    private void Awake()
    {
        // TODO: pre-compute path-finding
        enabled = false;

        GameManager.Instance.OnDayBegin += Instance_OnDayBegin;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;
    }

    private void Instance_OnDayBegin(object sender, EventArgs e)
    {
        enabled = false;
    }

    private void Instance_OnNightBegin(object sender, EventArgs e)
    {
        enabled = true;
        // TODO: change sprite
        GetComponentInChildren<SpriteRenderer>().color = Color.black;
    }

    // TODO: chasing
}
