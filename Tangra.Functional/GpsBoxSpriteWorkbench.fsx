#I @"..\packages"
#load @"FSharp.Charting.0.91.1\FSharp.Charting.fsx"
#load "SubPixelCalc.fs"

open Tangra.Functional 

#load "OsdLocator.fs"

open Tangra.Functional 
open System.IO
open System

open FSharp.Charting

let loadSingleLineFrame (rdr:BinaryReader) (top:int16) = 
    let bottom = rdr.ReadDecimal()
    let width = rdr.ReadInt32()
    let height = rdr.ReadInt32()
    let pix = [| for i in 1 .. width * height -> rdr.ReadUInt32() |]
    OsdLocator(width, height, [|1, 2|] |> Array.map (fun x -> (decimal top, bottom)), pix)

let loadMutipleLineFrame (rdr:BinaryReader) =
    let width = rdr.ReadInt32()
    let height = rdr.ReadInt32()
    let lines = rdr.ReadInt32()
    let osdLines = [| for i in 1 .. lines -> i |] |> Array.map (fun x -> rdr.ReadDecimal(),rdr.ReadDecimal())    
    let pix = [| for i in 1 .. width * height -> rdr.ReadUInt32() |]
    OsdLocator(width, height, osdLines, pix)   

let loadFrame (filePath : string) =
    let fs = new FileStream(filePath, FileMode.Open)
    let rdr = new BinaryReader(fs)
    let topOrMagic = rdr.ReadInt16()
    if topOrMagic = 17955s
    then loadMutipleLineFrame rdr
    else loadSingleLineFrame rdr topOrMagic
    

let frames path fromIdx toIdx = 
    [| for i in fromIdx .. toIdx -> loadFrame (path + i.ToString() + ".dat") |]

let trainSet = frames @"C:\Work\Tangra3\OcrTester\AutomatedTestingImages\PixelExport\" 0 26
//let validateSet = frames @"C:\Work\Tangra3\OcrTester\AutomatedTestingImages\PixelExport\" 0 26

//let points = trainSet |> Array.mapi (fun i x -> (float i, float (x.FindWidth x.Left)))
let points = 
    trainSet
    //|> Array.mapi (fun i x -> i, x.StartingWidth 0 0) 
    |> Array.mapi (fun i x -> i, x.Left) 
    //|> Array.map (fun x -> x, x.Left) 
    |> Array.map (fun x -> (fst x, snd x)) 
points |> Chart.Line

let testStartingWidth (propFun:OsdLocator -> decimal) = 
    [| for i in 0 .. 26 -> i |]
    |> Array.map (fun i -> i,propFun trainSet.[i])
    |> Seq.groupBy snd
    |> Seq.map (fun (k, v) -> k, v |> Seq.length)
    |> Array.ofSeq

let compareStartingWidth (propFun1:OsdLocator -> decimal) (propFun2:OsdLocator -> decimal) = 
    [| for i in 0 .. 26 -> i |]
    |> Array.map (fun i -> printfn "%i %f %f" i (propFun1 trainSet.[i]) (propFun2 trainSet.[i]))

(*
let compare st en = 
    let data = 
        [| for i in st .. en -> i |]
        |> Seq.map (fun i -> (trainSet.[i].FitLeftWidth |> snd,trainSet.[i].FitLeftWidthG|> snd), (trainSet.[i].StartingWidth,trainSet.[i].StartingWidthG))
    for x in data do
        printfn "(%f %f) (%f %f)" (fst (fst x)) (fst (snd x)) (snd (fst x)) (snd (snd x))

let compare2 st en = 
    let data = 
        [| for i in st .. en -> i |]
        |> Seq.map (fun i -> i,trainSet.[i].FitLeftWidthG)
        |> Seq.map (fun x -> (snd (snd x)), (trainSet.[(fst x)].ContinuityCostAllLines (fst (snd x)) (snd (snd x))))
    for x in data do
        printfn "%f %f" (fst x) (snd x)

let compare3 id = 
    let lw = trainSet.[id].FitLeftWidthG
    let data = 
        [| for i in -20M .. 20M -> snd lw + i / 10M |]
        |> Seq.map (fun w -> w,(trainSet.[id].ContinuityCostAllLines (fst lw) w, trainSet.[id].GapCostAllLines (fst lw) w, trainSet.[id].WidthCostAllLines (fst lw) w) )
        
    printfn "Frame:%d; Fitted Left:%f Width:%f" id (fst lw) (snd lw)
    for x in data do
        let cc,_,_ = snd x
        let _,gc,_ = snd x
        let _,_,wc = snd x
        printfn "%f %0.2f %0.2f %0.2f" (fst x) cc gc wc

let computeLF id = 
    let stW = trainSet.[id].StartingWidth
    let stL = trainSet.[id].Left
    let costs = 
        [| for i in -20M .. 20M -> stW + i / 10M |]
        |> Seq.map (fun w -> w,(trainSet.[id].ContinuityCostAllLines stL w + trainSet.[id].GapCostAllLines stL w ) )
    let bestFit = costs |> Seq.minBy (fun x -> snd x)
    let fineW = fst bestFit
    fineW
    //let startingWidth = this.StartingWidthGen costFun
    //this.FitLeftWidthEx

let computeLFAll st en = 
    let data = 
        [| for i in st .. en -> i |]
        |> Seq.map (fun i -> i,computeLF i)
    for x in data do
        printfn "%d %f" (fst x) (snd x)
*)    