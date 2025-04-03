# Set these values
$pluginDllPath = "bin\Debug\netstandard2.0\Plugin.dll" # Replace with your actual DLL name
$actionName = "new_ClearTables" # Replace with your unbound action name (case-sensitive)

# Optional: clear and re-authenticate
Write-Host "`nChecking PAC auth..."
pac auth whoami
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nRunning authentication..."
    pac auth create --url "https://orgf3ad7966.crm.dynamics.com"
}

# Register plugin
Write-Host "`nInitializing plugin registration..."
pac plugin init --path $pluginDllPath

Write-Host "`nRegistering plugin with Dataverse..."
pac plugin add --action $actionName

Write-Host "`n✅ Plugin registered and bound to Unbound Action '$actionName'. All done!"
