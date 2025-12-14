# PowerShell script to create Google Play Store assets
# Creates combined app icon SVG (512x512) and feature graphic SVG (1024x500)

# Output folder
$storeAssetsPath = "StoreAssets"

# Colors (matching the card table)
$casinoGreen = "#0C4A32"
$iconWhite = "#ffffff"

Write-Host "Creating Google Play Store assets..." -ForegroundColor Green
Write-Host ""

# Create output directory if it doesn't exist
if (-not (Test-Path $storeAssetsPath)) {
    New-Item -ItemType Directory -Path $storeAssetsPath | Out-Null
    Write-Host "  [OK] Created $storeAssetsPath directory" -ForegroundColor Cyan
}

# Create combined app icon SVG (512x512)
# This merges the background and foreground into a single file
$appIconCombinedSvg = @"
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg width="512" height="512" viewBox="0 0 512 512" version="1.1" xmlns="http://www.w3.org/2000/svg">
    <!-- Background: Casino Green -->
    <rect x="0" y="0" width="512" height="512" fill="$casinoGreen" />
    
    <!-- Foreground: Playing Cards with Spade -->
    <!-- Back card (slightly rotated and offset) -->
    <g transform="translate(256,256) rotate(-15) translate(-256,-256)">
        <rect x="144" y="99" width="202" height="292" rx="18" ry="18" fill="$iconWhite" opacity="0.7"/>
    </g>
    
    <!-- Front card -->
    <g transform="translate(256,256) rotate(10) translate(-256,-256)">
        <rect x="166" y="121" width="202" height="292" rx="18" ry="18" fill="$iconWhite"/>
        
        <!-- Spade symbol in center -->
        <g transform="translate(267, 267)">
            <!-- Spade shape -->
            <path d="M0,-56 C-34,-22 -56,11 -56,39 C-56,62 -39,79 -17,79 C-6,79 3,73 9,65 L0,101 L-22,101 L-22,112 L22,112 L22,101 L0,101 L-9,65 C-3,73 6,79 17,79 C39,79 56,62 56,39 C56,11 34,-22 0,-56 Z" 
                  fill="#1a1a1a"/>
        </g>
    </g>
</svg>
"@

$appIconCombinedSvg | Out-File -FilePath "$storeAssetsPath\appicon_512.svg" -Encoding UTF8 -NoNewline
Write-Host "  [OK] Created appicon_512.svg (512x512 combined icon)" -ForegroundColor Cyan

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Successfully created store assets!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Files created in $storeAssetsPath\:" -ForegroundColor Yellow
Write-Host "  - appicon_512.svg (512x512 - convert to PNG for Google Play)" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Convert appicon_512.svg to PNG using:" -ForegroundColor White
Write-Host "     - https://cloudconvert.com/svg-to-png" -ForegroundColor Gray
Write-Host "     - https://svgtopng.com/" -ForegroundColor Gray
Write-Host "     - Inkscape, ImageMagick, or similar tool" -ForegroundColor Gray
Write-Host ""
