/*
 * 
 * A lot of the code in these bindings we taken from the source code to Monogame
 * as well as looking at FNA/CS
 * 
 * The OpenGL C Headers can be viewed at
 * https://www.khronos.org/registry/OpenGL/api/
 * 
 */
using Monorail;
using Monorail.Graphics;
using Monorail.Mathlib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace OpenGL
{
    // TODO Convert these all to the GL_ naming conventions
    public enum BufferTarget
    {
        GL_ARRAY_BUFFER = 0x8892,
        GL_ELEMENT_ARRAY_BUFFER = 0x8893,
    }

    public enum BlendFunc
    {
        GL_ZERO =                           0,
        GL_ONE =                            1,
        GL_SRC_COLOR =                      0x0300,
        GL_ONE_MINUS_SRC_COLOR =            0x0301,
        GL_SRC_ALPHA =                      0x0302,
        GL_ONE_MINUS_SRC_ALPHA =            0x0303,
        GL_DST_ALPHA =                      0x0304,
        GL_ONE_MINUS_DST_ALPHA =            0x0305,
        GL_DST_COLOR =                      0x0306,
        GL_ONE_MINUS_DST_COLOR =            0x0307,
        GL_SRC_ALPHA_SATURATE =             0x0308
    }

    public enum DepthFunc
    {
        GL_NEVER =                          0x0200,
        GL_LESS =                           0x0201,
        GL_EQUAL =                          0x0202,
        GL_LEQUAL =                         0x0203,
        GL_GREATER =                        0x0204,
        GL_NOTEQUAL =                       0x0205,
        GL_GEQUAL =                         0x0206,
        GL_ALWAYS =                         0x0207
    }

    public enum BufferUsageHint
    {
        GL_STREAM_DRAW = 0x88E0,
        GL_STATIC_DRAW = 0x88E4,
        GL_DYNAMIC_DRAW = 0x88E8
    }

    public enum VertexAttribPointerType
    {
        Float = 0x1406,
        Short = 0x1402,
        UnsignedByte = 0x1401,
        HalfFloat = 0x140B,
    }

    public enum ShaderType
    {
        VertexShader = 0x8B31,
        FragmentShader = 0x8B30,
        // TODO:(Joshua)
        // Geometry or Compute Shader?
    }

    public enum ShaderParameter
    {
        LogLength = 0x8B84,
        CompileStatus = 0x8B81,
        SourceLength = 0x8B88,
    }

    public enum ProgramParameter
    {
        LogLength = 0x8B84,
        LinkStatus = 0x8B82,
        GL_ACTIVE_UNIFORMS = 0x8B86
    }

    public enum Capabilities
    {
        GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872,
        GL_MAX_SAMPLES = 0x8D57,
        GL_NUM_EXTENSIONS = 0x821D,
    }

    public enum Result
    {
        NotOk = 0,
        Ok = 1
    }

    internal enum GLStrings
    {
        GL_VENDOR = 0x1F00,
        GL_RENDERER = 0x1F01,
        GL_VERSION = 0x1F02,
        GL_EXTENSIONS = 0x1F03
    }

    public enum TextureType
    {
        GL_TEXTURE_2D = 0x0DE1,
        GL_TEXTURE_3D = 0x806F,
        GL_TEXTURE_CUBE_MAP = 0x8513,
        GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515
    }

    public enum StencilEnum
    {
        GL_ZERO = 0,
        GL_KEEP = 0x1E00,
        GL_REPLACE = 0x1E01,
        GL_INCR = 0x1E02,
        GL_DECR = 0x1E03,
        GL_INCR_WRAP = 0x8507,
        GL_DECR_WRAP = 0x8508,
        GL_INVERT = 0x150A
    }

    public enum CompareEnum
    {
        GL_NEVER = 0x0200,      // Always Fails
        GL_LESS = 0x0201,       // Passes if ( ref & mask ) < ( stencil & mask ).
        GL_LEQUAL = 0x0203,     // Passes if ( ref & mask ) <= ( stencil & mask ).
        GL_GREATER = 0x0204,    // Passes if ( ref & mask ) > ( stencil & mask ).
        GL_GEQUAL = 0x0206,     // Passes if ( ref & mask ) >= ( stencil & mask ).
        GL_EQUAL = 0x0202,      // Passes if ( ref & mask ) = ( stencil & mask ).
        GL_NOTEQUAL = 0x0205,   // Passes if ( ref & mask ) != ( stencil & mask ).
        GL_ALWAYS = 0x0207      // Always passes.
    }



    public enum FrameBuffer
    {
        GL_DRAW_FRAMEBUFFER = 0x8CA9,
        GL_READ_FRAMEBUFFER = 0x8CA8,
        GL_FRAMEBUFFER = 0x8D40
    }

    public enum RenderBuffer
    {
        GL_RENDERBUFFER = 0x8D41,
        GL_FRAMEBUFFER = 0x8D40
    }

    public enum InternalFormat
    {
        GL_DEPTH24_STENCIL8 = 0x88F0
    }

    public enum Attachment
    {
        GL_COLOR_ATTACHMENT0  = 0x8CE0,
        GL_DEPTH_ATTACHMENT = 0x8D00,
        GL_STENCIL_ATTACHMENT = 0x8D20,
        GL_DEPTH_STENCIL_ATTACHMENT = 0x821A
    }

    internal enum PixelInternalFormat
    {

        Rgba = 0x1908,
        Rgb = 0x1907,
        Luminance = 0x1909,
        CompressedTextureFormats = 0x86A3,
        Red = 0x1903,
        Rg = 0x8227,
    }

    internal enum PixelFormat
    {
        Rgba = 0x1908,
        Rgb = 0x1907,
        Luminance = 0x1909,
        CompressedTextureFormats = 0x86A3,
        Red = 0x1903,
        Rg = 0x8227,
    }

    internal enum PixelType
    {
        UnsignedByte = 0x1401,
        UnsignedShort565 = 0x8363,
        UnsignedShort4444 = 0x8033,
        UnsignedShort5551 = 0x8034,
        Float = 0x1406,
        HalfFloat = 0x140B,
        Byte = 0x1400,
        UnsignedShort = 0x1403,
        UnsignedInt1010102 = 0x8036,
    }

    public enum TextureAttribute
    {
        GL_TEXTURE_WRAP_S = 0x2802,
        GL_TEXTURE_WRAP_T = 0x2803,
        GL_TEXTURE_WRAP_R = 0x8072,
        GL_TEXTURE_MAG_FILTER = 0x2800,
        GL_TEXTURE_MIN_FILTER = 0x2801,
        GL_TEXTURE_MAX_ANISOTROPY_EXT = 0x84FE,
        GL_TEXTURE_BASE_LEVEL = 0x813C,
        GL_TEXTURE_MAX_LEVEL = 0x813D,
        GL_TEXTURE_LOD_BIAS = 0x8501,
        GL_UNPACK_ALIGNMENT = 0x0CF5,
    }

    public enum Mode
    {
        GL_POINT = 0x1B00,
        GL_LINE = 0x1B01,
        GL_FILL = 0x1B02
    }

    public enum Face
    {
        GL_FRONT = 0x0404,
        GL_BACK = 0x0405,
        GL_FRONT_AND_BACK = 0x0408
    }

    public enum TextureAttributeValue
    {
        GL_REPEAT = 0x2901,
        GL_CLAMP_TO_EDGE = 0x812F,
        GL_MIRRORED_REPEAT = 0x8370,

        GL_NEAREST = 0x2600,
        GL_LINEAR = 0x2601,
        GL_NEAREST_MIPMAP_NEAREST = 0x2700,
        GL_NEAREST_MIPMAP_LINEAR = 0x2702,
        GL_LINEAR_MIPMAP_NEAREST = 0x2701,
        GL_LINEAR_MIPMAP_LINEAR = 0x2703,
    }

    public enum TextureUnits
    {
        GL_TEXTURE0 = 0x84C0,
        GL_TEXTURE1 = 0x84C1,
        GL_TEXTURE2 = 0x84C2,
        GL_TEXTURE3 = 0x84C3,
        GL_TEXTURE4 = 0x84C4,
        GL_TEXTURE5 = 0x84C5,
        GL_TEXTURE6 = 0x84C6,
        GL_TEXTURE7 = 0x84C7,
        GL_TEXTURE8 = 0x84C8,
        GL_TEXTURE9 = 0x84C9,
        GL_TEXTURE10 = 0x84CA,
        GL_TEXTURE11 = 0x84CB,
        GL_TEXTURE12 = 0x84CC,
        GL_TEXTURE13 = 0x84CD,
        GL_TEXTURE14 = 0x84CE,
        GL_TEXTURE15 = 0x84CF
    }

    public enum Enable
    {
        GL_SCISSOR_TEST = 0x0C11,
        GL_DEPTH_TEST = 0x0B71,
        GL_STENCIL_TEST = 0x0B90,
        GL_MULTISAMPLE = 0x809D,
        GL_BLEND = 0x0BE2
    }

    public enum ReturnValue
    {
        GL_FRAMEBUFFER_COMPLETE = 0x8CD5
    }

    internal class EntryPointHelper
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_GetProcAddress", ExactSpelling = true)]
        internal static extern IntPtr GetProcAddress(IntPtr proc);
        internal static IntPtr GetAddress(string proc)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(proc);
            try
            {
                var addr = GetProcAddress(p);
                if (addr == IntPtr.Zero)
                    throw new EntryPointNotFoundException (proc);
                return addr;
            }
            finally
            {
                Marshal.FreeHGlobal(p);
            }
        }
    }

    public static class GlBindings
    {
        [SuppressUnmanagedCodeSecurity()]
        public delegate void ClearDelegate(ClearBufferMask mask);
        [BindMethod("glClear")]
        public static ClearDelegate Clear;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void ClearColorDelegate(float red, float green, float blue, float alpha);
        [BindMethod("glClearColor")]
        public static ClearColorDelegate ClearColor;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int CreateShaderDelegate(ShaderType type);
        [BindMethod("glCreateShader")]
        public static CreateShaderDelegate CreateShader;

        [SuppressUnmanagedCodeSecurity()]
        internal unsafe delegate void ShaderSourceDelegate(int shaderId, int count, IntPtr code, int* length);
        [BindMethod("glShaderSource")]
        internal static ShaderSourceDelegate ShaderSourceInternal;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void CompileShaderDelegate(int shaderId);
        [BindMethod("glCompileShader")]
        public static CompileShaderDelegate CompileShader;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void PolygonShaderDelegate(Face face, Mode mode);
        [BindMethod("glPolygonMode")]
        public static PolygonShaderDelegate PolygonMode;

        [SuppressUnmanagedCodeSecurity()]
        internal unsafe delegate void GetShaderDelegate(int shaderId, int parameter, int* value);
        [BindMethod("glGetShaderiv")]
        internal static GetShaderDelegate GetShaderiv;

        [SuppressUnmanagedCodeSecurity()]
        internal unsafe delegate void GetShaderInfoLogDelegate(int shader, int bufSize, IntPtr length, byte* infoLog);
        [BindMethod("glGetShaderInfoLog")]
        internal static GetShaderInfoLogDelegate GetShaderInfoLogInternal;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int CreateProgramDelegate();
        [BindMethod("glCreateProgram")]
        public static CreateProgramDelegate CreateProgram;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void AttachShaderDelegate(int programId, int shaderId);
        [BindMethod("glAttachShader")]
        public static AttachShaderDelegate AttachShader;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void LinkProgramDelegate(int programId);
        [BindMethod("glLinkProgram")]
        public static LinkProgramDelegate LinkProgram;

        [SuppressUnmanagedCodeSecurity()]
        internal unsafe delegate void GetProgramDelegate(int programId, int name, int* linked);
        [BindMethod("glGetProgramiv")]
        internal static GetProgramDelegate GetProgramiv;

        [SuppressUnmanagedCodeSecurity()]
        internal unsafe delegate void GetProgramInfoLogDelegate(int program, int bufSize, IntPtr length, byte* infoLog);
        [BindMethod("glGetProgramInfoLog")]
        internal static GetProgramInfoLogDelegate GetProgramInfoLogInternal;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void DeleteShaderDelegate(int shaderId);
        [BindMethod("glDeleteShader")]
        public static DeleteShaderDelegate DeleteShader;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void GenBuffersDelegate(int count, out int buffer);
        [BindMethod("glGenBuffers")]
        public static GenBuffersDelegate GenBuffers;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void GenerateMipmapDelegate(TextureType textureType);
        [BindMethod("glGenerateMipmap")]
        public static GenerateMipmapDelegate GenerateMipmap;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void GenTexturesDelegate(int count, out uint buffer);
        [BindMethod("glGenTextures")]
        public static GenTexturesDelegate GenTextures;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void GenFramebuffersDelegate(int count, out int buffer);
        [BindMethod("glGenFramebuffers")]
        public static GenFramebuffersDelegate GenFramebuffers;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void GenVertexArraysDelegate(int count, out int buffer);
        [BindMethod("glGenVertexArrays")]
        public static GenVertexArraysDelegate GenVertexArrays;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void BindVertexArrayDelegate(int count);
        [BindMethod("glBindVertexArray")]
        public static BindVertexArrayDelegate BindVertexArray;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void BindBufferDelegate(BufferTarget target, int buffer);
        [BindMethod("glBindBuffer")]
        public static BindBufferDelegate BindBuffer;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void BindTextureDelegate(TextureType target, uint textureId);
        [BindMethod("glBindTexture")]
        public static BindTextureDelegate BindTexture;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void BindFramebufferDelegate(FrameBuffer target, int framebufferId);
        [BindMethod("glBindFramebuffer")]
        public static BindFramebufferDelegate BindFrameBuffer;

        [SuppressUnmanagedCodeSecurity()]
        public unsafe delegate void BufferDataDelegate(BufferTarget target, int size, IntPtr data, BufferUsageHint usage);
        [BindMethod("glBufferData")]
        public static BufferDataDelegate BufferData;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void VertexAttribPointerDelegate(int location, int elementCount, VertexAttribPointerType type, int normalize, int stride, int offset);
        [BindMethod("glVertexAttribPointer")]
        public static VertexAttribPointerDelegate VertexAttribPointer;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void EnableVertexAttribArrayDelegate(int attrib);
        [BindMethod("glEnableVertexAttribArray")]
        public static EnableVertexAttribArrayDelegate EnableVertexAttribArray;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void UseProgramDelegate(int program);
        [BindMethod("glUseProgram")] public static UseProgramDelegate UseProgram;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void DrawArraysDelegate(PrimitiveType primitiveType, int offset, int vertexCount);
        [BindMethod("glDrawArrays")]
        public static DrawArraysDelegate DrawArrays;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void GetIntegervDelegate(Capabilities capability, out int value);
        [BindMethod("glGetIntegerv")]
        public static GetIntegervDelegate GetIntegerv;

        [SuppressUnmanagedCodeSecurity()]
        internal delegate IntPtr GetStringDelegate(GLStrings param);
        [BindMethod("glGetString")]
        internal static GetStringDelegate GetStringInternal;

        [SuppressUnmanagedCodeSecurity()]
        internal delegate IntPtr GetStringiDelegate(GLStrings param, uint index);
        [BindMethod("glGetStringi")]
        internal static GetStringiDelegate GetStringiInternal;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void DrawElementsDelegate(PrimitiveType primitiveType, int count, DrawElementsType elementType, int offset);
        [BindMethod("glDrawElements")]
        public static DrawElementsDelegate DrawElements;

        [SuppressUnmanagedCodeSecurity()]
        internal delegate void TexImage2DDelegate(TextureType target, int level, PixelInternalFormat internalFormat, int width, int height, int border, PixelFormat format, PixelType pixelType, IntPtr data);
        [BindMethod("glTexImage2D")]
        internal static TexImage2DDelegate TexImage2D;

        [SuppressUnmanagedCodeSecurity()]
        internal delegate void TexParameteriDelegate(TextureType target, TextureAttribute targetAttribute, TextureAttributeValue wrapMode);
        [BindMethod("glTexParameteri")]
        internal static TexParameteriDelegate TexParameteri;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void ActiveTextureDelegate(TextureUnits binding);
        [BindMethod("glActiveTexture")]
        public static ActiveTextureDelegate ActiveTexture;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void Uniform1iDelegate(int location, int value);
        [BindMethod("glUniform1i")]
        public static Uniform1iDelegate Uniform1i;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void Uniform3fDelegate(int location, float v1, float v2, float v3);
        [BindMethod("glUniform3f")]
        public static Uniform3fDelegate Uniform3f;

        [SuppressUnmanagedCodeSecurity()]
        internal delegate void glUniformMatrix4fvDelegate(int location, int count, int transpose, IntPtr data);
        [BindMethod("glUniformMatrix4fv")]
        internal static glUniformMatrix4fvDelegate UniformMatrix4fvInternal;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int GetUniformLocationDelegate(int programId, string name);
        [BindMethod("glGetUniformLocation")]
        public static GetUniformLocationDelegate GetUniformLocation;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int EnableDelegate(Enable enableFlags);
        [BindMethod("glEnable")]
        public static EnableDelegate Enable;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int DisableDelegate(Enable enableFlags);
        [BindMethod("glDisable")]
        public static DisableDelegate Disable;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glBufferSubDataDelegate(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
        [BindMethod("glBufferSubData")]
        public static glBufferSubDataDelegate BufferSubData;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glDepthFuncDelegate(DepthFunc depthFunc);
        [BindMethod("glDepthFunc")]
        public static glDepthFuncDelegate DepthFunc;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glBlendFuncDelegate(BlendFunc srcBlend, BlendFunc dstBlend);
        [BindMethod("glBlendFunc")]
        public static glBlendFuncDelegate BlendFunc;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glScissorFuncDelegate(int x​, int y​, uint width​, uint height​);
        [BindMethod("glScissor")]
        public static glScissorFuncDelegate glScissor;


        [SuppressUnmanagedCodeSecurity()]
        public delegate int glViewportFuncDelegate(int x​, int y​, uint width​, uint height​);
        [BindMethod("glViewport")]
        public static glViewportFuncDelegate glViewport;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glFramebufferTexture2DFuncDelegate(FrameBuffer target, Attachment attachment, TextureType textureTarget, uint textureId, int mipmap);
        [BindMethod("glFramebufferTexture2D")]
        public static glFramebufferTexture2DFuncDelegate glFramebufferTexture2D;


        [SuppressUnmanagedCodeSecurity()]
        public delegate int glGenRenderbuffersFuncDelegate(int size,out uint renderbuffers);
        [BindMethod("glGenRenderbuffers")]
        public static glGenRenderbuffersFuncDelegate glGenRenderbuffers;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glBindRenderbufferFuncDelegate(RenderBuffer target, uint renderbufferId);
        [BindMethod("glBindRenderbuffer")]
        public static glBindRenderbufferFuncDelegate glBindRenderbuffer;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void glRenderbufferStorageFuncDelegate(RenderBuffer target, InternalFormat internationalformat, int width, int height);
        [BindMethod("glRenderbufferStorage")]
        public static glRenderbufferStorageFuncDelegate glRenderbufferStorage;

        [SuppressUnmanagedCodeSecurity()]
        public delegate void glFramebufferRenderbufferFuncDelegate(RenderBuffer target, Attachment attachment, RenderBuffer renderbuffertarget, uint renderbuffer);
        [BindMethod("glFramebufferRenderbuffer")]
        public static glFramebufferRenderbufferFuncDelegate glFramebufferRenderbuffer;

        [SuppressUnmanagedCodeSecurity()]
        public delegate int glCheckFramebufferStatusFuncDelegate(FrameBuffer framebufferType);
        [BindMethod("glCheckFramebufferStatus")]
        public static glCheckFramebufferStatusFuncDelegate glCheckFramebufferStatus;


        [SuppressUnmanagedCodeSecurity()]
        public delegate void glStencilMaskFuncDelegate(uint value);
        [BindMethod("glStencilMask")]
        public static glStencilMaskFuncDelegate glStencilMask;


        [SuppressUnmanagedCodeSecurity()]
        public delegate void glStencilOpFuncDelegate(StencilEnum sfail, StencilEnum dpfail, StencilEnum dppass);
        [BindMethod("glStencilOp")]
        public static glStencilOpFuncDelegate glStencilOp;


        [SuppressUnmanagedCodeSecurity()]
        public delegate void glStencilFuncDelegate(CompareEnum func, int reference, uint mask);
        [BindMethod("glStencilFunc")]
        public static glStencilFuncDelegate glStencilFunc;


        [SuppressUnmanagedCodeSecurity()]
        public delegate void glClearStencilFuncDelegate(int value);
        [BindMethod("glClearStencil")]
        public static glClearStencilFuncDelegate glClearStencil;


        [SuppressUnmanagedCodeSecurity()]
        public delegate void glColorMaskFuncDelegate(int r, int g, int b, int a);
        [BindMethod("glColorMask")]
        public static glColorMaskFuncDelegate glColorMask;


        public static void InitaliseOpenGLEntryPoints()
        {
            using (TracedStopwatch.Start("Init OpenGL Bindings"))
            {
                // Load EntryPoints via refelction
                var fields = typeof(GlBindings).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.IsDefined(typeof(BindMethod), false))
                    {
                        var attribute = fieldInfo.GetCustomAttributes(typeof(BindMethod), false)[0] as BindMethod;
                        var delegateBinding = LoadEntryPoint(attribute.MethodName, fieldInfo.FieldType, true);
                        fieldInfo.SetValue(null, delegateBinding);
                    }
                }
            }
        }

        unsafe static IntPtr MarshalStringToPtr(string str)
        {
            if (string.IsNullOrEmpty(str)) {
                return IntPtr.Zero;
            }
            int num = Encoding.ASCII.GetMaxByteCount(str.Length) + 1;
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            if (intPtr == IntPtr.Zero) {
                throw new OutOfMemoryException();
            }
            fixed (char* chars = str + RuntimeHelpers.OffsetToStringData / 2) {
                int bytes = Encoding.ASCII.GetBytes(chars, str.Length, (byte*)((void*)intPtr), num);
                Marshal.WriteByte(intPtr, bytes, 0);
                return intPtr;
            }
        }

        static IntPtr MarshalStringArrayToPtr(string[] strings)
        {
            IntPtr intPtr = IntPtr.Zero;
            if (strings != null && strings.Length != 0) {
                intPtr = Marshal.AllocHGlobal(strings.Length * IntPtr.Size);
                if (intPtr == IntPtr.Zero) {
                    throw new OutOfMemoryException();
                }
                int i = 0;
                try {
                    for (i = 0; i < strings.Length; i++) {
                        IntPtr val = MarshalStringToPtr(strings[i]);
                        Marshal.WriteIntPtr(intPtr, i * IntPtr.Size, val);
                    }
                }
                catch (OutOfMemoryException) {
                    for (i--; i >= 0; i--) {
                        Marshal.FreeHGlobal(Marshal.ReadIntPtr(intPtr, i * IntPtr.Size));
                    }
                    Marshal.FreeHGlobal(intPtr);
                    throw;
                }
            }
            return intPtr;
        }

        public unsafe static void ShaderSource(int shaderId, string code)
        {
            int length = code.Length;
            IntPtr intPtr = MarshalStringArrayToPtr(new string[] { code });
            ShaderSourceInternal(shaderId, 1, intPtr, &length);
            FreeStringArrayPtr(intPtr, 1);
        }

        internal static System.Delegate LoadEntryPoint(string proc, Type delegateType, bool throwIfNotFound = false)
        {
            try
            {
                var addr = EntryPointHelper.GetAddress(proc);
                if (addr == IntPtr.Zero)
                    return null;
                return Marshal.GetDelegateForFunctionPointer(addr, delegateType);
            }
            catch (EntryPointNotFoundException)
            {
                if (throwIfNotFound)
                    throw;
                return null;
            }
        }

        internal static System.Delegate LoadEntryPoint<T>(string proc, bool throwIfNotFound = false)
        {
            try
            {
                var addr = EntryPointHelper.GetAddress(proc);
                if (addr == IntPtr.Zero)
                    return null;
                return Marshal.GetDelegateForFunctionPointer(addr, typeof(T));
            }
            catch (EntryPointNotFoundException)
            {
                if (throwIfNotFound)
                    throw;
                return null;
            }
        }

        static void FreeStringArrayPtr(IntPtr ptr, int length)
        {
            for (int i = 0; i < length; i++) {
                Marshal.FreeHGlobal(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
            }
            Marshal.FreeHGlobal(ptr);
        }


        public unsafe static void GetProgramInfo(int programId, ProgramParameter name, out int result)
        {
            fixed (int* ptr = &result)
            {
                GetProgramiv(programId, (int)name, ptr);
            }
        }

        public unsafe static void GetShaderInfo(int shaderId, ShaderParameter name, out int result)
        {
            fixed (int* ptr = &result)
            {
                GetShaderiv(shaderId, (int)name, ptr);
            }
        }

        public static unsafe string GetShaderInfoLog(int shaderId) {
            int length = 0;
            GetShaderInfo(shaderId, ShaderParameter.LogLength, out length);
            byte[] message = new byte[length];
            if (length == 0)
            {
                return "Unknown Error";
            }
            fixed (byte* ptr = &message[0]) {
                GetShaderInfoLogInternal(shaderId, length, IntPtr.Zero, ptr);
            }

            return Encoding.ASCII.GetString(message);
        }

        public static unsafe string GetActiveUniformInfo(int programId, int uniformId) 
        {
            return "TODO";
            //glGetActiveUniform(program, (GLuint)i, bufSize, &length, &size, &type, name);

        }

    public static unsafe string GetProgramInfoLog (int programId)
        {
            int length = 0;
            GetProgramInfo(programId, ProgramParameter.LogLength, out length);
            byte[] message = new byte[length];
            if (length == 0)
            {
                return "Unknown Error";
            }
            fixed(byte* ptr = &message[0]) {
                GetProgramInfoLogInternal (programId, length, IntPtr.Zero, ptr);
            }
            
            return Encoding.ASCII.GetString(message);
        }


        internal unsafe static string GetString(GLStrings name)
        {
            return Marshal.PtrToStringAnsi(GetStringInternal(name));
        }

        internal static string glGetStringi(GLStrings pname, uint index)
        {
            unsafe
            {
                return new string((sbyte*)GetStringiInternal(pname, index));
            }
        }

        public static void UniformMatrix4fv(int location, int count, int transpose, Matrix4 data)
        {
            unsafe
            {
                Matrix4* p = &data;
                {
                    UniformMatrix4fvInternal(location, count, transpose, (IntPtr) p);
                }
            }
        }

    }

    public class BindMethod : Attribute
    {
        private readonly string m_MethodName;

        public BindMethod(string methodName) {
            m_MethodName = methodName;
        }

        public string MethodName {
            get { return m_MethodName; }
        }
    }
}
