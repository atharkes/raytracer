#version 440 core
in vec3 aPosition;
in vec2 aTexCoord;
out vec2 texCoord;

void main() {
    gl_Position = vec4(aPosition, 1.0);
    texCoord = aTexCoord;
}