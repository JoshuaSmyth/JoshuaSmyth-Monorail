using Monorail.Graphics;
using OpenGL;
using System;
using System.Runtime.InteropServices;

namespace Monorail.Platform
{
    public class VertexArrayObject
    {
        private int m_VaoId;
        public int VaoId { get { return m_VaoId; } }

        private int m_VertexArrayBufferId;
        public int VertexArrayBufferId { get { return m_VertexArrayBufferId; } }

        private int m_VertexCount;
        public int VertexCount { get { return m_VertexCount; } }

        public bool HasElementsIndex;

        private int m_IndexBufferId;
        public int IndexBufferId { get { return m_IndexBufferId; } }

        private int m_Stride;
        public int Stride { get { return m_Stride; } }

        private BufferUsageHint m_BufferUsage;
        public BufferUsageHint BufferUsage { get { return m_BufferUsage; } }

        internal VertexArrayObject()
        {

        }

        public unsafe void SetData<T>(T[] verts, uint[] indicies)
        {
            GCHandle handleVerticies = GCHandle.Alloc(verts, GCHandleType.Pinned);
            GCHandle handleIndicies = GCHandle.Alloc(indicies, GCHandleType.Pinned);

            IntPtr ptrVerticies = handleVerticies.AddrOfPinnedObject();
            IntPtr ptrIndicies = handleIndicies.AddrOfPinnedObject();

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, m_VertexArrayBufferId);
            GlBindings.BufferData(BufferTarget.GL_ARRAY_BUFFER, m_Stride * verts.Length, ptrVerticies, m_BufferUsage);

            GlBindings.BindBuffer(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, m_IndexBufferId);
            GlBindings.BufferData(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, indicies.Length * 4, ptrIndicies, m_BufferUsage);   // TODO Support UnsignedInt and UnsignedShort

            handleVerticies.Free();
            handleIndicies.Free();

            m_VertexCount = indicies.Length;
        }

        public unsafe void UpdateVertexData<T>(T[] verts, int offset, int vertexCount)
        {
            GCHandle handleVerticies = GCHandle.Alloc(verts, GCHandleType.Pinned);

            IntPtr ptrVerticies = handleVerticies.AddrOfPinnedObject();

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, m_VertexArrayBufferId);
            GlBindings.BufferSubData(BufferTarget.GL_ARRAY_BUFFER, (IntPtr)0, (IntPtr)(m_Stride*vertexCount), ptrVerticies);

