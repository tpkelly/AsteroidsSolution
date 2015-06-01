module Physics

open System

open Domain
open Geometry
open Window

let updateGameState (state: GameState)  change = 
    match change with 
    | Accelerate acceleration -> 
        let pos = state.Ship.Position
        let vel = state.Ship.Velocity
        let newVel = {Magnitude = vel.Magnitude + acceleration; Trajectory = vel.Trajectory}
        let newShip = {state.Ship with Velocity = newVel}
        {state with Ship = newShip}
    | RotateDirection rotation ->
        let vel = state.Ship.Velocity
        let newVel = { Magnitude = vel.Magnitude; Trajectory = vel.Trajectory + rotation }
        let newShip = { state.Ship with Velocity = newVel }
        {state with Ship = newShip}
    | EndGame -> {state with Running=Stop}
    | NoChange -> state

let moveShip(state: GameState) : unit =
    let pos = state.Ship.Position
    let vel = state.Ship.Velocity
    let mutable newXPos = pos.X - vel.Magnitude * Math.Sin(vel.Trajectory)
    let mutable newYPos = pos.Y + vel.Magnitude * Math.Cos(vel.Trajectory)
    if (newXPos > 2.0 * aspectRatio) then newXPos <- -2.0 * aspectRatio
    if (newXPos < -2.0 * aspectRatio) then newXPos <- 2.0 * aspectRatio
    if (newYPos > 2.0) then newYPos <- -2.0
    if (newYPos < -2.0) then newYPos <- 2.0 
    let newPos = {X = newXPos; Y = newYPos}
    state.Ship.Position <- newPos
