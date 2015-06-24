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

let updatedPosition(position: Point, velocity: Vector) : Point =
    let mutable newXPos = position.X + velocity.Magnitude * Math.Sin(velocity.Trajectory)
    let mutable newYPos = position.Y + velocity.Magnitude * Math.Cos(velocity.Trajectory)

    if (newXPos > 2.0 * aspectRatio) then newXPos <- -2.0 * aspectRatio
    if (newXPos < -2.0 * aspectRatio) then newXPos <- 2.0 * aspectRatio
    if (newYPos > 2.0) then newYPos <- -2.0
    if (newYPos < -2.0) then newYPos <- 2.0 
    {X = newXPos; Y = newYPos}

// Trajectory of 0 = north. X/Y Coordinates similar to a graph (up is +ve Y, right is +ve X)
let moveShip(state: GameState) : Ship =
    let pos = state.Ship.Position
    let vel = state.Ship.Velocity
    let delta = state.Ship.Delta
    let newMagnitude = Math.Max(Math.Min(vel.Magnitude + delta.Magnitude, 0.15), -0.1)
    let newVel = { Magnitude = newMagnitude; Trajectory = vel.Trajectory + delta.Trajectory }
    
    let newPos = updatedPosition(pos, newVel)
    { Position = newPos; Delta = delta; Velocity = newVel }

let moveAsteroids(state: GameState) : List<Asteroid> =
    state.Asteroids |> List.map (fun a ->
        let newPos = updatedPosition(a.Position, a.Velocity)
        { Position = newPos; Velocity = a.Velocity; Nodes = a.Nodes }
    )

let objectsCollided(position1 : Point)(position2 : Point)(radius1 : float)(radius2: float) : bool =
    let xDiff = position1.X - position2.X;
    let yDiff = position1.Y - position2.Y;
    xDiff * xDiff + yDiff * yDiff < (radius1 + radius2) * (radius1 + radius2)

let detectCollisions(state: GameState) =
    let allObjects = List.concat
    let asteroidRadius = 0.45
    let shipRadius = 0.1
    for a in state.Asteroids do
        if (objectsCollided a.Position state.Ship.Position asteroidRadius shipRadius) then printfn "Ship collision"
    
    for i in 0..state.Asteroids.Length-2 do
        let a = state.Asteroids.Item i
        for j in i+1..state.Asteroids.Length-1 do
            let b = state.Asteroids.Item j
            if (objectsCollided b.Position state.Ship.Position asteroidRadius asteroidRadius) then printfn "Asteroid collision (%d + %d)" i j


let moveObjects(state: GameState) =
    state.Ship <- moveShip state
    state.Asteroids <- moveAsteroids state
    detectCollisions state
