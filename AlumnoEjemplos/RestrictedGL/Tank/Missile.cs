using System;
using AlumnoEjemplos.RestrictedGL;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

public class Missile : ITransformObject
{
    public float flightTime { set; get; }
    private TgcMesh mesh;
  
    public TgcBoundingBox BoundingBox
    {
        get { return this.mesh.BoundingBox; }
    }

    private const float GRAVITY = -0.2f;
    private const float INITIAL_HORIZONTAL_SPEED=-900f; //Es constante porque en el eje X no hay gravedad
    private const int ALTURA_CANION = 60;

    private float initialVeticalSpeed;
    public Matrix Transform { get; set; }
    public bool AutoTransformEnable { get; set; }
    
    public Vector3 Position {
        get { return this.mesh.Position; }
        set { throw new NotImplementedException(); }
    }

    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Missile(Vector3 tankPosition,Vector3 tankRotation) {
        var loader = new TgcSceneLoader();
        var scene = loader.loadSceneFromFile(Path.TankScene);

        this.mesh = scene.Meshes[1];
        this.mesh.Position = new Vector3(tankPosition.X, tankPosition.Y + ALTURA_CANION, tankPosition.Z);
        this.mesh.Rotation = new Vector3(tankRotation.X, tankRotation.Y, tankRotation.Z );
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
    public void rotateY(float angle){
        throw new NotImplementedException();
        
    }
    public void rotateZ(float angle) {
        this.mesh.rotateZ(angle);
    }

    public void render() {
        this.flightTime += Shared.ElapsedTime;
        this.moveOrientedY(INITIAL_HORIZONTAL_SPEED * Shared.ElapsedTime);
        this.moveVertically();
        this.rotateZ(0.1f);
        this.mesh.render();
    }

    public void dispose() {
        this.mesh.dispose();
    }

    public bool isCollidingWith(ITerrainCollision terrain) {
        return terrain.getYValueFor(this.Position.X, this.Position.Z) >= (int) this.Position.Y;
    }
}