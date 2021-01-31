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

        var inputTranslations = new NativeArray<Translation>(Sheeps.Count, Allocator.TempJob);
        var inputVelocities = new NativeArray<float2>(Sheeps.Count, Allocator.TempJob);
        for (var i = 0; i < Sheeps.Count; i++) {
            inputVelocities[i] = Sheeps[i].Rigidbody.velocity.normalized;
            inputTranslations[i] = new Translation {Value = Sheeps[i].transform.position};
        }
        
        var results = new NativeArray<float3>(Sheeps.Count, Allocator.TempJob);
        var sheepFlockingJobs = new SheepFlockingJob {
                InputTranslations = inputTranslations,
                InputVelocities = inputVelocities,
                AlignmentForce = Config.AlignmentForce,
                alignmentSqrRadius = Config.AlignmentRadius * Config.AlignmentRadius,
                CohesionForce = Config.CohesionForce,
                cohesionSqrRadius = Config.CohesionRadius * Config.CohesionRadius,
                SeparationForce = Config.SeparationForce,
                separationSqrRadius = Config.SeparationRadius * Config.SeparationRadius,
                OutputResults = results
        };

        sheepFlockingJobs.Schedule(Sheeps.Count, 64).Complete();
        for (var i = 0; i < Sheeps.Count; i++) {
            Sheeps[i].Rigidbody.AddForce(results[i].xy);
            // TODO: FIX;
            Sheeps[i].Tamed = results[i].z == 1.0f;
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
        public float alignmentSqrRadius;
        [ReadOnly]
        public float separationSqrRadius;
        [ReadOnly]
        public float cohesionSqrRadius;
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

                var posTwo = InputTranslations[j].Value;
                var velocityTwo = InputVelocities[j];

                var sqrDist = (posOne.x - posTwo.x) * (posOne.x - posTwo.x) + (posOne.y - posTwo.y) * (posOne.y - posTwo.y);

                if (sqrDist < separationSqrRadius) {
                    separationVector += posTwo.xy;
                    separationCount++;
                }

                if (sqrDist < alignmentSqrRadius) {
                    alignmentVector += velocityTwo;
                    alignmentCount++;
                }

                if (sqrDist < cohesionSqrRadius) {
                    cohesionVector += posTwo.xy;
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

            OutputResults[i] = new float3(totalForce.x, totalForce.y, tamed ? 1.0f : 0.0f);
        }
    }
}