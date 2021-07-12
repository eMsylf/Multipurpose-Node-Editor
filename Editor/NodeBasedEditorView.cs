/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System;
using BobJeltes.AI.BehaviorTree;
using Debug = UnityEngine.Debug;

namespace BobJeltes.NodeEditor
{
    public enum Orientation { LeftRight, TopBottom }

    public class NodeBasedEditorView : EditorWindow/*, ISerializationCallbackReceiver*/////
    {
        public Orientation orientation = Orientation.TopBottom;

        private float toolbarHeight = 20f;
        private bool toolbarEnabled = true;
        private const int toolbarButtonWidth = 40;
        private Rect toolbarToggle = new Rect(0, 0, 20, 20);
        private Rect toolbarToggleActive = new Rect(0, 20, 20, 20);
        private bool showSettings;
        private float toolbarSettingsHeight = EditorGUIUtility.singleLineHeight;
        private float toolbarSettingsWidth = 200;
        private GUIStyle toolbarSettingsStyle = new GUIStyle();

        public NodeStructure file;
        public string fileName = "New file";

        public List<NodeView> nodeViews = new List<NodeView>();
        public List<Connection> connections;
        public List<NodeView> SelectedNodes { get => nodeViews.FindAll(x => x.IsSelected); }
        private Connection connectionPreview = new Connection();

        internal GUIStyle nodeStyle;
        internal GUIStyle selectedNodeStyle;
        internal GUIStyle inPointStyle;
        internal GUIStyle outPointStyle;

        internal NodeView selectedNodeIn;
        internal NodeView selectedNodeOut;
        private bool allowLoops;

        private Vector2 offset;
        private Vector2 drag;

        private float zoom = 1f;
        private float zoomMin = .5f;
        private float zoomMax = 2f;

        private bool drawingSelectionRect = false;
        private Rect selectionRect;
        private GUIStyle selectionRectStyle;

        private Color gridSmallColor = new Color(.5f, .5f, .5f, .2f);
        private Color gridLargeColor = new Color(.5f, .5f, .5f, .4f);

        private bool multiSelecting;
        private bool isDragging;

        [MenuItem("Window/Bob Jeltes/Node Editor")]
        public static NodeBasedEditorView OpenWindow()
        {
            NodeBasedEditorView openedWindow = GetWindow<NodeBasedEditorView>("Node Structure");
            openedWindow.saveChangesMessage = "This " + "Node Structure" + " has not been saved. Would you like to save?";
            return openedWindow;
        }

