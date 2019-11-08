using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        //it replaces `OnValidate()` method in `Planet`
        using (var check = new EditorGUI.ChangeCheckScope()) { 
            base.OnInspectorGUI();

            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if(GUILayout.Button("Update Planet"))
        {
            planet.GeneratePlanet();
        }

        EditorGUILayout.HelpBox("If you need to increase the resolution, you'd better hide meshes by Face Render Mask.", MessageType.Info);

        //[A]
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }


    // I don't know who is going to call this, but I know that on each call, something must be changed
    // With defining `Action`, I tell the method to be called. Look [A]
    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {

                if (foldout)
                {
                    //it create the editor when it needs, not everytime
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }
}
