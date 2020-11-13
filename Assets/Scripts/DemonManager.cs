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
    private void Awake()
    {
        instance = this;
        activeDemons.AddRange(GetComponentsInChildren<Demon>());
        foreach(Demon demon in activeDemons)
        {
            demon.Speed = new Vector2Int(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
        }
    }
    public void FixedUpdate()
    {
        Transform[] transforms = new Transform[activeDemons.Count];
        Vector2Int[] speeds = new Vector2Int[activeDemons.Count];
        Vector3[] spawnPositions = new Vector3[activeDemons.Count];
        for(int i = 0; i < activeDemons.Count; i++)
        {
            transforms[i] = activeDemons[i].transform;
            speeds[i] = activeDemons[i].Speed;
            spawnPositions[i] = activeDemons[i].SpawnPostion;
        }
        NativeArray<Vector2Int> nativeSpeeds = new NativeArray<Vector2Int>(speeds, Allocator.TempJob);
        NativeArray<Vector3> nativeSpawns = new NativeArray<Vector3>(spawnPositions, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(transforms, 100);
        MoveDemonsJob moveDemonsJob = new MoveDemonsJob
        {
            sin = Mathf.Sin(Time.time) / 270f,
            speeds = nativeSpeeds,
            spawns = nativeSpawns,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))  
        };
        JobHandle handle = moveDemonsJob.Schedule(transformAccessArray);
        handle.Complete();
        for (int i = 0; i < activeDemons.Count; i++)
        {
            activeDemons[i].Speed = nativeSpeeds[i];
        }
        nativeSpeeds.Dispose();
        transformAccessArray.Dispose();
        nativeSpawns.Dispose();
    }
    public void remove(Demon demon)
    {
        activeDemons.Remove(demon);
    }
    public struct MoveDemonsJob : IJobParallelForTransform
    {
        public float sin;
        public NativeArray<Vector2Int> speeds;
        public NativeArray<Vector3> spawns;
        public Unity.Mathematics.Random random;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position += new Vector3(sin * speeds[index].x, sin * speeds[index].y);
            if (Vector3.Distance(transform.position, spawns[index]) < .01f)
            {
                speeds[index] = new Vector2Int(random.NextInt(-10, 10), random.NextInt(-10, 10));
            }
        }
    }
}