        internal virtual void OnEnable()
        {
            toolbarSettingsStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/dockarea back.png") as Texture2D;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.alignment = TextAnchor.MiddleCenter;

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
            selectedNodeStyle.alignment = TextAnchor.MiddleCenter;

            string inSide;
            string outSide;
            RectOffset border;
            switch (orientation)
            {
                default:
                case Orientation.LeftRight:
                    inSide = "left";
                    outSide = "right";
                    border = new RectOffset(4, 4, 12, 12);
                    break;
                case Orientation.TopBottom:
                    inSide = "mid";
                    outSide = "mid";
                    border = new RectOffset(4, 4, 2, 2);
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

            PopulateView();
            ClearConnectionSelection();
            hasUnsavedChanges = false;
        }

        internal virtual void OnGUI()
        {
            DrawGrid(20 * zoom, gridSmallColor);
            DrawGrid(100 * zoom, gridLargeColor);

            DrawNodes(zoom);
            DrawConnections();

            DrawConnectionPreview(Event.current);

            DrawToolbar();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        #region File Management
        internal virtual bool UnsavedChangesCheck()
        {
            if (hasUnsavedChanges)
            {
                int result = EditorUtility.DisplayDialogComplex("Unsaved Changes Detected", saveChangesMessage, "Yes", "Cancel", "Discard");
                switch (result)
                {
                    case 0:
                        // Yes
                        if (this is BehaviorTreeEditorView)
                        {
                            (this as BehaviorTreeEditorView).Save(); // Vies//
                        }
                        else
                        {
                            Save(); // TODO: Does not call overridden method
                        }
                        break;
                    case 1:
                        // Cancel
                        return false;
                    case 2:
                        // Discard
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        [Shortcut("Node Based Editor/New node structure", KeyCode.N, ShortcutModifiers.Alt)]
        public static void NewFile_Shortcut()
        {
            if (focusedWindow.GetType().BaseType == typeof(NodeBasedEditorView))
                (focusedWindow as NodeBasedEditorView).NewFile();
        }

        internal virtual void NewFile()
        {
            if (!UnsavedChangesCheck()) return;
            fileName = $"New {"Node Structure"}";
            file = default;
            nodeViews = new List<NodeView>();
            connections = new List<Connection>();
            tempConnectionIndices?.Clear();
            PopulateView();
            ClearConnectionSelection();
            hasUnsavedChanges = false;
        }

        [Shortcut("Node Based Editor/Save node structure", defaultKeyCode: KeyCode.S, defaultShortcutModifiers: ShortcutModifiers.Alt)]
        public static void SaveChanges_Shortcut()
        {
            // Kan zijn dat als de editor derivet en niet de directe type is, dat de if-statement false returnt
            if (focusedWindow.GetType().BaseType == typeof(NodeBasedEditorView))
                (focusedWindow as NodeBasedEditorView).Save();
        }

        public virtual void Save()
        {
            if (!hasUnsavedChanges) return;
            if (file == null)
            {
                file = CreateInstance<NodeStructure>();
            }
            foreach (var nodeView in nodeViews)
            {
                nodeView.node.positionOnView = new Vector2(nodeView.rect.position.x, nodeView.rect.position.y);
            }
            foreach (var connection in connections)
            {
                // Add children to connected nodes in the CreateConnection function
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Nodes"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Nodes");
            }
            if (string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(file)))
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/Nodes/{fileName}.asset");
                AssetDatabase.CreateAsset(file, assetPath);
            }
            SaveChanges();
        }

        public override void SaveChanges()
        {
            EditorUtility.SetDirty(file);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = file;
            EditorGUIUtility.PingObject(Selection.activeObject);
            // TODO: test if assetdatabase refresh is needed & if it has great performance impact in large projects

            Debug.Log("Nodes saved successfully to file " + file.name, file);
            base.SaveChanges();
        }

        public virtual bool Load(NodeStructure structure)
        {
            if (!UnsavedChangesCheck()) return false;
            if (structure == null)
            {
                NewFile();
                return true;
            }

            nodeViews.Clear();
            connections.Clear();
            //Debug.Log("Found " + structure.nodePositions.Count + " nodes");
            //Debug.Log("Found " + structure.connectionIndices.Count + " connections");

            //foreach (var nodePosition in structure.nodePositions)
            //{
            //    CreateNodeView(nodePosition, centered: false);
            //}
            //foreach (var connection in structure.connectionIndices)
            //{
            //    CreateConnection(nodeViews[connection.a], nodeViews[connection.b]);
            //}
            file = structure;
            hasUnsavedChanges = false;
            ClearConnectionSelection();
            return true;
        }

        private List<Int2> tempConnectionIndices;

        //public void OnBeforeSerialize()
        //{
        //    // Create temporary storage of connection indices
        //    if (connections == null) return;
        //    if (tempConnectionIndices == null) tempConnectionIndices = new List<Int2>();
        //    //Debug.Log("Connections found: " + connections.Count);
        //    foreach (var connection in connections)
        //    {
        //        tempConnectionIndices.Add(new Int2(
        //            nodeViews.IndexOf(connection.outNode),
        //            nodeViews.IndexOf(connection.inNode)
        //            ));
        //    }
        //}

        //public void OnAfterDeserialize()
        //{
        //    // Re-apply connections from temporary connection index storage
        //    connections.Clear();
        //    if (nodeViews == null) Debug.Log("Nodes list is null");
        //    else Debug.Log("Nodes found: " + nodeViews.Count);
        //    if (tempConnectionIndices == null)
        //        return;
        //    Debug.Log("Node views: " + nodeViews.Count);
        //    Debug.Log("Connection index sets: " + tempConnectionIndices.Count);
        //    tempConnectionIndices.RemoveAll(c => c.a < 0 || c.b < 0);
        //    foreach (var connectionData in tempConnectionIndices)
        //    {
        //        // Somewhere new connections are added and not removed. 9 connection indices and only 4 nodes is unlikely.
        //        Debug.Log("Connection indices requested: " + connectionData.a + " & " + connectionData.b);
        //        CreateConnection(nodeViews[connectionData.a], nodeViews[connectionData.b]);
        //    }
        //    tempConnectionIndices.Clear();
        //}

        internal virtual void PopulateView()
        {
            if (file == null)
            {
                if (nodeViews == null) nodeViews = new List<NodeView>();
                foreach (var nodeView in nodeViews)
                {
                    if (nodeView.OnClickNode != null) nodeView.OnDragNode = OnDragNode;
                    if (nodeView.OnRemoveNode != null) nodeView.OnRemoveNode = OnClickRemoveNode;
                    if (nodeView.OnClickNode != null) nodeView.OnClickNode = OnClickNode;
                    if (nodeView.OnClickUp != null) nodeView.OnClickUp = OnClickUpNode;
                    if (nodeView.inPoint.OnClickConnectionPoint != null) nodeView.inPoint.OnClickConnectionPoint = OnClickConnectionPoint;
                    if (nodeView.outPoint.OnClickConnectionPoint != null) nodeView.outPoint.OnClickConnectionPoint = OnClickConnectionPoint;

                    nodeView.isDragged = false;
                    nodeView.IsSelected = false;
                    nodeView.multiSelecting = false;
                }
            }
            else
            {
                if (nodeViews == null) nodeViews = new List<NodeView>();
                foreach (var node in file.nodes)
                {
                    CreateNodeView(node, node.positionOnView);
                }

                if (connections == null) connections = new List<Connection>();
                foreach (var connection in connections)
                {
                    connection.OnClickRemoveConnection = OnClickRemoveConnection;
                }
            }
        }
        #endregion

        #region Draw functions
        internal virtual void DrawGrid(float spacing, Color color)
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

            if (file == null)
                fileName = EditorGUILayout.TextField(fileName, GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
            NodeStructure oldReference = file;
            EditorGUI.BeginChangeCheck();
            NodeStructure newReference = (NodeStructure)EditorGUILayout.ObjectField(file, typeof(NodeStructure), false, GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("New file selected");
                if (Load(newReference))
                {
                    file = newReference;
                }
                else
                {
                    file = oldReference;
                }
            }

            if (!hasUnsavedChanges) GUI.enabled = false;
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(toolbarButtonWidth)))
            {
                Debug.Log("Save");
                Save();
            }
            GUI.enabled = true;
            if (file == null && !hasUnsavedChanges)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(toolbarButtonWidth)))
            {
                NewFile();
            }
            GUI.enabled = true;
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

