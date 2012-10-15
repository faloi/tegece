﻿using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

public class Missile : ITransformObject
{
    private bool isExploded ;
    private float flightTime;
    private TgcMesh mesh;
  
    public TgcBoundingBox BoundingBox
    {
        get { return this.mesh.BoundingBox; }
    }

    private const float GRAVITY = -0.2f;
    private const float INITIAL_HORIZONTAL_SPEED=-900f; //Es constante porque en el eje X no hay gravedad
    private float initialVeticalSpeed;
    public bool isOutOfRange;
    public Matrix Transform { get; set; }
    public bool AutoTransformEnable { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Missile(Vector3 tankPosition,Vector3 tankRotation) {
       
        var alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        var loader = new TgcSceneLoader();
        var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

        this.mesh = scene.Meshes[2];
        this.mesh.Position = new Vector3(tankPosition.X, tankPosition.Y + 10, tankPosition.Z);
        this.mesh.Rotation = new Vector3(tankRotation.X, tankRotation.Y, tankRotation.Z );
        this.isExploded = false;
        this.initialVeticalSpeed = 8f;
    }

    public void move(Vector3 v) {
        throw new NotImplementedException();
    }
    public void move(float x, float y, float z) {
        throw new NotImplementedException();
    }
    public void moveOrientedY(float movement) {
        this.mesh.moveOrientedY(movement);
    }
    public void moveVertically() {
        this.initialVeticalSpeed = this.initialVeticalSpeed + GRAVITY*this.flightTime;
        this.mesh.move(0,  this.initialVeticalSpeed * this.flightTime + 0.5f * GRAVITY * this.flightTime * this.flightTime, 0); 
    }
    public void getPosition(Vector3 pos) {
        throw new NotImplementedException();
    }
    public void rotateX(float angle) {
        throw new NotImplementedException();
    }
    public void rotateY(float angle) {
        throw new NotImplementedException();
    }
    public void rotateZ(float angle) {
        throw new NotImplementedException();
    }

    public void render(float elapsedTime) {
        this.flightTime += elapsedTime;
        this.moveOrientedY(INITIAL_HORIZONTAL_SPEED * elapsedTime);
        this.moveVertically();
        this.mesh.render();
    }

    public void dispose() {
        this.mesh.dispose();
    }
}