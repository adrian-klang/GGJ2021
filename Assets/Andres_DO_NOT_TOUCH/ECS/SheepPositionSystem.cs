using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SheepPositionSystem : ComponentSystem {
    protected override void OnUpdate() {
        // var pos = new float3(Mathf.Sin((float)Time.ElapsedTime), Mathf.Sin((float)Time.ElapsedTime * 2.0f), 1.0f - Mathf.Sin((float)Time.ElapsedTime)) * 10.0f;
        // 
        // var sheepQuery = GetEntityQuery(typeof(SheepRenderer), typeof(Rotation), typeof(Translation));
        // var inputEntities = sheepQuery.ToEntityArray(Allocator.TempJob);
        // var inputTranslations = sheepQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        // var rotationJob = new EntityRotationJob {
        //         InputEntities = inputEntities, inputTranslations = inputTranslations, InputPos = pos, InputCommandBuffer = this.PostUpdateCommands
        // };
        // 
        // rotationJob.Schedule().Complete();
        // 
        // inputEntities.Dispose();
        // inputTranslations.Dispose();
    }

    // [BurstCompile(CompileSynchronously = true)]
    // private struct EntityRotationJob : IJob {
    //     [ReadOnly]
    //     public NativeArray<Entity> InputEntities;
    //     [ReadOnly]
    //     public NativeArray<Translation> inputTranslations;
    //     [ReadOnly]
    //     public float3 InputPos;
    //     [WriteOnly]
    //     public EntityCommandBuffer InputCommandBuffer;
    // 
    //     public void Execute() {
    //         for (var i = 0; i < InputEntities.Length; i++) {
    //             InputCommandBuffer.AddComponent(InputEntities[i], new Rotation {Value = quaternion.LookRotation(inputTranslations[i].Value - InputPos, math.up())});
    //         }
    //     }
    // }
}