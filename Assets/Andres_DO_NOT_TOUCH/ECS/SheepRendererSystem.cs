using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class SheepRendererSystem : ComponentSystem {
    protected override void OnUpdate() {
        if (SheepScriptableRendererFeature.instance != null) {
            // Get all SheepRenderer and LocalToWorld entities.
            var sheepQuery = GetEntityQuery(typeof(SheepRenderer), typeof(LocalToWorld));
            var sheepRenderers = sheepQuery.ToComponentDataArray<SheepRenderer>(Allocator.TempJob);
            var sheepMatrices = sheepQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            // Sort all the SheepRenderers and submit them.
            SheepScriptableRendererFeature.instance.SubmitRenderers(sheepRenderers, sheepMatrices);
            
            sheepRenderers.Dispose();
            sheepMatrices.Dispose();
        }
    }
}