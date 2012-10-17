﻿using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain.Trees
{
    public static class Tree {
        private static TgcMesh baseTree;
        private static int count;
        private static readonly Vector3 scale = new Vector3(5, 5, 5);

        private static void createBaseTree() {
            var scene = new TgcSceneLoader().loadSceneFromFile(Path.TankScene);
            baseTree = scene.Meshes[2];
        }

        public static TgcMesh create(Vector3 initialPosition) {
            if (baseTree == null)
                createBaseTree();

            count++;
            
            var newTree = baseTree.createMeshInstance(baseTree.Name + count);
            newTree.Position = initialPosition;
            newTree.Scale = scale;
            newTree.AlphaBlendEnable = true;

            return newTree;
        }

        public static float baseSize {
            get {
                if (baseTree == null)
                    createBaseTree();

                return baseTree.BoundingBox.calculateSize().X * scale.X;
            }
        }
    }
}
