using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using System;

public class PeopleManager : MonoBehaviour
{
    private List<Person> people = new List<Person>();
    internal static PeopleManager instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        people.AddRange(GetComponentsInChildren<Person>());
    }

    // Update is called once per frame
    void Update()
    {
        Transform[] transforms = new Transform[people.Count];
        float[] influence = new float[people.Count];
        for (int i = 0; i < people.Count; i++)
        {
            transforms[i] = people[i].transform;
            influence[i] = people[i].influenceLevel;
        }
        TransformAccessArray transformAccessArray = new TransformAccessArray(transforms,100);
        NativeArray<float> influenceNA = new NativeArray<float>(influence, Allocator.TempJob);
        ApplyInfluenceJob applyInfluenceJob = new ApplyInfluenceJob
        {
            baseMovement = 1,
            deltaTime = Time.deltaTime,
            sin = math.sin(Time.time),
            Influence = influenceNA,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
        };
        JobHandle handle = applyInfluenceJob.Schedule(transformAccessArray);
        handle.Complete();
        transformAccessArray.Dispose();
        influenceNA.Dispose();
    }
    public void Remove(Person person)
    {
        people.Remove(person);
    }
    public struct ApplyInfluenceJob : IJobParallelForTransform
    {
        public NativeArray<float> Influence;
        public float sin;
        public float deltaTime;
        public float baseMovement;
        public Unity.Mathematics.Random random;
        public void Execute(int index, TransformAccess transform)
        {
            float xDelta = random.NextFloat(-baseMovement + Influence[index], baseMovement + Influence[index] * sin);
            Vector3 targetPos = new Vector3(xDelta, 0, 0) + transform.position;
            transform.position = Vector3.Lerp(transform.position, targetPos, deltaTime);
        }
    }

    public Vector3[] getPeoplePositions()
    {
        Vector3[] positions = new Vector3[people.Count];
        for (int i = 0; i < people.Count; i++)
        {
            positions[i] = people[i].transform.position;
        }
        return positions;
    }
}
