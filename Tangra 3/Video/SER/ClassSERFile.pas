unit ClassSERFile;

(*************************************************************************
**                                                                      **
**    Class TClassSERFile                                               **
**    Lumenera SER video format, version 3                              **
**                                                                      **
**    Author: Grischa Hahn, 2014-03-05                                  **
**                                                                      **
*************************************************************************)

interface

// -----------------------------------------------------------------------
(*
References:
int()     http://www.freepascal.org/docs-html/rtl/system/int.html
trunc()   http://www.freepascal.org/docs-html/rtl/system/trunc.html
frac()    http://www.freepascal.org/docs-html/rtl/system/frac.html
round()   http://www.freepascal.org/docs-html/rtl/system/round.html
*) 
// -----------------------------------------------------------------------

// -------------------------------------------------------------------------

const cSER_MONO       =  0;
      cSER_BAYER_RGGB =  8;
      cSER_BAYER_GRBG =  9;
      cSER_BAYER_GBRG = 10;
      cSER_BAYER_BGGR = 11;
      cSER_BAYER_CYYM = 16;
      cSER_BAYER_YCMY = 17;
      cSER_BAYER_YMCY = 18;
      cSER_BAYER_MYYC = 19;
      cSER_RGB        = 100; // (+) v3 GH 2014-01-01 (rgb point plane)
      cSER_BGR        = 101; // (+) v3 GH 2014-01-04 (bgr point plane)

const cBigEndian      = 0;
      cLittleEndian   = 1;

type  cFormatTyp = 0..101;

const cSetOfSupportedFormats: set of cFormatTyp =  [cSER_MONO,
                                                    cSER_BAYER_RGGB,
                                                    cSER_BAYER_GRBG,
                                                    cSER_BAYER_GBRG,
                                                    cSER_BAYER_BGGR,
                                                    cSER_BAYER_CYYM,
                                                    cSER_BAYER_YCMY,
                                                    cSER_BAYER_YMCY,
                                                    cSER_BAYER_MYYC,
                                                    cSER_RGB,  // (+) v3 GH 2014-01-01 
                                                    cSER_BGR]; // (+) v3 GH 2014-01-04

type  TSER_Color = (ctALL, ctRED, ctGREEN, ctBLUE);

type  TSER_PixelData_8     = Byte;
      TSER_PixelData_16    = Word;
      {$A-} // no data alignment !!
      TSER_PixelDataRGB_8  = record red, green, blue: Byte; end; // (+) v3 GH 2014-01-01
      TSER_PixelDataRGB_16 = record red, green, blue: Word; end; // (+) v3 GH 2014-01-01
      TSER_PixelDataBGR_8  = record blue, green, red: Byte; end; // (+) v3 GH 2014-01-04
      TSER_PixelDataBGR_16 = record blue, green, red: Word; end; // (+) v3 GH 2014-01-04
      {$A+}

      PSER_PixelData_8     = ^TSER_PixelData_8;
      PSER_PixelData_16    = ^TSER_PixelData_16;
      PSER_PixelDataRGB_8  = ^TSER_PixelDataRGB_8;   // (+) v3 GH 2014-01-01
      PSER_PixelDataRGB_16 = ^TSER_PixelDataRGB_16;  // (+) v3 GH 2014-01-01
      PSER_PixelDataBGR_8  = ^TSER_PixelDataBGR_8;   // (+) v3 GH 2014-01-04
      PSER_PixelDataBGR_16 = ^TSER_PixelDataBGR_16;  // (+) v3 GH 2014-01-04

const cSER_HeaderSize: Int64 = 178;

type TSER_FileID      = array[0..13] of Ansichar; // fill unused characters with null
     TSER_Observer    = array[0..39] of Ansichar; // fill unused characters with null
     TSER_Instrument  = array[0..39] of Ansichar; // fill unused characters with null
     TSER_Telescope   = array[0..39] of Ansichar; // fill unused characters with null
     TSER_Date        = Int64; // 62bit unsigned integer (little endian) described the number of 100ns from Gregorian year 0001-01-01 00:00

{$A-} // no data alignment !!
type TSER_Header = record
       mFileID          : TSER_FileID;
        // 'LUCAM-RECORDER'

       mLuID            : integer; // little endian
        // Lumenera camera series ID

       mColorID         : integer; // little endian
        // Data organisation (cSER_MONO...)

       mLittleEndian    : integer; // little endian
        // 0=BigEndian / 1=LittleEndian for 16bit pixel format

       mImageWidth      : integer; // little endian
       mImageHeight     : integer; // little endian

       mPixelDepth      : integer; // little endian
        // True bit depth of an pixel
            // 1.. 8 -> one byte per pixel
            // 9..16 -> two byte per pixel

       mFrameCount      : integer; // little endian
        // number of frames

       mObserver        : TSER_Observer;
       mInstrument      : TSER_Instrument;
       mTelescope       : TSER_Telescope;

       mStartTime       : TSER_Date; // little endian
       mStartTime_UTC   : TSER_Date; // little endian
     end;
{$A+}

type TSER_FileStatus = (_fsClosed, _fsOpenForRead, _fsOpenForWrite);

