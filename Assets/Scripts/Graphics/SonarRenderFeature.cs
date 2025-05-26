using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SonarRenderFeature : ScriptableRendererFeature
{
    private SonarRendererPass _sonarPass;
    public Color baseColor;
    public Color dotColor;
    public Vector3 dotPos;
    class SonarRendererPass : ScriptableRenderPass
    {
        public List<Vector3> Dots = new List<Vector3>();
        private const string ProfilerTag = "Sonar Pass";
        private readonly ComputeShader _sonarCS;
        private readonly Color _baseColor;
        private readonly Color _dotColor;
        private readonly Vector3 _dotPos;
        private int _dotCount = 0;
        private ComputeBuffer _dotBuf;

        public void UpdateDots(List<Vector3> dots)
        {
            Dots.Clear();
            dots.ForEach(d => Dots.Add(d));
            _dotCount = dots.Count;
            _dotBuf = new ComputeBuffer(Dots.Count, sizeof(float)*3);
            _dotBuf.SetData(Dots);
        }
        
        private int KernelIndex => _sonarCS.FindKernel("Main");
        public SonarRendererPass(Color baseColor, Color dotColor, Vector3 dotPos)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            _sonarCS = (ComputeShader)Resources.Load("SonarShader");
            _baseColor = baseColor;
            _dotColor = dotColor;
            _dotPos = dotPos;
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            renderingData.cameraData.camera.depthTextureMode = DepthTextureMode.Depth;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_dotBuf == null || _dotCount == 0) return;
            
            var camera = renderingData.cameraData;
            if (camera.cameraType == CameraType.Preview)
            {
                // Do not apply this render pass to preview windows
                return;
            }
            var colorTarget = camera.renderer.cameraColorTargetHandle;
            var depthTarget = camera.renderer.cameraDepthTargetHandle;
            var cmd = CommandBufferPool.Get(ProfilerTag);
            
            // Get temporary copy of the scene texture
            var tempColorTarget = RenderTexture.GetTemporary(camera.cameraTargetDescriptor);
            tempColorTarget.enableRandomWrite = true;
            cmd.Blit(colorTarget.rt,tempColorTarget);
            
            // Get temporary copy of the scene depth texture
            var tempDepthTarget = RenderTexture.GetTemporary(camera.cameraTargetDescriptor);
            tempColorTarget.enableRandomWrite = true;
            cmd.Blit(depthTarget.rt,tempColorTarget);
            
            // Setup compute params
            cmd.SetComputeTextureParam(_sonarCS, KernelIndex, "Scene", tempColorTarget);
            cmd.SetComputeTextureParam(_sonarCS, KernelIndex, "Depth", tempDepthTarget);
            cmd.SetComputeVectorParam(_sonarCS, "BaseColor", _baseColor);
            cmd.SetComputeVectorParam(_sonarCS, "DotColor", _dotColor);
            cmd.SetComputeFloatParam(_sonarCS, "DotRadius", 1f);
            cmd.SetComputeIntParam(_sonarCS, "DotCount", _dotCount);
            cmd.SetComputeBufferParam(_sonarCS, KernelIndex, "DotPositions", _dotBuf);
            
            // Dispatch according to thread count in shader
            _sonarCS.GetKernelThreadGroupSizes(KernelIndex,out uint groupSizeX, out uint groupSizeY, out _);
            int threadGroupsX = (int) Mathf.Ceil(tempColorTarget.width / (float)groupSizeX); 
            int threadGroupsY = (int) Mathf.Ceil(tempColorTarget.height / (float)groupSizeY);
            cmd.DispatchCompute(_sonarCS, KernelIndex, threadGroupsX, threadGroupsY, 1);
            
            // Sync compute with frame
            AsyncGPUReadback.Request(tempColorTarget).WaitForCompletion();
            
            // Copy temporary texture into colour buffer
            cmd.Blit(tempColorTarget, colorTarget);
            context.ExecuteCommandBuffer(cmd);
            
            // Clean up
            cmd.Clear();
            RenderTexture.ReleaseTemporary(tempColorTarget);
            CommandBufferPool.Release(cmd);
        }
    }

    public override void Create()
    {
        _sonarPass = new SonarRendererPass(baseColor, dotColor, dotPos);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_sonarPass);
    }
}
