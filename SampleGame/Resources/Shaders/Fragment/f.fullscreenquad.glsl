#version 330 core

in vec3 vColor;
in vec2 TexCoord;
out vec4 FragColor;


uniform sampler2D texture1;

void main()
{

	// todo multiple render buffer targets
	// todo hdr / tone mapping

	// check whether fragment output is higher than threshold, if so output as brightness color
    
	vec4 texColor = texture(texture1, TexCoord).rgba;
	vec4 BrightColor;

	
	float brightness = dot(texColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.0)
        BrightColor = vec4(texColor.rgb, 1.0);
    else
        BrightColor = vec4(0.0, 0.0, 0.0, 1.0);


	FragColor = BrightColor;
}
