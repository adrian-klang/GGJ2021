using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class SheepRendererSystem : ComponentSystem {
    protected override void OnUpdate() {
        if (SheepScriptableRendererFeature.instance != null) {
            // Get all SheepRenderer and LocalToWorld entities.
            var sheepQuery = GetEntityQuery(typeof(SheepRenderer), typeof(LocalToWorld));
            var sheepRenderers = sheepQuery.ToComponentDataArray<SheepRenderer>(Allocator.TempJob);
            // var sheepMatrices = sheepQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
            
            var sheepMatrices = new NativeArray<LocalToWorld>(SheepScriptableRendererFeature.MAX_SHEEP, Allocator.Temp);
            int sheepIdx = 0;
            foreach (var sheep in SheepManager.Sheeps) {
                sheepMatrices[sheepIdx++] = new LocalToWorld(){Value = sheep.transform.localToWorldMatrix};
            }

            // Sort all the SheepRenderers and submit them.
            SheepScriptableRendererFeature.instance.SubmitRenderers(sheepRenderers, sheepMatrices);
            
            sheepRenderers.Dispose();
            sheepMatrices.Dispose();
        }
    }
}