cd "C:\Windows\Microsoft.NET\Framework\v4.0.30319"

signtool sign /f "D:\Work\Code Signing Certificate\Certificate\Hristo.pfx" /p fVr06#aB /t http://timestamp.verisign.com/scripts/timstamp.dll  "D:\Work\tangra3\Tangra 3\bin\Release\Tangra3Update.exe"

signtool sign /f "D:\Work\Code Signing Certificate\Certificate\Hristo.pfx" /p fVr06#aB /t http://timestamp.verisign.com/scripts/timstamp.dll "D:\Work\tangra3\Tangra 3\bin\Release\Tangra.exe"

pause



