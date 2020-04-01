#include "Main.h"

extern "C"
{
  __declspec(dllexport) int init()
  {
    main(0, NULL);
    return 0;
  }
}

int main(int argc, char* argv[])
//int SDL_main()
{
  SDL_Window* main_window;
  init(main_window);

  SDL_Event Event;
  while (true) {
    glClearColor(0, 0, 1, 1);
    glClear(GL_COLOR_BUFFER_BIT);
    glFlush();

    while (SDL_PollEvent(&Event)) {}

    SDL_GL_SwapWindow(main_window);
  }

  return 0;
}