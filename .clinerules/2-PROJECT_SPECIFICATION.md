# Blackjack MAUI Application - Project Specification

## Document Information
- **Project Name**: Blackjack MAUI Application
- **Version**: 1.0
- **Last Updated**: November 22, 2025
- **Target Platforms**: Windows, Android, iOS
- **Technology Stack**: .NET 10 MAUI (Multi-platform App UI)

---
## 0. Absolute Rules
- DO NOT AUTOMATICALLY BUILD THE PROJECT/SOLUTION
- DO NOT AUTOMATICALLY RUN THE PROJECT/SOLUTION

## 1. Project Overview

### 1.1 Purpose
Develop a cross-platform single-player Blackjack application that simulates the authentic US casino Blackjack experience. The application will allow players to compete against AI-controlled computer players at a virtual Blackjack table.

### 1.2 Target Platforms
- **Windows**: Desktop application
- **Android**: Mobile application (tablets and phones)
- **iOS**: Mobile application (tablets and phones)

### 1.3 Key Features
- Authentic US casino Blackjack rules
- 7-seat table with player seat selection
- Configurable number of AI opponents (0-6)
- AI players using optimal Basic Strategy
- Standard casino betting and gameplay options

---

## 2. Game Specifications

### 2.1 Table Configuration
- **Number of Seats**: 7 positions (standard US casino table layout)
- **Positions**: Numbered 1-7 from dealer's left to right (first base to third base)
- **Player Selection**: User chooses one seat position before gameplay begins
- **AI Players**: User configures how many computer players (0-6) to fill remaining seats

