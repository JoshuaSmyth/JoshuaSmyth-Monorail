#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;

out vec3 vColor;
out vec2 TexCoord;
uniform mat4 mvp;

void main()
{
	// Operates in NDC Space
	// -1 = Near Clipping Plane 1 = Far Clipping Plane
	vColor = vec3(aColor.x,aColor.y,aColor.z);
	TexCoord = aTexCoord;
	vec4 pos = vec4(aPos.x, aPos.y, 0.99, 1.0f);
	gl_Position = pos;
}
