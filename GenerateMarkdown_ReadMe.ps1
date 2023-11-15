
Set-Location -Path "D:\GitHub\FabricUserApiDemo"

pandoc -s --extract-media ./images/ReadMe "ReadMe.docx" -t gfm -o "ReadMe.md"
