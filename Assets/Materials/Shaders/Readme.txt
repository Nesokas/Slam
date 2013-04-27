Dynamic forcefield for UNITY3D 
------------------------------
Version 1.0

--

* Introduction:

1. Dynamic forcefield is based on a collider hit point and shader iteraction. The demo scene has a simple forcefield script attached to
a gameobject. The script is passing a hitpoint coordinate and interpolation value to a shader as a vector4 component. The script has a decay parameter 
that is designed to fade out the field over time.

2. The forcefield script is using OnMouseHit method to cast a ray from camera against a collider and if successful will pass hit coordinates to UpdateMask method. 

3. UpdateMask method is responsible for passing hit point to each material instance of a mesh where all the magic is happening.

4. FadeMask method is called each Update to fade out the field.

--

* Setup:

1. Create new material and select Forcefield/Forcefield shader.
   Assign 'Sampler_A' and 'Sampler_B' textures. You can pick sample textures from the package.
   'Mask Power' range is responsible for "sparkles" just before the field is faded out.
   'Mesh offset' is designed to scale the mesh along the normals.
   'Position' is used by script and should not be modified by hand.
   
2. Crate new gameobject that will contain the actual model you will use in game. Assign your mesh and materials as usual.

3. Duplicate the gameobject created in step 2 and attach it to a previously created object. This gameobject will represent the forcefield itself.   
   Add mesh collider and change its material to forcefield material created in step 1.
   
4. Add 'Forcefield' script to a forcefield gameobject. Drag forcefield gameobject to 'Force Field' slot in the forcefield script.

5. Hit play and click the model in the game viewport. 

--

You are free to modify downloaded package and its source code to suit your needs.
Feel free to contact me via email: spectrumk7@gmail.com or SKYPE: dandare84




