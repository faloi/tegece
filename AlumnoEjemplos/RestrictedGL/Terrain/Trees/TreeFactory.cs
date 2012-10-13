using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class TreeFactory : IRenderObject {
        private Terrain terrain;
        private readonly List<OrientedMesh> trees;

        public TreeFactory(Terrain terrain) {
            this.terrain = terrain;

            this.trees = new List<OrientedMesh>();
        }

        private void addTree(OrientedMesh instance) {
            this.trees.Add(instance);
        }

        public void createTrees(int count, int maxRadius) {
            var treeSize = Tree.baseSize;
            var maxValue = (int) (maxRadius - 2 * Tree.baseSize / terrain.ScaleXZ);
            var randomizer = new Randomizer(10, maxValue);

            for (var i = 1; i < count; i++) {
                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();

                var adjust = (int) (maxRadius + treeSize / terrain.ScaleXZ);
                var offsetY = terrain.HeightmapData[offsetX + adjust, offsetZ + adjust];

                var initialPosition = new Vector3(offsetX * terrain.ScaleXZ, offsetY * terrain.ScaleY, offsetZ * terrain.ScaleXZ);
                var newTreeMesh = Tree.create(initialPosition);
                OrientedMesh newTree = new OrientedMesh(newTreeMesh);
                newTree.gridPos[0] = offsetX + adjust;
                newTree.gridPos[1] = offsetZ + adjust;
                
                this.addTree(newTree);
            }
        }

        private void updateTrees() {
            foreach (var t in trees) {
                var offsetY = terrain.HeightmapData[t.gridPos[0], t.gridPos[0]];
                t.mesh.Position = new Vector3(t.mesh.Position.X, offsetY * terrain.ScaleY, t.mesh.Position.Z);
            }
        }

        public void render() {
            updateTrees();
            this.trees.ForEach(t => t.mesh.render());
        }

        public void dispose() {
            this.trees.ForEach(t => t.mesh.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}