type  TClassSERFile = class (TObject)
        private
          mHeader:         TSER_Header;
          mTimeStamps:     array of TSER_Date;
          mTimeStampsOffsetToUTC: TSER_Date;

          mIsValid:        boolean;
          mFileName:       WideString;
          mFileStatus:     TSER_FileStatus;
          mIsOpen:         boolean;
          mFile:           TFileStream;
          mShiftValue:     integer;

          function GetFrameSize: Int64;
          function GetFilePosOfFrame(const index: integer): Int64;
          function GetFilePosOfTimeStampsTrailer: Int64;

          // Point serialisation
          function  GetPointerIncrement(const header: TSER_Header): integer;
          procedure SwapByteOrder(var v: TSER_PixelData_16);
          function  ConvertSERPointToRGB48(const pPoint: Pointer; const swap: boolean; const colorType: integer; const is16bit: boolean): TRGBTriple48;          function  ConvertSERPointToRGB24(const pPoint: Pointer; const swap: boolean; const colorType: integer; const is16bit: boolean): TRGBTriple;

          procedure ConvertRGB48ToSERPoint(const c16: TRGBTriple48;
                                 const x, y: integer;
                                 const pPoint: Pointer;
                                 const swap: boolean;
                                 const colorType: integer;
                                 const is16bit: boolean;
                                 const colorToBeUsed: TSER_Color = ctALL);

        public
          constructor Create(const fileName: WideString);
          destructor  Destroy; override;

          function    OpenForRead: boolean;
          function    OpenForWrite(const header: TSER_Header): boolean;

          function    GetHeaderInfo: TSER_Header;
          function    GetBitDepth: integer;

          function    SetToFirstFrame: boolean;
          function    SetToIndex(const index: integer): boolean;
          function    GetCurrentFrameIndex: integer;

          function    ReadFrame: TImageMatrix48; overload;
          function    ReadFrame(const index: integer): TImageMatrix48; overload;

          function    GetFrameTimeInUTCFromIndex(const index: integer): TSER_Date;

          function    WriteFrame(const image: TImageMatrix48; const frameTimeInUTC: TSER_Date; const colorToBeUsed: TSER_Color = ctALL): boolean; overload;
          
          function    Close: boolean;

          function    GetMinMaxFrameTimeInUTC(var min, max: TSER_Date): boolean;
          function    GetFramesPerSecond: double;
          function    GetDurationInSeconds: double;

          function    GetFrameTimeFromJulianDate(const jd: double): TSER_Date;
          function    GetJulianDateFromFrameTime(const frameTime: TSER_Date): double;

          function    IsPreProcessedSERFile: boolean;
      end;

procedure GetCalendarDateFormJulianDate (const jd: double; var day, month, year, hour, minute, second: integer; const useJulianCalendar: boolean = false);
function  GetJulianDateFromCalendarDate (year: integer; month, day, hour: integer; minute: double; const isJulianCalendar: boolean = false): double;      
      
// -----------------------------------------------------------------------

implementation

// -----------------------------------------------------------------------

constructor TClassSERFile.Create(const fileName: WideString);
begin
  inherited Create;

  FillChar(mHeader, SizeOf(mHeader), 0);
  SetLength(mTimeStamps, 0);
  mIsValid    := false;
  mFileName   := fileName;
  mFileStatus := _fsClosed;
  mFile       := nil;
  mShiftValue := 0;
  mTimeStampsOffsetToUTC := 0;
end;

// -----------------------------------------------------------------------

destructor TClassSERFile.Destroy;
begin
  Close;
  SetLength(mTimeStamps, 0);
  inherited Destroy;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetFrameSize: Int64;
var bytesPerPixelPerPlane, numberOfPlanes: Int64;
begin
  bytesPerPixelPerPlane := 1; // 8 bit
  if (mHeader.mPixelDepth > 8) then
    bytesPerPixelPerPlane := 2; // 16 bit

  numberOfPlanes := 1;
  if (mHeader.mColorID in [cSER_RGB, cSER_BGR]) then // (+) v3 GH 2014-01-04
    numberOfPlanes := 3;

  Result := bytesPerPixelPerPlane * numberOfPlanes * mHeader.mImageWidth * mHeader.mImageHeight;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetFilePosOfFrame(const index: integer): Int64;
begin
  Result := cSER_HeaderSize + index * GetFrameSize;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetFilePosOfTimeStampsTrailer: Int64;
begin
  Result := GetFilePosOfFrame(mHeader.mFrameCount);
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetHeaderInfo: TSER_Header;
begin
  Result := mHeader;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetBitDepth: integer;
begin
  if (mHeader.mPixelDepth <= 8) then
    Result := mHeader.mPixelDepth
  else
    Result := 16 - mShiftValue;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetPointerIncrement(const header: TSER_Header): integer;
begin
  Result := 1;  case header.mColorID of    cSER_RGB: (* v3 (+) GH 2014-01-01 *)      begin        if (header.mPixelDepth <= 8) then          Result := SizeOf(TSER_PixelDataRGB_8)
        else
          Result := SizeOf(TSER_PixelDataRGB_16);      end;    cSER_BGR: (* v3 (+) GH 2014-01-04 *)      begin        if (header.mPixelDepth <= 8) then          Result := SizeOf(TSER_PixelDataBGR_8)
        else
          Result := SizeOf(TSER_PixelDataBGR_16);      end;    else      begin        if (header.mPixelDepth <= 8) then          Result := SizeOf(TSER_PixelData_8)
        else
          Result := SizeOf(TSER_PixelData_16);
      end;
  end;
