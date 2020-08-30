#version 330 core

in vec4 vColor;
in vec2 TexCoord;
in vec3 nNormal;
in vec3 vPos;

out vec4 FragColor;

uniform vec3 lightdir;
uniform vec3 viewpos;

// Goals for
// Base Material/Shader

// [+] Blinn-Phong
// [+] Half-Lambert
// [-] HDR / Tonemapping
// [-] Bloom
// [-] SSAO

void main()
{
	float specularStrength = 0.5f;
	float shineyness = 32;

	// TODO Work out specular but prob just for snow?

	vec3 specLight = lightdir;
	vec3 specDir = normalize(specLight);
	vec3 specNorm = normalize(nNormal);

	// TODO Work out specular but prob just for snow?


	// Blinn-Phong

	vec3 viewDir    = normalize(viewpos - vPos);
	vec3 halfwayDir = normalize(lightdir + viewDir);


	float spec = pow(max(dot(viewDir, halfwayDir), 0.0), shineyness);
	vec3 specular = specularStrength * spec * vec3(1.0,1.0,1.0);  

	vec3 dir = normalize(lightdir);
	vec3 norm = normalize(nNormal);


		// Half Lambert
	float lightIntensity = 1.0f;
	float dp = lightIntensity*dot(norm, dir) * 0.75 + 0.25f;

	//FragColor =  vec4(dp*specular, 1.0f);

	// The 4th component is the specular value
    vec4 colorA = vec4(1.0f, 1.0f, 1.0f, 1.0f);						// Snow
	vec4 colorB = vec4(0.5,0.4,0.5, 0.1f);							// Rock
	vec4 colorC = vec4(46/255.0f, 180/255.0f, 47/255.0f, 0.0f);		// Grass
	vec4 colorD = vec4(0.89, 0.84, 0.57, 0.5f);						// Sand
	

	// Shadow Colors
	vec4 ambientA = vec4(0.0f, 0.25f, 0.5f, 1.0f);
	vec4 ambientB = vec4(0.2, 0.15, 0.3, 0.1f) * 0.35f;
	vec4 ambientC = vec4(0.3, 0.3, 0.25, 0.0f) * 0.35f;
	vec4 ambientD = vec4(0.0, 0.3, 0.1, 0.5f) * 0.35f;


	// Shadow Mix
	vec4 mixedA = mix(ambientA, colorA, dp);
	vec4 mixedB = mix(ambientB, colorB, dp);
	vec4 mixedC = mix(ambientC, colorC, dp);
	vec4 mixedD = mix(ambientD, colorD, dp);
	
	
	// Snow-Rock
	float dpMix = (vPos.y-16)/32.0f;
	vec4 mixed = mix(mixedB, mixedA, dpMix);
	
	// Rock-Grass
	float dpMix2 = clamp((vPos.y-12)/24.0f, 0, 1);
	mixed = mix(mixedC, mixed, dpMix2);
	
	// Grass-Sand
	float dpMix3 = clamp((vPos.y-12)/6.0f,0,1);
	mixed = mix(mixedD, mixed, dpMix3);
	

	FragColor =  vec4(1.0*(mixed.rgb + specular*mixed.a), 1.0f);

}
