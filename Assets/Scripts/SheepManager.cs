using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour {
    private static List<Sheep> Sheeps = new List<Sheep>(128);

    public GameConfig Config;

    private float separationSqrRadius;
    private float alignmentSqrRadius;
    private float cohesionSqrRadius;
    
    private List<Vector2> separationVectors = new List<Vector2>();
    private List<Vector2> alignmentVectors = new List<Vector2>();
    private List<Vector2> cohesionVectors = new List<Vector2>();
    
    public static void RegisterSheep(Sheep sheep) {
        Sheeps.Add(sheep);
    }

    public static void DeregisterSheep(Sheep sheep) {
        Sheeps.Remove(sheep);
    }

    private void Update() {
        separationSqrRadius = Config.SeparationRadius * Config.SeparationRadius;
        alignmentSqrRadius = Config.AlignmentRadius * Config.AlignmentRadius;
        cohesionSqrRadius = Config.CohesionRadius * Config.CohesionRadius;
    }

    private void FixedUpdate() {
        foreach (var sheepOne in Sheeps) {
            
            separationVectors.Clear();
            alignmentVectors.Clear();
            cohesionVectors.Clear();
            
            Vector2 posOne = sheepOne.transform.position;
            
            foreach (var sheepTwo in Sheeps) {
                if (sheepOne == sheepTwo) {
                    continue;
                }
                
                Vector2 posTwo = sheepTwo.transform.position;

                var sqrDist = (posOne.x - posTwo.x) * (posOne.x - posTwo.x) + (posOne.y - posTwo.y) * (posOne.y - posTwo.y);

                if (sqrDist < separationSqrRadius) {
                    separationVectors.Add(posTwo);
                }

                if (sqrDist < alignmentSqrRadius) {
                    alignmentVectors.Add(sheepTwo.Rigidbody.velocity.normalized);
                }

                if (sqrDist < cohesionSqrRadius) {
                    cohesionVectors.Add(posTwo);
                }
            }

            var totalForce = Vector2.zero;

            if (cohesionVectors.Count > 0) {
                var avgPos = Vector2.zero;
                foreach (var vector2 in cohesionVectors) {
                    avgPos += vector2;
                }

                avgPos /= cohesionVectors.Count;

                totalForce += (avgPos - posOne).normalized * Config.CohesionForce;
            }
            
            
            if (separationVectors.Count > 0) {
                var avgPos = Vector2.zero;
                foreach (var vector2 in separationVectors) {
                    avgPos += vector2;
                }

                avgPos /= separationVectors.Count;

                totalForce += (posOne - avgPos).normalized * Config.SeparationForce;
            }

            if (alignmentVectors.Count > 0) {
                var avgDir = Vector2.zero;
                foreach (var vector2 in alignmentVectors) {
                    avgDir += vector2;
                }

                avgDir /= alignmentVectors.Count;

                totalForce += avgDir.normalized * Config.AlignmentForce;
            }
            
            sheepOne.Rigidbody.AddForce(totalForce);
        }
    }
}
