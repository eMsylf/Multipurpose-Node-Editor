/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;

namespace NodeEditor
{
    public enum Direction { LeftRight, TopBottom }

    public class NodeBasedEditor : EditorWindow
    {
        public Direction direction = Direction.TopBottom;

        private float toolbarHeight = 20f;
        private bool toolbarEnabled = true;
        private Rect toolbarToggle = new Rect(0, 0, 20, 20);
        private Rect toolbarToggleActive = new Rect(0, 20, 20, 20);
        private bool showSettings;
        private float toolbarSettingsHeight = EditorGUIUtility.singleLineHeight;
        private float toolbarSettingsWidth = 200;
        private GUIStyle toolbarSettingsStyle = new GUIStyle();

        private List<Node> nodes = new List<Node>();
        private List<Connection> connections = new List<Connection>();
        public NodeStructure reference;
        string fileName = "New Node Structure";

        private GUIStyle nodeStyle;
        private GUIStyle selectedNodeStyle;
        private GUIStyle inPointStyle;
        private GUIStyle outPointStyle;

        private Node selectedNodeIn;
        private Node selectedNodeOut;
        private bool allowLoops;

        private Vector2 offset;
        private Vector2 drag;

        private float zoom = 1f;
        private float zoomMin = .5f;
        private float zoomMax = 2f;

        private bool drawingSelectionRect = false;
        private Rect selectionRect;
        private GUIStyle selectionRectStyle;

        static NodeBasedEditor window;

        [MenuItem("Window/Node Based Editor")]
        public static NodeBasedEditor OpenWindow()
        {
            window = GetWindow<NodeBasedEditor>();
            window.titleContent = new GUIContent("Node Based Editor");
            window.saveChangesMessage = "This node structure has not been saved. Would you like to save?";
            return window;
        }

        private void OnEnable()
        {
            toolbarSettingsStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/dockarea back.png") as Texture2D;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            string inSide;
            string outSide;
            RectOffset border;
            switch (direction)
            {
                default:
                case Direction.LeftRight:
                    inSide = "left";
                    outSide = "right";
                    border = new RectOffset(4, 4, 12, 12);
                    break;
                case Direction.TopBottom:
                    inSide = "mid";
                    outSide = "mid";
                    border = new RectOffset(4, 4, 12, 12);
                    break;
            }

            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn " + inSide + ".png") as Texture2D;
            inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn " + inSide + " on.png") as Texture2D;
            inPointStyle.border = border;

            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn " + outSide + ".png") as Texture2D;
            outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn " + outSide + " on.png") as Texture2D;
            outPointStyle.border = border;

            selectionRectStyle = new GUIStyle();
            selectionRectStyle.normal.background = new Texture2D(1, 1);
            selectionRectStyle.normal.background.SetPixel(1, 1, new Color(.5f, .5f, .5f, .3f));
            //selectionRectStyle.border = new RectOffset();

        }

