#version 330 core

in vec4 vColor;
in vec2 TexCoord;
in vec3 nNormal;
in vec3 vPos;

out vec4 FragColor;

vec3 vUp = vec3(0.0, 1.0, 0.0);
uniform vec3 lightdir = vec3(1.0f, 1.0f, 1.0f);

void main()
{
	FragColor =  vec4(0.0f, 0.8f, 1.0f, 0.5f);
}
