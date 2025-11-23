# PowerShell script to create custom playing card SVG files
# Modern, minimalist design with consistent style

# Card dimensions
$cardWidth = 200
$cardHeight = 280

# Suit symbols (Unicode)
$suits = @{
    'hearts' = [char]0x2665
    'diamonds' = [char]0x2666
    'clubs' = [char]0x2663
    'spades' = [char]0x2660
}

# Suit colors
$suitColors = @{
    'hearts' = '#DC143C'
    'diamonds' = '#DC143C'
    'clubs' = '#000000'
    'spades' = '#000000'
}

# Rank name mapping
$rankNames = @{
    2 = 'two'; 3 = 'three'; 4 = 'four'; 5 = 'five'; 6 = 'six'
    7 = 'seven'; 8 = 'eight'; 9 = 'nine'; 10 = 'ten'
}

# Function to create all cards with unified design
function Create-Card($suit, $rank, $rankDisplay, $symbol, $color) {
    $suitName = $suit.ToLower()
    $fileName = "${rank}_${suitName}.svg"
    
    $centerX = $cardWidth / 2
    $centerY = $cardHeight / 2
    
    $svg = @"
<?xml version="1.0" encoding="UTF-8"?>
<svg width="$cardWidth" height="$cardHeight" viewBox="0 0 $cardWidth $cardHeight" xmlns="http://www.w3.org/2000/svg">
  <!-- Card background -->
  <rect x="5" y="5" width="190" height="270" rx="10" fill="#ffffff" stroke="#333333" stroke-width="2"/>
  
  <!-- Top-left corner -->
  <text x="20" y="45" font-family="Arial, sans-serif" font-size="40" font-weight="bold" fill="$color">$rankDisplay</text>
  <text x="20" y="80" font-family="Arial, sans-serif" font-size="36" fill="$color">$symbol</text>
  
  <!-- Bottom-right corner (rotated) -->
  <g transform="rotate(180, $centerX, $centerY)">
    <text x="20" y="45" font-family="Arial, sans-serif" font-size="40" font-weight="bold" fill="$color">$rankDisplay</text>
    <text x="20" y="80" font-family="Arial, sans-serif" font-size="36" fill="$color">$symbol</text>
  </g>
  
  <!-- Center symbol -->
  <text x="$centerX" y="$($centerY + 40)" font-family="Arial, sans-serif" font-size="120" fill="$color" text-anchor="middle">$symbol</text>
</svg>
"@
    
    $svg | Out-File -FilePath "src\Blackjack\Resources\Images\Cards\$fileName" -Encoding UTF8 -NoNewline
}

# Function to create card back
function Create-CardBack() {
    $centerX = $cardWidth / 2
    $centerY = $cardHeight / 2
    
    $svg = @"
<?xml version="1.0" encoding="UTF-8"?>
<svg width="$cardWidth" height="$cardHeight" viewBox="0 0 $cardWidth $cardHeight" xmlns="http://www.w3.org/2000/svg">
  <!-- Card background -->
  <rect x="5" y="5" width="190" height="270" rx="10" fill="#1a472a" stroke="#333333" stroke-width="2"/>
  
  <!-- Inner border -->
  <rect x="15" y="15" width="170" height="250" rx="8" fill="none" stroke="#FFD700" stroke-width="2"/>
  
  <!-- Diamond pattern -->
  <pattern id="diamonds" x="0" y="0" width="40" height="40" patternUnits="userSpaceOnUse">
    <path d="M 20 0 L 40 20 L 20 40 L 0 20 Z" fill="none" stroke="#FFD700" stroke-width="1" opacity="0.3"/>
  </pattern>
  <rect x="15" y="15" width="170" height="250" rx="8" fill="url(#diamonds)"/>
  
  <!-- Center design -->
  <circle cx="$centerX" cy="$centerY" r="30" fill="none" stroke="#FFD700" stroke-width="3"/>
  <circle cx="$centerX" cy="$centerY" r="20" fill="none" stroke="#FFD700" stroke-width="2"/>
  <text x="$centerX" y="$($centerY + 8)" font-family="Arial, sans-serif" font-size="24" fill="#FFD700" text-anchor="middle" font-weight="bold">$([char]0x2660)$([char]0x2665)$([char]0x2663)$([char]0x2666)</text>
</svg>
"@
    
    $svg | Out-File -FilePath "src\Blackjack\Resources\Images\Cards\card_back.svg" -Encoding UTF8 -NoNewline
}

# Generate all cards
Write-Host "Generating custom playing cards..." -ForegroundColor Green
Write-Host ""

$cardCount = 0

foreach ($suitKey in $suits.Keys) {
    $symbol = $suits[$suitKey]
    $color = $suitColors[$suitKey]
    $suitName = $suitKey.Substring(0,1).ToUpper() + $suitKey.Substring(1)
    
    Write-Host "Creating $suitName cards..." -ForegroundColor Cyan
    
    # Ace
    Create-Card -suit $suitKey -rank 'ace' -rankDisplay 'A' -symbol $symbol -color $color
    $cardCount++
    Write-Host "  [OK] Ace of $suitName"
    
    # Number cards 2-10
    for ($r = 2; $r -le 10; $r++) {
        $rankName = $rankNames[$r]
        Create-Card -suit $suitKey -rank $rankName -rankDisplay $r -symbol $symbol -color $color
        $cardCount++
        Write-Host "  [OK] $r of $suitName"
    }
    
    # Jack
    Create-Card -suit $suitKey -rank 'jack' -rankDisplay 'J' -symbol $symbol -color $color
    $cardCount++
    Write-Host "  [OK] Jack of $suitName"
    
    # Queen
    Create-Card -suit $suitKey -rank 'queen' -rankDisplay 'Q' -symbol $symbol -color $color
    $cardCount++
    Write-Host "  [OK] Queen of $suitName"
    
    # King
    Create-Card -suit $suitKey -rank 'king' -rankDisplay 'K' -symbol $symbol -color $color
    $cardCount++
    Write-Host "  [OK] King of $suitName"
    
    Write-Host ""
}

# Card back
Write-Host "Creating card back..." -ForegroundColor Cyan
Create-CardBack
$cardCount++
Write-Host "  [OK] Card back"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Successfully created $cardCount card files!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Features:" -ForegroundColor Yellow
Write-Host "  - Unified minimalist design for all cards"
Write-Host "  - Proper naming: two_hearts.svg, jack_spades.svg, etc."
Write-Host "  - All cards have consistent layout"
