module Physics

open System

open Domain
open Geometry
open Window

let updateGameState (state: GameState)  change = 
    match change with 
    | Accelerate acceleration ->
        let newDelta = { Magnitude = acceleration; Trajectory = state.Ship.Delta.Trajectory }
        let newShip = {state.Ship with Delta = newDelta}
        {state with Ship = newShip}
    | RotateDirection rotation ->
        let newDelta = { Magnitude = state.Ship.Delta.Magnitude; Trajectory = rotation }
        let newShip = {state.Ship with Delta = newDelta}
        {state with Ship = newShip}
    | EndGame -> {state with Running=Stop}
    | NoChange -> state

// Trajectory of 0 = north. X/Y Coordinates similar to a graph (up is +ve Y, right is +ve X)
let moveShip(state: GameState) : unit =
    let pos = state.Ship.Position
    let vel = state.Ship.Velocity
    let delta = state.Ship.Delta
    let newVel = { Magnitude = vel.Magnitude + delta.Magnitude; Trajectory = vel.Trajectory + delta.Trajectory }
    
    let mutable newXPos = pos.X + newVel.Magnitude * Math.Sin(newVel.Trajectory)
    let mutable newYPos = pos.Y + newVel.Magnitude * Math.Cos(newVel.Trajectory)

    if (newXPos > 2.0 * aspectRatio) then newXPos <- -2.0 * aspectRatio
    if (newXPos < -2.0 * aspectRatio) then newXPos <- 2.0 * aspectRatio
    if (newYPos > 2.0) then newYPos <- -2.0
    if (newYPos < -2.0) then newYPos <- 2.0 
    let newPos = {X = newXPos; Y = newYPos}
    let newShip = { Position = newPos; Delta = delta; Velocity = newVel }
    state.Ship <- newShip

