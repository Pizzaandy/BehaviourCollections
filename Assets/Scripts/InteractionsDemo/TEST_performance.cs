using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TEST_performance : MonoBehaviour
{
    public int IterationsPerUpdate = 1_000;

    public InteractionBehaviour testComponent;

    private Carryable carryable;

    private void Update()
    {
        Profiler.BeginSample("GetComponent method");
        for (int i = 0; i < IterationsPerUpdate; i++)
        {
            carryable = testComponent.GetComponent<Carryable>();
        }
        Profiler.EndSample();

        Profiler.BeginSample("TryGetComponent method");
        for (int i = 0; i < IterationsPerUpdate; i++)
        {
            testComponent.TryGetComponent(out carryable);
        }
        Profiler.EndSample();

        Profiler.BeginSample("TryGetBehaviour method");
        for (int i = 0; i < IterationsPerUpdate; i++)
        {
            testComponent.TryGetBehaviour(out carryable);
        }
        Profiler.EndSample();
    }
}
