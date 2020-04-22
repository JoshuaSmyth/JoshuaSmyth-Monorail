#version 330 core

in vec4 vColor;
in vec2 TexCoord;
in vec3 nNormal;


out vec4 FragColor;


uniform vec3 lightdir = vec3(1.0f, 1.0f, 1.0f);

void main()
{
// -ve Y is down

    vec3 color = vec3(0.9f, 0.9f, 0.9f);
	vec3 ambient = vec3(0.0f, 0.25f, 0.5f);


    vec3 dir = normalize(lightdir);
	vec3 norm = normalize(nNormal);

	float dp = -1* dot(norm, dir);

	
	vec3 mixed = mix(ambient, color, dp);
	FragColor =  vec4(mixed, 1.0f);
}
