namespace Tangra.Functional

    open System

    type OnCheckDelegate = delegate of int -> bool

    type OsdLocator (width, height, topBottoms:(decimal*decimal)[], pixels: uint32[]) =

        let calc = SubPixelCalc (width, height, pixels)
        let minOsdEdgeGap = int (40M * decimal width / 720M)
    
        let numOsdLines = topBottoms |> Seq.length
        let averageHeight = topBottoms |> Seq.averageBy (fun x -> (snd x - fst x))
        let minFullnessPerRow = averageHeight / 10M
        let minFullnessAllRows = decimal numOsdLines * averageHeight / 10M
        let approximateWidth = 13.0M * averageHeight / 32.0M

        let sumForLineVert line vert =
            seq { for y in fst topBottoms.[line] .. snd topBottoms.[line] -> (calc.At vert y) } 
            |> Seq.sum

        let isOnSingleLine line x = 
            let on = sumForLineVert line x > minFullnessPerRow 
            on

        let isOn x = 
            let total = [ for i in 0 .. numOsdLines - 1 -> i ] |> Seq.map (fun l -> sumForLineVert l x) |> Seq.sum 
            total > minFullnessAllRows

        let left = 
            let leftInt = seq { for i in minOsdEdgeGap .. width -> i }
            let lR = leftInt |> Seq.find (fun x -> isOn (decimal x))
            let leftPrec = seq { for i in -10M .. 10M -> i/10M + (decimal lR) }
            let lP = leftPrec |> Seq.find (fun x -> isOn (decimal x))
            lP

        let getCombination xs ys = [|
            for x in xs do
            for y in ys do
            yield (x, y) |]

        let rights = 
            let density line x1 x2 = 
                let sum = seq { for i in x1 .. x2 -> i }
                            |> Seq.map (fun x -> sumForLineVert line x)
                            |> Seq.sum
                sum / round ((x2 - x1) / approximateWidth)

            let findRight line =
                let markerCalcArea = approximateWidth * 5M
                let marker = density line left (left + markerCalcArea)
                let MarkerPass x = 
                    let rm = density line (x - markerCalcArea) x
                    rm > marker * 0.33M
                let rightInt = seq { for i = width - minOsdEdgeGap downto 0 do yield i }
                let rR = rightInt |> Seq.find (fun x -> isOnSingleLine line (decimal x) && MarkerPass (decimal x))
                let rightPrec = seq { for i = 10 downto -10 do yield decimal i/10M + (decimal rR) }
                let rP = rightPrec |> Seq.find (fun x -> isOnSingleLine line (decimal x))
                rP
            [| for i in 0 .. numOsdLines - 1 -> i |] 
            |> Array.map (fun x -> findRight x)

        let widthProbeAll = [| for i in -40M .. 40M -> i/20M + approximateWidth |]

        member this.NumOsdLines = 
            numOsdLines

        member this.Left = 
            left

        member this.Right line = 
            rights.[line]

        member private this.OffOnTransitionCost x =
            let transition = 
                if isOn x 
                then 
                    let trOff = seq { for i in 1M .. 20M -> x - i/10M } |> Seq.tryFind (fun x -> not (isOn x))
                    if trOff.IsSome then trOff.Value + 0.1M else x + 2M
                else 
                    let trOn = seq { for i in 1M .. 20M -> x + i/10M } |> Seq.tryFind (fun x -> isOn x)
                    if trOn.IsSome then trOn.Value else x + 2M
            abs (transition - x)

        member this.WidthCost left width costFun = 
            let boxIdx = [| for i in 0M .. 15M -> i |]
            let cost = boxIdx |> Array.map (fun id -> costFun left + id * width) |> Array.sum
            cost

        member private this.CalcStartingWidth costFunWidth costFunOnOff = 
            let probeCosts = 
                widthProbeAll
                |> Array.map (fun w -> Convert.ToInt32((rights.[0] - left) / w) )
                |> Seq.distinct |> Array.ofSeq
                |> Array.map (fun i ->  (rights.[0] - left) / (decimal i) )
                |> Array.map (fun x -> x,(costFunWidth this.Left  x costFunOnOff) )
            let best = probeCosts |> Array.minBy (fun x -> snd x)
            Math.Round(fst best, 2)

        member this.StartingWidth costFunWidthId costFunOnOffId = 
            let costFunWidth = 
                match costFunWidthId with
                | 1 -> this.WidthCost
                | _ -> this.WidthCost
            let costFunOnOff = 
                match costFunOnOffId with
                | 1 -> this.OffOnTransitionCost
                | _ -> this.OffOnTransitionCost
            this.CalcStartingWidth costFunWidth costFunOnOff