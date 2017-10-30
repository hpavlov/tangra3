namespace Tangra.Functional

type SubPixelCalc (width, height, pixels: uint32[]) =
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

type OsdFrame (width, height, top, bottom, pixels: uint32[]) =
    member this.Width = width
    member this.Height = height
    member this.Top = top
    member this.Bottom = bottom
    member this.Pixels = pixels
    member this.Calc = SubPixelCalc (width, height, pixels)

    member this.MinFullnessPerRow = (bottom - top) / 10M     

    member private this.IsOn x = 
        let s = seq { for y in this.Top .. this.Bottom -> (this.Calc.At x y) } |> Seq.sum
        let on = s > this.MinFullnessPerRow 
        on

    member private this.OffOnTransition x =
        let transition = 
            if this.IsOn x 
            then 
                let trOff = seq { for i in 1M .. 20M -> x - i/10M } |> Seq.tryFind (fun x -> not (this.IsOn x))
                if trOff.IsSome then trOff.Value + 0.1M else x - 2M
            else 
                let trOn = seq { for i in 1M .. 20M -> x + i/10M } |> Seq.tryFind (fun x -> this.IsOn x)
                if trOn.IsSome then trOn.Value else x + 2M
        transition

    member private this.OffOnTransitionDelta x =
        let transition = this.OffOnTransition x
        abs (transition - x)

    member private this.WidthCost left width = 
        let boxIdx = [| for i in 0M .. 15M -> i |]
        let cost = boxIdx |> Array.map (fun id -> this.OffOnTransitionDelta (left + id * width)) |> Array.sum
        cost

    member this.FindLeft minLeft =
        let leftInt = seq { for i in minLeft .. this.Width -> i }
        let lR = leftInt |> Seq.find (fun x -> this.IsOn (decimal x))
        let leftPrec = seq { for i in -10M .. 10M -> i/10M + (decimal lR) }
        let lP = leftPrec |> Seq.find (fun x -> this.IsOn (decimal x))
        lP

    member this.FindWidth left approximateWidth =
        let widthProbe = [| for i in -20M .. 20M -> i/10M + approximateWidth |]
        let costs = widthProbe |> Array.map (fun w -> (w, this.WidthCost left w))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    member private this.Combinations xs ys = [|
        for x in xs do
        for y in ys do
        yield (x, y) |]

    member this.FindLeftWidth approxLeft approxWidth =
        let rndApproxLeft = round(approxLeft * 100M) / 100M
        let rndApproxWidth = round(approxWidth * 100M) / 100M
        let widthProbe = [| for i in -20M .. 20M -> (i * 1M)/10M + rndApproxWidth |]
        let leftProbe = [| for i in -8M .. 8M -> (i * 2.5M)/10M + rndApproxLeft |]
        let probes = this.Combinations leftProbe widthProbe
        let costs = probes |> Array.map (fun p -> (p, this.WidthCost (fst p) (snd p)))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    member this.ImproveLeft approxLeft width =
        let rndApproxLeft = round(approxLeft * 100M) / 100M
        let leftProbe = [| for i in -20M .. 20M -> i/10M + rndApproxLeft |]
        let costs = leftProbe |> Array.map (fun l -> (l, this.WidthCost l width))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    static member FindBestLeft (frames: OsdFrame[]) =
        frames |> Array.map (fun frame -> frame.FindLeft 25) |> Array.average