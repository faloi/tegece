using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class TreeFactory : IRenderObject {
        private readonly float scaleXZ;
        private readonly float scaleY;
        
        private readonly List<TgcMesh> trees;

        public TreeFactory(float scaleXZ, float scaleY) {
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;

            this.trees = new List<TgcMesh>();
        }

        private void addTree(TgcMesh instance) {
            this.trees.Add(instance);
        }

        public void createTrees(int count, int maxRadius, int [,] yValues) {
            var treeSize = Tree.baseSize;
            var maxValue = (int) (maxRadius - 2 * Tree.baseSize / this.scaleXZ);
            var randomizer = new Randomizer(10, maxValue);

            for (var i = 1; i < count; i++) {
                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();

                var adjust = (int) (maxRadius + treeSize / this.scaleXZ);
                var offsetY = yValues[offsetX + adjust, offsetZ + adjust];

                var initialPosition = new Vector3(offsetX * this.scaleXZ, offsetY * this.scaleY, offsetZ * this.scaleXZ);
                var newTree = Tree.create(initialPosition);
                
                this.addTree(newTree);
            }
        }

        public void render() {
            this.trees.ForEach(t => t.render());
        }

        public void dispose() {
            this.trees.ForEach(t => t.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}
