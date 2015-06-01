module Main

open System

open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL
open OpenTK.Input

open Geometry
open Domain
open Renderer

(*
    Note: While we are calling this an Asteroids clone, we may deviate from it...

    References: 
    http://www.opentk.com/
    Reactive Programming intro with observables and f# http://fsharpforfunandprofit.com/posts/concurrency-reactive/ 

    Tasks:
        - Refactor the open GL Program.fs 
        - Reach functionality goal of : 
            Deal with basic ship movement: 
                1. Left and Right arrow should rotate the ship
                2. Forward arrow should cause acceleration
                3. Backwards arrow should cause deceleration
                4. Asteroids style position wrap around when an object leaves the screen. PLay the asteroids game if you cant remember it!
        - Test as much as possible!
*)

[<EntryPoint>]
let main _ = 
    
    //Handle keydownEvents and transform them into state changes 
    //Hint (To get the best behaviour, you may need to deal with key up, etc events)
    let keyDown (args: KeyboardKeyEventArgs) =
        match args.Key with
        | Key.Escape ->  UserStateChange.EndGame
        | Key.Up -> UserStateChange.Accelerate 0.01
        | Key.Down -> UserStateChange.Accelerate -0.01
        | Key.Right -> UserStateChange.RotateDirection 0.1
        | Key.Left -> UserStateChange.RotateDirection -0.1
        | _ -> UserStateChange.NoChange

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


    use loadSubscription = game.Load.Subscribe load
    use resizeSubscription = game.Resize.Subscribe resize 

    (*Below, the game state is being stored in a reference cell instead of being passed through the observable functions
        The reason is that my original approach that passed the state on directly caused a memory leak. 
        I'm not sure if it was due to my original code, or an issue with the library, but this a simple alternative that means the code is nearly the same,
        with a little local mutability.
        
        Also, reference cell is used instead of a mutable value because mutable values cannot be captured by lambdas. 
        For a longer explaination see: https://lorgonblog.wordpress.com/2008/11/12/on-lambdas-capture-and-mutability/ 
        Also msdn reference : https://msdn.microsoft.com/en-us/library/dd233186.aspx*)
    let currentGameState = ref Domain.initialState

    let updateFrame (state :GameState) =
        match state.Running with 
        | Continue -> moveShip(state)
        | Stop -> game.Exit()

    use updateGameStateSub = 
        game.KeyDown
        |> Observable.map keyDown
        |> Observable.scan updateGameState Domain.initialState 
        |> Observable.subscribe (fun state -> currentGameState := state)

    use renderFrameSub = 
        game.RenderFrame
        |> Observable.subscribe(fun _ -> renderFrame !currentGameState)

    use updateFrameSub = 
        game.UpdateFrame
        |> Observable.subscribe(fun _ -> updateFrame !currentGameState)
        
    game.Run(60.0)

    0 
