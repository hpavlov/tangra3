#load "OsdFrame.fs"
open Tangra.Functional 
open System.IO

let loadSingleLineFrame (rdr:BinaryReader) (top:int16) = 
    let bottom = rdr.ReadDecimal()
    let width = rdr.ReadInt32()
    let height = rdr.ReadInt32()
    let pix = [| for i in 1 .. width * height -> rdr.ReadUInt32() |]
    OsdFrame(width, height, [|1, 2|] |> Array.map (fun x -> (decimal top, bottom)), pix)

let loadMutipleLineFrame (rdr:BinaryReader) =
    let width = rdr.ReadInt32()
    let height = rdr.ReadInt32()
    let lines = rdr.ReadInt32()
    let osdLines = [| for i in 1 .. lines -> i |] |> Array.map (fun x -> rdr.ReadDecimal(),rdr.ReadDecimal())    
    let pix = [| for i in 1 .. width * height -> rdr.ReadUInt32() |]
    OsdFrame(width, height, osdLines, pix)   

let loadFrame (filePath : string) =
    let fs = new FileStream(filePath, FileMode.Open)
    let rdr = new BinaryReader(fs)
    let topOrMagic = rdr.ReadInt16()
    if topOrMagic = 17955s
    then loadMutipleLineFrame rdr
    else loadSingleLineFrame rdr topOrMagic
    

let frames path fromIdx toIdx = 
    [| for i in fromIdx .. toIdx -> loadFrame (path + i.ToString() + ".dat") |]

let trainSet = frames @"C:\Work\Tangra3\OcrTester\AutomatedTestingImages\PixelExport\" 1 10
let validateSet = frames @"C:\Work\Tangra3\OcrTester\AutomatedTestingImages\PixelExport\" 11 26