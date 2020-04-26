#version 330 core

in vec3 vColor;
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	vec3 tex1 = texture(texture1, TexCoord);
	vec3 tex2 = texture(texture2, TexCoord);

	FragColor =  vec4(mix(tex1, tex2, 0.5),1.0f) * vec4(vColor, 1.0);
}
