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

    member private this.IsOn x = 
        let s = [| for y in this.Top .. this.Bottom -> (this.Calc.At x y) |] |> Array.sum
        let on = s > (this.Bottom - this.Top) * 0.2M 
        on

    member private this.OffOnTransition x =
        let transition = 
            if this.IsOn x 
            then 
                let trOff = ([| for i in 1M .. 20M -> x - i/10M |] |> Seq.tryFind (fun x -> not (this.IsOn x)))
                if trOff.IsSome then trOff.Value + 0.1M else x - 2M
            else 
                let trOn = [| for i in 1M .. 20M -> x + i/10M |] |> Seq.tryFind (fun x -> this.IsOn x)
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
        let leftInt = [| for i in minLeft .. this.Width -> i |]
        let lR = leftInt |> Seq.find (fun x -> this.IsOn (decimal x))
        let leftPrec = [| for i in -10M .. 10M -> i/10M + (decimal lR) |]
        let lP = leftPrec |> Seq.find (fun x -> this.IsOn (decimal x))
        lP

    member this.FindWidth left approximateWidth =
        let widthProbe = [| for i in -20M .. 20M -> i/10M + approximateWidth |]
        let costs = widthProbe |> Array.map (fun w -> (w, this.WidthCost left w))
        let bestFit = costs |> Array.minBy (fun x -> snd x)
        fst bestFit

    static member FindBestLeft (frames: OsdFrame[]) =
        frames |> Array.map (fun frame -> frame.FindLeft 25) |> Array.average