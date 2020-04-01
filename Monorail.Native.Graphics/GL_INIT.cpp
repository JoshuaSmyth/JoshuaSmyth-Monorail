#include "GL_Init.h"

/*
Call to create a window, initialize a GL context and initialize the GLEW library
Necessary to start drawing to a window
*/
void init(SDL_Window*& window) {
  SDL_Init(SDL_INIT_EVERYTHING);
  initGLAttributes(3, 3, 1);
  window = SDL_CreateWindow("Hello Window!", 20, 40, 600, 600, SDL_WINDOW_OPENGL);
  if (window == NULL) {
    std::cout << "Failed to create window at startup";
  }
  SDL_GL_CreateContext(window);
  //swap buffer at the monitors rate
  SDL_GL_SetSwapInterval(1);

  //GLEW is an OpenGL Loading Library used to reach GL functions
  //Sets all functions available
  glewExperimental = GL_TRUE;
  glewInit();
}

/*
Call before creating a GL context to set attributes
Used by init
*/
void initGLAttributes(int glMajorVersion, int glMinorVersion, int useDoubleBuffering) {
  //Disable deprecated functions
  SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
  //Select version
  SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, glMajorVersion);
  SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, glMinorVersion);
  //Use double buffer
  SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, useDoubleBuffering);
}