end;

// -----------------------------------------------------------------------

procedure TClassSERFile.SwapByteOrder(var v: TSER_PixelData_16);
begin  v := (($FF00 and v) shr 8) or (($00FF and v) shl 8);end;

// -----------------------------------------------------------------------

function TClassSERFile.ConvertSERPointToRGB48(const pPoint: Pointer; const swap: boolean; const colorType: integer; const is16bit: boolean): TRGBTriple48;

  // call in ReadFrame
var c16: TRGBTriple48;
    g16: word;
    rgb16: TSER_PixelDataRGB_16;
    rgb8: TSER_PixelDataRGB_8;
    bgr16: TSER_PixelDataBGR_16;
    bgr8: TSER_PixelDataBGR_8;

begin
  case colorType of
    cSER_RGB: (* v3 (+) GH 2014-01-01 *)
      begin
        if (is16bit) then
          begin
            // 16 bit
            rgb16 := PSER_PixelDataRGB_16(pPoint)^;
            if (swap) then
              begin
                SwapByteOrder(rgb16.red);
                SwapByteOrder(rgb16.green);
                SwapByteOrder(rgb16.blue);
              end;
            c16.rgbtBlue  := rgb16.blue  shl mShiftValue;
            c16.rgbtGreen := rgb16.green shl mShiftValue;
            c16.rgbtRed   := rgb16.red   shl mShiftValue;
            Result := c16;
          end else
          begin
            // 8 bit
            rgb8 := PSER_PixelDataRGB_8(pPoint)^;
            c16.rgbtBlue  := word(rgb8.blue)  shl 8;
            c16.rgbtGreen := word(rgb8.green) shl 8;
            c16.rgbtRed   := word(rgb8.red)   shl 8;
            Result := c16;
          end;
      end;
    cSER_BGR: (* v3 (+) GH 2014-01-04 *)
      begin
        if (is16bit) then
          begin
            // 16 bit
            bgr16 := PSER_PixelDataBGR_16(pPoint)^;
            if (swap) then
              begin
                SwapByteOrder(bgr16.red);
                SwapByteOrder(bgr16.green);
                SwapByteOrder(bgr16.blue);
              end;
            c16.rgbtBlue  := bgr16.blue  shl mShiftValue;
            c16.rgbtGreen := bgr16.green shl mShiftValue;
            c16.rgbtRed   := bgr16.red   shl mShiftValue;
            Result := c16;
          end else
          begin
            // 8 bit
            bgr8 := PSER_PixelDataBGR_8(pPoint)^;
            c16.rgbtBlue  := word(bgr8.blue)  shl 8;
            c16.rgbtGreen := word(bgr8.green) shl 8;
            c16.rgbtRed   := word(bgr8.red)   shl 8;
            Result := c16;
          end;
      end;
    else
      begin
        if (is16bit) then
          begin
            // 16 bit
            g16 := PSER_PixelData_16(pPoint)^;
            if (swap) then
               SwapByteOrder(g16);
            c16.rgbtBlue  := g16 shl mShiftValue;
            c16.rgbtGreen := c16.rgbtBlue;
            c16.rgbtRed   := c16.rgbtBlue;
            Result := c16;
          end else
          begin
            // 8 bit
            c16.rgbtBlue  := word(PSER_PixelData_8(pPoint)^) shl 8;
            c16.rgbtGreen := c16.rgbtBlue;
            c16.rgbtRed   := c16.rgbtBlue;
            Result := c16;
          end;
      end;
  end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.ConvertSERPointToRGB24(const pPoint: Pointer; const swap: boolean; const colorType: integer; const is16bit: boolean): TRGBTriple;

var c8: TRGBTriple;
    g16, shiftValue: word;
    rgb16: TSER_PixelDataRGB_16;
    rgb8: TSER_PixelDataRGB_8;
    bgr16: TSER_PixelDataBGR_16;
    bgr8: TSER_PixelDataBGR_8;

begin
  case colorType of
    cSER_RGB: (* v3 (+) GH 2014-01-01 *)
      begin
        if (is16bit) then
          begin
            // 16 bit
            rgb16 := PSER_PixelDataRGB_16(pPoint)^;
            if (swap) then
              begin
                SwapByteOrder(rgb16.red);
                SwapByteOrder(rgb16.green);
                SwapByteOrder(rgb16.blue);
              end;
            c8.rgbtBlue  := (rgb16.blue shl mShiftValue)  shr 8;
            c8.rgbtGreen := (rgb16.green shl mShiftValue) shr 8;
            c8.rgbtRed   := (rgb16.red shl mShiftValue)   shr 8;
            Result := c8;
          end else
          begin
            // 8 bit
            rgb8 := PSER_PixelDataRGB_8(pPoint)^;
            c8.rgbtBlue  := rgb8.blue;
            c8.rgbtGreen := rgb8.green;
            c8.rgbtRed   := rgb8.red;
            Result := c8;
          end;
      end;
    cSER_BGR: (* v3 (+) GH 2014-01-04 *)
      begin
        if (is16bit) then
          begin
            // 16 bit
            bgr16 := PSER_PixelDataBGR_16(pPoint)^;
            if (swap) then
              begin
                SwapByteOrder(bgr16.red);
                SwapByteOrder(bgr16.green);
                SwapByteOrder(bgr16.blue);
              end;
            c8.rgbtBlue  := (bgr16.blue shl mShiftValue)  shr 8;
            c8.rgbtGreen := (bgr16.green shl mShiftValue) shr 8;
            c8.rgbtRed   := (bgr16.red shl mShiftValue)   shr 8;
            Result := c8;
          end else
          begin
            // 8 bit
            bgr8 := PSER_PixelDataBGR_8(pPoint)^;
            c8.rgbtBlue  := bgr8.blue;
            c8.rgbtGreen := bgr8.green;
            c8.rgbtRed   := bgr8.red;
            Result := c8;
          end;
      end;
    else
      begin
        if (is16bit) then
          begin
            // 16 bit
            g16 := PSER_PixelData_16(pPoint)^;
            if (swap) then
               SwapByteOrder(g16);
            c8.rgbtBlue  := (g16 shl mShiftValue) shr 8;
            c8.rgbtGreen := c8.rgbtBlue;
            c8.rgbtRed   := c8.rgbtBlue;
            Result := c8;
          end else
          begin
            // 8 bit
            c8.rgbtBlue  := PSER_PixelData_8(pPoint)^;
            c8.rgbtGreen := c8.rgbtBlue;
            c8.rgbtRed   := c8.rgbtBlue;
            Result := c8;
          end;
      end;
  end;
