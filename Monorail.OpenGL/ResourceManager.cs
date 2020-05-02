using Monorail.Graphics;
using Monorail.Platform;
using System;
using System.Collections.Generic;

namespace Monorail
{
    // TODO Put these into their own files
    public class VertexBuffer<T> where T : IInterleavedVertex
    {
        public int Id;
        public T[] Data = new T[0];

        public int Stride;

        public int Size
        {
            get
            {
                return Data.Length * this.Stride;
            }
        }
    }

    // TODO Put this into it's own files
    public class IndexBuffer
    {
        public int Id;
        public int vertexBufferSize;
        public ushort Data;
    }

    public class ResourceManager
    {
        // TODO What about QuadBatches?
        private Dictionary<int, ShaderProgram> Shaders = new Dictionary<int, ShaderProgram>();
        private Dictionary<int, Texture2D> Textures = new Dictionary<int, Texture2D>();
        private Dictionary<int, TextureFont> TextureFonts = new Dictionary<int, TextureFont>();

        private Dictionary<int, TextureCubeMap> CubeMaps = new Dictionary<int, TextureCubeMap>();
        private Dictionary<int, VertexArrayObject> VertexArrayObjects = new Dictionary<int, VertexArrayObject>();

        // TODO Set Resources Folder
        const string TexturesFolder = "Resources/Textures/";
        const string VertexShadersFolder = "Resources/Shaders/Vertex/";
        const string FragmentShaderFolder = "Resources/Shaders/Fragment/";
        const string FontsFolder = "Resources/Fonts/";

        internal ResourceManager()
        {

        }

        public TextureFont LoadTextureFont(string filename)
        {
            var rv = TextureFont.CreateFromFile(FontsFolder + filename);
            TextureFonts.Add(rv.FontTexture.TextureId, rv);
            return rv;
        }

        public Texture2D LoadTexture2d(string filename, bool generateMipmaps=false)
        {
            // TODO Setup Texture hotswapping
            var rv = Texture2D.CreateFromFile(TexturesFolder + filename, generateMipmaps);
            Textures.Add(rv.TextureId, rv);
            return rv;
        }

        public TextureCubeMap LoadCubeMap(string front, string back, string bottom, string top, string left, string right)
        {
            // TODO Setup Cubemap hotswapping
            var rv = TextureCubeMap.CreateFromFile(TexturesFolder + front, TexturesFolder + back, TexturesFolder + bottom, TexturesFolder + top, TexturesFolder + left, TexturesFolder + right);
            CubeMaps.Add(rv.TextureId, rv);
            return rv;
        }

        public ShaderProgram LoadShaderFromString(string vertexShaderCode, string fragmentShaderCode)
        {
            // TODO Setup shader hotswapping
            var rv = ShaderProgram.CreateFromString(vertexShaderCode, fragmentShaderCode);
            Shaders.Add(rv.ShaderProgramId, rv);
            return rv;
        }

        public ShaderProgram LoadShaderFromFile(string vShaderFileName, string fShaderFileName)
        {
            // TODO Setup shader hotswapping
            var rv = ShaderProgram.CreateFromFile(VertexShadersFolder + vShaderFileName, FragmentShaderFolder + fShaderFileName);
            Shaders.Add(rv.ShaderProgramId, rv);
            return rv;
        }

        public VertexArrayObject LoadVAO<T>(T[] verts, int stride, int[] attributeLengths, int[] attributeOffsets) where T : IInterleavedVertex
        {
            var rv = new VertexArrayObject();
            rv.BindArrayBuffer(verts, stride, attributeLengths, attributeOffsets);

            // TODO Error Checking!
            VertexArrayObjects.Add(rv.VaoId, rv);

            return rv;
        }
        public VertexArrayObject LoadVAO<T>(T[] verts, uint[] indicies, int stride, int[] attributeLengths, int[] attributeOffsets) where T : IInterleavedVertex
        {
            var rv = new VertexArrayObject();
            rv.BindElementsArrayBuffer(verts, indicies, stride, attributeLengths, attributeOffsets);

            // TODO Error Checking!
            VertexArrayObjects.Add(rv.VaoId, rv);

            return rv;
        }

        public void UnloadAll()
        {
            Console.WriteLine("TODO!");
        }
    }
}
