using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monorail.Platform
{
    public class GraphicsCapabilities
    {
        private int m_MaxTextureImageUnits;
        public int MaxFragmentTextureUnits { get { return m_MaxTextureImageUnits; } }
        public int MinFragmentTextureUnits {  get { return 16; } } // Defined by OpenGL spec

        private int m_MaxMultiSampleCount;
        public int MaxMultiSampleCount { get { return m_MaxMultiSampleCount; } }

        private int m_NumExtensions;
        public int NumExtensions {  get { return m_NumExtensions; } }

        private string m_OpenGlVersion;
        public string OpenGlVersion {  get { return m_OpenGlVersion;  } }

        private string m_OpenGlRenderer;
        public string OpenGlRenderer { get { return m_OpenGlRenderer; } }

        private string m_OpenGlVendor;
        public string OpenGlVendor { get { return m_OpenGlVendor; } }

        private HashSet<string> m_Extensions = new HashSet<string>();

        public GraphicsCapabilities()
        {
            m_OpenGlVersion = GlBindings.GetString(GLStrings.GL_VERSION);
            m_OpenGlRenderer = GlBindings.GetString(GLStrings.GL_RENDERER);
            m_OpenGlVendor = GlBindings.GetString(GLStrings.GL_VENDOR);

            Console.WriteLine("OpenGL Renderer:" + OpenGlRenderer);
            Console.WriteLine("OpenGL Version:" + m_OpenGlVersion);
            Console.WriteLine("OpenGL Vendor:" + m_OpenGlVendor);

            GlBindings.GetIntegerv(Capabilities.GL_MAX_TEXTURE_IMAGE_UNITS, out m_MaxTextureImageUnits);
            GlBindings.GetIntegerv(Capabilities.GL_MAX_SAMPLES, out m_MaxMultiSampleCount);
            GlBindings.GetIntegerv(Capabilities.GL_NUM_EXTENSIONS, out m_NumExtensions);

            Console.WriteLine("GL_MAX_TEXTURE_IMAGE_UNITS:" + m_MaxTextureImageUnits);
            Console.WriteLine("GL_MAX_SAMPLES:" + m_MaxMultiSampleCount);
            Console.WriteLine("GL_NUM_EXTENSIONS:" + m_NumExtensions);
            
            for (uint i = 0; i < m_NumExtensions; i++)
            {
                m_Extensions.Add(GlBindings.glGetStringi(GLStrings.GL_EXTENSIONS, i));
            }
            Console.WriteLine("");
            foreach (var e in m_Extensions)
            {
                Console.WriteLine(e);
            }
        }

        private bool HasExtension(string extensionName)
        {
            return m_Extensions.Contains(extensionName);
        }
    }
}