            handleVerticies.Free();
        }

        public unsafe void SetVertexData<T>(T[] verts)
        {
            GCHandle handleVerticies = GCHandle.Alloc(verts, GCHandleType.Pinned);

            IntPtr ptrVerticies = handleVerticies.AddrOfPinnedObject();
            
            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, m_VertexArrayBufferId);
            GlBindings.BufferData(BufferTarget.GL_ARRAY_BUFFER, m_Stride * verts.Length, ptrVerticies, m_BufferUsage);
            
            handleVerticies.Free();
        }

        public unsafe void SetIndexData32(uint[] indicies)
        {
            GCHandle handleIndicies = GCHandle.Alloc(indicies, GCHandleType.Pinned);

            IntPtr ptrIndicies = handleIndicies.AddrOfPinnedObject();

            GlBindings.BindBuffer(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, m_IndexBufferId);
            GlBindings.BufferData(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, indicies.Length * 4, ptrIndicies, m_BufferUsage);   // TODO Support UnsignedInt and UnsignedShort
            handleIndicies.Free();

            m_VertexCount = indicies.Length;
        }

        public unsafe void UpdateIndexData32(uint[] indicies)
        {
            GCHandle handleIndicies = GCHandle.Alloc(indicies, GCHandleType.Pinned);

            IntPtr ptrIndicies = handleIndicies.AddrOfPinnedObject();

            GlBindings.BindBuffer(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, m_IndexBufferId);
            GlBindings.BufferSubData(BufferTarget.GL_ARRAY_BUFFER, (IntPtr)0, (IntPtr)(4 * indicies.Length), ptrIndicies);
            handleIndicies.Free();

            m_VertexCount = indicies.Length;
        }

        /// <summary>
        /// Initalises the Vertex Array Object without seting any vertex or index data
        /// </summary>
        public unsafe void InitElementsArrayBuffer<T>(int stride, int[] attributeLengths, int[] attributeOffsets, BufferUsageHint hint = BufferUsageHint.GL_STATIC_DRAW) where T : IInterleavedVertex
        {
            GlBindings.GenVertexArrays(1, out m_VaoId);

            GlBindings.GenBuffers(1, out m_VertexArrayBufferId);
            GlBindings.GenBuffers(1, out m_IndexBufferId);

            GlBindings.BindVertexArray(m_VaoId);

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, m_VertexArrayBufferId);

            GlBindings.BindBuffer(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, m_IndexBufferId);


            // Setup Attributes
            for (int i = 0; i < attributeLengths.Length; i++)
            {
                GlBindings.VertexAttribPointer(i, attributeLengths[i], VertexAttribPointerType.Float, 0, stride, attributeOffsets[i]);
                GlBindings.EnableVertexAttribArray(i);
            }

            //m_VertexCount = indicies.Length;
            m_Stride = stride;
            m_BufferUsage = hint;

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, 0);
            GlBindings.BindVertexArray(0);
        }

        /// <summary>
        /// Initalises the Vertex Buffer Object and sets the given vertex and index data
        /// </summary>
        public unsafe void BindElementsArrayBuffer<T>(T[] verts, uint[] indicies, int stride, int[] attributeLengths, int[] attributeOffsets, BufferUsageHint hint = BufferUsageHint.GL_STATIC_DRAW) where T : IInterleavedVertex
        {
            GlBindings.GenVertexArrays(1, out m_VaoId);

            GlBindings.GenBuffers(1, out m_VertexArrayBufferId);
            GlBindings.GenBuffers(1, out m_IndexBufferId);

            GlBindings.BindVertexArray(m_VaoId);

            GCHandle handleVerticies = GCHandle.Alloc(verts, GCHandleType.Pinned);
            GCHandle handleIndicies = GCHandle.Alloc(indicies, GCHandleType.Pinned);

            IntPtr ptrVerticies = handleVerticies.AddrOfPinnedObject();
            IntPtr ptrIndicies = handleIndicies.AddrOfPinnedObject();

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, m_VertexArrayBufferId);
            GlBindings.BufferData(BufferTarget.GL_ARRAY_BUFFER, stride * verts.Length, ptrVerticies, hint);

            GlBindings.BindBuffer(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, m_IndexBufferId);
            GlBindings.BufferData(BufferTarget.GL_ELEMENT_ARRAY_BUFFER, indicies.Length * 4, ptrIndicies, hint);   // TODO Support UnsignedInt and UnsignedShort

            handleVerticies.Free();
            handleIndicies.Free();

            // Setup Attributes
            for (int i = 0; i < attributeLengths.Length; i++)
            {
                GlBindings.VertexAttribPointer(i, attributeLengths[i], VertexAttribPointerType.Float, 0, stride, attributeOffsets[i]);
                GlBindings.EnableVertexAttribArray(i);
            }

            m_VertexCount = indicies.Length;

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, 0);
            GlBindings.BindVertexArray(0);
        }

        public unsafe void BindArrayBuffer<T>(T[] verts, int stride, int[] attributeLengths, int[] attributeOffsets, BufferUsageHint hint = BufferUsageHint.GL_STATIC_DRAW) where T: IInterleavedVertex
        {
            GlBindings.GenVertexArrays(1, out m_VaoId);

            GlBindings.GenBuffers(1, out m_VertexArrayBufferId);
            GlBindings.BindVertexArray(m_VaoId);
            
            GCHandle handle = GCHandle.Alloc(verts, GCHandleType.Pinned);
            IntPtr ptrVerticies = handle.AddrOfPinnedObject();

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, m_VertexArrayBufferId);
            GlBindings.BufferData(BufferTarget.GL_ARRAY_BUFFER, stride * verts.Length, ptrVerticies, hint);
            handle.Free();

            // Setup Attributes
            for (int i = 0; i < attributeLengths.Length; i++)
            {
                GlBindings.VertexAttribPointer(i, attributeLengths[i], VertexAttribPointerType.Float, 0, stride, attributeOffsets[i]);
                GlBindings.EnableVertexAttribArray(i);
            }

            m_VertexCount = verts.Length;

            GlBindings.BindBuffer(BufferTarget.GL_ARRAY_BUFFER, 0);
            GlBindings.BindVertexArray(0);
        }
    }
}