### 2.2 Deck and Shoe Configuration
- **Number of Decks**: 6-deck shoe (standard in most US casinos)
- **Cards per Deck**: 52 cards (13 ranks × 4 suits)
- **Total Cards**: 312 cards in play
- **Shuffle Point**: Reshuffle when approximately 75% of shoe is depleted (~234 cards dealt)
- **Card Values**:
  - Number cards (2-10): Face value
  - Face cards (J, Q, K): 10 points
  - Ace: 1 or 11 points (player's choice, automatically optimized)

### 2.3 Dealer Rules
- **Dealer Hit Rule**: Dealer MUST hit on soft 17 (H17)
- **Dealer Stand Rule**: Dealer MUST stand on hard 17 and all 18+
- **Dealer Blackjack**: Dealer checks for Blackjack when showing Ace or 10-value card
- **Hole Card**: Dealer receives one face-up card and one face-down card initially

### 2.4 Betting and Payouts
- **Minimum Bet**: Configurable (default: $5)
- **Maximum Bet**: Configurable (default: $500)
- **Blackjack Payout**: 3:2 (e.g., $10 bet pays $15)
- **Regular Win Payout**: 1:1 (even money)
- **Insurance Payout**: 2:1
- **Push**: Original bet returned (no win/loss)

### 2.5 Game Flow
1. **Betting Phase**: All players place bets
2. **Initial Deal**: Each player receives 2 cards face-up, dealer receives 1 up and 1 down
3. **Dealer Blackjack Check**: If dealer shows Ace or 10-value, check for Blackjack
4. **Insurance Offer**: If dealer shows Ace, offer insurance to players
5. **Player Actions**: Each player acts in turn (positions 1-7)
6. **Dealer Action**: Dealer reveals hole card and plays according to house rules
7. **Settlement**: Winnings/losses calculated and distributed
8. **Next Round**: Cards cleared, betting phase begins again

---

## 3. Player Actions

### 3.1 Standard Actions

#### 3.1.1 Hit
- **Description**: Request an additional card
- **Availability**: Available on any hand total of 20 or less
- **Effect**: Player receives one additional card
- **Result**: If total exceeds 21, player busts and loses bet

#### 3.1.2 Stand
- **Description**: Keep current hand and end turn
- **Availability**: Always available
- **Effect**: No additional cards received, turn passes to next player
- **Result**: Hand is finalized and awaits dealer outcome

#### 3.1.3 Double Down
- **Description**: Double the original bet and receive exactly one more card
- **Availability**: 
  - Available on initial 2-card hand only
  - Typically allowed on any two cards (most US casinos)
  - Available after splitting (except on split Aces)
- **Effect**: 
  - Bet is doubled
  - Receive exactly one card
  - Turn automatically ends
- **Result**: Higher risk/reward play

#### 3.1.4 Split
- **Description**: Separate a pair into two independent hands
- **Availability**:
  - Available when dealt two cards of the same rank
  - Examples: [8,8], [A,A], [K,Q] (all 10-value cards)
- **Rules**:
  - Each hand requires a bet equal to the original bet
  - Each hand is played independently
  - May re-split if another pair is dealt (maximum 4 hands total)
  - Split Aces receive only one additional card each
  - Blackjack after split counts as 21, not Blackjack (pays 1:1, not 3:2)
- **Effect**: Create two separate hands from one
- **Result**: Two independent hands to play

#### 3.1.5 Insurance
- **Description**: Side bet that dealer has Blackjack
- **Availability**: 
  - Offered only when dealer shows an Ace
  - Available before any other player actions
- **Rules**:
  - Costs half of original bet
  - Pays 2:1 if dealer has Blackjack
  - Lost if dealer does not have Blackjack
- **Effect**: Protection against dealer Blackjack
- **Result**: Side bet resolved before main hand

#### 3.1.6 Even Money
- **Description**: Accept 1:1 payout immediately when player has Blackjack and dealer shows Ace
- **Availability**:
  - Offered only when player has Blackjack (natural 21) and dealer shows an Ace
  - Functionally equivalent to taking insurance on a Blackjack
- **Rules**:
  - Player receives 1:1 payout immediately (even money)
  - Hand is settled before dealer checks hole card
  - Guarantees a win regardless of dealer's hole card
- **Effect**: Guaranteed win instead of risking a push
- **Result**: Immediate 1:1 payout

### 3.2 Actions NOT Included
- **Surrender**: NOT available (per project requirements)
- **Late Surrender**: NOT available

---

## 4. Computer AI Strategy

### 4.1 Basic Strategy Overview
Computer players will implement the mathematically optimal Basic Strategy for H17 (dealer hits soft 17) rules. This strategy is based on:
- Dealer's up-card
- Player's hand total
- Whether the hand is hard or soft
- Whether the hand is a pair eligible for splitting

### 4.2 Hard Totals Strategy (No Ace or Ace counted as 1)

| Player Hand | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | A |
|-------------|---|---|---|---|---|---|---|---|----|----|
| 17-20       | S | S | S | S | S | S | S | S | S  | S  |
| 16          | S | S | S | S | S | H | H | H | H  | H  |
| 15          | S | S | S | S | S | H | H | H | H  | H  |
| 13-14       | S | S | S | S | S | H | H | H | H  | H  |
| 12          | H | H | S | S | S | H | H | H | H  | H  |
| 11          | D | D | D | D | D | D | D | D | D  | D  |
| 10          | D | D | D | D | D | D | D | D | H  | H  |
| 9           | H | D | D | D | D | H | H | H | H  | H  |
| 5-8         | H | H | H | H | H | H | H | H | H  | H  |

**Legend**: S = Stand, H = Hit, D = Double (if not allowed, then Hit)

### 4.3 Soft Totals Strategy (Ace counted as 11)

| Player Hand | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | A |
|-------------|---|---|---|---|---|---|---|---|----|----|
| A,9 (20)    | S | S | S | S | S | S | S | S | S  | S  |
| A,8 (19)    | S | S | S | S | D | S | S | S | S  | S  |
| A,7 (18)    | D | D | D | D | D | S | S | H | H  | H  |
| A,6 (17)    | H | D | D | D | D | H | H | H | H  | H  |
| A,5 (16)    | H | H | D | D | D | H | H | H | H  | H  |
| A,4 (15)    | H | H | D | D | D | H | H | H | H  | H  |
| A,3 (14)    | H | H | H | D | D | H | H | H | H  | H  |
| A,2 (13)    | H | H | H | D | D | H | H | H | H  | H  |

**Legend**: S = Stand, H = Hit, D = Double (if not allowed, then Hit)

### 4.4 Pair Splitting Strategy

| Player Pair | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | A |
|-------------|---|---|---|---|---|---|---|---|----|----|
| A,A         | P | P | P | P | P | P | P | P | P  | P  |
| 10,10       | S | S | S | S | S | S | S | S | S  | S  |
| 9,9         | P | P | P | P | P | S | P | P | S  | S  |
| 8,8         | P | P | P | P | P | P | P | P | P  | P  |
| 7,7         | P | P | P | P | P | P | H | H | H  | H  |
| 6,6         | P | P | P | P | P | H | H | H | H  | H  |
| 5,5         | D | D | D | D | D | D | D | D | H  | H  |
| 4,4         | H | H | H | P | P | H | H | H | H  | H  |
| 3,3         | P | P | P | P | P | P | H | H | H  | H  |
| 2,2         | P | P | P | P | P | P | H | H | H  | H  |

**Legend**: P = Split, S = Stand, H = Hit, D = Double (if not allowed, then Hit)

### 4.5 Insurance Strategy
- **Basic Strategy Rule**: NEVER take insurance
- **Reasoning**: Insurance is a side bet with a negative expected value for the player
- **Exception**: Card counters may take insurance when the count indicates a 10-rich deck (NOT implemented in this version)

### 4.6 AI Decision-Making Implementation
1. **Check if pair**: If dealt a pair, consult pair splitting table
2. **Check if soft hand**: If hand contains an Ace counted as 11, consult soft totals table
3. **Default to hard total**: Consult hard totals table
4. **Double Down fallback**: If double is optimal but not allowed (e.g., after hitting), default to Hit
5. **Split re-evaluation**: After splitting, treat each hand independently and re-evaluate using steps 1-4

---

## 5. Game Features and Configuration

### 5.1 Pre-Game Setup
- **Seat Selection Screen**:
  - Display 7 seats around virtual table
  - Allow player to select their preferred position
  - Position 1: First base (leftmost, acts first)
  - Position 7: Third base (rightmost, acts last)
  
- **AI Player Configuration**:
  - Slider or number input to select AI player count (0-6)
  - AI players randomly placed at available seats (excluding player's chosen seat)
  - Each AI player assigned a randomly generated name
  - Each AI player assigned a randomly generated avatar
  - Display AI player avatars/names for each occupied seat

- **Betting Configuration**:
  - Set table minimum bet
  - Set table maximum bet
  - Set player's starting bankroll

### 5.2 User Interface Elements

#### 5.2.1 Game Table View
- **Dealer Area**:
  - Dealer's up-card (visible)
  - Dealer's hole card (face-down until dealer's turn)
  - Dealer's hand total (shown after reveal)
  
- **Player Positions** (7 seats):
  - Player cards displayed face-up
  - Current bet amount
  - Hand total
  - Active player indicator
  - Player name/avatar
  
- **Human Player Controls**:
  - Hit button
  - Stand button
  - Double Down button (enabled when applicable)
  - Split button (enabled when applicable)
  - Insurance button (enabled when dealer shows Ace)
  
- **Game Information**:
  - Current player bankroll
  - Shoe status (cards remaining indicator)
  - Round outcome notifications
  
#### 5.2.2 Betting Interface
- Betting chips with denominations ($1, $5, $25, $100, $500)
- Current bet display
- Clear bet button
- Confirm bet button

#### 5.2.3 Animation and Feedback
- Card dealing animations
- Win/loss notifications
- Chip movement animations
- Sound effects (optional, toggleable)

### 5.3 Game State Management
- **Save/Load Game**: Ability to save current game state and resume later
- **Statistics Tracking**:
  - Hands played
  - Hands won/lost/pushed
  - Total winnings/losses
  - Blackjacks dealt
  - Win percentage
  
- **Settings Menu**:
  - Sound on/off
  - Animation speed
  - Table minimum/maximum
  - Starting bankroll
  - Number of decks (locked at 6 for this version)

---

## 6. Technical Architecture

### 6.1 MAUI Project Structure
```
Blackjack/
├── App.xaml / App.xaml.cs          (Application entry point)
├── AppShell.xaml / AppShell.cs     (Shell navigation)
├── MauiProgram.cs                   (Dependency injection setup)
├── Models/                          (Data models)
│   ├── Card.cs
│   ├── Deck.cs
│   ├── Hand.cs
│   ├── Player.cs
│   ├── Dealer.cs
│   ├── GameState.cs
│   └── GameSettings.cs
├── ViewModels/                      (MVVM view models)
│   ├── MainMenuViewModel.cs
│   ├── GameTableViewModel.cs
│   ├── SettingsViewModel.cs
│   └── StatisticsViewModel.cs
├── Views/                           (XAML pages)
│   ├── MainMenuPage.xaml
│   ├── SeatSelectionPage.xaml
│   ├── GameTablePage.xaml
│   ├── SettingsPage.xaml
│   └── StatisticsPage.xaml
├── Services/                        (Business logic)
│   ├── GameEngine.cs
│   ├── BasicStrategy.cs
│   ├── AIPlayer.cs
│   └── StatisticsService.cs
├── Resources/                       (Images, styles, etc.)
│   ├── Images/
│   │   └── Cards/                   (52 card images + card back - open source)
│   ├── Styles/
│   └── Fonts/
└── Platforms/                       (Platform-specific code)
    ├── Windows/
    ├── Android/
    └── iOS/
```

### 6.2 Core Data Models

#### 6.2.1 Card Model
```csharp
public class Card
{
    public Suit Suit { get; set; }
    public Rank Rank { get; set; }
    public int Value { get; set; }
    public string ImagePath { get; set; }
}

public enum Suit { Hearts, Diamonds, Clubs, Spades }
public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }
```

#### 6.2.2 Hand Model
```csharp
public class Hand
{
    public List<Card> Cards { get; set; }
    public int TotalValue { get; }
    public bool IsSoft { get; }
    public bool IsBlackjack { get; }
    public bool IsBusted { get; }
    public bool IsPair { get; }
    public decimal Bet { get; set; }
    public HandStatus Status { get; set; }
}

public enum HandStatus { Active, Standing, Busted, Blackjack, Won, Lost, Push }
```

#### 6.2.3 Player Model
```csharp
public class Player
{
    public string Name { get; set; }
    public int SeatPosition { get; set; }
    public decimal Bankroll { get; set; }
    public List<Hand> Hands { get; set; }
    public bool IsHuman { get; set; }
    public bool IsActive { get; set; }
}
```

#### 6.2.4 GameState Model
```csharp
public class GameState
{
    public List<Player> Players { get; set; }
    public Dealer Dealer { get; set; }
    public Deck Shoe { get; set; }
    public GamePhase CurrentPhase { get; set; }
    public Player ActivePlayer { get; set; }
    public GameSettings Settings { get; set; }
}

public enum GamePhase { Betting, Dealing, InsuranceOffer, PlayerActions, DealerAction, Settlement, Shuffling }
```

### 6.3 Key Services

#### 6.3.1 GameEngine
- Manages game flow and state transitions
- Enforces game rules
- Handles betting and payouts
- Coordinates AI and human player actions

#### 6.3.2 BasicStrategy
- Implements optimal strategy tables
- Returns recommended action based on game state
- Used by AI players for decision-making

#### 6.3.3 AIPlayer
- Simulates computer player behavior
- Uses BasicStrategy for decisions
- Manages AI betting patterns

### 6.4 Multi-Platform Considerations

#### 6.4.1 Windows
- Desktop-optimized layout with mouse/keyboard controls
- Larger screen real estate for detailed table view
- Window resizing support

#### 6.4.2 Android
- Touch-optimized controls
- Portrait and landscape orientations
- Adaptive layout for phones and tablets
- Gesture support (swipe to view stats, etc.)

#### 6.4.3 iOS
- Touch-optimized controls
- Portrait and landscape orientations
- Adaptive layout for iPhones and iPads
- iOS-specific gestures and navigation patterns

### 6.5 Performance Considerations
- Efficient card dealing animations (avoid blocking UI)
- Optimize card image loading (use image caching)
- Async/await for AI decision delays (simulate "thinking")
- Responsive UI during AI turns

### 6.6 Assets and Graphics
- **Card Graphics**: Use open-source, royalty-free playing card images
  - Sources: Public domain card decks (e.g., from OpenGameArt.org, Wikimedia Commons)
  - License: Must be free to use commercially and distributable
  - Format: PNG or SVG for scalability across devices
  - Requirements: All 52 cards plus card back design
- **UI Elements**: Use open-source icons and graphics where possible
- **Avatars**: Generate or source open-source avatar graphics for AI players
- **Table Felt**: Simple, programmatically generated or open-source texture

---

## 7. Acceptance Criteria

### 7.1 Functional Requirements
- ✅ Application runs on Windows, Android, and iOS
- ✅ 7-seat table with player selection
- ✅ Configurable AI player count (0-6)
- ✅ All standard player actions implemented (Hit, Stand, Double, Split, Insurance)
- ✅ Dealer hits soft 17
- ✅ No surrender option
- ✅ AI players use optimal Basic Strategy
- ✅ Correct payouts (3:2 for Blackjack, 1:1 for wins, 2:1 for insurance)
- ✅ 6-deck shoe with appropriate shuffle point

### 7.2 User Experience Requirements
- ✅ Intuitive, easy-to-use interface
- ✅ Smooth animations and transitions
- ✅ Clear visual feedback for game events
- ✅ Responsive touch controls (mobile)
- ✅ Accessible mouse/keyboard controls (desktop)

### 7.3 Performance Requirements
- ✅ Fast game load time (< 3 seconds)
- ✅ Smooth 60 FPS animations
- ✅ No lag during gameplay
- ✅ Efficient memory usage

### 7.4 Quality Requirements
- ✅ No game-breaking bugs
- ✅ Accurate rule implementation
- ✅ Correct mathematical calculations (payouts, hand totals)
- ✅ Proper state management (no invalid game states)

---

## 8. Glossary

**Blackjack**: A hand consisting of an Ace and a 10-value card, totaling 21. Pays 3:2.

**Basic Strategy**: Mathematically optimal way to play each hand based on probability.

**Bust**: When a hand exceeds 21, resulting in an automatic loss.

**Double Down**: Doubling the original bet in exchange for receiving exactly one more card.

**Hard Hand**: A hand without an Ace, or with an Ace counted as 1 to avoid busting.

**Hit**: Taking an additional card.

**Hole Card**: The dealer's face-down card.

**Insurance**: A side bet offered when the dealer shows an Ace, betting that the dealer has Blackjack.

**Push**: A tie between player and dealer, original bet is returned.

**Shoe**: The device holding multiple decks of cards.

**Soft Hand**: A hand containing an Ace counted as 11.

**Split**: Separating a pair into two independent hands.

**Stand**: Keeping the current hand and ending the turn.

**Up Card**: The dealer's face-up card.

---

## Document Control

**Version History**:
- v1.0 (November 22, 2025) - Initial specification document

**Approvals**:
- Project Owner: [Pending]
- Technical Lead: [Pending]
- QA Lead: [Pending]

**Review Schedule**:
- Review this document before starting each implementation phase
- Update as requirements evolve or clarifications are needed
