#version 330 core

in vec4 vColor;
in vec2 TexCoord;
in vec3 nNormal;

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;
uniform vec3 directionalLight;

void main()
{
// -ve Y is down


    vec3 color = vec3(1.0f, 1.0f, 1.0f);
    vec3 dir = normalize(vec3(1.0f,-1.0f,1.0f));
	vec3 norm = normalize(nNormal.rgb);
	float diff = -1* dot(norm, dir);

	vec3 ambient = vec3(0.0f, 0.25f, 0.5f);

	vec3 mixed = mix(ambient, color, diff);
	FragColor =  vec4(mixed, 1.0f);
}
