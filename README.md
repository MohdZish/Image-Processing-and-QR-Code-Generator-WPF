# Image-Processing-and-QR-Code-Generator-WPF

A really cool project with a lot of great features and interesting algorithms. 
THere's two main parts in this software : Image Processing (filtering) and QR Code generation (all worked using Binaries and no librairies)

Preview of Image Processing :
<img src="Images/imageprocess.gif" width="650">

Preview of QR Code generation :
<img src="Images/githubgen.gif" width="650">

Basically this C# WPF software allows you to do the following :
- Convert any bitmap image to black and white or Grayscale
- Change image brightness
- Edge Detection using Matrix Convolution.
- Blurring image using convolution.
- Create a mirror image.
- Create a Mandelbrot Set Fractal
- Generate a Version 1 QR Code !

All of these were done without using any plugins or libraries, so I had to work using binary of the bitmap image files.
I have commented as much as possible (in french though) so it should be a bit easier to understand.

The QR Code is version 1, meaning it can take upto 25 characters of text. I have also used a L mask for security purposes.
A Reed Solomons algorithm was used, but that part was not written by myself, as it involves a lot of heavy maths and is quite complicated.

Feel free to use any of these codes or even ask me if any questions on my email: mohdzish2000@gmail.com

