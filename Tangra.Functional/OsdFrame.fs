namespace Tangra.Functional

open System

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

    let IsOnEx x factor = 
        let total = [| for i in 0 .. NumOsdLines - 1 -> i |] |> Array.map (fun l -> sumForLineVert l x) |> Array.sum 
        total > (decimal NumOsdLines) * AverageHeight / factor

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

    let combine2 xs ys = [|
        for x in xs do
        for y in ys do
        yield (x, y) |]

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

    let widthProbeAll = [| for i in -40M .. 40M -> i/20M + ApproximateWidth |]

    // Only take those width probes for which (right - left) mod width is less than 15% of the width
    let widthProbe = 
            widthProbeAll
            |> Array.filter (fun w -> (rights |> Array.map (fun x -> (x - left) % w) |> Array.exists ( fun e ->  e < 0.15M * w || e > 0.85M * w)))
            |> Array.map (fun x -> Math.Round(x, 2))

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

    member private this.OffOffTransCostAllLines x =
        let transition =
            if IsOn x 
            then 4M, 0M
            else 
                let trOnNext = seq { for i in 1M .. 20M -> x + i/10M } |> Seq.tryFind (fun x -> this.IsOnAllLines x)
                let trOnPrev = seq { for i in 1M .. 20M -> x - 1M * i/10M } |> Seq.tryFind (fun x -> this.IsOnAllLines x)
                if trOnNext.IsSome && trOnPrev.IsSome
                then Math.Min(abs(trOnNext.Value - x), abs(trOnPrev.Value - x)), abs(abs(trOnNext.Value - x) - abs(trOnPrev.Value - x))
                else 
                    if trOnNext.IsSome 
                    then trOnNext.Value, 0M
                    else
                        if trOnPrev.IsSome 
                        then trOnPrev.Value, 0M
                        else 0M, 0M
        if fst transition > 2M
        then 0M, false
        else abs( fst transition - 0.5M ), true
                       
    member private this.ContinuityCalcAllLines x w f = 
        let loc = [| for i in 0M .. round(w) * 2M -> x + i * 0.5M |]
        let startOn = IsOnEx x f
        let mutable currOn = startOn
        let mutable interruptions = 0M
        for l in loc do
            let on = IsOnEx l f
            if not (on = currOn)
            then
                currOn <- on
                interruptions <- interruptions + 1M
        if not currOn && interruptions = 0M
        then 0M, false
        else 
            if not startOn 
            then Math.Max(0M, interruptions - 2M), true // Off -> On -> Off should not be counted as interruption
            else Math.Max(0M, interruptions - 1M), true // On -> Off shold not be counted as interruption      

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

    member this.WidthCostAllLinesL2R (left:decimal) (width:decimal) =         
        let maxId = rights |> Array.map (fun x -> int (ceil (x - left) /width)) |> Array.max
        let boxIdx = [| for i in 0 .. maxId -> decimal i |]
        let cost = 
            boxIdx 
            |> Array.map (fun id -> (this.OffOnLeftTransCostAllLines (left + id * width) 1M)) 
            |> Array.filter (fun x -> snd x)
            |> Array.map (fun x -> fst x)
        let len = decimal (cost |> Array.length)
        let costLeft = (cost |> Array.sum) / len
        costLeft

    member this.WidthCostAllLinesR2L (left:decimal) (width:decimal) =         
        let maxId = rights |> Array.map (fun x -> int (ceil (x - left) /width)) |> Array.max
        let boxIdx = [| for i in 0 .. maxId -> decimal i |]
        let cost2 = 
            boxIdx 
            |> Array.map (fun id -> (this.OffOnLeftTransCostAllLines (left + (id + 1M) * width - 0.5M) -1M)) 
            |> Array.filter (fun x -> snd x)
            |> Array.map (fun x -> fst x)
        let len2 = decimal (cost2 |> Array.length)
        let costRight = (cost2 |> Array.sum) / len2
        costRight

    member this.WidthCostAllLines (left:decimal) (width:decimal) =         
        this.WidthCostAllLinesL2R left width + this.WidthCostAllLinesR2L left width 

    member this.GapCostAllLines (left:decimal) (width:decimal) =
        let maxId = rights |> Array.map (fun x -> int (ceil (x - left) /width)) |> Array.max
        let boxIdx = [| for i in 0 .. maxId -> decimal i |]
        let cost = 
            boxIdx 
            |> Array.map (fun id -> (this.OffOffTransCostAllLines (left + id * width))) 
            |> Array.filter (fun x -> snd x)
            |> Array.map (fun x -> fst x)
        let len = decimal (cost |> Array.length)
        let costLeft = 
            if len > 0M
            then (cost |> Array.sum) / len
            else 0M
        costLeft

    member this.ContinuityCostAllLinesImpl (left:decimal) (width:decimal) f =
        let maxId = rights |> Array.map (fun x -> int (ceil (x - left) /width)) |> Array.max
        let boxIdx = [| for i in 0 .. maxId -> decimal i |]
        let cost = 
            boxIdx 
            |> Array.map (fun id -> (this.ContinuityCalcAllLines (left + id * width) width f)) 
            |> Array.filter (fun x -> snd x)
            |> Array.map (fun x -> fst x)
        let len = decimal (cost |> Array.length)
        let totalCost = cost |> Array.sum
        totalCost

    member this.ContinuityCostAllLines x w = 
        this.ContinuityCostAllLinesImpl x w 10M


    member this.WidthCostAllLinesCG (left:decimal) (width:decimal) =         
        this.ContinuityCostAllLines left width + this.GapCostAllLines left width 

    member this.StartingWidthGen (costFun:CostDelegate) = 
        let probeCosts = 
            widthProbeAll
            |> Array.map (fun w -> Convert.ToInt32((rights.[0] - left) / w) )
            |> Seq.distinct |> Array.ofSeq
            |> Array.map (fun i ->  (rights.[0] - left) / (decimal i) )
            |> Array.map (fun x -> x,(costFun.Invoke(this.Left, x)) )
        let best = probeCosts |> Array.minBy (fun x -> snd x)
        Math.Round(fst best, 2)

    member this.StartingWidth =
            let costDel = new CostDelegate(this.WidthCostAllLines)
            this.StartingWidthGen costDel

    member this.StartingWidthFine =
            let coarseWidth = this.StartingWidth
            let costs = 
                [| for i in -20M .. 20M -> coarseWidth + i / 10M |]
                |> Seq.map (fun w -> w,(this.ContinuityCostAllLines left w + this.GapCostAllLines left w ) )
            let bestFit = costs |> Seq.minBy (fun x -> snd x)
            let fineW = fst bestFit
            fineW
    
    member this.StartingWidthRL =
            let costDel = new CostDelegate(this.WidthCostAllLinesR2L)
            this.StartingWidthGen costDel

    member this.StartingWidthG =
            let costDel = new CostDelegate(this.GapCostAllLines)
            this.StartingWidthGen costDel

    member this.StartingWidthC = 
            let costDel = new CostDelegate(this.ContinuityCostAllLines)
            this.StartingWidthGen costDel

    member this.FindWidth left =
        let costs = widthProbe |> Array.map (fun w -> (w, this.WidthCost left w))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    member private this.FitLeftWidthEx (startingWidth:decimal) (costFun:CostDelegate) = 
        //let leftProbes = [| for i in -2M .. 2M -> i + left|]   // 1.00 intervals 
        let leftProbes = [| for i in -10M .. 10M -> i/5M + left|]   // 0.20 intervals 
        //let leftProbes = [| for i in -10M .. 10M -> i/5M + left|]   // 0.25 intervals
        //let leftProbes = [| for i in -5M .. 5M -> i/2M + left|]    // 0.50 intervals
        //let widthProbes = [| for i in -2M .. 2M -> i/10M + startingWidth |] // [-0.2, 0.2], 0.1 intervals
        //let widthProbes = [| for i in -5M .. 5M -> i/10M + startingWidth |] // [-0.5, 0.5], 0.1 intervals
        let widthProbes = [| for i in -6M .. 6M -> i/20M + startingWidth |] // [-0.3, 0.3], 0.05 intervals
        let probes = combine2 leftProbes widthProbes
        let costs = probes |> Array.map (fun p -> (p, costFun.Invoke( (fst p), (snd p) ) ))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    member private this.FitLeftWidthImpl (costFun:CostDelegate) = 
        let startingWidth = this.StartingWidthGen costFun
        this.FitLeftWidthEx startingWidth costFun

    member this.FitLeftWidth =
        let costDel = new CostDelegate(this.WidthCostAllLinesCG)
        let startWidth = this.StartingWidth
        this.FitLeftWidthEx startWidth costDel

    member this.FitLeftWidthRL =
        let costDel = new CostDelegate(this.WidthCostAllLinesR2L)
        this.FitLeftWidthImpl costDel

    member this.FitLeftWidthG =
        let costDel = new CostDelegate(this.GapCostAllLines)
        this.FitLeftWidthImpl costDel

    member this.FitLeftWidthC =
        let costDel = new CostDelegate(this.ContinuityCostAllLines)
        this.FitLeftWidthImpl costDel

//
//    member this.FindWidthAllLines left =
//        let costs = 
//                widthProbe                 
//                |> Array.map (fun w -> (w, this.WidthCostAllLines left w))
//        let bestFits = costs |> Array.sortBy (fun x -> snd x)
//        bestFits |> Array.map (fun w -> this.AdjustLeft left w)
//        fst bestFits

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

//    member this.ImproveLeftAllLines approxLeft width =
//        let rndApproxLeft = round(approxLeft * 100M) / 100M
//        let leftProbe = [| for i in -20M .. 20M -> i/10M + rndApproxLeft |]
//        let costs = leftProbe |> Array.map (fun l -> (l, this.WidthCostAllLines l width))
//        let bestFit = costs |> Array.minBy (fun x -> snd x)
//        fst bestFit

//    static member FindBestLeft (frames: OsdFrame[]) =
//        frames |> Array.map (fun frame -> frame.FindLeft) |> Array.average