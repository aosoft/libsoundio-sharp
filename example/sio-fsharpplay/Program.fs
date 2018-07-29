open System
open System.Linq
open SoundIOSharp.Extensions
open SoundIOSharp.Extensions

let sampleRate = 48000
let pitch = 440.0


[<EntryPoint>]
let main argv =
    let s =
        [0..sampleRate * 2]
        |> List.map(fun x -> (float32)(Math.Sin(2.0 * Math.PI * pitch * (float)x / (float)sampleRate)))
    s.PlaySound(sampleRate)
    0 // return an integer exit code
