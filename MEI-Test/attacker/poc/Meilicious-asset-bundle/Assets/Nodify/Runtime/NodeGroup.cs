﻿/**
Copyright (c) 2016, Antihero Studios LLC
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL Antihero Studios LLC BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Nodify.Runtime
{
    [CustomRenderer("Nodify.Rendering.NodeGroupRenderer")]
    public class NodeGroup : Node
    {
        #region Public Fields
        /// <summary>
        /// The visual "overall" panning of the child elements.
        /// </summary>
		[HideInInspector]
        public Vector2 editorWindowOffset;

        [HideInInspector]
        public float editorZoomAmount = 1;

        [HideInInspector]
        public bool editorShowComments = true;
        #endregion

        #region Public Properties
        /// <summary>
        /// Provides a list of Nodes
        /// that exist as a child of this
        /// Transform.
        /// </summary>
		public List<Node> childNodes
        {
            get
            {
                List<Node> children = new List<Node>();

                foreach (Transform child in transform)
                {
                    if (child.GetComponent<Node>())
                    {
                        children.Add(child.GetComponent<Node>());
                    }
                }

                return children;
            }
        }

        /// <summary>
        /// Provides a list of NodeGroup
        /// objects that exists as a child of
        /// this Transform.
        /// </summary>
		public List<NodeGroup> childGroups
        {
            get
            {
                List<NodeGroup> children = new List<NodeGroup>();

                foreach (Transform child in transform)
                {
                    if (child.GetComponent<NodeGroup>())
                    {
                        children.Add(child.GetComponent<NodeGroup>());
                    }
                }

                return children;
            }
        }

        #endregion

        #region Methods
        [Expose(true)]
        public void OnExit()
        {

        }

        protected override void OnExecute()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Nodes.NodeGroupEntry>())
                {
                    child.GetComponent<Nodes.NodeGroupEntry>().Execute();
                }
            }
        }

        protected override void OnEditorUpdate()
        {

        }

        public override void OnEditorNodeCreated()
        {
            base.OnEditorNodeCreated();

            var entry = CreateNode<Nodify.Runtime.Nodes.NodeGroupEntry>();
            var exitAnchor = entry.CreateExposedAnchor("OnComplete", AnchorType.METHOD);
            exitAnchor.editorPosition = new Vector2(100, 15);

            var exit = CreateNode<Nodify.Runtime.Nodes.NodeGroupExit>();
            exit.editorPosition = new Vector2(250, 75);

            exitAnchor.ConnectToNode(exit);
        }

        protected override void OnShowHideInHierarchy()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetBool("nodify.show_node_groups_in_hierarchy") == false)
            {
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                gameObject.hideFlags = HideFlags.None;
            }
#endif
        }

        public virtual void SetHideStateChildrenNonNodes(HideFlags hideFlags)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (!transform.GetChild(i).GetComponent<Node>())
                {
                    transform.GetChild(i).hideFlags = hideFlags;
                }

                if (transform.GetChild(i).GetComponent<NodeGroup>())
                {
                    transform.GetChild(i).GetComponent<NodeGroup>().SetHideStateChildrenNonNodes(hideFlags);
                }
            }
        }

        public  T CreateNode<T>() where T : Node
        {
            GameObject nodeInstance = new GameObject(typeof(T).Name, new System.Type[] { typeof(T) } );
            nodeInstance.transform.SetParent(transform);

            CreateMenu createMenu = Node.GetCreateMenu<T>();

            if (createMenu != null)
            {
                nodeInstance.name = createMenu.name;
            }

            nodeInstance.GetComponent<Node>().editorPosition = new Vector2(25, 75);
            nodeInstance.GetComponent<Node>().OnEditorNodeCreated();

            return nodeInstance.GetComponent<T>();
        }

        /// <summary>
        /// Returns a list of all anchors that are connected to this node as a method.
        /// </summary>
        /// <returns>The anchors connected to.</returns>
        /// <param name="node">Node.</param>
        public Anchor[] GetConnectingMethodAnchors(Node node)
        {
            var connectedAnchors = from otherNode in this.childNodes
                                   from otherAnchor in otherNode.anchors
                                   where otherAnchor.type == AnchorType.METHOD
                                   from otherAnchorNodeConnection in otherAnchor.nodeConnections
                                   where otherAnchorNodeConnection.target == node
                                   select otherAnchor;

            return connectedAnchors.ToArray();

        }

        /// <summary>
        /// Returns a list of all anchors that are connected to this node's variable anchors.
        /// </summary>
        /// <returns>The connecting variable anchors.</returns>
        /// <param name="node">Node.</param>
        public Anchor[] GetConnectingVariableAnchors(Node node)
        {
            var connectedAnchors = from otherNode in this.childNodes
                                   from otherAnchor in otherNode.anchors
                                   where otherAnchor.type == AnchorType.VARIABLE
                                   from nodeAnchor in node.anchors
                                   from otherAnchorConnection in otherAnchor.anchorConnections
                                   where otherAnchorConnection.target == nodeAnchor
                                   select otherAnchor;

            return connectedAnchors.ToArray();
        }
        #endregion
    }
}
