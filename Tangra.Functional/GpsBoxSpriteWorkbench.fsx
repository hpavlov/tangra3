open System.IO
open System.Math

type SubPixelImage (width, height, pixels: uint32[]) =
    member this.Width = width
    member this.Height = height
    member this.Pixels = pixels
    member this.At (xf:decimal) (yf:decimal) = 
        let x = (int)(xf)
        let y = (int)(yf)
        let x1 = min (x + 1) (this.Width - 1)
        let y1 = min (y + 1) (this.Height - 1)

        let xFactor = (1M - (xf - (decimal)x))
        let x1Factor = (xf - (decimal)x)
        let yFactor = (1M - (yf - (decimal)y))
        let y1Factor = (yf - (decimal)y)

        let sum = (decimal)this.Pixels.[x + this.Width * y] * xFactor * yFactor + (decimal)this.Pixels.[x1 + this.Width * y] * x1Factor * yFactor + (decimal)this.Pixels.[x + this.Width * y1] * xFactor * y1Factor + (decimal)this.Pixels.[x1 + this.Width * y1] * x1Factor * y1Factor
        sum

type Frame = { Width: int; Height: int; Top: decimal; Bottom : decimal; Pixels : uint32[,]; Image: SubPixelImage }

let loadFrame (filePath : string) =
    let fs = new FileStream(filePath, FileMode.Open)
    let rdr = new BinaryReader(fs)
    let top = rdr.ReadDecimal()
    let bottom = rdr.ReadDecimal()
    let width = rdr.ReadInt32()
    let height = rdr.ReadInt32()
    let pix = [| for i in 1 .. width * height -> rdr.ReadUInt32() |]
    { Top = top; Bottom = bottom; Width = width; Height = height; Pixels = Array2D.init width height (fun x y -> pix.[y * width + x]); Image = SubPixelImage (width, height, pix) }


let frames path fromIdx toIdx = 
    [| for i in fromIdx .. toIdx -> loadFrame (path + i.ToString() + ".dat") |]

let trainSet = frames @"F:\WORK\tangra3\OcrTester\AutomatedTestingImages\PixelExport\" 1 10
let validateSet = frames @"F:\WORK\tangra3\OcrTester\AutomatedTestingImages\PixelExport\" 11 26

let isOn frame x =
    let s = [| for y in frame.Top .. frame.Bottom -> (frame.Image.At x y) |] |> Array.sum
    let on = s > (frame.Bottom - frame.Top) * 0.2M 
    on

let findLeft frame minLeft =
    let leftInt = [| for i in minLeft .. frame.Width -> i |]
    let lR = leftInt |> Seq.find (fun x -> isOn frame (decimal x))
    let leftPrec = [| for i in -10M .. 10M -> i/10M + (decimal lR) |]
    let lP = leftPrec |> Seq.find (fun x -> isOn frame (decimal x))
    lP

let leftPos = trainSet |> Array.map (fun frame -> findLeft frame 25) |> Array.average
let heightAve = trainSet |> Array.map (fun frame -> frame.Bottom - frame.Top) |> Array.average
let widthAve = 13.0M * heightAve / 32.0M; // NOTE: Nominal ratio of 13x32 from a sample image

let offOnTransition frame x =
    let transition = 
        if isOn frame x 
        then 
            let trOff = ([| for i in 1M .. 20M -> x - i/10M |] |> Seq.tryFind (fun x -> not (isOn frame x)))
            if trOff.IsSome then trOff.Value + 0.1M else x - 2M
        else 
            let trOn = [| for i in 1M .. 20M -> x + i/10M |] |> Seq.tryFind (fun x -> isOn frame x)
            if trOn.IsSome then trOn.Value else x + 2M
    transition

let offOnTransitionDelta frame x =
    let transition = offOnTransition frame x
    abs (transition - x)

let widthCost frame left width = 
    let boxIdx = [| for i in 0M .. 15M -> i |]
    let cost = boxIdx |> Array.map (fun id -> offOnTransitionDelta frame (left + id * width)) |> Array.sum
    cost

let combine2 xs ys = [|
    for x in xs do
    for y in ys do
    yield (x, y) |]

let findLeftWidth frame approxLeft approxWidth =
    let rndApproxLeft = round(approxLeft * 10M) / 10M
    let rndApproxWidth = round(approxWidth * 10M) / 10M
    let widthProbe = [| for i in -20M .. 20M -> i/10M + rndApproxWidth |]
    let leftProbe = [| for i in -20M .. 20M -> i/20M + rndApproxLeft |]
    let probes = combine2 leftProbe widthProbe
    let costs = probes |> Array.map (fun p -> (p, widthCost frame (fst p) (snd p)))
    let bestFit = costs |> Array.minBy (fun x -> snd x)
    fst bestFit

let findWidth frame approxLeft approxWidth =
    let rndApproxLeft = round(approxLeft * 10M) / 10M
    let rndApproxWidth = round(approxWidth * 10M) / 10M
    let widthProbe = [| for i in -20M .. 20M -> i/10M + rndApproxWidth |]
    let costs = widthProbe |> Array.map (fun w -> (w, widthCost frame rndApproxLeft w))
    let bestFit = costs |> Array.minBy (fun x -> snd x)
    rndApproxLeft, fst bestFit

let findSetWidth set left aveWidth =
    let widthProbe = [| for i in -20M .. 20M -> i/10M + aveWidth |]
    let costs = widthProbe |> Array.map (fun w -> (w, (set |> Array.map (fun f -> widthCost f left w) |> Array.sum)))
    let bestFit = costs |> Array.minBy (fun x -> snd x)
    fst bestFit
