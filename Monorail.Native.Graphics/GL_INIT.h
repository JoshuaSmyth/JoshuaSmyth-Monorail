#pragma once
#ifndef GL_INIT_H
#define GL_INIT_H

#include <iostream>

#include "SDL.h"
#include "glew.h"

/*
Call to create a window, initialize a GL context and initialize the GLEW library
Necessary to start drawing to a window
*/
void init(SDL_Window*& window);

/*
Call before creating a GL context to set attributes
*/
void initGLAttributes(int glMajorVersion, int glMinorVersion, int useDoubleBuffering);

#endif