using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements;
using UnityEditor;
using System;

using StatusFlags = UnityEngine.Experimental.UIElements.DropdownMenu.MenuAction.StatusFlags;

namespace GraphProcessor
{
	public class ToolbarView : VisualElement
	{
		class ToolbarButtonData
		{
			public string			name;
			public bool				toggle;
			public bool				value;
			public Action			buttonCallback;
			public Action< bool >	toggleCallback;
		}

		List< ToolbarButtonData >	leftButtonDatas = new List< ToolbarButtonData >();
		List< ToolbarButtonData >	rightButtonDatas = new List< ToolbarButtonData >();
		protected BaseGraphView		graphView;

		public ToolbarView(BaseGraphView graphView)
		{
			name = "ToolbarView";
			this.graphView = graphView;

			graphView.initialized += AddButtons;

			Add(new IMGUIContainer(DrawImGUIToolbar));
		}

		protected void AddButton(string name, Action callback, bool left = true)
		{
			((left) ? leftButtonDatas : rightButtonDatas).Add(new ToolbarButtonData{
				name = name,
				toggle = false,
				buttonCallback = callback
			});
		}

		protected void AddToggle(string name, bool defaultValue, Action< bool > callback, bool left = true)
		{
			((left) ? leftButtonDatas : rightButtonDatas).Add(new ToolbarButtonData{
				name = name,
				toggle = true,
				value = defaultValue,
				toggleCallback = callback
			});
		}

		protected virtual void AddButtons()
		{
			AddButton("Center", graphView.ResetPositionAndZoom);

			bool processorVisible = graphView.GetPinnedElementStatus< ProcessorView >() != StatusFlags.Hidden;
			AddToggle("Show Processor", processorVisible, (v) => graphView.ToggleView< ProcessorView>());

			AddButton("Show In Project", () => EditorGUIUtility.PingObject(graphView.graph), false);
		}

		void DrawImGUIButtonList(List< ToolbarButtonData > buttons)
		{
			foreach (var button in buttons)
			{
				if (button.toggle)
				{
					EditorGUI.BeginChangeCheck();
					button.value = GUILayout.Toggle(button.value, button.name, EditorStyles.toolbarButton);
					if (EditorGUI.EndChangeCheck() && button.toggleCallback != null)
						button.toggleCallback(button.value);
				}
				else
				{
					if (GUILayout.Button(button.name, EditorStyles.toolbarButton) && button.buttonCallback != null)
						button.buttonCallback();
				}
			}
		}

		void DrawImGUIToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);

			DrawImGUIButtonList(leftButtonDatas);

			GUILayout.FlexibleSpace();

			DrawImGUIButtonList(rightButtonDatas);

			GUILayout.EndHorizontal();
		}
	}
}
