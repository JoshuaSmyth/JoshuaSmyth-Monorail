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
	// TODO Work out specular but prob just for snow?


    vec3 colorA = vec3(1.0f, 1.0f, 1.0f);						// Snow
	vec3 colorB = vec3(0.5,0.4,0.5);							// Rock
	vec3 colorC = vec3(46/255.0f, 180/255.0f, 47/255.0f);		// Grass
	vec3 colorD = vec3(0.89, 0.84, 0.57);						// Sand
	

	// Shadow Colors
	vec3 ambientA = vec3(0.0f, 0.25f, 0.5f);
	vec3 ambientB = vec3(0.2, 0.15, 0.3) * 0.25f;
	vec3 ambientC = vec3(0.3, 0.3, 0.25) * 0.25f;
	vec3 ambientD = vec3(0.0, 0.3, 0.1) * 0.25f;


    vec3 dir = normalize(lightdir);
	vec3 norm = normalize(nNormal);

	float dp = -1* dot(norm, dir);

	// Shadow Mix
	vec3 mixedA = mix(ambientA, colorA, dp);
	vec3 mixedB = mix(ambientB, colorB, dp);
	vec3 mixedC = mix(ambientC, colorC, dp);
	vec3 mixedD = mix(ambientD, colorD, dp);
	
	
	// Snow-Rock
	float dpMix = (vPos.y-16)/32.0f;
	vec3 mixed = mix(mixedB, mixedA, dpMix);
	
	// Rock-Grass
	float dpMix2 = clamp((vPos.y-12)/24.0f, 0, 1);
	mixed = mix(mixedC, mixed, dpMix2);
	
	// Grass-Sand
	float dpMix3 = clamp((vPos.y-12)/6.0f,0,1);
	mixed = mix(mixedD, mixed, dpMix3);
	

	FragColor =  vec4(mixed, 1.0f);
}
