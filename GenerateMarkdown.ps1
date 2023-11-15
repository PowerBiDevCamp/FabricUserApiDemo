
Set-Location -Path "D:\GitHub\FabricUserApiDemo"

pandoc -s --extract-media ./images/UserGuide "Getting Started with the Fabric User API.docx" -t gfm -o "ReadMe.md"
