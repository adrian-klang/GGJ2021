using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SheepPass : ScriptableRenderPass, IDisposable {
    private const string ProfilerTag = "SheepPass";

    private readonly Mesh mesh;
    private readonly Material material;

    private GraphicsBuffer indexBuffer;
    private GraphicsBuffer drawArgsBuffer;

    public SheepPass(Mesh mesh, Material material) {
        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        this.mesh = mesh;
        this.material = material;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
        base.Configure(cmd, cameraTextureDescriptor);
    }

    public void Update(GraphicsBuffer indexBuffer, GraphicsBuffer drawArgsBuffer) {
        this.indexBuffer = indexBuffer;
        this.drawArgsBuffer = drawArgsBuffer;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        var cmd = CommandBufferPool.Get(ProfilerTag);
        cmd.DrawProceduralIndirect(indexBuffer, Matrix4x4.identity, material, 0, MeshTopology.Triangles, drawArgsBuffer, 0);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd) {
        base.FrameCleanup(cmd);
    }

    public void Dispose() {
#if UNITY_EDITOR
        // Object.DestroyImmediate(Material);
#else
        // Object.Destroy(Material);
#endif
    }
}

public class SheepScriptableRendererFeature : ScriptableRendererFeature {
    public static SheepScriptableRendererFeature instance;

    public const int MAX_SHEEP = 10000;

    public Mesh sheepMesh;
    public Material sheepMaterial;
    [Space, Range(0, 10000)]
    public int sheepCount;

    private readonly Dictionary<int, SheepPass> passes = new Dictionary<int, SheepPass>();
    private GraphicsBuffer vertexBuffer;
    private GraphicsBuffer indexBuffer;
    private GraphicsBuffer instancedResourcesBuffer;
    private GraphicsBuffer drawArgsBuffer;

    private static readonly int VertexBufferId = Shader.PropertyToID("VertexBuffer");
    private static readonly int InstanceResourcesBufferId = Shader.PropertyToID("InstanceResourcesBuffer");
    static readonly int[] _idSHA = {Shader.PropertyToID("sheep_SHAr"), Shader.PropertyToID("sheep_SHAg"), Shader.PropertyToID("sheep_SHAb")};
    static readonly int[] _idSHB = {Shader.PropertyToID("sheep_SHBr"), Shader.PropertyToID("sheep_SHBg"), Shader.PropertyToID("sheep_SHBb")};
    static readonly int _idSHC = Shader.PropertyToID("sheep_SHC");
    private const int VertexBufferStride = 36;
    private const int InstanceResourcesStride = 64;

#if UNITY_EDITOR
    private Mesh lastMesh;
#endif

    public override void Create() {
        if (vertexBuffer == null && sheepMesh != null) {
            InitializeVertexBuffer(sheepMesh);
        }

        if (indexBuffer == null && sheepMesh != null) {
            InitializeIndexBuffer(sheepMesh);
        }

        if (instancedResourcesBuffer == null) {
            InitializeInstanceResourcesBuffer();
        }

        if (drawArgsBuffer == null) {
            drawArgsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1 * 1 * 5, 4);
        }

        instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (renderingData.cameraData.renderType != CameraRenderType.Base) {
            return;
        }

        var cameraID = renderingData.cameraData.camera.GetInstanceID();

        // Recreate resources on domain reload.
        var firstFrame = !passes.TryGetValue(cameraID, out var pass);
        // if (pass?.Material == null) {
        //     passes.Remove(cameraID);
        //     firstFrame = true;
        // }

        // Create a new pass per-camera (or get it if already created).
        if (firstFrame) {
            pass = new SheepPass(sheepMesh, sheepMaterial);
            passes.Add(cameraID, pass);
        }

#if UNITY_EDITOR
        if (lastMesh != sheepMesh) {
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            InitializeVertexBuffer(sheepMesh);
            InitializeIndexBuffer(sheepMesh);
            lastMesh = sheepMesh;
        }
#endif

