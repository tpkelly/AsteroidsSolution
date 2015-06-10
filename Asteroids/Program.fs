module Main

open System

open OpenTK.Input

open Window
open Geometry
open Domain
open Physics
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
    
    let accelerationPerFrame = 0.01
    let rotationPerFrame = 0.1

    // Keyboard state takes a while to set up first time, so give it a kick before we receive user input
    ignore (Keyboard.GetState())

    let keyResult (key: Key, inverseKey: Key, amount: float) : float =
        let keyDown = Keyboard.GetState().IsKeyDown(key)
        let inverseKeyDown = Keyboard.GetState().IsKeyDown(inverseKey)
        if (keyDown = inverseKeyDown) then 0.0
        else if (keyDown) then amount else -amount
            

    //Handle keydownEvents and transform them into state changes 
    //Hint (To get the best behaviour, you may need to deal with key up, etc events)
    let keyChange (args: KeyboardKeyEventArgs) =
        match args.Key with
        | Key.Escape ->  UserStateChange.EndGame
        | Key.Up -> UserStateChange.Accelerate (keyResult(Key.Up, Key.Down, accelerationPerFrame))
        | Key.Down -> UserStateChange.Accelerate (keyResult(Key.Down, Key.Up, -accelerationPerFrame))
        | Key.Left -> UserStateChange.RotateDirection (keyResult(Key.Left, Key.Right, rotationPerFrame))
        | Key.Right -> UserStateChange.RotateDirection (keyResult(Key.Right, Key.Left, -rotationPerFrame))
        | _ -> UserStateChange.NoChange

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
        | Continue -> state.Ship <- moveShip(state)
        | Stop -> game.Exit()

    use updateGameStateKeyDownSub = 
        Event.merge game.KeyDown game.KeyUp
        |> Observable.map keyChange
        |> Observable.scan updateGameState Domain.initialState
        |> Observable.subscribe (fun state -> currentGameState := state)

    use renderFrameSub = 
        game.RenderFrame
        |> Observable.subscribe(fun _ -> renderFrame !currentGameState)

    use updateFrameSub = 
        game.UpdateFrame
        |> Observable.subscribe(fun _ -> updateFrame !currentGameState )
        
    game.Run(30.0)

    0 
