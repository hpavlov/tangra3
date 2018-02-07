namespace Tangra.Functional

    open System

    type CostDelegate = delegate of decimal * decimal -> decimal

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

            let sum = 
                (decimal)this.Pixels.[x + this.Width * y] * xFactor * yFactor + 
                (decimal)this.Pixels.[x1 + this.Width * y] * x1Factor * yFactor + 
                (decimal)this.Pixels.[x + this.Width * y1] * xFactor * y1Factor + 
                (decimal)this.Pixels.[x1 + this.Width * y1] * x1Factor * y1Factor
            sum   