        // Update drawArgs.
        if (drawArgsBuffer != null && sheepMesh != null) {
            var drawArgs = new int[1 * 1 * 5];
            drawArgs[0] = sheepMesh.GetSubMesh(0).indexCount;
            drawArgs[1] = sheepCount;
            drawArgs[2] = 0;
            drawArgs[3] = 0;
            drawArgs[4] = 0;
            drawArgsBuffer.SetData(drawArgs);
        }

        // Update the ambient light.
        SetSHCoefficients(sheepMaterial);

        // Update the pass values and enqueue it.
        pass.Update(indexBuffer, drawArgsBuffer);
        renderer.EnqueuePass(pass);
    }

    private void InitializeVertexBuffer(Mesh mesh) {
        vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.vertexCount, VertexBufferStride);
        Shader.SetGlobalBuffer(VertexBufferId, this.vertexBuffer);

        var meshBuffer = new float[mesh.vertexCount * VertexBufferStride / 4];
        var positions = new List<Vector3>();
        var uvs = new List<Vector2>();
        var normals = new List<Vector3>();

        uint meshBufferIdx = 0;
        mesh.GetVertices(positions);
        mesh.GetUVs(0, uvs);
        mesh.GetNormals(normals);
        for (var j = 0; j < positions.Count; ++j) {
            meshBuffer[meshBufferIdx++] = positions[j].x;
            meshBuffer[meshBufferIdx++] = positions[j].y;
            meshBuffer[meshBufferIdx++] = positions[j].z;
            meshBuffer[meshBufferIdx++] = 1;
            meshBuffer[meshBufferIdx++] = uvs[j].x;
            meshBuffer[meshBufferIdx++] = uvs[j].y;
            meshBuffer[meshBufferIdx++] = normals[j].x;
            meshBuffer[meshBufferIdx++] = normals[j].y;
            meshBuffer[meshBufferIdx++] = normals[j].z;
        }

        vertexBuffer.SetData(meshBuffer);
        Debug.Log("Init Vertex Buffer");
    }

    private void InitializeIndexBuffer(Mesh mesh) {
        indexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Index, sheepMesh.GetSubMesh(0).indexCount, 4);
        indexBuffer.SetData(mesh.GetIndices(0));
        Debug.Log("Init Index Buffer");
    }

    private void InitializeInstanceResourcesBuffer() {
        instancedResourcesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, MAX_SHEEP, InstanceResourcesStride);
        Shader.SetGlobalBuffer(InstanceResourcesBufferId, instancedResourcesBuffer);
        Debug.Log("Init Resources Buffer");
    }

    private void SetSHCoefficients(Material material) {
        // LightProbes.GetInterpolatedProbe(position, null, out var sh);
        var sh = RenderSettings.ambientProbe;

        // Constant + Linear
        for (var i = 0; i < 3; i++) {
            material.SetVector(_idSHA[i], new Vector4(sh[i, 3], sh[i, 1], sh[i, 2], sh[i, 0] - sh[i, 6]));
        }

        // Quadratic polynomials
        for (var i = 0; i < 3; i++) {
            material.SetVector(_idSHB[i], new Vector4(sh[i, 4], sh[i, 6], sh[i, 5] * 3, sh[i, 7]));
        }

        // Final quadratic polynomial
        material.SetVector(_idSHC, new Vector4(sh[0, 8], sh[2, 8], sh[1, 8], 1));
    }

    public void SubmitRenderers(NativeArray<SheepRenderer> renderers, NativeArray<LocalToWorld> localToWorlds) {
        instancedResourcesBuffer?.SetData(localToWorlds);
    }

    private void OnDestroy() {
        vertexBuffer?.Dispose();
        indexBuffer?.Dispose();
        drawArgsBuffer?.Dispose();
        instancedResourcesBuffer?.Dispose();

        foreach (var entry in passes) {
            entry.Value.Dispose();
        }

        passes.Clear();
    }
}