end;

// -----------------------------------------------------------------------

procedure TClassSERFile.ConvertRGB48ToSERPoint(const c16: TRGBTriple48;
                                     const x, y: integer;
                                     const pPoint: Pointer;
                                     const swap: boolean;
                                     const colorType: integer;
                                     const is16bit: boolean;
                                     const colorToBeUsed: TSER_Color);

  procedure setRed;
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := c16.rgbtRed;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelData_16(pPoint)^ := g16;
      end else
      begin
        // 8 bit
        PSER_PixelData_8(pPoint)^ := c16.rgbtRed shr 8;
      end;
  end;

  procedure setGreen;
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := c16.rgbtGreen;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelData_16(pPoint)^ := g16;
      end else
      begin
        // 8 bit
        PSER_PixelData_8(pPoint)^ := c16.rgbtGreen shr 8;
      end;
  end;

  procedure setBlue;
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := c16.rgbtBlue;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelData_16(pPoint)^ := g16;
      end else
      begin
        // 8 bit
        PSER_PixelData_8(pPoint)^ := c16.rgbtBlue shr 8;
      end;
  end;

  procedure setYellow;
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := _MaxPointValue - c16.rgbtBlue;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelData_16(pPoint)^ := g16;
      end else
      begin
        // 8 bit
        PSER_PixelData_8(pPoint)^ := (_MaxPointValue - c16.rgbtBlue) shr 8;
      end;
  end;

  procedure setMagenta;
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := _MaxPointValue - c16.rgbtGreen;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelData_16(pPoint)^ := g16;
      end else
      begin
        // 8 bit
        PSER_PixelData_8(pPoint)^ := (_MaxPointValue - c16.rgbtGreen) shr 8;
      end;
  end;

  procedure setCyan;
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := _MaxPointValue - c16.rgbtRed;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelData_16(pPoint)^ := g16;
      end else
      begin
        // 8 bit
        PSER_PixelData_8(pPoint)^ := (_MaxPointValue - c16.rgbtRed) shr 8;
      end;
  end;

  procedure setRGB; (* v3 (+) GH 2014-01-01 *)
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := c16.rgbtRed;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelDataRGB_16(pPoint)^.red := g16;

        g16 := c16.rgbtGreen;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelDataRGB_16(pPoint)^.green := g16;

        g16 := c16.rgbtBlue;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelDataRGB_16(pPoint)^.blue := g16;
      end else
      begin
        // 8 bit
        PSER_PixelDataRGB_8(pPoint)^.red   := c16.rgbtRed   shr 8;
        PSER_PixelDataRGB_8(pPoint)^.green := c16.rgbtGreen shr 8;
        PSER_PixelDataRGB_8(pPoint)^.blue  := c16.rgbtBlue  shr 8;
      end;
  end;

  procedure setBGR; (* v3 (+) GH 2014-01-04 *)
  var g16: word;
  begin
    if (is16bit) then
      begin
        g16 := c16.rgbtRed;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelDataBGR_16(pPoint)^.red := g16;

        g16 := c16.rgbtGreen;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelDataBGR_16(pPoint)^.green := g16;

        g16 := c16.rgbtBlue;
        if (swap) then
          SwapByteOrder(g16);
        PSER_PixelDataBGR_16(pPoint)^.blue := g16;
      end else
      begin
        // 8 bit
        PSER_PixelDataBGR_8(pPoint)^.red   := c16.rgbtRed   shr 8;
        PSER_PixelDataBGR_8(pPoint)^.green := c16.rgbtGreen shr 8;
        PSER_PixelDataBGR_8(pPoint)^.blue  := c16.rgbtBlue  shr 8;
      end;
  end;

