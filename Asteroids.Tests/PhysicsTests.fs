module PhysicsTests

open System
open NUnit.Framework
open FsUnit
open Window
open Domain
open Physics

[<Test>]
let ``NoChange leaves the ship as is``() = 
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.3; Trajectory = 1.2}; Delta = { Magnitude = 0.0; Trajectory = 0.0 } }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    let change = NoChange
    
    // When
    let newState = updateGameState dummyState change

    // Then
    newState.Ship.Position.X |> should equal 0.5
    newState.Ship.Position.Y |> should equal 0.5
    newState.Ship.Velocity.Magnitude |> should equal 0.3
    newState.Ship.Velocity.Trajectory |> should equal 1.2
    newState.Running |> should equal Continue

[<Test>]
let ``EndGame stops the game`` () =
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.3; Trajectory = 1.2}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }}
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    let change = EndGame
    
    // When
    let newState = updateGameState dummyState change

    // Then
    newState.Running |> should equal Stop

[<Test>]
let ``Accelerate increases the velocity of the ship in the same direction`` () =
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.3; Trajectory = 1.2}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    let change = Accelerate 0.05
    
    // When
    let newState = updateGameState dummyState change

    // Then
    newState.Ship.Position.X |> should equal 0.5
    newState.Ship.Position.Y |> should equal 0.5
    newState.Ship.Velocity.Magnitude |> should equal 0.35
    newState.Ship.Velocity.Trajectory |> should equal 1.2
    newState.Running |> should equal Continue

[<Test>]
let ``Negative acceleration slows the ship enough to go in reverse in the opposite direction`` () =
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.3; Trajectory = 1.2}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    let change = Accelerate -0.5
    
    // When
    let newState = updateGameState dummyState change

    // Then
    newState.Ship.Position.X |> should equal 0.5
    newState.Ship.Position.Y |> should equal 0.5
    newState.Ship.Velocity.Magnitude |> should equal -0.2
    newState.Ship.Velocity.Trajectory |> should equal 1.2
    newState.Running |> should equal Continue

[<Test>]
let ``Positive rotation turns the ship to the right`` () =
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.3; Trajectory = 1.2}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    let change = RotateDirection 0.1
    
    // When
    let newState = updateGameState dummyState change

    // Then
    newState.Ship.Position.X |> should equal 0.5
    newState.Ship.Position.Y |> should equal 0.5
    newState.Ship.Velocity.Magnitude |> should equal 0.3
    newState.Ship.Velocity.Trajectory |> should equal 1.3
    newState.Running |> should equal Continue

[<Test>]
let ``Negative rotation turns the ship to the right`` () =
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.3; Trajectory = 1.0}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    let change = RotateDirection -10.0
    
    // When
    let newState = updateGameState dummyState change

    // Then
    newState.Ship.Position.X |> should equal 0.5
    newState.Ship.Position.Y |> should equal 0.5
    newState.Ship.Velocity.Magnitude |> should equal 0.3
    newState.Ship.Velocity.Trajectory |> should equal -9.0
    newState.Running |> should equal Continue

[<Test>]
let ``Moving the ship continues the game`` () =
    // Given
    let dummyShip = { Position = {X = 0.0; Y = 0.0;}; Velocity = {Magnitude = 0.5; Trajectory = 0.0 }; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Running |> should equal Continue

[<Test>]
let ``Moving the ship foes not change the velocity`` () =
    // Given
    let dummyShip = { Position = {X = 0.0; Y = 0.0;}; Velocity = {Magnitude = 0.5; Trajectory = Math.PI / 3.0 }; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyShip.Velocity.Magnitude |> should equal 0.5
    dummyShip.Velocity.Trajectory |> should equal (Math.PI / 3.0)

[<Test>]
let ``Updating initial positions moves the ship`` () =
    // Given
    let dummyShip = { Position = {X = 0.0; Y = 0.0;}; Velocity = {Magnitude = 0.5; Trajectory = 0.0 }; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should equal 0.0
    dummyState.Ship.Position.Y |> should equal 0.5

[<Test>]
let ``Updating positions moves the ship`` () =
    // Given
    let dummyShip = { Position = {X = 0.5; Y = 0.5;}; Velocity = {Magnitude = 0.5; Trajectory = Math.PI / 2.0}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should equal 1.0
    dummyState.Ship.Position.Y |> should equal 0.5

[<Test>]
let ``Updating positions with ship in reverse moves the ship backwards`` () =
    // Given
    let dummyShip = { Position = {X = 1.0; Y = 1.0;}; Velocity = {Magnitude = -2.0; Trajectory = Math.PI / 4.0}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should (equalWithin 0.00001) (1.0 - Math.Sqrt(2.0))
    dummyState.Ship.Position.Y |> should (equalWithin 0.00001) (1.0 - Math.Sqrt(2.0))

[<Test>]
let ``Updating positions wraps the ship around X positively`` () =
    // Given
    let dummyShip = { Position = {X = 2.0 * aspectRatio - 0.1; Y = 1.0;}; Velocity = {Magnitude = 0.2; Trajectory = Math.PI / 2.0}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should equal (-2.0 * aspectRatio)
    dummyState.Ship.Position.Y |> should equal 1.0

[<Test>]
let ``Updating positions wraps the ship around X negatively`` () =
    // Given
    let dummyShip = { Position = {X = -2.0 * aspectRatio + 0.1; Y = 1.0;}; Velocity = {Magnitude = -0.2; Trajectory = Math.PI / 2.0}; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should equal (2.0 * aspectRatio)
    dummyState.Ship.Position.Y |> should equal 1.0

[<Test>]
let ``Updating positions wraps the ship around Y positively`` () =
    // Given
    let dummyShip = { Position = {X = 1.0; Y = 1.9;}; Velocity = {Magnitude = 0.2; Trajectory = 0.0 }; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should equal 1.0
    dummyState.Ship.Position.Y |> should equal -2.0

[<Test>]
let ``Updating positions wraps the ship around Y negatively`` () =
    // Given
    let dummyShip = { Position = {X = 1.0; Y = -1.9;}; Velocity = {Magnitude = -0.2; Trajectory = 0.0 }; Delta = { Magnitude = 0.0; Trajectory = 0.0 }  }
    let dummyState = { Ship = dummyShip; Running = Continue; Asteroids = [] }
    
    // When
    moveShip dummyState

    // Then
    dummyState.Ship.Position.X |> should equal 1.0
    dummyState.Ship.Position.Y |> should equal 2.0
