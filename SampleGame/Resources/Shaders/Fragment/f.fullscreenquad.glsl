#version 330 core

in vec3 vColor;
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D texture1;

void main()
{
	vec4 texColor = texture(texture1, TexCoord).rgba;

	//if(texColor.a < 0.5)
    //    discard;

	FragColor = texColor;
}
