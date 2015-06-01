module Window

open OpenTK
open OpenTK.Graphics

let game = new GameWindow(800, 600, GraphicsMode.Default, "Asteroids")
let aspectRatio = float game.Width / float game.Height;