        private void OnGUI()
        {
            DrawGrid(20 * zoom, new Color(.5f, .5f, .5f, .2f));
            DrawGrid(100 * zoom, new Color(.5f, .5f, .5f, .4f));

            DrawNodes(zoom);
            DrawConnections();

            DrawConnectionLine(Event.current);
            
            DrawToolbar();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        [Shortcut("Node Based Editor/New node structure", KeyCode.N, ShortcutModifiers.Alt)]
        public static void New()
        {
            if (focusedWindow.GetType() == typeof(NodeBasedEditor))
                (focusedWindow as NodeBasedEditor).NewFile();
        }

        private bool UnsavedChangesCheck()
        {
            if (hasUnsavedChanges)
            {
                int result = EditorUtility.DisplayDialogComplex("Unsaved Changes Detected", saveChangesMessage, "Yes", "Cancel", "Discard");
                switch (result)
                {
                    case 0:
                        SaveChanges();
                        break;
                    case 1:
                        // Cancel
                        return false;
                    case 2:
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private void NewFile()
        {
            if (!UnsavedChangesCheck()) return;
            fileName = "New Node Structure";
            reference = null;
            nodes = new List<Node>();
            connections = new List<Connection>();
            hasUnsavedChanges = false;
        }

        [Shortcut("Node Based Editor/Save node structure", defaultKeyCode: KeyCode.S, defaultShortcutModifiers: ShortcutModifiers.Alt)]
        public static void Save()
        {
            // Is dit netjes? Mag dit? Kan dit fout gaan?
            if (focusedWindow.GetType() == typeof(NodeBasedEditor))
                (focusedWindow as NodeBasedEditor).SaveChanges();
        }

        public override void SaveChanges()
        {
            if (reference == null)
            {
                reference = CreateInstance<NodeStructure>();
            }

            reference.nodes = nodes.ToList();
            reference.connections = connections.ToList();
            EditorUtility.SetDirty(reference);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Nodes"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Nodes");
            }
            if (string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(reference)))
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/Nodes/{fileName}.asset");
                AssetDatabase.CreateAsset(reference, assetPath);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = reference;
            EditorGUIUtility.PingObject(Selection.activeObject);
            // TODO: testen of dit nodig is & testen of dit een grote performance impact heeft met veel assets
            //AssetDatabase.Refresh();

            Debug.Log($"{this} saved successfully to file {reference.name}", reference);
            base.SaveChanges();
        }

        public bool Load(NodeStructure structure)
        {
            if (!UnsavedChangesCheck()) return false;
            nodes?.Clear();
            connections?.Clear();
            ClearConnectionSelection();
            Debug.Log("Found " + structure.nodes.Count + " nodes");
            Debug.Log("Found " + structure.connections.Count + " connections");
            nodes = structure.nodes.ToList();
            connections = structure.connections.ToList();
            reference = structure;
            return true;
        }

        private void DrawGrid(float spacing, Color color)
        {
            int widthDivs = Mathf.CeilToInt(position.width / spacing);
            int heightDivs = Mathf.CeilToInt(position.height / spacing);

            Handles.BeginGUI();
            Handles.color = color;

            offset += drag * .5f;
            Vector3 newOffset = new Vector3(offset.x % spacing, offset.y % spacing, 0);
            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(
                    new Vector3(
                        spacing * i, 
                        -spacing, 
                        0) + newOffset, 
                    new Vector3(
                        spacing * i, 
                        position.height, 
                        0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(
                    new Vector3(
                        -spacing, 
                        spacing * j, 
                        0) + newOffset, 
                    new Vector3(
                        position.width,
                        spacing * j,
                        0) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        public void DrawToolbar()
        {
            if (toolbarEnabled)
            {
                if (GUI.Button(toolbarToggleActive, "^", EditorStyles.miniButton)) toolbarEnabled = false;
            }
            else
            {
                if (GUI.Button(toolbarToggle, "v", EditorStyles.miniButton)) toolbarEnabled = true;
                return;
            }

            Rect menuBarRect = new Rect(0, 0, position.width, toolbarHeight);
            GUILayout.BeginArea(menuBarRect, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            if (reference == null) 
                fileName = EditorGUILayout.TextField(fileName, GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
            NodeStructure oldReference = reference;
            EditorGUI.BeginChangeCheck();
            NodeStructure newReference = (NodeStructure)EditorGUILayout.ObjectField(reference, typeof(NodeStructure), false, GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("New file selected");
                if (Load(newReference))
                {
                    reference = newReference;
                }
                else
                {
                    reference = oldReference;
                }
            }

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                Debug.Log("Save");
                SaveChanges();
            }

            if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                NewFile();
            }

            //GUILayout.FlexibleSpace();

            //if (GUILayout.Button("Settings", EditorStyles.toolbarDropDown, GUILayout.Width(70)))
            //{
            //    showSettings = !showSettings;
            //    Debug.Log("Show settings: " + showSettings);
            //}
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            // TODO: Re-enable settings dropdown
            //if (showSettings)
            //{
                //Rect toolbarSettingsRect = new Rect(position.width - toolbarSettingsWidth - 5, EditorGUIUtility.singleLineHeight + 5, toolbarSettingsWidth, toolbarSettingsHeight * 1 + 5);
                //GUILayout.BeginArea(toolbarSettingsRect, toolbarSettingsStyle);
                //GUILayout.BeginHorizontal();
                //GUILayout.Label("Zoom");
                //zoom = GUILayout.HorizontalSlider(zoom, zoomMin, zoomMax);
                //GUILayout.EndHorizontal();

                //GUILayout.EndArea();
            //}

            // Unsaved changes message
        }

        private void DrawNodes(float zoom)
        {
            if (nodes == null)
                return;

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw(direction, zoom);
            }
        }

        private void DrawConnections()
        {
            if (connections == null) return;
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw(direction);
            }
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (selectedNodeIn == null && selectedNodeOut != null)
                        {
                            Node newNode = OnClickAddNode(e.mousePosition, true);
                            OnClickConnectionPoint(newNode, ConnectionPointType.In);
                        }
                        if (selectedNodeIn != null && selectedNodeOut == null)
                        {
                            Node newNode = OnClickAddNode(e.mousePosition, true);
                            OnClickConnectionPoint(newNode, ConnectionPointType.Out);
                        }
                        ClearConnectionSelection();
                    }

                    if (e.button == 1) 
                        ProcessContextMenu(e.mousePosition);
                    break;

                case EventType.MouseDrag:
                    // TODO: Selection rect
                    //if (Tools.current == Tool.Rect)
                    //{
                    //    if (!drawingSelectionRect)
                    //    {
                    //        selectionRect = new Rect(e.mousePosition, new Vector2());
                            
                    //        Debug.Log("Start new rect");
                    //        drawingSelectionRect = true;
                    //    }
                    //    else
                    //    {
                    //        selectionRect.size += e.delta;
                    //        // Draw selection rect
                    //        Debug.Log("Draw selection rect");
                    //        GUI.Box(selectionRect, "Aaaaaa", selectionRectStyle);
                    //    }
                    //    GUI.changed = true;
                    //}
                    if (e.button == 2 || Tools.viewToolActive)
                    {
                        OnDrag(e.delta);
                    }
                    break;
                case EventType.MouseUp:
                    drawingSelectionRect = false;
                    break;
                // TODO: make zoom affect nodes
                //case EventType.ScrollWheel:
                //    zoom = Mathf.Clamp(zoom - e.delta.y * .1f, zoomMin, zoomMax);
                //    //Debug.Log("Adjust zoom: " + zoom);
                //    GUI.changed = true;
                //    break;
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.S)
                        Tools.current = Tool.Rect;
                    if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
                    {
                        Debug.Log("Delete");
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            Node node = nodes[i];
                            if (node.isSelected)
                            {
                                nodes.RemoveAt(i);
                                i--;
                                GUI.changed = true;
                            }
                        }
                    }
                    if (e.keyCode == KeyCode.Space)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;
            }
        }

