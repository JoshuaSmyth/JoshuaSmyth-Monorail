#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 aNormal;

out vec3 vColor;
out vec2 TexCoord;
out vec3 nNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
	vColor = vec3(aColor.x,aColor.y,aColor.z);
	TexCoord = aTexCoord;
	nNormal = aNormal; //mat3(transpose(inverse(model))) * aNormal;
	

	gl_Position = proj * view * model * vec4(aPos, 1.0);
}
