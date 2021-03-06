﻿module Renderer

open System

open OpenTK
open OpenTK.Graphics.OpenGL

open Window
open Domain

let perspective = 2.

let renderShip (ship: Ship) =
    // Draw triangle based on ship position
    PrimitiveType.Triangles |> GL.Begin
    let shipPos = ship.Position
    let shipRot = ship.Velocity.Trajectory
    (*Note the 4. (or 4.0) for the z coordinate of the vertices is 4, instead of zero because of the specific projection. 
        For now, simply keep it and abstract out the coordinates so that you can just use X and Y, while keeping Z contstant. 

        One other thing to note about the coordinates: The screen coordinate system is not between nice numbers. 
        I attempted to clean that up, but I've had no luck so far. 
        *) 

    let tripointAngle = Math.PI * 2.0 / 3.0

    // Back-left
    GL.Color3(1., 0., 0.); GL.Vertex3(shipPos.X + 0.1 * Math.Sin(shipRot - tripointAngle), shipPos.Y + 0.1 * Math.Cos(shipRot - tripointAngle), perspective)
    // Back-right
    GL.Color3(1., 0., 0.); GL.Vertex3(shipPos.X + 0.1 * Math.Sin(shipRot + tripointAngle), shipPos.Y + 0.1 * Math.Cos(shipRot + tripointAngle), perspective) 
    // Nose
    GL.Color3(0.2, 0.9, 1.); GL.Vertex3(shipPos.X + 0.1 * Math.Sin(shipRot), shipPos.Y + 0.1 * Math.Cos(shipRot), perspective)
    GL.End()

    //Draw Ship Centre - Note: I've added this so you can see where the ship position is. 
    PrimitiveType.Points |> GL.Begin

    GL.Color3(1., 1., 1.); GL.Vertex3(shipPos.X, shipPos.Y, perspective) 
    GL.End()

let renderAsteroid (asteroid: Asteroid) =
    PrimitiveType.Polygon |> GL.Begin
    let position = asteroid.Position
    let rotation = asteroid.Velocity.Trajectory

    // Back-left
    GL.Color3(0.5, 0.5, 0.5)
    let mutable i = 0
    let perAngle = Math.PI * 2.0 / (float asteroid.Nodes.Length)
    while (i < asteroid.Nodes.Length) do
        let vertexMagnitude = asteroid.Nodes.Item i
        GL.Vertex3(position.X + vertexMagnitude * Math.Sin((float i) * perAngle),
                   position.Y + vertexMagnitude * Math.Cos((float i) * perAngle),
                   perspective)
        i <- i + 1
    GL.End()

let renderFrame (state: GameState)  =

    //OpenGL Stuff to set view
    GL.Clear(ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)
    let mutable modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY)
    GL.MatrixMode(MatrixMode.Modelview)
    GL.LoadMatrix(&modelview)

    renderShip state.Ship

    for asteroid in state.Asteroids do
        renderAsteroid asteroid

    // Game is double buffered
    game.SwapBuffers()

let load _ =
    // Some game and OpenGL Setup
    game.VSync <- VSyncMode.On

let resize _ = 
    //Setup of projection matrix for game
    GL.Viewport(game.ClientRectangle.X, game.ClientRectangle.Y, game.ClientRectangle.Width, game.ClientRectangle.Height)
    let mutable projection = Matrix4.CreatePerspectiveFieldOfView(float32 (Math.PI / 2.), float32 aspectRatio, 1.0f, 100.0f)
    GL.MatrixMode(MatrixMode.Projection)
    GL.LoadMatrix(&projection)
