#version 330 core

in vec3 vColor;
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D texture1;

void main()
{
	vec4 texColor = 1 - texture(texture1, TexCoord).rgba;
	 // vec4 texColor = vec4(vec3(1.0 - texture(texture1, TexCoord)), 0.5);

	//if(texColor.a < 0.5)
    //    discard;

	FragColor = texColor;
}
