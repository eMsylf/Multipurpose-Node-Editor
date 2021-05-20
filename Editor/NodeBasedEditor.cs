/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace NodeEditor
{
    public enum Direction { LeftRight, TopBottom }

    public class NodeBasedEditor : EditorWindow
    {
        private List<Node> nodes;
        private List<Connection> connections;
        private Direction direction = Direction.TopBottom;
        public NodeStructure reference;
        string fileName = "New Node Structure";

        private GUIStyle nodeStyle;
        private GUIStyle selectedNodeStyle;
        private GUIStyle inPointStyle;
        private GUIStyle outPointStyle;

        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;

        private Vector2 offset;
        private Vector2 drag;

        private float zoom = 1f;
        private float zoomFactor = .1f;
        private float zoomMin = .3f;
        private float zoomMax = 2f;

        [MenuItem("Window/Node Based Editor")]
        public static NodeBasedEditor OpenWindow()
        {
            NodeBasedEditor window = GetWindow<NodeBasedEditor>();
            window.titleContent = new GUIContent("Node Based Editor");
            window.saveChangesMessage = "This node structure has not been saved. Would you like to save?";
            return window;
        }

        private void OnEnable()
        {
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

        }

        private void OnGUI()
        {
            DrawGrid(20 * zoom, new Color(.5f, .5f, .5f, .2f));
            DrawGrid(100 * zoom, new Color(.5f, .5f, .5f, .4f));

            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);

            DrawToolbar();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }
        public override void SaveChanges()
        {
            if (reference == null)
            {
                reference = ScriptableObject.CreateInstance<NodeStructure>();
                
                //reference = new NodeStructure();
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

            Debug.Log($"{this} saved successfully to file {reference.name}!", reference);
            base.SaveChanges();
        }

        // TODO: Make loading work
        public void Load()
        {
            if (reference == null) return;
            
            nodes?.Clear();
            connections?.Clear();
            ClearConnectionSelection();
            Debug.Log("Found " + reference.nodes.Count + " nodes");
            Debug.Log("Found " + reference.connections.Count + " connections");
            nodes = reference.nodes.ToList();
            connections = reference.connections.ToList();
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
            Rect background = new Rect(0, 0, position.width, 20f);
            GUI.Box(background, "");
            GUILayout.BeginHorizontal();
            
            if (reference == null) fileName = EditorGUILayout.TextField(fileName);
            else reference = (NodeStructure)EditorGUILayout.ObjectField(reference, typeof(NodeStructure));
            if (GUILayout.Button("Save"))
            {
                Debug.Log("Save");
                SaveChanges();
            }
            if (GUILayout.Button("Load"))
            {
                Debug.Log("Load");
                Load();
            }

            // Unsaved changes message
            //saveChangesMessage = EditorGUILayout.TextField(saveChangesMessage);

            //EditorGUILayout.LabelField(hasUnsavedChanges ? "I have changes!" : "No changes.", EditorStyles.wordWrappedLabel);
            //EditorGUILayout.LabelField("Try to close the window.");

            //if (GUILayout.Button("Create unsaved changes"))
            //    hasUnsavedChanges = true;

            //if (GUILayout.Button("Save"))
            //    SaveChanges();
            //GUILayout.EndHorizontal();

            // Old toolbar
            //Rect fileField = new Rect(2, 1, 50f, 18f);
            //GUI.Label(fileField, "File name");
            //Rect buttonRect = new Rect(52, 1, 50f, 18f);
            //if (GUI.Button(buttonRect, "Save"))
            //{
            //    Debug.Log("Save");
            //}
        }

        private void DrawNodes()
        {
            if (nodes == null)
                return;

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw(direction);
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
                        // TODO: Add node when dragging a connection endpoint, and clicking on nothing.
                        if (selectedInPoint != null)
                            OnClickAddNode(e.mousePosition);
                        ClearConnectionSelection();
                    }

                    if (e.button == 1) 
                        ProcessContextMenu(e.mousePosition);
                    break;

                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        OnDrag(e.delta);
                    }
                    break;
                // TODO: make zoom affect nodes
                //case EventType.ScrollWheel:
                //    zoom = Mathf.Clamp(zoom - e.delta.y * zoomFactor, zoomMin, zoomMax);
                //    //Debug.Log("Adjust zoom: " + zoom);
                //    GUI.changed = true;
                //    break;
                // TODO: Allow dragging with spacebar
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Space)
                    {
                        Debug.Log("Space");
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
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
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

            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + dir * 50f,
                    e.mousePosition - dir * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - dir * 50f,
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

        private void OnClickAddNode(Vector2 mousePosition)
        {
            if (nodes == null) nodes = new List<Node>();
            nodes.Add(new Node(mousePosition, new Vector2(200, 50), direction, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;
            if (selectedOutPoint == null) return;

            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
            }
            ClearConnectionSelection();
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;
            if (selectedInPoint == null) return;

            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
            }
            ClearConnectionSelection();
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
        }

        private void CreateConnection()
        {
            if (connections == null) 
                connections = new List<Connection>();
            connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
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
                // Is this necessary? Doesn't the list get disposed of once the if-statement is exited?
                connectionsToRemove = null;
            }

            nodes.Remove(node);
        }
    }
}