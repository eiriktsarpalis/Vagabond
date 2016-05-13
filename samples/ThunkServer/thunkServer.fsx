(**
A Vagabond Demo: ThunkServer

This script offers a demo of the functionality of Vagabond, through an ad-hoc application.
ThunkServer, as its name suggests, is a server that receives and executes arbitrary thunks,
that is functions of type unit -> 'T. ThunkServer uses the Vagabond library to correctly
resolve and submit code dependencies, even if those happen to be dynamic assemblies as
is the case with F# Interactive.

The actual implementation of ThunkServer is a straightforward 100 lines of code.
Dependency resolution and exportation logic is handled transparently by Vagabond
**)

#I "../../bin/"

#r "FsPickler.dll"
#r "ThunkServer.exe"

open ThunkServer

// initialize & test a local instance
ThunkClient.Executable <- __SOURCE_DIRECTORY__ + "/../../bin/ThunkServer.exe"
let client = ThunkClient.InitLocal()

// Example 1: Hello World








let foo() = async {
    let! results = Async.Parallel [for i in 1 .. 10 -> async { printfn "Entry %d" i }]
    return ()
}

client.EvaluateThunk(fun () -> Async.RunSynchronously(foo()))






// Example 2: Deploy an actor

#r "Thespian.dll"
open Nessos.Thespian



type Counter =
    | Increment of int
    | GetCount of IReplyChannel<int>



let rec counter state (self : Actor<Counter>) = async {
    let! msg = self.Receive ()
    match msg with
    | Increment i -> 
        printfn "Increment by %d" i
        return! counter (i + state) self
    | GetCount rc ->
        do! rc.Reply state
        return! counter state self
}



let ref = client.EvaluateThunk(fun () ->
    printfn "deploying actor..."
    let actor = counter 0 |> Actor.bind |> Actor.Publish
    actor.Ref)

ref <-- Increment 1
ref <-- Increment 2
ref <-- Increment 3
ref <!= GetCount