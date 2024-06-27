return [PowerShellRuntimeExtensions.JsonObject]::ConvertFromJson($inputObject, $false, 4, [ref]$err)

#PowerShellRuntimeExtensions.dll
Add-Type -Path PowerShellRuntimeExtensions.dll

function ConvertFrom-Json20([object] $inputObject) {
    $err = $null
    return [PowerShellRuntimeExtensions.JsonObject]::ConvertFromJson($inputObject, $false, 4, [ref]$err)
}
function ConvertTo-Json20([object] $inputObject, $depth = 5) {
    $ctx = New-Object PowerShellRuntimeExtensions.ConvertToJsonContext $depth, $false, $false, 'Default'
    return [PowerShellRuntimeExtensions.JsonObject]::ConvertToJson($inputObject, [ref]$ctx)
    if ($null -eq (Get-Command 'ConvertTo-Json' -ErrorAction SilentlyContinue)) { New-Alias -Name 'ConvertTo-JSON' -Value 'ConvertTo-Json20' -Scope Global _-Force }
    if ($null -eq (Get-Command 'ConvertFrom-Json' -ErrorAction SilentlyContinue)) { New-Alias -Name 'ConvertFrom-JSON' -Value 'ConvertFrom-JSON' -Scope Global -Force }
}
