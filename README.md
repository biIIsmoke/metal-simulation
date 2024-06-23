# metal-simulation

My project topic is simulating mechanical properties of metals such as plasticity, elasticity and hardness. 

I started by testing geometry shaders in Unity. I wrote a shader that created pyramid faces on triangles following a tutorial.

[![Watch the video](https://img.youtube.com/vi/7C-mA08mp8o/0.jpg)](https://www.youtube.com/watch?v=7C-mA08mp8o)

End result was great but i wanted to get back calculated vertex data from the GPU. During my research, i found out that Compute Shaders was the best fit for it.

First, i created a position, color struct, put them in an array then in the GPU, i calculated the color of each position using their x position as an offset.

https://github.com/biIIsmoke/metal-simulation/assets/51231331/9f68a9d6-72ef-423d-8b9b-fe25600272c0

After that i tried changing their positions at each call.

https://github.com/biIIsmoke/metal-simulation/assets/51231331/5b1b5672-355c-434a-8531-3a4e4d37d3d7

Now that i had figured out the syntax, time to continue. During my research, i stumbled upon this video and paper:

[![Watch the video](https://img.youtube.com/vi/m7js12tGFVA/0.jpg)](https://www.youtube.com/watch?v=m7js12tGFVA)

It was a great video and i had the idea to shoot balls onto the mesh to test it. So, i created a script to shoot balls using mouse position. At first ball impact test, i had some issues.

https://github.com/biIIsmoke/metal-simulation/assets/51231331/c730f0a7-acf1-47ad-a876-284e4a541dd6

After some time testing and bug fixing, i found out that i was doing the calculation in reverse and not clamping the result. I created a working CPU version of it using a tutorial video.

[![Watch the video](https://img.youtube.com/vi/-dsRIyzAcqg/0.jpg)](https://www.youtube.com/watch?v=-dsRIyzAcqg)

https://github.com/biIIsmoke/metal-simulation/assets/51231331/1e572f22-a1aa-4bb1-88e8-b43476a0e5f2

After that, i created the algorithm in my compute shader and it worked like a charm.

https://github.com/biIIsmoke/metal-simulation/assets/51231331/7124c980-ecf2-49ed-9efc-aa8766440295

But having a mesh collider for collision detection had complicated things because after changing the vertices, you have to change the mesh reference in the mesh collider but if you do individual changes, Unity won't recognize the change and the mesh collider won't reload.

That's why i wanted to change my approach on the matter and started working on a separate grabbing feature that allows user to place a sphere on a mesh then use it to play with the mesh. So, i created a tool that is controlled by mouse screen position and scroll wheel for the z displacement.

https://github.com/biIIsmoke/metal-simulation/assets/51231331/aef63ccb-1dd3-4e28-901a-da66355140ae

After that, i created a new script called grabbable then connected it with a new compute shader called GrabShader. My aim is to use the sphere as a grabbing tool to pull or push vertices to modify the mesh. First test:

https://github.com/biIIsmoke/metal-simulation/assets/51231331/9a488de9-b2d4-42c1-8431-dbeafe40f5ca

I'm doing some of the calculations wrong because as opposed to following the sphere at hand, all of the vertices transform to a single spherical volume. If i reverse it, they originate their direction vector from it then go outwards.




