using System;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

public class Missile : ITransformObject
{
    private bool isExploded ;
    private float flightTime;
    private TgcMesh mesh;
    private Vector3 currentVelocity;
    private Vector3 currentPosition;  
    private Vector3 gravity = new Vector3(0,-0.5f,0);

    public Matrix Transform { get; set; }
    public bool AutoTransformEnable { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Missile(Vector3 tankPosition) {

        var alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        var loader = new TgcSceneLoader();
        var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

        this.mesh = scene.Meshes[2];
        this.currentVelocity = new Vector3(0,0,0);
        this.currentPosition = new Vector3(tankPosition.X, tankPosition.Y + 5, tankPosition.Z);
        this.isExploded = false;
    }

    private void calcVelocity(float elapsedTime) {
        //V = V0 + A*dt
        var initialVelocity = (this.currentVelocity == new Vector3(0, 0, 0)) ? currentVelocity : new Vector3(2, 0, 2);
        currentVelocity = initialVelocity + gravity * this.flightTime;
    }

    private void calcPosition(float elapsedTime) {
        //X = X0 + V*dt + (A*dt^2)/2
        var initialPosition = this.currentPosition;
        currentPosition = initialPosition + currentVelocity * this.flightTime + (gravity * (this.flightTime * this.flightTime));
    }

    public void move(Vector3 v) {
        this.mesh.move(this.currentPosition.X,this.currentPosition.Y,this.currentPosition.Z);
    }
    public void move(float x, float y, float z) {
        throw new NotImplementedException();
    }
    public void moveOrientedY(float movement) {
        throw new NotImplementedException();
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
        this.calcVelocity(elapsedTime);
        this.calcPosition(elapsedTime);
        this.move(this.currentPosition);
        this.mesh.render();
    }

    public void dispose() {
        this.mesh.dispose(); ;
    }
}