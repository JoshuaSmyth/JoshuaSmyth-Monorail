#include "Main.h"
#include <stdio.h>

// Callbacks to C#
typedef void(__stdcall * CallbackInt)(int);
CallbackInt fpCallback;

// Forward declares
int init();
int update();
int render();

// Exports to C#
extern "C"
{
  __declspec(dllexport) int Init(int a)
  {
    printf("%d\n", a);
    return init();
  }

  __declspec(dllexport) int Update()
  {
    return update();
  }

  __declspec(dllexport) int Render()
  {
    return render();
  }

  __declspec(dllexport) void RegisterCallback(CallbackInt callback)
  {
    fpCallback = callback;
  }
}

// Globals
SDL_Window* main_window;
SDL_Event Event;

// TODO Render our stuff!


unsigned int VBO;
unsigned int VAO;
unsigned int EBO;

const char *vertexShaderSource = "#version 330 core\n"
"layout (location = 0) in vec3 aPos;\n"
"void main()\n"
"{\n"
"   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n"
"}\0";

const char *fragmentShaderSource = "#version 330 core\n"
"out vec4 FragColor;\n"
"void main()\n"
"{\n"
"  FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);\n"
"}\0";

unsigned int vertexShader;
unsigned int fragmentShader;

unsigned int shaderProgram;

float vertices[] = {
     0.5f,  0.5f, 0.0f,  // top right
     0.5f, -0.5f, 0.0f,  // bottom right
    -0.5f, -0.5f, 0.0f,  // bottom left
    -0.5f,  0.5f, 0.0f   // top left 
};
unsigned int indices[] = {  // note that we start from 0!
    0, 1, 3,   // first triangle
    1, 2, 3    // second triangle
};

int init()
{
  printf("Hello from C++");
  init(main_window);

  // InitOPEN GL STUFF
  glGenBuffers(1, &VBO);
  glGenBuffers(1, &EBO);

  glBindBuffer(GL_ARRAY_BUFFER, VBO);
  glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

  glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
  glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

  int  success;
  char infoLog[512];

  // Vertex Shader
  {
    vertexShader = glCreateShader(GL_VERTEX_SHADER);

    glShaderSource(vertexShader, 1, &vertexShaderSource, NULL);
    glCompileShader(vertexShader);

    glGetShaderiv(vertexShader, GL_COMPILE_STATUS, &success);
    if (!success)
    {
      glGetShaderInfoLog(vertexShader, 512, NULL, infoLog);
      printf("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n%s\n", infoLog);
    }
  }

  // Fragment Shader
  {

    fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
    glShaderSource(fragmentShader, 1, &fragmentShaderSource, NULL);
    glCompileShader(fragmentShader);
  }

  // Link the Shaders into a program
  {
    shaderProgram = glCreateProgram();
    glAttachShader(shaderProgram, vertexShader);
    glAttachShader(shaderProgram, fragmentShader);
    glLinkProgram(shaderProgram);

    glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
    if (!success) {
      printf("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n%s\n", infoLog);
    }
  }

  glUseProgram(shaderProgram);

  glDeleteShader(vertexShader);
  glDeleteShader(fragmentShader);

  // Link Vertex Attributes
  {
    glGenVertexArrays(1, &VAO);
    glGenBuffers(1, &VBO);

    glBindVertexArray(VAO);
    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glBindVertexArray(0);
  }

  return 0;
}

int update()
{
  while (SDL_PollEvent(&Event) != 0)
  {
    if (fpCallback != NULL)
    {
      fpCallback((int)(Event.type));
    }
    else
    {
      printf("FP Callback is null\n");
    }
  }
  return 0;
}



int render()
{
  //glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

  glClearColor(0, 0, 1, 1);
  glClear(GL_COLOR_BUFFER_BIT);
  

  glUseProgram(shaderProgram);
  glBindVertexArray(VAO); // seeing as we only have a single VAO there's no need to bind it every time, but we'll do so to keep things a bit more organized
  glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
  glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);


  glFlush();
  SDL_GL_SwapWindow(main_window);

  return 0;
}