TODO
// https://learnopengl.com/

 - Create IndexedTriangle
 - Create Textured Triangle
 - Create Textured Quad
 - Create Sprite Batch
 - Create Sprite Font


 - QuadBatcher (For UI)
  Combines Sprite Batch and Sprite Font
   - uses Array Textures 
		https://sites.google.com/site/john87connor/texture-object/tutorial-09-6-array-texture (This one seems liket he most useful)
		https://ferransole.wordpress.com/2014/06/09/array-textures/
		https://www.khronos.org/opengl/wiki/Array_Texture#Creation_and_Management
   - uses a ClipRect (cpu side clipping)
   - Maintain a 'PageTree'


   // Treasure Hunters Renderer Features //

   - Multipass Forward Renderer
		- MSAA
		- Depth Pass
		- Additive per light pass (with sissor test)
		- Instanced model rendering (rather than static batching)
		- Default lighting equation = Half Lambert
		- Graphics Options
			- PostProcessing on/off
			- MSAA 0-8
   - Auto-Portal Visibility Testing
		- Portal Cull
		- Fustrum Cull (for distant objects that should be seen from afar)
   - Post Process (Requires depth buffer)
		- Bloom
		- SSAO (depth only)
	- Soft Particles
		- Alpha		(Sort from back to front)
		- Additive	(no sort required)
	- Lights
		- Directional (Sun)
		- Point (texture baked per level)
		- Torch (indoors only)

	Texturing
		- Usually RGB / RGBA or Alpha mostly non-compressed.

	Shaders
		- Triplanar mapping
		- Foliage
			- Normal remapping
			- Hue ramp?
		- 360 Planar skybox
		- Water (Inc depth edges)
		- Diffuse + Ambient Lighting
		- Additive Light w/Sissor

		+ Custom set piece Shaders

	Post-Effects
		- Bloom
		- SSAO

	Possible Shaders
		- Shadow Mapping (Probably outdoors only)
		- Rim lighting
		- GodRays?
		- Hemisphere rendering (Outdoors only, possibly pre-baked)

	Still Unknown
		- Fog