        internal virtual void DrawNodes(float zoom)
        {
            if (nodeViews == null)
                return;

            for (int i = 0; i < nodeViews.Count; i++)
            {
                nodeViews[i].Draw(orientation);
            }
        }

        internal virtual void DrawConnections()
        {
            if (connections == null) return;
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw(orientation);
            }
        }

        internal virtual void DrawConnectionPreview(Event e)
        {
            if (selectedNodeIn != null && selectedNodeOut == null)
            {
                connectionPreview.Draw(selectedNodeIn.inPoint.rect.center, e.mousePosition, orientation, false);
                GUI.changed = true;
            }

            if (selectedNodeOut != null && selectedNodeIn == null)
            {
                connectionPreview.Draw(e.mousePosition, selectedNodeOut.outPoint.rect.center, orientation, false);
                GUI.changed = true;
            }
        }
        #endregion

        #region Editor events
        internal virtual void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        OnClickBackground();
                    }

                    if (e.button == 1)
                    {
                        if (selectedNodeIn != null || selectedNodeOut != null)
                        {
                            ClearConnectionSelection();
                        }
                        else
                        {
                            ProcessContextMenu(e.mousePosition);
                        }
                    }

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
                        OnDragView(e.delta);
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
                        for (int i = 0; i < nodeViews.Count; i++)
                        {
                            NodeView node = nodeViews[i];
                            if (node.IsSelected)
                            {
                                OnClickRemoveNode(node);
                                i--;
                                GUI.changed = true;
                            }
                        }
                    }
                    if (e.keyCode == KeyCode.Space)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    if (e.keyCode == KeyCode.LeftControl)
                    {
                        multiSelecting = true;
                    }
                    break;
                case EventType.KeyUp:
                    if (e.keyCode == KeyCode.LeftControl)
                    {
                        multiSelecting = false;
                    }
                    break;
            }
        }

        internal virtual void OnDragView(Vector2 delta)
        {
            drag = delta;
            if (nodeViews != null)
            {
                for (int i = 0; i < nodeViews.Count; i++)
                {
                    nodeViews[i].SetPosition(nodeViews[i].rect.position + delta);
                }
            }

            GUI.changed = true;
        }
        #endregion

        #region Node Events
        internal virtual void ProcessNodeEvents(Event e)
        {
            if (nodeViews == null) return;

            for (int i = nodeViews.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodeViews[i].ProcessEvents(e);

                if (guiChanged) GUI.changed = true;
            }
        }

        internal virtual void ProcessContextMenu(Vector2 mousePosition)
        {
            Debug.Log("Show context menu");
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => CreateNodeView(mousePosition));
            genericMenu.ShowAsContext();
        }

        // TODO: Implement. Check if the focused window is of type editor, if the mouse is over the window, then use the mouse position to call ProcessContextMenu.
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

        internal virtual NodeView CreateNodeView(Vector2 position, float width = 200, float height = 50, bool centered = true)
        {
            if (centered)
            {
                position.x -= width * .5f;
                position.y -= height * .5f;
            }
            NodeView nodeView = new NodeView(new Rect(position, new Vector2(width, height)),
                                 nodeStyle,
                                 selectedNodeStyle,
                                 new ConnectionPoint(ConnectionPointType.In, NodeView.connectionPointSizeByOrientation[orientation], inPointStyle, OnClickConnectionPoint),
                                 new ConnectionPoint(ConnectionPointType.Out, NodeView.connectionPointSizeByOrientation[orientation], outPointStyle, OnClickConnectionPoint),
                                 onClickNode: OnClickNode, OnClickRemoveNode: OnClickRemoveNode,
                                 onDragNode: OnDragNode,
                                 onClickUp: OnClickUpNode,
                                 selectable: true);
            AddNodeView(nodeView);
            return nodeView;
        }

        public NodeView CreateNodeView(Node node)
        {
            return CreateNodeView(node, node.positionOnView, false);
        }

        /// <summary>
        /// Creates a node view and adds it to the nodeViews list
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeView CreateNodeView(Node node, Vector2 position, bool centered = true)
        {
            GUIStyle normalStyle;
            GUIStyle selectedStyle;
            Vector2 size;
            if (node.GetType() == typeof(RootNode))
            {
                NodeStyles.rootStyle.Load(out normalStyle, out selectedStyle, out size);
            }
            else
            {
                NodeStyles.standard1.Load(out normalStyle, out selectedStyle, out size);
            }
            if (centered)
            {
                position -= size * .5f;
            }
            NodeView nodeView = new NodeView(new Rect(position, size), normalStyle, selectedStyle, inPointStyle, outPointStyle, orientation, OnClickConnectionPoint, OnClickNode, OnClickRemoveNode, OnDragNode, OnClickUpNode, node);
            AddNodeView(nodeView);
            return nodeView;
        }

        internal virtual void AddNodeView(NodeView nodeView)
        {
            if (nodeViews == null) nodeViews = new List<NodeView>();
            nodeViews.Add(nodeView);
            //Debug.Log("Add node view");
            if (selectedNodeIn != null &&selectedNodeOut == null)
            {
                Debug.Log("Has node in selected");
                CreateConnection(nodeView, selectedNodeIn);
            }
            if (selectedNodeIn == null && selectedNodeOut != null)
            {
                Debug.Log("Has node out selected");
                CreateConnection(selectedNodeOut, nodeView);
            }
            ClearConnectionSelection();
            hasUnsavedChanges = true;
        }

        internal virtual void OnClickRemoveNode(NodeView node)
        {
            if (connections != null)
            {
                List<Connection> connectionsToRemove = new List<Connection>();

                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].inNodeView == node || connections[i].outNodeView == node)
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

            nodeViews.Remove(node);
            hasUnsavedChanges = true;
        }

        internal virtual void OnDragNode(NodeView node)
        {
            StartDrag();
            if (nodeViews.Count > 1)
                SelectedNodes.ForEach(x =>
                {
                    if (x != node)
                        x.SetPosition(x.rect.position + Event.current.delta);
                });
            hasUnsavedChanges = true;
        }

        internal virtual void StartDrag()
        {
            isDragging = true;
        }

        internal virtual void EndDrag()
        {
            isDragging = false;
        }

        internal virtual void OnClickNode(NodeView node)
        {
            if (multiSelecting)
            {
                if (!SelectedNodes.Contains(node)) SelectNode(node);
                else DeselectNode(node);
                return;
            }
            if (node.IsSelected)
            {
                return;
            }
            DeselectAllNodes();
            SelectNode(node);
        }

        internal virtual void OnClickUpNode(NodeView node)
        {
            Debug.Log("Click up on node " + node.rect.position);
            if (!multiSelecting && !isDragging && SelectedNodes.Count > 1)
            {
                DeselectAllNodes();
                SelectNode(node);
            }

            EndDrag();
        }

        internal virtual void SelectNode(NodeView node)
        {
            node.IsSelected = true;
            GUI.changed = true;
        }

        internal virtual void DeselectNode(NodeView node)
        {
            node.IsSelected = false;
            GUI.changed = true;
        }

        internal virtual void DeselectAllNodes()
        {
            Debug.Log($"Deselect {SelectedNodes.Count} nodes");
            SelectedNodes.ForEach(node => node.IsSelected = false);
            GUI.changed = true;
        }

        [Shortcut("Node Based Editor/Select All", KeyCode.A, ShortcutModifiers.Alt)]
        public static void SelectAllNodes_Shortcut()
        {
            if (focusedWindow.GetType().BaseType == typeof(NodeBasedEditorView))
                (focusedWindow as NodeBasedEditorView).SelectAllNodes();
        }

        protected virtual void SelectAllNodes()
        {
            nodeViews.ForEach(node => SelectNode(node));
            Repaint();
        }

        protected virtual void OnClickBackground()
        {
            DeselectAllNodes();
            if ((selectedNodeIn == null && selectedNodeOut != null) || (selectedNodeIn != null && selectedNodeOut == null))
            {
                OnClickBackgroundWithConnection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }

        protected virtual void OnClickBackgroundWithConnection()
        {
            SelectNode(CreateNodeView(Event.current.mousePosition, centered: true));
        }

        internal virtual List<NodeView> GetChildren(NodeView nodeView)
        {
            // TODO: Get all children of this node by finding all connections that have this nodeView as output node, then adding the corresponding input node to the list
            List<NodeView> children = new List<NodeView>();
            foreach (var connection in connections)
            {
                if (connection.outNodeView == nodeView)
                {
                    children.Add(connection.inNodeView);
                }
            }
            return children;
        }
        #endregion

        #region Connection Events
        internal virtual void OnClickConnectionPoint(NodeView node, ConnectionPointType type)
        {
            Debug.Log("Click " + type + " connection point of node at index " + nodeViews.IndexOf(node));
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
                if (connections.Exists(c => c.inNodeView == selectedNodeIn && c.outNodeView == selectedNodeOut))
                    Debug.LogWarning("A connection has already been established between these two nodes.");
                else
                    CreateConnection(selectedNodeOut, selectedNodeIn);
            }
            ClearConnectionSelection();
        }

        internal virtual void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
            hasUnsavedChanges = true;
        }

        internal virtual void CreateConnection(NodeView nodeOut, NodeView nodeIn)
        {
            int indexOfNodeOut = nodeViews.IndexOf(nodeOut);
            if (indexOfNodeOut < 0) return;
            int indexOfNodeIn = nodeViews.IndexOf(nodeIn);
            if (indexOfNodeIn < 0) return;
            if (connections == null)
                connections = new List<Connection>();
            List<NodeView> children = GetChildren(nodeOut);
            List<Connection> connectionsToRemove = new List<Connection>();
            if (nodeOut.node == null)
            {
                return;
            }
            Type nodeOutType = nodeOut.node.GetType();

            // Node can have one connection
            if (typeof(NodeInterfaces.ISingleConnection).IsAssignableFrom(nodeOutType))
            {
                // Node has too many connections. Set back to single
                if (connections.Where(c => nodeOut == c.outNodeView).ToList().Count >= 1)
                {
                    connectionsToRemove = connections.Where(c => c.outNodeView == nodeOut).ToList();
                    connectionsToRemove.ForEach(c => connections.Remove(c));
                }
                connections.Add(new Connection(nodeOut, nodeIn, OnClickRemoveConnection));
                (nodeOut.node as NodeInterfaces.ISingleConnection).SetChild(nodeIn.node);
            }
            // Node can have multiple connections
            else if (typeof(NodeInterfaces.IMultipleConnection).IsAssignableFrom(nodeOutType))
            {
                connections.Add(new Connection(nodeOut, nodeIn, OnClickRemoveConnection));
            }

            hasUnsavedChanges = true;
            Debug.Log("Created connection between " + indexOfNodeOut + " and " + indexOfNodeIn);
        }

        // TODO: new method of handling connections where the parent node keeps track of what children are connected to it
        //internal virtual void CreateConnection(Node parent, Node child)
        //{
        //    parent.childNodes.Add(child);
        //    hasUnsavedChanges = true;
        //}

        internal virtual void ClearConnectionSelection()
        {
            selectedNodeIn = null;
            selectedNodeOut = null;
        }

        //public virtual void DrawCustomInspector()
        //{
        //    if (SelectedNodes.Count == 0) return;
        //    Vector2 inspectorSize = new Vector2(400f, this.position.height);
        //    Vector2 inspectorPosition = new Vector2(this.position.width - inspectorSize.x * .5f, 0f);
        //    Rect inspectorRect = new Rect(inspectorPosition, inspectorSize);
        //    GUI.Box(inspectorRect, GUIContent.none);
        //    foreach (NodeView nodeView in SelectedNodes)
        //    {
                
        //    }
        //}
        #endregion
    }
}
