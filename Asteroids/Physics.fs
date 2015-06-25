module Physics

open System

open Domain
open Geometry
open Window

let pythag (x : float, y : float) : float =
    Math.Sqrt(x * x + y * y)

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

let objectsCollided(position1 : Point, position2 : Point, radius1 : float, radius2: float) : bool =
    let xDiff = position1.X - position2.X;
    let yDiff = position1.Y - position2.Y;
    pythag(xDiff, yDiff) < (radius1 + radius2)

let detectCollisions(state: GameState) =
    let allObjects = List.concat
    let shipRadius = 0.1
    let collisionChange = 0.001
    let elasticity = 0.9

    for a in state.Asteroids do
        if (objectsCollided(a.Position, state.Ship.Position, asteroidRadius / 2.0, shipRadius / 2.0)) then printfn "Ship collision"
    
    for i in 0..state.Asteroids.Length-2 do
        let a = state.Asteroids.Item i
        for j in i+1..state.Asteroids.Length-1 do
            let b = state.Asteroids.Item j
            if (objectsCollided(a.Position, b.Position, asteroidRadius/2.0, asteroidRadius/2.0))
            then
                let vectorAX = a.Velocity.Magnitude * Math.Sin(a.Velocity.Trajectory)
                let vectorAY = a.Velocity.Magnitude * Math.Cos(a.Velocity.Trajectory)
                let vectorBX = b.Velocity.Magnitude * Math.Sin(b.Velocity.Trajectory)
                let vectorBY = b.Velocity.Magnitude * Math.Cos(b.Velocity.Trajectory)

                let midpoint = { X = (a.Position.X + b.Position.X) / 2.0; Y = (a.Position.Y + b.Position.Y) / 2.0 }
                let angle = Math.Atan2(a.Position.Y - b.Position.Y, a.Position.X - b.Position.X)

                let rMagnitude = pythag(vectorAX + vectorBX, vectorAY + vectorBY) * elasticity / 2.0
                
                let newMagnitudeA = pythag(vectorAX + 0.05 * Math.Sin(angle), vectorAY + 0.05 * Math.Cos(angle)) * elasticity
                let newMagnitudeB = pythag(vectorBX + 0.05 * Math.Sin(-angle), vectorBY + 0.05 * Math.Cos(-angle)) * elasticity

                let newAngleA = ((a.Velocity.Trajectory - angle) / 2.0 + a.Velocity.Trajectory) % Math.PI * 2.0
                let newAngleB = ((b.Velocity.Trajectory - angle) / 2.0 + b.Velocity.Trajectory) % Math.PI * 2.0

                a.Velocity <- { Magnitude = newMagnitudeA; Trajectory = newAngleA }
                b.Velocity <- { Magnitude = newMagnitudeB; Trajectory = newAngleB }

                // Move back from eachother a bit
                a.Position <- { X = midpoint.X + asteroidRadius * Math.Sin(angle); Y = midpoint.Y + asteroidRadius * Math.Cos(angle) }
                b.Position <- { X = midpoint.X - asteroidRadius * Math.Sin(angle); Y = midpoint.Y - asteroidRadius * Math.Cos(angle) }

let moveObjects(state: GameState) =
    state.Ship <- moveShip state
    state.Asteroids <- moveAsteroids state
    detectCollisions state
