using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects.Editors
{
    [CustomEditor(typeof(StreamerWithRamp))]
    public class StreamerWithRampEditor : Editor
    {
        SerializedProperty _spProgress;
        private SerializedProperty _spStreamerTexture;
        private SerializedProperty _spStreamerColor;
        private SerializedProperty _spMoveSpeed;
        private SerializedProperty _spPower;
        
        SerializedProperty _spPlay;
        SerializedProperty _spLoop;
        SerializedProperty _spLoopDelay;
        SerializedProperty _spDuration;
        SerializedProperty _spInitialPlayDelay;
        SerializedProperty _spUpdateMode;

        private void OnEnable()
        {
            _spProgress = serializedObject.FindProperty("m_Progress");
            _spStreamerTexture = serializedObject.FindProperty("m_StreamerTexture");
            _spStreamerColor = serializedObject.FindProperty("m_StreamerColor");
            _spMoveSpeed = serializedObject.FindProperty("m_MoveSpeed");
            _spPower = serializedObject.FindProperty("m_Power");
            var player = serializedObject.FindProperty("m_Player");
            _spPlay = player.FindPropertyRelative("play");
            _spDuration = player.FindPropertyRelative("duration");
            _spInitialPlayDelay = player.FindPropertyRelative("initialPlayDelay");
            _spLoop = player.FindPropertyRelative("loop");
            _spLoopDelay = player.FindPropertyRelative("loopDelay");
            _spUpdateMode = player.FindPropertyRelative("updateMode");
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();

            //================
            // Effect setting.
            //================
            EditorGUILayout.PropertyField(_spProgress);
            EditorGUILayout.PropertyField(_spStreamerTexture);
            EditorGUILayout.PropertyField(_spStreamerColor);
            EditorGUILayout.PropertyField(_spMoveSpeed);
            EditorGUILayout.PropertyField(_spPower);
            //================
            // Effect player.
            //================
            EditorGUILayout.PropertyField(_spPlay);
            EditorGUILayout.PropertyField(_spDuration);
            EditorGUILayout.PropertyField(_spInitialPlayDelay);
            EditorGUILayout.PropertyField(_spLoop);
            EditorGUILayout.PropertyField(_spLoopDelay);
            EditorGUILayout.PropertyField(_spUpdateMode);

            // Debug.
            using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Debug");

                if (GUILayout.Button("Play", "ButtonLeft"))
                {
                    (target as StreamerWithRamp).Play();
                }

                if (GUILayout.Button("Stop", "ButtonRight"))
                {
                    (target as StreamerWithRamp).Stop();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}