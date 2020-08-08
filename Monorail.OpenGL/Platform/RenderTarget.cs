using System;
using OpenGL;

namespace Monorail.Platform
{
    public class RenderTarget
    {
        private int m_RanderTargetId;
        public int RenderTargetId { get { return m_RanderTargetId; } }

        private uint m_TextureColorBuffer;
        public uint TextureColorBufferId { get { return m_TextureColorBuffer; } }

        private uint m_RenderBufferObjectId;
        public uint RenderBufferObjectId {  get { return m_RenderBufferObjectId; } }

        // TODO Will probably need to handle resizing the framebuffer

            // TODO Make internal and store
        public static RenderTarget Create(int width, int height)
        {
            var rv = new RenderTarget();

            // Create the frame buffer
            GlBindings.GenFramebuffers(1, out rv.m_RanderTargetId);
            GlBindings.BindFrameBuffer(OpenGL.FrameBuffer.GL_FRAMEBUFFER, rv.m_RanderTargetId);

            // Attach the texture component
            GlBindings.GenTextures(1, out rv.m_TextureColorBuffer);
            GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, rv.m_TextureColorBuffer);
            GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, OpenGL.PixelInternalFormat.Rgb, width, height, 0, OpenGL.PixelFormat.Rgb, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);

            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

            GlBindings.glFramebufferTexture2D(FrameBuffer.GL_FRAMEBUFFER, Attachment.GL_COLOR_ATTACHMENT0, TextureType.GL_TEXTURE_2D, rv.m_TextureColorBuffer, 0);


            // TODO Depth buffer!

            GlBindings.glGenRenderbuffers(1, out rv.m_RenderBufferObjectId);
            GlBindings.glBindRenderbuffer(RenderBuffer.GL_RENDERBUFFER, rv.m_RenderBufferObjectId);
            GlBindings.glRenderbufferStorage(RenderBuffer.GL_RENDERBUFFER, InternalFormat.GL_DEPTH24_STENCIL8, width, height);
            GlBindings.glFramebufferRenderbuffer(RenderBuffer.GL_FRAMEBUFFER, Attachment.GL_DEPTH_STENCIL_ATTACHMENT, RenderBuffer.GL_RENDERBUFFER, rv.RenderBufferObjectId);

            if (GlBindings.glCheckFramebufferStatus(FrameBuffer.GL_FRAMEBUFFER) != (int) ReturnValue.GL_FRAMEBUFFER_COMPLETE)
            {
                throw new Exception("Error with framebuffer");
            }


            // Store previous
            GlBindings.BindFrameBuffer(OpenGL.FrameBuffer.GL_FRAMEBUFFER, 0);
            return rv;
        }
    }
}
