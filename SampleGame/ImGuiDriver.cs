using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGame
{
    class ImGuiDriver
    {
        string fragShader = @"#version 330 core
                               uniform sampler2D FontTexture;

                               in vec4 color;
                               in vec2 texCoord;

                               out vec4 outputColor;

                               void main()
                               {
                                    outputColor = color * texture(FontTexture, texCoord);
                               }";

        string vertShader = @"#version 330 core
                               uniform ProjectionMatrixBuffer
                               {
                                   mat4 projection_matrix;
                               };

                               in vec2 in_position;
                               in vec2 in_texCoord;
                               in vec4 in_color;

                               out vec4 color;
                               out vec2 texCoord;

                               void main()
                               {
                                   gl_Position = projection_matrix * vec4(in_position, 0, 1);
                                   color = in_color;
                                   texCoord = in_texCoord;
                               }";

        public void Initalise()
        {
            // TODO
        }
    }
}
