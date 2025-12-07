# PowerShell script to create app icon and splash screen SVG files
# Creates a playing cards themed icon with casino green background

# Output paths
$appIconPath = "src\Blackjack\Resources\AppIcon"
$splashPath = "src\Blackjack\Resources\Splash"

# Colors (matching the card table)
$casinoGreen = "#0C4A32"
$iconWhite = "#ffffff"

Write-Host "Creating app icon and splash screen SVG files..." -ForegroundColor Green
Write-Host ""

# Create appicon.svg (background - solid green)
$appiconSvg = @"
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg width="456" height="456" viewBox="0 0 456 456" version="1.1" xmlns="http://www.w3.org/2000/svg">
    <rect x="0" y="0" width="456" height="456" fill="$casinoGreen" />
</svg>
"@

$appiconSvg | Out-File -FilePath "$appIconPath\appicon.svg" -Encoding UTF8 -NoNewline
Write-Host "  [OK] Created appicon.svg (casino green background)" -ForegroundColor Cyan

# Create appiconfg.svg (foreground - playing cards icon)
# This is a simplified playing cards icon similar to Fluent UI PlayingCards
$appiconfgSvg = @"
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg width="456" height="456" viewBox="0 0 456 456" version="1.1" xmlns="http://www.w3.org/2000/svg">
    <!-- Back card (slightly rotated and offset) -->
    <g transform="translate(228,228) rotate(-15) translate(-228,-228)">
        <rect x="128" y="88" width="180" height="260" rx="16" ry="16" fill="$iconWhite" opacity="0.7"/>
    </g>
    
    <!-- Front card -->
    <g transform="translate(228,228) rotate(10) translate(-228,-228)">
        <rect x="148" y="108" width="180" height="260" rx="16" ry="16" fill="$iconWhite"/>
        
        <!-- Spade symbol in center -->
        <g transform="translate(238, 238)">
            <!-- Spade shape -->
            <path d="M0,-50 C-30,-20 -50,10 -50,35 C-50,55 -35,70 -15,70 C-5,70 3,65 8,58 L0,90 L-20,90 L-20,100 L20,100 L20,90 L0,90 L-8,58 C-3,65 5,70 15,70 C35,70 50,55 50,35 C50,10 30,-20 0,-50 Z" 
                  fill="#1a1a1a"/>
        </g>
    </g>
</svg>
"@

$appiconfgSvg | Out-File -FilePath "$appIconPath\appiconfg.svg" -Encoding UTF8 -NoNewline
Write-Host "  [OK] Created appiconfg.svg (playing cards with spade)" -ForegroundColor Cyan

# Create splash.svg (playing cards icon for splash screen)
$splashSvg = @"
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg width="456" height="456" viewBox="0 0 456 456" version="1.1" xmlns="http://www.w3.org/2000/svg">
    <!-- Back card (slightly rotated and offset) -->
    <g transform="translate(228,228) rotate(-15) translate(-228,-228)">
        <rect x="128" y="88" width="180" height="260" rx="16" ry="16" fill="$iconWhite" opacity="0.7"/>
    </g>
    
    <!-- Front card -->
    <g transform="translate(228,228) rotate(10) translate(-228,-228)">
        <rect x="148" y="108" width="180" height="260" rx="16" ry="16" fill="$iconWhite"/>
        
        <!-- Spade symbol in center -->
        <g transform="translate(238, 238)">
            <!-- Spade shape -->
            <path d="M0,-50 C-30,-20 -50,10 -50,35 C-50,55 -35,70 -15,70 C-5,70 3,65 8,58 L0,90 L-20,90 L-20,100 L20,100 L20,90 L0,90 L-8,58 C-3,65 5,70 15,70 C35,70 50,55 50,35 C50,10 30,-20 0,-50 Z" 
                  fill="#1a1a1a"/>
        </g>
    </g>
</svg>
"@

$splashSvg | Out-File -FilePath "$splashPath\splash.svg" -Encoding UTF8 -NoNewline
Write-Host "  [OK] Created splash.svg (playing cards with spade)" -ForegroundColor Cyan

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Successfully created app icon files!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Files created:" -ForegroundColor Yellow
Write-Host "  - $appIconPath\appicon.svg (green background)"
Write-Host "  - $appIconPath\appiconfg.svg (playing cards foreground)"
Write-Host "  - $splashPath\splash.svg (splash screen)"
Write-Host ""
Write-Host "Remember to update the colors in Blackjack.csproj:" -ForegroundColor Yellow
Write-Host "  Change Color=`"#512BD4`" to Color=`"$casinoGreen`"" -ForegroundColor Yellow
