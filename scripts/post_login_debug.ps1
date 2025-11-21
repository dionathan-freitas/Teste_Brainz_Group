$ErrorActionPreference = 'Stop'
$baseUrl = 'http://localhost:5035'
$body = @{ username = 'admin'; password = 'admin123' } | ConvertTo-Json
try {
  $r = Invoke-WebRequest -Uri "$baseUrl/api/auth/login" -Method Post -Body $body -ContentType 'application/json' -UseBasicParsing
  Write-Output 'STATUS:'; Write-Output $r.StatusCode
  Write-Output 'CONTENT:'; Write-Output $r.Content
} catch {
  if ($_.Exception.Response) {
    $resp = $_.Exception.Response
    $sr = New-Object System.IO.StreamReader($resp.GetResponseStream())
    Write-Output 'STATUS_EXCEPTION:'
    try { Write-Output $resp.StatusCode } catch { }
    Write-Output 'CONTENT_EXCEPTION:'
    Write-Output $sr.ReadToEnd()
  } else {
    Write-Output 'NO_RESPONSE'
    Write-Output $_.Exception.Message
  }
}
