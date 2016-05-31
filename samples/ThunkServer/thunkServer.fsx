// BUG REPRODUCTION STEPS
// Please run each of the provided steps in distinct interactions,
// as opposed to evaluating the entire script

// Step 1: Init a "thunk server"

#I "../../bin/"

#r "FsPickler.dll"
#r "Thespian.dll"
#r "ThunkServer.exe"

open ThunkServer

// initialize & test a local instance
ThunkClient.Executable <- __SOURCE_DIRECTORY__ + "/../../bin/ThunkServer.exe"
let client = ThunkClient.InitLocal()

// Step 2: create a top-level value in fsi

let x = client.EvaluateThunk (fun () -> 1 + 1)

// Step 3: create a computation that captures the value in a quotation literal

client.EvaluateThunk (fun () -> <@ x @>) // works in Visual F#/Windows, fails in FSCS and open source fsi/mono