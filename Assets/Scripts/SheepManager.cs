﻿using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SheepManagerSystem : ComponentSystem {
    public static List<Sheep> Sheeps = new List<Sheep>(128);
    private static GameConfig Config;

    protected override void OnUpdate() {
        if (Config == null) {
            Config = Resources.Load<GameConfig>("GameConfig");
            NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        }

        var sheepQuery = GetEntityQuery(typeof(SheepRenderer), typeof(Translation));
        var inputTranslations = sheepQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var inputVelocities = new NativeArray<float2>(Sheeps.Count, Allocator.TempJob);
        for (var i = 0; i < Sheeps.Count; i++) {
            inputVelocities[i] = Sheeps[i].Rigidbody.velocity.normalized;
        }

        NativeArray<SheepRenderer> sheepRenderers = new NativeArray<SheepRenderer>();
        NativeArray<LocalToWorld> sheepMatrices = new NativeArray<LocalToWorld>();
        if (SheepScriptableRendererFeature.instance != null) {
            sheepRenderers = sheepQuery.ToComponentDataArray<SheepRenderer>(Allocator.TempJob);
            sheepMatrices = new NativeArray<LocalToWorld>(SheepScriptableRendererFeature.MAX_SHEEP, Allocator.Temp);
        }

        var results = new NativeArray<float3>(Sheeps.Count, Allocator.TempJob);
        var sheepFlockingJobs = new SheepFlockingJob {
                InputTranslations = inputTranslations,
                InputVelocities = inputVelocities,
                AlignmentForce = Config.AlignmentForce,
                AlignmentSqrRadius = Config.AlignmentRadius * Config.AlignmentRadius,
                CohesionForce = Config.CohesionForce,
                CohesionSqrRadius = Config.CohesionRadius * Config.CohesionRadius,
                SeparationForce = Config.SeparationForce,
                SeparationSqrRadius = Config.SeparationRadius * Config.SeparationRadius,
                MaxDistance = Mathf.Max(Config.AlignmentRadius, Config.CohesionRadius, Config.SeparationRadius),
                OutputResults = results
        };

        sheepFlockingJobs.Schedule(Sheeps.Count, 64).Complete();
        for (var i = 0; i < Sheeps.Count; i++) {
            Sheeps[i].Rigidbody.AddForce(results[i].xy);
            // TODO: FIX;
            Sheeps[i].Tamed = results[i].z == 1.0f;

            if (SheepScriptableRendererFeature.instance != null) {
                sheepMatrices[i] = new LocalToWorld(){Value = Sheeps[i].transform.localToWorldMatrix};
            }
        }
        
        if (SheepScriptableRendererFeature.instance != null) {
            SheepScriptableRendererFeature.instance.SubmitRenderers(sheepRenderers, sheepMatrices);
            sheepRenderers.Dispose();
            sheepMatrices.Dispose();
        }

        inputTranslations.Dispose();
        inputVelocities.Dispose();
        results.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct SheepFlockingJob : IJobParallelFor {
        [ReadOnly]
        public NativeArray<Translation> InputTranslations;
        [ReadOnly]
        public NativeArray<float2> InputVelocities;
        [ReadOnly]
        public float AlignmentSqrRadius;
        [ReadOnly]
        public float SeparationSqrRadius;
        [ReadOnly]
        public float CohesionSqrRadius;
        [ReadOnly]
        public float MaxDistance;
        [ReadOnly]
        public float AlignmentForce;
        [ReadOnly]
        public float SeparationForce;
        [ReadOnly]
        public float CohesionForce;

        [WriteOnly]
        public NativeArray<float3> OutputResults;

        public void Execute(int i) {
            var posOne = InputTranslations[i].Value.xy;

            var cohesionVector = new float2();
            var cohesionCount = 0;
            var separationVector = new float2();
            var separationCount = 0;
            var alignmentVector = new float2();
            var alignmentCount = 0;

            for (var j = 0; j < InputTranslations.Length; j++) {
                if (i == j) {
                    continue;
                }

                var posTwo = InputTranslations[j].Value.xy;
                if (math.length(posOne - posTwo) > MaxDistance) {
                    continue;
                }

                var sqrDist = math.distancesq(posOne, posTwo);
                if (sqrDist < SeparationSqrRadius) {
                    separationVector += posTwo;
                    separationCount++;
                }
                if (sqrDist < AlignmentSqrRadius) {
                    alignmentVector += InputVelocities[j];
                    alignmentCount++;
                }
                if (sqrDist < CohesionSqrRadius) {
                    cohesionVector += posTwo;
                    cohesionCount++;
                }
            }

            var tamed = false;
            var totalForce = new float2();

            if (cohesionCount > 0) {
                cohesionVector /= cohesionCount;
                totalForce += math.normalize(cohesionVector - posOne) * CohesionForce;
                tamed = true;
            }

            if (separationCount > 0) {
                separationVector /= separationCount;
                totalForce += math.normalize(posOne - separationVector) * SeparationForce;
                tamed = true;
            }

            if (alignmentCount > 0) {
                if (alignmentVector.x != 0.0f && alignmentVector.y != 0.0f) {
                    alignmentVector /= alignmentCount;
                    totalForce += math.normalize(alignmentVector) * AlignmentForce;
                }

                tamed = true;
            }

            OutputResults[i] = new float3(totalForce.xy, tamed ? 1.0f : 0.0f);
        }
    }
}