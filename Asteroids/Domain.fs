﻿module Domain

open Geometry
      
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
}

//For state changes based on user events. 
//Probably a good idea to have a seperate union for state changes base on internal game events
type UserStateChange = 
    | EndGame
    | Accelerate of float
    | RotateDirection of float
    | NoChange

let initialState = { 
    Running = Continue
    Ship = {
        Position = {X = 0.0; Y = 0.0;};
        Velocity = {Magnitude = 0.0; Trajectory = 0.0};
        Delta = { Magnitude = 0.0; Trajectory = 0.0; }
    }
}