        private void ProcessNodeEvents(Event e)
        {
            if (nodes == null) return;

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged) GUI.changed = true;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            Debug.Log("Show context menu");
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition, true));
            genericMenu.ShowAsContext();
        }

        private void DrawConnectionLine(Event e)
        {
            Vector2 dir;
            switch (direction)
            {
                default:
                case Direction.LeftRight:
                    dir = Vector2.left;
                    break;
                case Direction.TopBottom:
                    dir = -Vector2.up;
                    break;
            }

            if (selectedNodeIn != null && selectedNodeOut == null)
            {
                Handles.DrawBezier(
                    selectedNodeIn.inPoint.rect.center,
                    e.mousePosition,
                    selectedNodeIn.inPoint.rect.center + dir * 50f,
                    e.mousePosition - dir * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedNodeOut != null && selectedNodeIn == null)
            {
                Handles.DrawBezier(
                    selectedNodeOut.outPoint.rect.center,
                    e.mousePosition,
                    selectedNodeOut.outPoint.rect.center - dir * 50f,
                    e.mousePosition + dir * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }

        // TODO: Move this to the 
        //[Shortcut("Node Based Editor/New Node", KeyCode.Space)]
        //public static void AddNode()
        //{
        //    if (focusedWindow.GetType() == typeof(NodeBasedEditor))
        //    {
        //        if (mouseOverWindow.GetType() == typeof(NodeBasedEditor))
        //        {
        //            NodeBasedEditor editor = (focusedWindow as NodeBasedEditor);
        //            editor.ProcessContextMenu(new Vector2(editor.position.width * .5f, editor.position.height * .5f));
        //        }
        //    }
        //}

        private Node OnClickAddNode(Vector2 mousePosition, bool centered)
        {
            Debug.Log("Add node at " + mousePosition);
            if (nodes == null) nodes = new List<Node>();
            Node node = new Node(mousePosition, new Vector2(200, 50), direction, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickConnectionPoint, OnClickRemoveNode);
            if (centered)
            {
                node.rect.x -= node.rect.width * .5f;
                node.rect.y -= node.rect.height * .5f;
            }
            nodes.Add(node);
            hasUnsavedChanges = true;
            return node;
        }

        private void OnClickConnectionPoint(Node node, ConnectionPointType type)
        {
            switch (type)
            {
                case ConnectionPointType.In:
                    selectedNodeIn = node;
                    if (selectedNodeOut == null) return;
                    if (!allowLoops)
                    {
                        // TODO Check if the node would be connected downward with itself
                    }
                    break;
                case ConnectionPointType.Out:
                    selectedNodeOut = node;
                    if (selectedNodeIn == null) return;
                    if (!allowLoops)
                    {
                        // TODO Check if the node would be connected upward with itself
                    }
                    break;
                default:
                    break;
            }
            if (selectedNodeOut != selectedNodeIn)
            {
                CreateConnection();
            }
            ClearConnectionSelection();
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
            hasUnsavedChanges = true;
        }

        private void CreateConnection()
        {
            if (connections == null) 
                connections = new List<Connection>();
            connections.Add(new Connection(selectedNodeIn.inPoint, selectedNodeOut.outPoint, OnClickRemoveConnection));
            hasUnsavedChanges = true;
        }

        private void ClearConnectionSelection()
        {
            selectedNodeIn = null;
            selectedNodeOut = null;
        }

        private void OnClickRemoveNode(Node node)
        {
            if (connections != null)
            {
                List<Connection> connectionsToRemove = new List<Connection>();

                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                    {
                        connectionsToRemove.Add(connections[i]);
                    }
                }

                for (int i = 0; i < connectionsToRemove.Count; i++)
                {
                    connections.Remove(connectionsToRemove[i]);
                }
                // TODO: Is this necessary? Doesn't the list get disposed of once the if-statement is exited?
                connectionsToRemove = null;
            }

            nodes.Remove(node);
            hasUnsavedChanges = true;
            // TODO: Nodes are not disposed, it seems. Possible memory leak?
        }
    }
}