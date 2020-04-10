#include "GL_Init.h"
#include <stdio.h>

/*
Call to create a window, initialize a GL context and initialize the GLEW library
Necessary to start drawing to a window
*/
void init(SDL_Window*& window) 
{
  SDL_Init(SDL_INIT_EVERYTHING);

  //Disable deprecated functions
  SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

  SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
  SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
  
  SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);

  window = SDL_CreateWindow("Hello Window!", 20, 40, 640, 400, SDL_WINDOW_OPENGL);
  if (!window)
  {
    printf("Failed to create window at startup");
  }

  SDL_GL_CreateContext(window);
  
  //swap buffer at the monitors rate
  SDL_GL_SetSwapInterval(1);

  //GLEW is an OpenGL Loading Library used to reach GL functions
  //Sets all functions available
  glewExperimental = GL_TRUE;
  glewInit();
}