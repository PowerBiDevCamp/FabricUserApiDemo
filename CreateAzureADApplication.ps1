Clear-Host

if (!(Get-AzContext)) { Connect-AzAccount }

$appName = "Fabric User API Demo Application"

$app = New-AzADApplication -DisplayName $appName -PublicClientRedirectUri "http://localhost"

Write-Host " > New app created with Application Id of " $app.AppId




Write-Host  


$app = Get-AzADApplication -DisplayName $appName

Write-Host "Creating new application named '$appName'"


if($app){
  Write-Host " > Deleting exisitng application named '$appName'..."
  Remove-AzADApplication -ObjectId $app.id
}
else {
  Write-Host " > The app named $appName does not exist"
}

$app = New-AzADApplication -DisplayName $appName  `
                           -PublicClientRedirectUri "http://localhost"


Write-Host " > New app created with Application Id of" $app.AppId
Write-Host  
