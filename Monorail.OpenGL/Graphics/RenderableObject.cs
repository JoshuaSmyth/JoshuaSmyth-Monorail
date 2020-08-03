using Monorail.Platform;
namespace Monorail.Graphics
{
    public enum BlendModes
    {
        Opaque = 0,
        Alpha = 1
    }

    public abstract class RenderableObject
    {
        public int ShaderId { get; set; }
        public int VaoId { get; set; }
        public int VertexCount { get; set; }
        public bool HasIndexBuffer { get; set; }

        // TODO switch to array of textures
        public uint TextureIdA { get; set; }
        public uint TextureIdB { get; set; }
        public uint CubemapTextureId { get; set; }
        public bool DepthBufferEnabled { get; set; }
        public bool IsWireframe { get; set; }
        public BlendModes BlendMode { get; set; }

        public abstract void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera);
    }
}
