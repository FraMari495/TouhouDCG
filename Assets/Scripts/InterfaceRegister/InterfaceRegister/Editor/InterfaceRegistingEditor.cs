using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(PrefabInterface<>))]
public class InterfaceRegistingEditor : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		//このスクリプトが監視する変数
		var targetProp = property.FindPropertyRelative("scriptableObject");

		string targetTypeName = property.FindPropertyRelative("typeName").stringValue;




		//以前登録されていたゲームオブジェクト & 現在登録されているゲームオブジェクト
		var oldObject = (ScriptableObject)targetProp.objectReferenceValue;
		var newObject = (ScriptableObject)EditorGUI.ObjectField(position, label, oldObject, typeof(ScriptableObject),false);




		//現在登録されているシーンがnullである場合、シーン名として "" を登録
		if (newObject == null) targetProp.objectReferenceValue = null;
		else if (newObject != oldObject)
		{
			//newSceneがnullではなく、かつ
			//以前のものと現在のものが異なる場合
			//つまり、別のSceneAssetに置き換えられた場合

			//ドロップされたScriptableObjectがインターフェースを持っているか確認
			bool existsInBuildSettings = newObject.GetType().GetInterface(targetTypeName) !=null;
			
			//持っている場合更新
			if (existsInBuildSettings) targetProp.objectReferenceValue = newObject;
			else
			{
				//含まれていない場合、その旨をログに出力
				Debug.LogError($"<b>必要なインターフェースが実装されていません</b>\n {targetTypeName} を実装したコンポーネントを持つゲームオブジェクトを登録してください");
			}
		}
	}

}
