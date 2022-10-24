# AncientAi
A recreation of the first AI (Perceptron) written in C# 

Basically, Perceptron is an articial inteligence created with the intent of being able to differentiate between two types of images.
In my program's case it differentiates between circles and rectangles by creating two sets of images, one for training and
another one for testing, both sets contain mixed images of circles and rectangles, that within my code are named `Layer`s. For more details
about how training and testing works I recommend seeing the links in the bottom of the readme.

Something interesting about Perceptron is that you can visualize the way it differentiates two types of images with an image,
that's because the weights that the AI applies to each pixel of the image can be converted to a image itself,
to demonstrate how it works, I will show some images I got from running my code.

![weights3](https://user-images.githubusercontent.com/66211581/186183296-621ba135-d782-4701-a370-92a1025095fd.png)

This is an interesting example because you can clearly see where the circles and squares were in the training step,
I got this result by having a very big image widht and height with a relatively small amount of images in each set.
To achieve a close result you can tweak the same values described in the Main function of my application.

Even though the result of the previous image was very good, in most cases the representation of the weights would be:

![weights4](https://user-images.githubusercontent.com/66211581/186185148-891fd5af-2696-4204-8776-b6db78f7c97b.png)

(Sorry for the small image size, but having a lot of big images makes running the code considerably slower)

Which looks a lot more like a very pixelated [perlin noise](https://en.wikipedia.org/wiki/Perlin_noise), you can have fun setting
your own values and running the program to have different results,
the program will create a .ppm file that represents the weights of the AI, if you want to know more about how the ppm file format works
you can check these two links: [how to create a ppm file](http://netpbm.sourceforge.net/doc/ppm.html) and [fileinfo.com](https://fileinfo.com/extension/ppm)

You can change the `imgWidth`, `imgHeight` and `imgAmount` to change the size and amount of images the code will generate.
Have in mind that using bigger values will make the program slow and depending on the values use a lot of ram,
but you can still achieve great results.

If you want to know more about Perceptron and how it works, I recommend:
- its [wikipedia](https://en.wikipedia.org/wiki/Perceptron) page
- and personally this [video](https://youtu.be/GVsUOuSjvcg) from minute 3:58 to 7:00
