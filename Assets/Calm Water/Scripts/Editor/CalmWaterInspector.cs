using System;
using UnityEngine;

namespace UnityEditor
{
	public class CalmWaterInspector: ShaderGUI
	{
		//Color
		MaterialProperty shallowColor = null;
		MaterialProperty depthColor = null;
		MaterialProperty depth = null;
				
		//Spec
		MaterialProperty specColor = null;
		MaterialProperty smoothness = null;

		//Distortion
		MaterialProperty bumpMap = null;
		MaterialProperty bumpStrength = null;

		//Animation
		MaterialProperty speedX = null;
		MaterialProperty speedY = null;

		//Distortion
		MaterialProperty distortion = null;
		MaterialProperty distortionQuality = null;
				
		//Reflection
		MaterialProperty reflectionType = null;
		MaterialProperty cubeColor = null;
		MaterialProperty cubeMap = null;

		//MaterialProperty reflectionTex = null;
		MaterialProperty reflection = null;
		MaterialProperty fresnel = null;
				
		//Foam
		MaterialProperty foamToggle = null;
		MaterialProperty foamColor = null;
		MaterialProperty foamTex = null;
		MaterialProperty foamSize = null;

		//Displacement
		MaterialProperty displacementToggle = null;
		MaterialProperty displacementSpeed = null;
		MaterialProperty displacementFrequency = null;
		MaterialProperty displacementStrength = null;


		MaterialEditor m_MaterialEditor;

		
		public void FindProperties(MaterialProperty[] props)
		{

			//Color
			shallowColor 	= FindProperty ("_Color", props);
			depthColor 		= FindProperty ("_DepthColor", props);
			depth 			= FindProperty ("_Depth", props);
			
			//Spec
			specColor 		= FindProperty ("_SpecColor", props);
			smoothness 		= FindProperty ("_Smoothness", props);

			//Distortion
			bumpMap 		= FindProperty ("_BumpMap", props);
			bumpStrength 	= FindProperty ("_BumpStrength", props);

			//Animation
			speedX 			= FindProperty ("_SpeedX", props);
			speedY 			= FindProperty ("_SpeedY", props);
			
			//Distortion
			distortion 			= FindProperty ("_Distortion", props);
			distortionQuality 	= FindProperty ("_DistortionQuality", props);
			
			//Reflection
			reflectionType 	= FindProperty ("_ReflectionType", props);
			cubeColor 		= FindProperty ("_CubeColor", props);
			cubeMap 		= FindProperty ("_Cube", props);
			reflection 		= FindProperty ("_Reflection", props);
			fresnel 		= FindProperty ("_RimPower", props);
			
			//Foam
			foamToggle 		= FindProperty ("_FOAM", props);
			foamColor 		= FindProperty ("_FoamColor", props);
			foamTex			= FindProperty ("_FoamTex", props);
			foamSize 		= FindProperty ("_FoamSize", props);

			//Displacement
			displacementToggle = FindProperty ("_DISPLACEMENT", props);
			displacementSpeed = FindProperty ("_NoiseSpeed", props);
			displacementFrequency = FindProperty ("_NoiseFrequency", props);
			displacementStrength = FindProperty ("_NoisePower", props);

		}
		
		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
		{
			FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
			m_MaterialEditor = materialEditor;
			Material material = materialEditor.target as Material;

			ShaderPropertiesGUI(material);
		}
		
		public void ShaderPropertiesGUI(Material material)
		{
			// Use default labelWidth
			EditorGUIUtility.labelWidth = 0f;
			EditorGUIUtility.fieldWidth = 64f;

			Texture2D tex = Resources.Load("CalmWaterLogo") as Texture2D;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(tex);	
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();


			// Detect any changes to the material
			EditorGUI.BeginChangeCheck();
			{
				// Color
				GUILayout.BeginVertical("", GUI.skin.box);
				GUILayout.Label ("Color", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(shallowColor,"Shallow Color");
				m_MaterialEditor.ShaderProperty(depthColor,"Depth Color");
				m_MaterialEditor.ShaderProperty(depth,"Depth");
				GUILayout.EndVertical();

				// Spec
				GUILayout.BeginVertical("", GUI.skin.box);
				GUILayout.Label ("Specular", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(specColor,"Specular Color");
				m_MaterialEditor.ShaderProperty(smoothness,"Smoothness");
				GUILayout.EndVertical();

				// NormalMap
				GUILayout.BeginVertical("", GUI.skin.box);
				GUILayout.Label ("Bump", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(bumpMap,"NormalMap");
				m_MaterialEditor.ShaderProperty(bumpStrength,"Bump Strength");
				GUILayout.Label ("Scroll Animation", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(speedX,"Scroll Speed [X]");
				m_MaterialEditor.ShaderProperty(speedY,"Scroll Speed [Y]");
				//Distortion
				GUILayout.Label ("Distortion", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(distortion,"Distortion");
				m_MaterialEditor.ShaderProperty(distortionQuality,"Distortion Quality");
				GUILayout.EndVertical();


				// Reflections
				GUILayout.BeginVertical("", GUI.skin.box);
				GUILayout.Label ("Reflections", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(reflectionType,"Reflection Type");

				switch((int)reflectionType.floatValue){
					// Mixed Mode Reflections
					case 0 :
						EditorGUILayout.HelpBox("You need to add MirrorReflection script to your object.",MessageType.Info);
						m_MaterialEditor.ShaderProperty(cubeColor,"Cube Color");
						m_MaterialEditor.ShaderProperty(cubeMap,"Cube Map");
						m_MaterialEditor.ShaderProperty(reflection,"Reflection");
					break;
					// RealTime Mode Reflections
					case 1 :
						EditorGUILayout.HelpBox("You need to add MirrorReflection script to your object.",MessageType.Info);
						m_MaterialEditor.ShaderProperty(reflection,"Reflection");
					break;
					// CubeMap Reflections
					case 2:
						m_MaterialEditor.ShaderProperty(cubeColor,"Cube Color");
						m_MaterialEditor.ShaderProperty(cubeMap,"Cube Map");
					break;
				}

				m_MaterialEditor.ShaderProperty(fresnel,"Fresnel");

				GUILayout.EndVertical();

				// Foam
				GUILayout.BeginVertical("", GUI.skin.box);
				GUILayout.Label ("Foam", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(foamToggle,"Enable Foam");

				if(foamToggle.floatValue == 1)
				{
					m_MaterialEditor.ShaderProperty(foamColor, "Foam Color");
					m_MaterialEditor.ShaderProperty(foamTex, "Foam Texture");
					m_MaterialEditor.ShaderProperty(foamSize,"Foam Size");
				}

				GUILayout.EndVertical();

				// Displacement
				GUILayout.BeginVertical("", GUI.skin.box);
				GUILayout.Label ("Displacement", EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(displacementToggle,"Enable Displacement");

				if(displacementToggle.floatValue == 1)
				{
					EditorGUILayout.HelpBox("You need enough subdivisions in your Geometry.",MessageType.Info);
					m_MaterialEditor.ShaderProperty(displacementSpeed,"Displacement Speed");
					m_MaterialEditor.ShaderProperty(displacementStrength,"Displacement Strength");
					m_MaterialEditor.ShaderProperty(displacementFrequency,"Displacement Frequency");
				}

				GUILayout.EndVertical();

			}
			
		}
		

	}
}