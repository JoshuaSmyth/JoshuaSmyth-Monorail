#version 330 core

in vec3 vColor;
in vec2 TexCoord;
in vec3 nNormal;

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	FragColor =  vec4(nNormal, 1.0f);
}
