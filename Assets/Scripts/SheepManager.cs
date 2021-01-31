using System.Collections.Generic;
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
        NativeArray<SheepRenderer> sheepRenderers = new NativeArray<SheepRenderer>();
        NativeArray<LocalToWorld> sheepMatrices = new NativeArray<LocalToWorld>();
        NativeArray<float> sheepVelocities = new NativeArray<float>();
        if (SheepScriptableRendererFeature.instance != null) {
            sheepRenderers = sheepQuery.ToComponentDataArray<SheepRenderer>(Allocator.TempJob);
            sheepMatrices = new NativeArray<LocalToWorld>(SheepScriptableRendererFeature.MAX_SHEEP, Allocator.Temp);
            sheepVelocities = new NativeArray<float>(SheepScriptableRendererFeature.MAX_SHEEP, Allocator.Temp);
        }

        var inputTranslations = new NativeArray<Translation>(Sheeps.Count, Allocator.TempJob);
        var inputVelocities = new NativeArray<float3>(Sheeps.Count, Allocator.TempJob);
        var properties = new NativeArray<int2>(Sheeps.Count, Allocator.TempJob);
        for (var i = 0; i < Sheeps.Count; i++) {
            inputVelocities[i] = Sheeps[i].Rigidbody.velocity;
            properties[i] = new int2(Sheeps[i].gameObject.activeSelf ? 1 : 0, Sheeps[i].Tamed ? 1 : 0);
            inputTranslations[i] = new Translation {Value = Sheeps[i].transform.position};
        }

        var results = new NativeArray<float3>(Sheeps.Count, Allocator.TempJob);
        var sheepFlockingJobs = new SheepFlockingJob {
                Properties = properties,
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
            var fwd = Sheeps[i].transform.forward;
            Sheeps[i].transform.forward = Vector3.Slerp(fwd, -Vector3.Normalize(Sheeps[i].Rigidbody.velocity), Config.SheepRotationLerp * Time.DeltaTime);
            Sheeps[i].Rigidbody.AddForce(new float3(results[i].x, 0, results[i].y));
            Sheeps[i].Tamed = results[i].z == 1.0f;

            if (SheepScriptableRendererFeature.instance != null) {
                sheepMatrices[i] = new LocalToWorld() {Value = Sheeps[i].transform.localToWorldMatrix};
                sheepVelocities[i] = Sheeps[i].Rigidbody.velocity.sqrMagnitude;
            }
        }

        if (SheepScriptableRendererFeature.instance != null) {
            SheepScriptableRendererFeature.instance.SubmitRenderers(sheepMatrices, sheepVelocities);
            sheepRenderers.Dispose();
            sheepMatrices.Dispose();
            sheepVelocities.Dispose();
        }

        inputTranslations.Dispose();
        inputVelocities.Dispose();
        results.Dispose();
        properties.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct SheepFlockingJob : IJobParallelFor {
        [ReadOnly]
        public NativeArray<int2> Properties;
        [ReadOnly]
        public NativeArray<Translation> InputTranslations;
        [ReadOnly]
        public NativeArray<float3> InputVelocities;
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
            var posOne = InputTranslations[i].Value.xz;

            var cohesionVector = new float2();
            var cohesionCount = 0;
            var separationVector = new float2();
            var separationCount = 0;
            var alignmentVector = new float2();
            var alignmentCount = 0;

            bool firstSheepTamed = Properties[i].y == 1;
            
            if (Properties[i].x == 1) {
                for (var j = 0; j < InputTranslations.Length; j++) {
                    if (Properties[j].x == 0) {
                        continue;
                    }

                    if (i == j) {
                        continue;
                    }

                    var posTwo = InputTranslations[j].Value.xz;
                    if (math.length(posOne - posTwo) > MaxDistance) {
                        continue;
                    }

                    var sqrDist = math.distancesq(posOne, posTwo);
                    if (sqrDist < SeparationSqrRadius) {
                        separationVector += posTwo;
                        separationCount++;
                    }

                    if (sqrDist < AlignmentSqrRadius) {
                        alignmentVector += InputVelocities[j].xz;
                        alignmentCount++;
                    }

                    if (sqrDist < CohesionSqrRadius) {
                        cohesionVector += posTwo;
                        cohesionCount++;
                    }
                }
            }

            var tamed = false;
            var totalForce = new float2();

            if (cohesionCount > 0) {
                cohesionVector /= cohesionCount;
                totalForce += math.normalize(cohesionVector - posOne) * CohesionForce;
                
                if (firstSheepTamed)
                    tamed = true;
            }

            if (separationCount > 0) {
                separationVector /= separationCount;
                totalForce += math.normalize(posOne - separationVector) * SeparationForce;
                
                if (firstSheepTamed)
                    tamed = true;
            }

            if (alignmentCount > 0) {
                if (alignmentVector.x != 0.0f && alignmentVector.y != 0.0f) {
                    alignmentVector /= alignmentCount;
                    totalForce += alignmentVector * AlignmentForce;
                }

                if (firstSheepTamed)
                    tamed = true;
            }

            if (totalForce.x != 0.0f || totalForce.y != 0.0f) {
                totalForce.xy = math.normalize(totalForce.xy);
            }

            OutputResults[i] = new float3(totalForce, (tamed || Properties[i].y == 1) ? 1.0f : 0.0f);
        }
    }
}