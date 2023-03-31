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
            carryable = null;
            carryable = testComponent.GetComponent<Carryable>();
        }
        Profiler.EndSample();

        Profiler.BeginSample("TryGetComponent method");
        for (int i = 0; i < IterationsPerUpdate; i++)
        {
            carryable = null;
            testComponent.TryGetComponent(out Carryable _carryable);
            carryable = _carryable;
        }
        Profiler.EndSample();

        Profiler.BeginSample("TryGetBehaviour method");
        for (int i = 0; i < IterationsPerUpdate; i++)
        {
            carryable = null;
            testComponent.TryGetBehaviour(out Carryable _carryable);
            carryable = _carryable;
        }
        Profiler.EndSample();
    }
}
