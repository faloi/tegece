﻿using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Interfaces {
    public interface ITerrainCollision {

        float getYValueFor(float x, float z);

        void deform(float x, float z, float radius, int power);

        bool isOutOfBounds(ITransformObject tankOrMissile);

        float ScaleY { get; }

    }
}