begin
 case colorType of
    cSER_MONO:
      begin
        case colorToBeUsed of
            ctALL: setGreen; // todo: setGray
            ctRED: setRed;
          ctGREEN: setGreen;
           ctBLUE: setBlue;
        end;
      end;

    // Lossy color dithering:
    cSER_BAYER_RGGB:
      begin
        if odd(y) then
          begin // odd line = GB
            if odd(x) then
              setBlue   // odd column = B
            else
              setGreen; // even column = G
          end else
          begin // even line = RG
            if odd(x) then
              setGreen  // odd column = G
            else
              setRed;   // even column = R
          end;
      end;
    cSER_BAYER_GRBG:
      begin
        if odd(y) then
          begin // odd line = BG
            if odd(x) then
              setGreen  // odd column = G
            else
              setBlue;  // even column = B
          end else
          begin // even line = GR
            if odd(x) then
              setRed    // odd column = R
            else
              setGreen; // even column = G
          end;
      end;
    cSER_BAYER_GBRG:
      begin
        if odd(y) then
          begin // odd line = RG
            if odd(x) then
              setGreen   // odd column = G
            else
              setRed;    // even column = R
          end else
          begin // even line = GB
            if odd(x) then
              setBlue    // odd column = B
            else
              setGreen;  // even column = G
          end;
      end;
    cSER_BAYER_BGGR:
      begin
        if odd(y) then
          begin // odd line = GR
            if odd(x) then
              setRed    // odd column = R
            else
              setGreen; // even column = G
          end else
          begin // even line = BG
            if odd(x) then
              setGreen  // odd column = G
            else
              setBlue;  // even column = B
          end;
      end;
    cSER_BAYER_CYYM:
      begin
        if odd(y) then
          begin // odd line = YM
            if odd(x) then
              setMagenta // odd column = M
            else
              setYellow; // even column = Y
          end else
          begin // even line = CY
            if odd(x) then
              setYellow  // odd column = Y
            else
              setCyan;   // even column = C
          end;
      end;
    cSER_BAYER_YCMY:
      if odd(y) then
        begin // odd line = MY
          if odd(x) then
            setYellow   // odd column = Y
          else
            setMagenta; // even column = M
        end else
        begin // even line = YC
          if odd(x) then
            setCyan     // odd column = C
          else
            setYellow;  // even column = Y
        end;
    cSER_BAYER_YMCY:
      if odd(y) then
        begin // odd line = CY
          if odd(x) then
            setYellow   // odd column = Y
          else
            setCyan;    // even column = C
        end else
        begin // even line = YM
          if odd(x) then
            setMagenta  // odd column = M
          else
            setYellow;  // even column = Y
        end;
    cSER_BAYER_MYYC:
      if odd(y) then
        begin // odd line = YC
          if odd(x) then
            setCyan     // odd column = C
          else
            setYellow;  // even column = Y
        end else
        begin // even line = MY
          if odd(x) then
            setYellow   // odd column = Y
          else
            setMagenta; // even column = M
        end;
    cSER_RGB: (* v3 (+) GH 2014-01-01 *)
      setRGB;
    cSER_BGR: (* v3 (+) GH 2014-01-04 *)
      setBGR;
  end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.OpenForRead: boolean;

var i: Integer;
    timeStamp, dFirstFrameMinusStartTime, dFirstFrameMinusStartTimeUTC: TSER_Date;
    frame: TImageMatrix48;
      

