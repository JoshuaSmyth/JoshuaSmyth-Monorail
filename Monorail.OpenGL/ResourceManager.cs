using Monorail.Graphics;
using Monorail.Platform;
using System;
using System.Collections.Generic;

namespace Monorail
{
    public class ResourceManager
    {
        // TODO What about QuadBatches?
        private Dictionary<int, ShaderProgram> Shaders = new Dictionary<int, ShaderProgram>();
        private Dictionary<int, Texture2D> Textures = new Dictionary<int, Texture2D>();
        private Dictionary<int, TextureFont> TextureFonts = new Dictionary<int, TextureFont>();

        private Dictionary<int, TextureCubeMap> CubeMaps = new Dictionary<int, TextureCubeMap>();
        private Dictionary<int, VertexArrayObject> VertexArrayObjects = new Dictionary<int, VertexArrayObject>();

        // TODO Set Resources Folder

        public ShaderProgram LoadShader(string vShaderFileName, string fShaderFileName)
        {
            // TODO Setup shader hotswapping
            var rv = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/" + vShaderFileName, "Resources/Shaders/Fragment/" + fShaderFileName);
            Shaders.Add(rv.ShaderProgramId, rv);
            return rv;
        }

        public VertexArrayObject LoadVAO<T>(T[] verts, int stride, int[] attributeLengths, int[] attributeOffsets) where T : IInterleavedVertex
        {
            var rv = new VertexArrayObject();
            rv.BindArrayBuffer(verts, VertexPosition.Stride, VertexPosition.AttributeLengths, VertexPosition.AttributeOffsets);

            // TODO Error Checking!
            VertexArrayObjects.Add(rv.VertexArrayObjectId, rv);

            return rv;
        }


        public void UnloadAll()
        {
            Console.WriteLine("TODO!");
        }
    }
}
