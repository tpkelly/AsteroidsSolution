module Domain

open Geometry
open System

type Asteroid = {
    Position: Point
    Velocity: Vector
    Nodes: List<float>
}

type Ship = {
    Position: Point
    Velocity: Vector
    Delta: Vector
}

type GameRunning =
    | Continue
    | Stop

type GameState = {
    Running : GameRunning
    mutable Ship : Ship
    mutable Asteroids : List<Asteroid>
}

//For state changes based on user events. 
//Probably a good idea to have a seperate union for state changes base on internal game events
type UserStateChange = 
    | EndGame
    | Accelerate of float
    | RotateDirection of float
    | NoChange

let Rand = Random();

let generateAsteroid numberOfVertices =
    let pos = {
        X = if (Rand.Next() % 2 = 0) then (Rand.NextDouble() + 1.0) / 2.0 else (Rand.NextDouble() - 2.0) / 2.0;
        Y = if (Rand.Next() % 2 = 0) then (Rand.NextDouble() + 1.0) / 2.0 else (Rand.NextDouble() - 2.0) / 2.0;
    }
    let vel = { Magnitude = Rand.NextDouble() / 20.0; Trajectory = Math.PI * 2.0 * Rand.NextDouble() }
    let nodes = [ for i in 1..numberOfVertices -> 0.2 + Rand.NextDouble() / 4.0] // 0.2 to 0.45
    { Position = pos; Velocity = vel; Nodes = nodes}


let initialState = { 
    Running = Continue
    Ship = {
        Position = {X = 0.0; Y = 0.0;};
        Velocity = {Magnitude = 0.0; Trajectory = 0.0};
        Delta = { Magnitude = 0.0; Trajectory = 0.0; }
    }
    Asteroids = [ for i in 1..10 -> generateAsteroid (Rand.Next() % 8 + 4) ]
}
