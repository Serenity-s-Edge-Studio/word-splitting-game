using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class DemonManager : MonoBehaviour
{
    public static DemonManager instance;
    private List<Demon> activeDemons = new List<Demon>();
    [SerializeField]
    private Rect patrolBounds;
    [SerializeField]
    [Range(0, 100)]
    private float attackPercentage;
    [SerializeField]
    [Min(0)]
    private int waveFrequency;
    [SerializeField]
    private int currentWave;

    private void Awake()
    {
        instance = this;
        activeDemons.AddRange(GetComponentsInChildren<Demon>());
        foreach(Demon demon in activeDemons)
        {
            demon.TargetPos = new Vector3(UnityEngine.Random.Range(patrolBounds.xMin, patrolBounds.xMax), 
                                          UnityEngine.Random.Range(patrolBounds.yMin, patrolBounds.yMax));
        }
    }
    public void FixedUpdate()
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000));
        Transform[] transforms = new Transform[activeDemons.Count];
        Vector3[] targetPositions = new Vector3[activeDemons.Count];
        Vector3[] spawnPositions = new Vector3[activeDemons.Count];
        DemonState[] demonStates = new DemonState[activeDemons.Count];
        for(int i = 0; i < activeDemons.Count; i++)
        {
            transforms[i] = activeDemons[i].transform;
            targetPositions[i] = activeDemons[i].TargetPos;
            spawnPositions[i] = activeDemons[i].SpawnPostion;
            demonStates[i] = activeDemons[i].state;
        }
        NativeArray<Vector3> nativeTargets = new NativeArray<Vector3>(targetPositions, Allocator.TempJob);
        NativeArray<Vector3> nativeSpawns = new NativeArray<Vector3>(spawnPositions, Allocator.TempJob);
        NativeArray<DemonState> nativeStates = new NativeArray<DemonState>(demonStates, Allocator.TempJob);
        NativeArray<Vector3> nativePeoplePositions = new NativeArray<Vector3>(PeopleManager.instance.getPeoplePositions(), Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(transforms, 100);
        CalculateTargetPositions calculateTargetPositionsJob = new CalculateTargetPositions
        {
            targets = nativeTargets,
            people = nativePeoplePositions,
            spawnPositions = nativeSpawns,
            states = nativeStates,
            random = random,
            rect = patrolBounds
        };
        float sin = (Mathf.Sin(Time.time) + 1) / 2;
        if (1 - sin < 0.00001f) currentWave++;
        MoveDemonsJob moveDemonsJob = new MoveDemonsJob
        {
            sin = sin,
            targetPositions = nativeTargets,
            spawns = nativeSpawns,
            random = random,
            states = nativeStates,
            waveNumber = currentWave,
            waveFrequency = waveFrequency,
            wavePercent = attackPercentage
        };
        JobHandle calculateTargetPositionsJobHandle = calculateTargetPositionsJob.Schedule(transformAccessArray);
        JobHandle moveJobHandle = moveDemonsJob.Schedule(transformAccessArray, calculateTargetPositionsJobHandle);
        moveJobHandle.Complete();
        for (int i = 0; i < activeDemons.Count; i++)
        {
            activeDemons[i].TargetPos = nativeTargets[i];
            activeDemons[i].state = nativeStates[i];
        }
        //Clean up native collections
        nativePeoplePositions.Dispose();
        nativeStates.Dispose();
        nativeTargets.Dispose();
        transformAccessArray.Dispose();
        nativeSpawns.Dispose();
    }
    public void Remove(Demon demon)
    {
        activeDemons.Remove(demon);
    }

    // Implement this OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube((Vector3)patrolBounds.center, new Vector3(patrolBounds.width, patrolBounds.height));
    }
    public struct MoveDemonsJob : IJobParallelForTransform
    {
        public float sin;
        public float waveNumber;
        public float waveFrequency;
        public float wavePercent;
        public NativeArray<Vector3> targetPositions;
        public NativeArray<Vector3> spawns;
        public NativeArray<DemonState> states;
        public Unity.Mathematics.Random random;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = Vector3.Lerp(spawns[index], targetPositions[index], sin);
            if (Vector3.Distance(transform.position, spawns[index]) < .1f)
            {
                if (waveNumber % waveFrequency == 0)
                {
                    if (random.NextFloat(0, 100) < wavePercent)
                    {
                        states[index] = DemonState.TargetNewPerson;
                    }
                    else
                    {
                        states[index] = DemonState.TargetRandomPatrol;
                    }
                }
            }
            else if (states[index] != DemonState.TargetNewPerson && Vector3.Distance(transform.position, targetPositions[index]) > .01f)
            {
                states[index] = DemonState.HeadingToTarget;
            }
        }
    }
    public struct CalculateTargetPositions : IJobParallelForTransform
    {
        public NativeArray<DemonState> states;
        [ReadOnly]
        public NativeArray<Vector3> people;
        [ReadOnly]
        public NativeArray<Vector3> spawnPositions;
        public NativeArray<Vector3> targets;
        public Unity.Mathematics.Random random;
        [ReadOnly]
        public Rect rect;

        public void Execute(int index, TransformAccess transform)
        {
            switch (states[index])
            {
                case DemonState.HeadingToTarget:
                    break;
                case DemonState.TargetNewPerson:
                    Vector3 closest = Vector3.zero;
                    float lastDistance = float.MaxValue;
                    for (int i = 0; i < people.Length; i++)
                    {
                        float distance = Vector3.Distance(transform.position, people[i]);
                        if (distance < lastDistance)
                        {
                            closest = people[i];
                            lastDistance = distance;
                        }
                    }
                    targets[index] = closest;
                    break;
                case DemonState.TargetRandomPatrol:
                    targets[index] = new Vector3(random.NextFloat(rect.xMin, rect.xMax), random.NextFloat(rect.yMin, rect.yMax));
                    break;
                case DemonState.ReturningFromTarget:
                    targets[index] = spawnPositions[index];
                    break;
            }
        }
    }
    public enum DemonState
    {
        HeadingToTarget,
        TargetNewPerson,
        TargetRandomPatrol,
        ReturningFromTarget
    }
}