begin
  Result := false;

  if (mFileStatus = _fsOpenForWrite) then
    begin
      Result := false;
      exit;
    end;

  if (mFileStatus = _fsOpenForRead) then
    begin
      Result := true;
      exit;
    end;

  if (mFileStatus = _fsClosed) then
    begin
      try
        mFile := TFileStream.Create(mFileName, fmOpenRead or fmShareDenyRead);
        mFile.ReadBuffer(mHeader, SizeOf (mHeader));

        if (not (mHeader.mColorID in cSetOfSupportedFormats)) then
          raise Exception.Create('Not implemented.');

        mShiftValue := 0; // default is full 16 bit

        // TimStampUTC := TimeStamp + mTimeStampsOffsetToUTC
        mTimeStampsOffsetToUTC := 0; // no offset, time stamps in UTC

        // read TimeStamps
        SetLength(mTimeStamps, 0);
        if ((mHeader.mFrameCount > 0) and (mHeader.mStartTime > 0)) then // valid ?
          begin
            try
              // handling of files with erroneous data:
			  if (mHeader.mStartTime_UTC <= 0) then
                mHeader.mStartTime_UTC := mHeader.mStartTime; 

              mFile.Position := GetFilePosOfTimeStampsTrailer;
              SetLength(mTimeStamps, mHeader.mFrameCount);
              for i:=0 to mHeader.mFrameCount-1 do
                begin
                  mFile.ReadBuffer(timeStamp, SizeOf (timeStamp)); // throws exception, if not exists
                  mTimeStamps[i] := timeStamp;
                end;

			  // handling of files with erroneous data:
              // Format of TimeStamps either LocalTime or UTC
              dFirstFrameMinusStartTime    := mTimeStamps[0] (* FF *) - mHeader.mStartTime     (* ST *) ;
              dFirstFrameMinusStartTimeUTC := mTimeStamps[0] (* FF *) - mHeader.mStartTime_UTC (* ST_UTC *) ;

              if ((dFirstFrameMinusStartTimeUTC >= 0) and (dFirstFrameMinusStartTime >= 0) and
                  (dFirstFrameMinusStartTime < dFirstFrameMinusStartTimeUTC))
                 (* ST_UTC ...... ST . FF *)

                or
                 ((dFirstFrameMinusStartTimeUTC < 0) and (dFirstFrameMinusStartTime >= 0))
                 (* ST . FF ...... ST_UTC *)

                or
                 ((dFirstFrameMinusStartTimeUTC > 0) and (dFirstFrameMinusStartTime < 0) and
                  (abs(dFirstFrameMinusStartTime) < abs(dFirstFrameMinusStartTimeUTC)))
                 (* ST_UTC ...... FF . ST  - PlxCapture_ARRANDALE *)

                or
                 ((dFirstFrameMinusStartTimeUTC < 0) and (dFirstFrameMinusStartTime < 0) and
                  (abs(dFirstFrameMinusStartTime) < abs(dFirstFrameMinusStartTimeUTC)))
                 (* FF . ST ...... ST_UTC  - PlxCapture_ARRANDALE *)

                then begin
                  // TimeStamps are in LocalTime
                  // TimStampUTC := TimeStamp + mTimeStampsOffsetToUTC
                  mTimeStampsOffsetToUTC := mHeader.mStartTime_UTC - mHeader.mStartTime;
                end;

            except
              // handling of files with erroneous data:
              SetLength(mTimeStamps, 0);
            end;

          end;

        // seek to first frame
        mFile.Position := GetFilePosOfFrame(0);

        // OK
        mFileStatus := _fsOpenForRead;
        Result := true;

        // get pixel depth from first frame
        if ((mHeader.mFrameCount > 0) and (mHeader.mPixelDepth > 8)) then
          begin
            if (mHeader.mPixelDepth = 16) then
              begin
                // handling of files with erroneous data:
				// automatic detection of bit depth in first frame
                frame := ReadFrame;
                if (frame <> nil) then
                  begin
                    i := frame.GetMaxPixelValue;

                    if (i < $0100) then
                      mShiftValue := 8 else //  8 bit
                    if (i < $0400) then
                      mShiftValue := 6 else // 10 bit
                    if (i < $1000) then
                      mShiftValue := 4 else // 12 bit
                    if (i < $4000) then
                      mShiftValue := 2 else // 14 bit

                      mShiftValue := 0;     // 16 bit (default)

                    frame.free;
                  end;
              end else
              begin
                mShiftValue := 16 - mHeader.mPixelDepth;
              end;

            // seek to first frame
            mFile.Position := GetFilePosOfFrame(0);
          end;

      except
        // Error
        mFile.Free;
        mFile := nil;
        FillChar (mHeader, SizeOf (mHeader), #0);
        SetLength(mTimeStamps, 0);
        exit;
      end;
    end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.OpenForWrite(const header: TSER_Header): boolean;
var i: Integer;
    fp: Int64;
    timeStamp: TSER_Date;

begin
  Result := false;

  if (mFileStatus = _fsOpenForWrite) then
    begin
      Result := false;
      exit;
    end;

  if (mFileStatus = _fsOpenForRead) then
    begin
      Result := true;
      exit;
    end;

  if (mFileStatus = _fsClosed) then
    begin
      try
        DeleteFile(mFileName);

        mFile := TFileStream.Create(mFileName, fmCreate or fmShareExclusive);

        mHeader := header;
        mHeader.mFrameCount := 0;

        if (not (mHeader.mColorID in cSetOfSupportedFormats)) then
          raise Exception.Create('Not implemented.');

        mFile.WriteBuffer(mHeader, SizeOf (mHeader));

        // OK
        SetLength(mTimeStamps, 0);
        mFileStatus := _fsOpenForWrite;
        Result := true;

      except
        // Fehler
        mFile.Free;
        mFile := nil;
        FillChar (mHeader, SizeOf (mHeader), #0);
        SetLength(mTimeStamps, 0);
        exit;
      end;
    end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.SetToFirstFrame: boolean;
begin
  Result := false;

  try
    if (mFileStatus = _fsOpenForRead) then
      begin
       mFile.Position := cSER_HeaderSize;
       Result := true;
      end;
  except
    Result := false;
  end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.SetToIndex(const index: integer): boolean;
begin
  Result := false;

  try
    if (mFileStatus = _fsOpenForRead) and (index < mHeader.mFrameCount) then
      begin
        mFile.Position := GetFilePosOfFrame(index);
        Result := true;
      end;
  except
    Result := false;
  end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetCurrentFrameIndex: integer;
begin
  Result := 0;

  if (mFileStatus = _fsOpenForRead) then
    Result := (mFile.Position - cSER_HeaderSize) div GetFrameSize;

  if (mFileStatus = _fsOpenForWrite) then
    Result := mHeader.mFrameCount;
end;

// -----------------------------------------------------------------------

function TClassSERFile.WriteFrame(const image: TImageMatrix48; const frameTimeInUTC: TSER_Date; const colorToBeUsed: TSER_Color): boolean;

var pFrame, pPixel: PBYTE;
    x, y, curIndex: integer;
    increment: integer;
    swap, is16bit: boolean;
    colorType: integer;

begin
  Result := false;
  pFrame := nil;

  if (mFileStatus <> _fsOpenForWrite) then
    exit;

  if (mHeader.mImageWidth <> image.mWidth) or (mHeader.mImageHeight <> image.mHeight) then
    exit;

  try
    curIndex := GetCurrentFrameIndex;

    increment := GetPointerIncrement(mHeader);
    swap      := (mHeader.mLittleEndian = 1);
    colorType := mHeader.mColorID;
    is16bit   := (mHeader.mPixelDepth > 8);

    GetMem(pFrame, GetFrameSize);
    pPixel := pFrame;


    for y:=0 to mHeader.mImageHeight-1 do
      for x:=0 to mHeader.mImageWidth-1 do
        begin
          ConvertRGB48ToSERPoint (image.mMatrix[x,y], x, y, pPixel, swap, colorType, is16bit, colorToBeUsed);
          inc(pPixel, increment);
        end;


    mFile.WriteBuffer(pFrame^, GetFrameSize);
    FreeMem(pFrame);

    if (mHeader.mStartTime > 0) then
      begin
        SetLength(mTimeStamps, curIndex + 1);
        mTimeStamps[curIndex] := frameTimeInUTC;
      end;

    mHeader.mFrameCount := curIndex + 1;

    Result := true;

  except
    FreeMem(pFrame);
    Result := false;
  end;

end;

// -----------------------------------------------------------------------

function TClassSERFile.ReadFrame: TImageMatrix48;

var image: TImageMatrix48;
    pFrame, pPixel: PBYTE;
    x, y: integer;
    increment: integer;
    swap, is16bit: boolean;
    colorType: integer;

begin
  Result := nil;
  image  := nil;
  pFrame := nil;

  if (mFileStatus <> _fsOpenForRead) then
    exit;

  if (mHeader.mImageWidth <= 0) or (mHeader.mImageHeight <= 0) then
    exit;

  if (mHeader.mPixelDepth < 0) or (mHeader.mPixelDepth > 16) then
    exit;

  try
    increment := GetPointerIncrement(mHeader);
    swap      := (mHeader.mLittleEndian = 1);
    colorType := mHeader.mColorID;
    is16bit   := (mHeader.mPixelDepth > 8);

    GetMem(pFrame, GetFrameSize);
    mFile.ReadBuffer(pFrame^, GetFrameSize);
    image := TImageMatrix48.Create(mHeader.mImageWidth, mHeader.mImageHeight, cZeroPoint);
    pPixel := pFrame;
    for y:=0 to mHeader.mImageHeight-1 do
      for x:=0 to mHeader.mImageWidth-1 do
        begin
          image.mMatrix[x,y] := ConvertSERPointToRGB48(pPixel, swap, colorType, is16bit);
          inc(pPixel, increment);
        end;
    FreeMem(pFrame);

    // De-Bayering:
    case colorType of
      cSER_BAYER_RGGB: image.ConvertBayerRasterToRGB(rtRGGB);
      cSER_BAYER_GRBG: image.ConvertBayerRasterToRGB(rtGRBG);
      cSER_BAYER_GBRG: image.ConvertBayerRasterToRGB(rtGBRG);
      cSER_BAYER_BGGR: image.ConvertBayerRasterToRGB(rtBGGR);
      cSER_BAYER_CYYM: image.ConvertBayerRasterToRGB(rtCYYM);
      cSER_BAYER_YCMY: image.ConvertBayerRasterToRGB(rtYCMY);
      cSER_BAYER_YMCY: image.ConvertBayerRasterToRGB(rtYMCY);
      cSER_BAYER_MYYC: image.ConvertBayerRasterToRGB(rtMYYC);
    end;
    
    Result := image;
    
  except
    image.Free;
    FreeMem(pFrame);
    Result := nil;
  end;

end;

// -----------------------------------------------------------------------

function TClassSERFile.ReadFrame(const index: integer): TImageMatrix48;
begin
  Result := nil;

  if (SetToIndex(index)) then
    Result := ReadFrame;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetFrameTimeInUTCFromIndex(const index: integer): TSER_Date;
begin
  Result := 0;

  if (index >= 0) and (index < mHeader.mFrameCount) and (Length(mTimeStamps) = mHeader.mFrameCount) then
    Result := mTimeStamps[index] + mTimeStampsOffsetToUTC;
end;

// -----------------------------------------------------------------------

function TClassSERFile.Close: boolean;
var i: integer;
    fp: Int64;
begin
  Result := false;

  if (mFileStatus = _fsClosed) then
    begin
      Result := true;
      exit;
    end;

  if (mFileStatus = _fsOpenForWrite) then
    begin
      try
        // refresh Header (FrameCount)
        mFile.Position := 0;
        mFile.WriteBuffer(mHeader, SizeOf (mHeader));

        // write TimeStamps at file end
        mFile.Position := mFile.Size;
        if (Length(mTimeStamps) > 0) then
          for i:=0 to Length(mTimeStamps)-1 do
            mFile.WriteBuffer(mTimeStamps[i], SizeOf(TSER_Date));

      except
        // Error
        mFile.Free;
        mFileStatus := _fsClosed;
        Result := false;
        exit;
      end;
    end;

  if (mFileStatus in [_fsOpenForWrite, _fsOpenForRead]) then
    begin
      mFile.Free;
      mFileStatus := _fsClosed;
      Result := true;
    end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetMinMaxFrameTimeInUTC(var min, max: TSER_Date): boolean;
var i: integer;
    t: TSER_Date;
begin
  Result := false;
  if (mFileStatus in [_fsClosed, _fsOpenForRead]) then
    begin
      if (mHeader.mStartTime > 0) and (Length(mTimeStamps) = mHeader.mFrameCount) then
        begin
          if (mHeader.mFrameCount < 1) then
            exit;

          min := 1000000000000000000;
          max := 0;
          for I := 0 to mHeader.mFrameCount-1 do
            begin
              t := mTimeStamps[i] + mTimeStampsOffsetToUTC;
              if (t > max) then
                max := t;
              if (t < min) then
                min := t;
            end;
          Result := true;
        end;
    end;
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetDurationInSeconds: double;
var dtimeStamps: TSER_Date;
    min, max: TSER_Date;
begin
  Result := 0;

  if (not GetMinMaxFrameTimeInUTC(min, max)) then
    exit;

  dtimeStamps := max - min;
  Result := (dtimeStamps / 10000000.0);
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetFramesPerSecond: double;
var dtimeStamps: TSER_Date;
    min, max: TSER_Date;
begin
  Result := 0;

  if (not GetMinMaxFrameTimeInUTC(min, max)) then
    exit;

  dtimeStamps := max - min;
  if (dtimeStamps > 0) then
    Result := mHeader.mFrameCount / (dtimeStamps / 10000000.0); // estimation !
 end;

// -----------------------------------------------------------------------

function TClassSERFile.GetFrameTimeFromJulianDate(const jd: double): TSER_Date;
var jd0001_01_01_0h_UT: double;
begin
  jd0001_01_01_0h_UT := GetJulianDateFromCalendarDate (1, 1, 1, 0, 0, false); // Gregorian calender
  Result := round((jd - jd0001_01_01_0h_UT) * (10000000.0 * 3600.0 * 24.0));
end;

// -----------------------------------------------------------------------

function TClassSERFile.GetJulianDateFromFrameTime(const frameTime: TSER_Date): double;
var jd0001_01_01_0h_UT: double;
begin
  jd0001_01_01_0h_UT := GetJulianDateFromCalendarDate (1, 1, 1, 0, 0, false); // Gregorian calender
  Result := (frameTime / 10000000.0 / 3600.0 / 24.0) + jd0001_01_01_0h_UT;
end;

// -----------------------------------------------------------------------

function TClassSERFile.IsPreProcessedSERFile: boolean;
// PIPP deletes bad frames from file or resort frame order
var i: integer;
    t0, t, dt: TSER_Date;
begin
  Result := false;

  if (mFileStatus in [_fsClosed, _fsOpenForRead]) then
    begin
      if (mHeader.mStartTime > 0) and (Length(mTimeStamps) = mHeader.mFrameCount) then
        begin
          if (mHeader.mFrameCount <= 2) then
            exit;

          t0 := GetFrameTimeInUTCFromIndex(0);
          t  := GetFrameTimeInUTCFromIndex(1);
          dt := abs((t - t0) * 3 div 2); // maximum deviation 50%
          t0 := t;
          for i:=2 to mHeader.mFrameCount-1 do
            begin
              t := GetFrameTimeInUTCFromIndex(i);
              if (abs(t - t0) > dt) then
                begin
                  Result := true;
                  exit;
                end;
              t0 := t;
            end;
        end;
    end;
end;

// -----------------------------------------------------------------------

procedure GetCalendarDateFormJulianDate (const jd: double; var day, month, year, hour, minute, second: integer; const useJulianCalendar: boolean = false);
 
var b, d, f : integer;
    c, e    : double;
    JD0, ut : double;
    seconds : integer;
 
begin
 
  JD0     := trunc(JD + 0.5);
  ut      := 24.0 * frac (JD + 0.5);
  seconds := round(ut * 3600.0);
 
  if (useJulianCalendar) then
    begin
 
      // ends with 1582 Oct 4 (Julianian calendar)
 
      B := 0;
      C := JD0 + 1524.0;
 
    end else
    begin
 
      // begins with 1582 Oct 15 (Gregorian calendar)
 
      B := trunc((JD0 - 1867216.25) / 36524.25);
      C := JD0 + (B - trunc(B / 4.0)) + 1525.0;
 
    end;
 
  D             := trunc ((C - 122.1) / 365.25);
  E             := 365.0 * D + trunc (D / 4.0);
  F             := trunc ((C - E) / 30.6001);
 
  day           := trunc (C - E + 0.5) - trunc (30.6001 * F);
  month         := F - 1 - 12 * trunc (F / 14.0);
  year          := D - 4715 - trunc ((7.0 + month) / 10.0);
 
  hour          := seconds div 3600;
  minute        := (seconds - hour * 3600) div 60;
  second        := (seconds - hour * 3600) mod 60;
end;
 
// -----------------------------------------------------------------------
 
function GetJulianDateFromCalendarDate (year: integer; month, day, hour: integer; minute: double; const isJulianCalendar: boolean = false): double;
var A, B, JD: double;
begin
  if (month < 3) then
    begin
      month := month + 12;
      year  := year - 1;
    end;
 
  if (isJulianCalendar) then
   begin
     // ends with 1582 Oct 4 (Julianian calendar)
     A := int(year/100.0);
     B := 0.0;
   end else
   begin
     // begins with 1582 Oct 15 (Gregorian calendar)
     A := int(year/100.0);
     B := 2.0 - A + int(A/4.0);
   end;
 
  JD := int(365.25*(year + 4716.0)) + int(30.6001*(month + 1.0)) + B + day + hour/24.0 + minute/1440.0 - 1524.5;
 
  Result := JD;
end;
 
// -----------------------------------------------------------------------


end.
