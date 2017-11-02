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

type OsdFrame (width, height, topBottoms:(decimal*decimal)[], pixels: uint32[]) =

    let Calc = SubPixelCalc (width, height, pixels)
    let MinOsdEdgeGap = int (40M * decimal width / 720M)
    
    let NumOsdLines = topBottoms |> Array.length
    let AverageHeight = topBottoms |> Array.averageBy (fun x -> (snd x - fst x))
    let MinFullnessPerRow = AverageHeight / 10M
    let MinFullnessAllRows = decimal NumOsdLines * AverageHeight / 10M
    let ApproximateWidth = 13.0M * AverageHeight / 32.0M

    let sumForLineVert line vert =
        seq { for y in fst topBottoms.[line] .. snd topBottoms.[line] -> (Calc.At vert y) } 
        |> Seq.sum

    let IsOn x = 
        let total = [| for i in 0 .. NumOsdLines - 1 -> i |] |> Array.map (fun l -> sumForLineVert l x) |> Array.sum 
        total > MinFullnessAllRows

    let SingleLineVSum line x = 
        seq { for y in fst topBottoms.[line] .. snd topBottoms.[line] -> (Calc.At x y) } 
        |> Seq.sum

    let IsOnSingleLine line x = 
        let on = SingleLineVSum line x > MinFullnessPerRow 
        on

    let left = 
        let leftInt = seq { for i in MinOsdEdgeGap .. width -> i }
        let lR = leftInt |> Seq.find (fun x -> IsOn (decimal x))
        let leftPrec = seq { for i in -10M .. 10M -> i/10M + (decimal lR) }
        let lP = leftPrec |> Seq.find (fun x -> IsOn (decimal x))
        lP


    let rights = 
        let Density line x1 x2 = 
            let sum = seq { for i in x1 .. x2 -> i }
                        |> Seq.map (fun x -> SingleLineVSum line x)
                        |> Seq.sum
            sum / round ((x2 - x1) / ApproximateWidth)

        let FindRight line =
            let markerCalcArea = ApproximateWidth * 5M
            let marker = Density line left (left + markerCalcArea)
            let MarkerPass x = 
                let rm = Density line (x - markerCalcArea) x
                rm > marker * 0.33M
            let rightInt = seq { for i = width - MinOsdEdgeGap downto 0 do yield i }
            let rR = rightInt |> Seq.find (fun x -> IsOnSingleLine line (decimal x) && MarkerPass (decimal x))
            let rightPrec = seq { for i = 10 downto -10 do yield decimal i/10M + (decimal rR) }
            let rP = rightPrec |> Seq.find (fun x -> IsOnSingleLine line (decimal x))
            rP
        [| for i in 0 .. NumOsdLines - 1 -> i |] 
        |> Array.map (fun x -> FindRight x)

    let widthProbe = [| for i in -20M .. 20M -> i/10M + ApproximateWidth |]

    member this.Width = width
    member this.Height = height
    member this.TopBottoms = topBottoms
    member this.Pixels = pixels
    member this.Left = left
    member this.Rights = rights

    member private this.IsOnAllLines x = 
        // NOTE: FindWidth returns the same value with IsOn and IsOnAllLines so this function is not totally wrong
        let sumForLine line =
            seq { for y in fst this.TopBottoms.[line] .. snd this.TopBottoms.[line] -> (Calc.At x y) } 
            |> Seq.sum
        let lineNos = 
            [| for i in 0 .. this.TopBottoms.Length - 1 -> i |] 
            |> Array.filter (fun l -> rights.[l] > x) 
        let total = lineNos |> Array.map (fun x -> sumForLine x) |> Array.sum 
        total > MinFullnessPerRow * decimal lineNos.Length

    member private this.OffOnTransitionCost x =
        let transition = 
            if IsOn x 
            then 
                let trOff = seq { for i in 1M .. 20M -> x - i/10M } |> Seq.tryFind (fun x -> not (IsOn x))
                if trOff.IsSome then trOff.Value + 0.1M else x + 2M
            else 
                let trOn = seq { for i in 1M .. 20M -> x + i/10M } |> Seq.tryFind (fun x -> IsOn x)
                if trOn.IsSome then trOn.Value else x + 2M
        abs (transition - x)


    member this.WidthCost left width = 
        let boxIdx = [| for i in 0M .. 15M -> i |]
        let cost = boxIdx |> Array.map (fun id -> this.OffOnTransitionCost (left + id * width)) |> Array.sum
        cost

    member private this.OffOnLeftTransCostAllLines x dir =
        let transition = 
            if IsOn x 
            then 
                let trOff = seq { for i in 1M .. 20M -> x - dir * i/10M } |> Seq.tryFind (fun x -> not (this.IsOnAllLines x))
                if trOff.IsSome 
                then trOff.Value + 0.1M, true
                else x + 8M, true
            else 
                let trOn = seq { for i in 1M .. 20M -> x + dir * i/10M } |> Seq.tryFind (fun x -> this.IsOnAllLines x)
                if trOn.IsSome 
                then trOn.Value, true 
                else
                    let trOnPrev = seq { for i in 1M .. 20M -> x - dir * i/10M } |> Seq.tryFind (fun x -> this.IsOnAllLines x)
                    if trOn.IsSome 
                    then x + 4M, true
                    else x, false // We are be in a 'gap', don't count it towards a cost
        abs (fst transition - x), snd transition

    member this.WidthCostAllLines (left:decimal) (width:decimal) =         
        let maxId = rights |> Array.map (fun x -> int (ceil (x - left) /width)) |> Array.max
        let boxIdx = [| for i in 0 .. maxId -> decimal i |]
        let cost = 
            boxIdx 
            |> Array.map (fun id -> (this.OffOnLeftTransCostAllLines (left + id * width) 1M)) 
            |> Array.filter (fun x -> snd x)
            |> Array.map (fun x -> fst x)
        let len = decimal (cost |> Array.length)
        let costLeft = (cost  |> Array.sum) / len
        let cost2 = 
            boxIdx 
            |> Array.map (fun id -> (this.OffOnLeftTransCostAllLines (left + (id + 1M) * width - 0.5M) -1M)) 
            |> Array.filter (fun x -> snd x)
            |> Array.map (fun x -> fst x)
        let len2 = decimal (cost2 |> Array.length)
        let costRight = (cost2  |> Array.sum) / len2
        costRight

    member this.FindWidth left =
        let costs = widthProbe |> Array.map (fun w -> (w, this.WidthCost left w))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    member this.FindWidthAllLines left =
        let costs = widthProbe |> Array.map (fun w -> (w, this.WidthCostAllLines left w))
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

    member this.ImproveLeftAllLines approxLeft width =
        let rndApproxLeft = round(approxLeft * 100M) / 100M
        let leftProbe = [| for i in -20M .. 20M -> i/10M + rndApproxLeft |]
        let costs = leftProbe |> Array.map (fun l -> (l, this.WidthCostAllLines l width))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

//    static member FindBestLeft (frames: OsdFrame[]) =
//        frames |> Array.map (fun frame -> frame.FindLeft) |> Array.average