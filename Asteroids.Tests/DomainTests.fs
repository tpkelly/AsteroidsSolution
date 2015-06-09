module DomainTests

open NUnit.Framework
open FsUnit
open Domain

[<Test>]
let ``The ship starts in the center``() = 
    initialState.Ship.Position.X |> should equal 0
    initialState.Ship.Position.Y |> should equal 0

[<Test>]
let ``The ship is not moving initially``() =
    initialState.Ship.Velocity.Magnitude |> should equal 0

[<Test>]
let ``The ship is facing north initially``() =
    initialState.Ship.Velocity.Trajectory |> should equal 0

[<Test>]
let ``The game is running initially``() =
    initialState.Running |> should equal Continue