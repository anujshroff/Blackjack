# PowerShell script to create casino chip SVG files
# Based on standard casino chip denominations and colors

# Chip dimensions
$chipSize = 100
$centerX = 50
$centerY = 50

# Chip configurations
$chips = @(
    @{
        Value = 1
        OuterRing = '#2c2c2c'
        OuterStroke = '#1a1a1a'
        EdgeDots = '#000000'
        Center = '#f5f5f5'
        CenterStroke = '#d0d0d0'
        DashedRing = '#333333'
        TextColor = '#333333'
        FontSize = 24
    },
    @{
        Value = 5
        OuterRing = '#8B0000'
        OuterStroke = '#5a0000'
        EdgeDots = '#ffffff'
        Center = '#DC143C'
        CenterStroke = '#B00020'
        DashedRing = '#ffffff'
        TextColor = '#ffffff'
        FontSize = 24
    },
    @{
        Value = 25
        OuterRing = '#0d5e0d'
        OuterStroke = '#084508'
        EdgeDots = '#ffffff'
        Center = '#228B22'
        CenterStroke = '#1a6b1a'
        DashedRing = '#ffffff'
        TextColor = '#ffffff'
        FontSize = 22
    },
    @{
        Value = 100
        OuterRing = '#0a0a0a'
        OuterStroke = '#000000'
        EdgeDots = '#ffffff'
        Center = '#1a1a1a'
        CenterStroke = '#333333'
        DashedRing = '#ffffff'
        TextColor = '#ffffff'
        FontSize = 20
    },
    @{
        Value = 500
        OuterRing = '#4B0082'
        OuterStroke = '#2d004e'
        EdgeDots = '#FFD700'
        Center = '#7B2CBF'
        CenterStroke = '#5a1f8f'
        DashedRing = '#FFD700'
        TextColor = '#FFD700'
        FontSize = 20
    }
)

# Function to create a chip SVG
function Create-Chip($config) {
    $value = $config.Value
    $fileName = "chip_${value}.svg"
    
    # Calculate edge dot positions (8 dots evenly spaced around circle)
    $dotPositions = @(
        @{x=50; y=8},           # Top
        @{x=79.4; y=20.6},      # Top-right
        @{x=92; y=50},          # Right
        @{x=79.4; y=79.4},      # Bottom-right
        @{x=50; y=92},          # Bottom
        @{x=20.6; y=79.4},      # Bottom-left
        @{x=8; y=50},           # Left
        @{x=20.6; y=20.6}       # Top-left
    )
    
    # Generate edge dots XML
    $dotsXml = ""
    foreach ($pos in $dotPositions) {
        $dotsXml += "  <circle cx=`"$($pos.x)`" cy=`"$($pos.y)`" r=`"4`" fill=`"$($config.EdgeDots)`"/>`n"
    }
    
    $svg = @"
<?xml version="1.0" encoding="UTF-8"?>
<svg width="$chipSize" height="$chipSize" viewBox="0 0 $chipSize $chipSize" xmlns="http://www.w3.org/2000/svg">
  <circle cx="$centerX" cy="$centerY" r="48" fill="$($config.OuterRing)" stroke="$($config.OuterStroke)" stroke-width="1"/>
$dotsXml  <circle cx="$centerX" cy="$centerY" r="38" fill="$($config.Center)" stroke="$($config.CenterStroke)" stroke-width="2"/>
  <circle cx="$centerX" cy="$centerY" r="28" fill="none" stroke="$($config.DashedRing)" stroke-width="1.5" stroke-dasharray="2,2"/>
  <text x="$centerX" y="42" font-family="Arial, sans-serif" font-size="14" font-weight="bold" fill="$($config.TextColor)" text-anchor="middle">`$</text>
  <text x="$centerX" y="62" font-family="Arial, sans-serif" font-size="$($config.FontSize)" font-weight="bold" fill="$($config.TextColor)" text-anchor="middle">$value</text>
</svg>
"@
    
    $svg | Out-File -FilePath "Resources\Images\Chips\$fileName" -Encoding UTF8 -NoNewline
}

# Generate all chips
Write-Host "Generating casino chip SVG files..." -ForegroundColor Green
Write-Host ""

$chipCount = 0

foreach ($chip in $chips) {
    Create-Chip -config $chip
    $chipCount++
    
    $colorName = switch ($chip.Value) {
        1 { "White/Gray" }
        5 { "Red" }
        25 { "Green" }
        100 { "Black" }
        500 { "Purple" }
    }
    
    Write-Host "  [OK] Created chip_$($chip.Value).svg ($colorName - `$$($chip.Value))" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Successfully created $chipCount chip files!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Chip denominations:" -ForegroundColor Yellow
Write-Host "  - `$1 (White/Gray)"
Write-Host "  - `$5 (Red)"
Write-Host "  - `$25 (Green)"
Write-Host "  - `$100 (Black)"
Write-Host "  - `$500 (Purple)"
