﻿#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec4 aColor;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 aNormal;

out vec4 vColor;
out vec2 TexCoord;
out vec3 nNormal;
out vec3 vPos;


uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
	vColor = aColor.rgba;
	TexCoord = aTexCoord.xy;

	gl_Position = proj * view * model * vec4(aPos, 1.0);

	vPos = vec3(model * vec4(aPos, 1.0));

	// Todo this should be cpu side and passed as uniform
	mat3 newNorm = mat3(model);
	nNormal = newNorm * aNormal;
	
}
