﻿using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject {
        private const float SKYBOX_DEPTH = 9000f;
        private readonly List<IRenderObject> components;

        private const int HEIGHTMAP_SIZE = 64;
        private const float INITIAL_SCALE_XZ = 20f;
        private const float INITIAL_SCALE_Y = 0.8f;
        private const float INITIAL_THRESHOLD = 0.075f;

        private const int TREES_COUNT = 100;

        public Terrain() {
            this.components = new List<IRenderObject> {
               new AdaptativeHeightmap(INITIAL_SCALE_XZ, INITIAL_SCALE_Y, INITIAL_THRESHOLD),
               new SkyBox(new Vector3(0, 0, 0), new Vector3(SKYBOX_DEPTH, SKYBOX_DEPTH, SKYBOX_DEPTH))
            };

            var trees = TreeFactory.createTrees(TREES_COUNT, HEIGHTMAP_SIZE * Convert.ToInt32(INITIAL_SCALE_XZ));
            this.components.AddRange(trees);
        }

        public void render() {
            this.components.ForEach(o => o.render());
        }

        public void dispose() {
            this.components.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}