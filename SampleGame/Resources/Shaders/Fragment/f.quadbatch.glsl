﻿#version 330 core

in vec3 vColor;
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	FragColor = mix(texture(texture1, TexCoord) * vec4(vColor, 1.0), texture(texture2, TexCoord), 0.2);
}
