# Blackjack MAUI Application - Implementation Plan

## Document Information
- **Project Name**: Blackjack MAUI Application
- **Version**: 1.0
- **Last Updated**: November 22, 2025
- **Related Document**: PROJECT_SPECIFICATION.md
- **Target Platforms**: Windows, Android, iOS
- **Technology Stack**: .NET 10 MAUI (Multi-platform App UI)

---

## Asset Strategy and Resources

### Generated Assets (Completed)

All core visual assets have been generated using PowerShell scripts and are ready for use:

#### 1. Playing Cards
- **Location**: `Resources/Images/Cards/`
- **Content**: All 52 playing cards + card back design
- **Format**: SVG (scalable vector graphics)
- **Generation Script**: `create_cards.ps1`
- **Details**: 
  - Cards use classic playing card designs
  - All four suits (Hearts, Diamonds, Clubs, Spades)
  - All ranks (2-10, Jack, Queen, King, Ace)
  - Card back design for face-down cards

#### 2. Betting Chips
- **Location**: `Resources/Images/Chips/`
- **Content**: 5 chip denominations ($1, $5, $25, $100, $500)
- **Format**: SVG
- **Generation Script**: `create_chips.ps1`
- **Details**:
  - $1 chip: White with blue accents
  - $5 chip: Red
  - $25 chip: Green
  - $100 chip: Black
  - $500 chip: Purple
  - All chips include denomination labels and casino-style designs

#### 3. Table Felt
- **Location**: `Resources/Images/table_felt.svg`
- **Format**: SVG (1920x1080, 16:9 ratio)
- **Generation Script**: `create_table_felt.ps1`
- **Details**:
  - Realistic casino green felt with radial gradient
  - Subtle texture overlay for authentic felt appearance
  - 7 player betting positions arranged in arc (labeled 1-7)
  - Dealer area at top with card outline
  - Insurance line with "INSURANCE PAYS 2:1" text
  - Professional gold accents (#d4af37)
  - Decorative corner elements
  - "BLACKJACK" and "Blackjack Pays 3 to 2" branding

### Icon Strategy (Using Existing Libraries)

Instead of generating custom icons, the application will use **Fluent UI System Icons** and/or **Material Design Icons** for all UI elements:

#### AI Player Avatars
- **Source**: Fluent UI `Person` icon
- **Implementation**: Use same Person icon with different color variations to distinguish AI players
- **Colors**: Assign unique colors (e.g., red, blue, green, yellow, purple, orange) to each AI player
- **Benefit**: Clean, consistent design without additional asset generation

#### Gameplay Action Buttons
| Action | Fluent Icon | Material Icon Alternative |
|--------|-------------|---------------------------|
| Hit | `arrow_upward` or `add` | `arrow_upward` |
| Stand | `pan_tool` (hand) | `stop` or `back_hand` |
| Double Down | `keyboard_double_arrow_down` | `unfold_more` |
| Split | `call_split` | `unfold_more_horizontal` |
| Insurance | `shield` | `security` |
| Confirm | `check` or `check_circle` | `done` |
| Cancel/Clear | `close` or `cancel` | `clear` |

#### App Interface Icons
| Element | Fluent Icon | Material Icon Alternative |
|---------|-------------|---------------------------|
| Settings | `settings` | `tune` |
| Statistics | `bar_chart` | `analytics` |
| Info/Help | `info` | `help` |
| Sound On | `volume_up` | `volume_up` |
| Sound Off | `volume_off` | `volume_mute` |
| Home | `home` | `home` |
| Menu | `menu` | `menu` |
| Back | `arrow_back` | `arrow_back` |

### Icon Library Resources

**Fluent UI System Icons**
- GitHub: https://github.com/microsoft/fluentui-system-icons
- License: MIT (free for commercial use)
- Format: SVG, PNG, Font
- Installation: NuGet package or direct SVG download

**Material Design Icons**
- Website: https://fonts.google.com/icons
- License: Apache 2.0 (free for commercial use)
- Format: SVG, PNG, Font
- Installation: NuGet package or direct download

### Optional Future Assets

The following assets are not currently required but may be added in future iterations:

- **Custom App Icon**: Replace default dotnet_bot placeholder with blackjack-themed icon
- **Custom Splash Screen**: Replace default splash with branded casino design
- **Win/Loss Animations**: Particle effects or celebratory graphics
- **Game State Indicators**: Custom visual indicators for specific game states

---

## Implementation Phases

### Phase 1: Core Game Logic (Foundation) - ✅ COMPLETE
**Goal**: Build the fundamental game mechanics and data structures
**Status**: 100% Complete (5 of 5 sections complete)
**Last Updated**: November 23, 2025, 3:11 AM

- [x] **Data Models** ✅ COMPLETE
  - [x] Implement Card model with Suit and Rank enums
  - [x] Implement Deck model with 6-deck shoe (312 cards)
  - [x] Implement Hand model with value calculation
  - [x] Implement Player model with bankroll management
  - [x] Implement Dealer model
  - [x] Implement GameState model with phase tracking
  - [x] Implement GameSettings model

- [x] **Deck Management** ✅ COMPLETE
  - [x] Create 6-deck shoe initialization
  - [x] Implement shuffle algorithm (Fisher-Yates)
  - [x] Add shuffle point detection (75% penetration ~234 cards dealt)
  - [x] Implement card dealing logic

- [x] **Hand Evaluation** ✅ COMPLETE
  - [x] Calculate hand total value (automatic Ace handling)
  - [x] Detect soft hands (Ace as 11)
  - [x] Detect hard hands (Ace as 1 or no Ace)
  - [x] Detect blackjack (natural 21)
  - [x] Detect bust (over 21)
  - [x] Detect pairs for splitting

- [x] **Basic Strategy Tables** ✅ COMPLETE
  - [x] Build hard totals strategy table (H17 rules) - BasicStrategy.cs
  - [x] Build soft totals strategy table (H17 rules) - BasicStrategy.cs
  - [x] Build pair splitting strategy table - BasicStrategy.cs
  - [x] Create strategy lookup service - GetRecommendedAction()

- [x] **Game Rule Enforcement** ✅ COMPLETE
  - [x] Implement H17 dealer rule (dealer hits soft 17) - GameRules.cs
  - [x] Implement split rules (max 4 hands, Aces get 1 card) - GameRules.cs
  - [x] Implement double down rules (available after split except Aces) - GameRules.cs
  - [x] Implement insurance logic (2:1 payout) - GameRules.cs
  - [x] Implement even money logic (1:1 on blackjack vs Ace) - GameRules.cs
  - [x] Implement payout calculations (3:2 blackjack, 1:1 win, push) - GameRules.cs

### Phase 2: Basic UI (Visual Foundation) - ✅ COMPLETE
**Goal**: Create the user interface structure and visual components
**Status**: 100% Complete (8 of 8 tasks complete)
**Last Updated**: November 23, 2025, 3:11 AM

- [x] **Asset Acquisition** ✅ COMPLETE
  - [x] Playing card graphics (52 cards + back) - GENERATED via create_cards.ps1
  - [x] Betting chip graphics (5 denominations) - GENERATED via create_chips.ps1
  - [x] Table felt background - GENERATED via create_table_felt.ps1
  - [x] AI player avatars strategy - USE Fluent UI Person icon with color variations
  - [x] UI icons strategy - USE Fluent UI / Material Design icon libraries
  - [x] Integrate Fluent UI icon library into MAUI project - FluentIcons.Maui v1.2.315

- [x] **MVVM Setup** ✅ COMPLETE
  - [x] Set up dependency injection in MauiProgram.cs
  - [x] Create ViewModelBase class (with IsBusy, Title, InitializeAsync)
  - [x] Set up navigation shell (AppShell.xaml configured with MainMenuPage)
  - [x] Add CommunityToolkit.Mvvm v8.4.0 for source generators

- [x] **Color Theme** ✅ COMPLETE
  - [x] Updated Colors.xaml with modern blue color palette
    - Primary (Deep Blue): #1E3A8A
    - Secondary (Sky Blue): #3B82F6
    - Tertiary (Light Blue): #DBEAFE
    - Slate Gray: #64748B
    - Success (Emerald): #10B981
    - Warning (Amber): #F59E0B
    - Error (Rose): #EF4444
  - [x] Updated Styles.xaml to use new color palette (fixed MidnightBlue and Magenta references)

- [x] **Main Menu Page** ✅ COMPLETE
  - [x] Create MainMenuPage.xaml layout with modern blue gradient background
  - [x] Create MainMenuViewModel with RelayCommands
  - [x] Add start game button with navigation (shows "Coming Soon" placeholder)
  - [x] Add settings button (shows "Coming Soon" placeholder for Phase 5)
  - [x] Add statistics button (shows "Coming Soon" placeholder for Phase 5)
  - [x] Add platform-specific exit button (visible on Windows only)
  - [x] Add app icon with Cards FluentIcon in circular badge
  - [x] Add title "Blackjack" and subtitle "Play Vegas-style Blackjack"
  - [x] Add version footer "v1.0"
  - [x] Implement dependency injection for ViewModel and Page


- [x] **Seat Selection Interface** ✅ COMPLETE
  - [x] Create SeatSelectionPage.xaml
  - [x] Create SeatSelectionViewModel
  - [x] Display 7-seat table layout
  - [x] Allow player to select seat (positions 1-7)
  - [x] Add AI player count configuration (0-6)
  - [x] Show seat positions (first base to third base)
  - [x] Implement code-behind for dynamic seat rendering

- [x] **Game Table Layout** ✅ COMPLETE
  - [x] Create GameTablePage.xaml
  - [x] Create GameTableViewModel
  - [x] Design dealer area (up card, hole card, total)
  - [x] Design 7 player position areas with arc layout
  - [x] Add player name/avatar display
  - [x] Add bet amount display per seat
  - [x] Add hand total display per seat
  - [x] Add active player indicator
  - [x] Implement code-behind for dynamic player position rendering
  - [x] Add action buttons (Hit, Stand, Double, Split, Insurance)
  - [x] Add status bar with bankroll and menu button

- [x] **Betting Interface** ✅ COMPLETE
  - [x] Create chip buttons ($1, $5, $25, $100, $500)
  - [x] Add current bet display with large formatted text
  - [x] Add clear bet button
  - [x] Add confirm bet button (DEAL)
  - [x] Display player bankroll in status bar
  - [x] Implement bet validation (min/max, bankroll check)
  - [x] Add GameSettings property to ViewModel
  - [x] Implement AddChipCommand with validation
  - [x] Implement ClearBetCommand
  - [x] Implement ConfirmBetCommand with deduction
  - [x] Add visual feedback (opacity for disabled state)
  - [x] Display table limits
  - [x] Show/hide betting UI based on IsBetting property

- [x] **Card Display Components** ✅ COMPLETE
  - [x] Create card image component (CardToImageConverter.cs)
  - [x] Add card positioning logic (CollectionView in GameTablePage.xaml)
  - [x] Add face-down card rendering (CardFaceConverter.cs for future use)
  - [x] Create card display system with test data (Dealer shows Ace of Spades + King of Hearts)

### Phase 3: Game Flow (Integration)
**Goal**: Connect game logic with UI and implement complete game rounds

- [ ] **Betting Phase**
  - [ ] Enable player betting controls
  - [ ] Implement AI betting logic
  - [ ] Validate bet amounts (min/max)
  - [ ] Handle bet confirmation
  - [ ] Deduct bets from bankrolls

- [ ] **Card Dealing Sequence**
  - [ ] Implement initial deal (2 cards to each player)
  - [ ] Deal dealer cards (1 up, 1 down)
  - [ ] Add dealing animation timing
  - [ ] Update UI as cards are dealt

- [ ] **Dealer Blackjack Check**
  - [ ] Check for dealer blackjack when showing Ace or 10
  - [ ] Offer insurance when dealer shows Ace
  - [ ] Offer even money when player has blackjack vs dealer Ace
  - [ ] Resolve early if dealer has blackjack

- [ ] **Player Action Handling**
  - [ ] Implement Hit action
  - [ ] Implement Stand action
  - [ ] Implement Double Down action
  - [ ] Implement Split action (with re-split support)
  - [ ] Implement Insurance action
  - [ ] Implement Even Money action
  - [ ] Enable/disable buttons based on valid actions
  - [ ] Handle bust detection
  - [ ] Progress through players in order (positions 1-7)

- [ ] **Dealer Action**
  - [ ] Reveal dealer hole card
  - [ ] Implement H17 dealer logic
  - [ ] Dealer draws until standing
  - [ ] Display dealer total

- [ ] **Settlement and Payout System**
  - [ ] Compare player hands to dealer hand
  - [ ] Calculate winnings (3:2 blackjack, 1:1 win, push)
  - [ ] Update player bankrolls
  - [ ] Display win/loss/push notifications
  - [ ] Clear table for next round

- [ ] **Round Transitions**
  - [ ] Clear cards from previous round
  - [ ] Check if shoe needs shuffling
  - [ ] Return to betting phase
  - [ ] Handle player bankrupt condition

### Phase 4: AI Players (Intelligence)
**Goal**: Implement intelligent computer opponents

- [ ] **BasicStrategy Service Integration**
  - [ ] Create BasicStrategy service class
  - [ ] Implement hard totals strategy lookup
  - [ ] Implement soft totals strategy lookup
  - [ ] Implement pair splitting strategy lookup
  - [ ] Handle fallback logic (e.g., can't double, so hit)

- [ ] **AIPlayer Decision-Making**
  - [ ] Create AIPlayer service class
  - [ ] Implement decision logic using BasicStrategy
  - [ ] Check for pairs first
  - [ ] Check for soft hands second
  - [ ] Default to hard totals
  - [ ] Handle split hand re-evaluation

- [ ] **AI Seat Placement**
  - [ ] Randomly place AI players at available seats
  - [ ] Exclude player's chosen seat
  - [ ] Ensure correct number of AI players

- [ ] **AI Identity Generation**
  - [ ] Generate random names for AI players
  - [ ] Assign random avatars to AI players
  - [ ] Display AI identity in UI

- [ ] **AI Betting Logic**
  - [ ] Implement varied betting patterns
  - [ ] Respect table min/max
  - [ ] Add slight randomization for realism

- [ ] **Turn Delays and Animations**
  - [ ] Add thinking delay for AI turns
  - [ ] Animate AI card draws
  - [ ] Show AI decisions in UI
  - [ ] Maintain game pacing

### Phase 5: Polish and Features (Enhancement)
**Goal**: Add polish, additional features, and platform optimizations

- [ ] **Statistics Tracking**
  - [ ] Create StatisticsService
  - [ ] Track hands played
  - [ ] Track hands won/lost/pushed
  - [ ] Track total winnings/losses
  - [ ] Track blackjacks dealt
  - [ ] Calculate win percentage
  - [ ] Create statistics display page

- [ ] **Save/Load Game**
  - [ ] Implement game state serialization
  - [ ] Save game to local storage
  - [ ] Load game from local storage
  - [ ] Handle save game on app close
  - [ ] Add continue game option

- [ ] **Settings Menu**
  - [ ] Create SettingsPage.xaml and ViewModel
  - [ ] Add sound on/off toggle
  - [ ] Add animation speed control
  - [ ] Add table minimum/maximum configuration
  - [ ] Add starting bankroll configuration
  - [ ] Add number of decks display (locked at 6)

- [ ] **Animations**
  - [ ] Smooth card dealing animations
  - [ ] Card flip animations
  - [ ] Chip movement animations
  - [ ] Win/loss celebration animations
  - [ ] Shuffle animation

- [ ] **Sound Effects**
  - [ ] Add card dealing sound
  - [ ] Add chip placement sound
  - [ ] Add win/loss sounds
  - [ ] Add shuffle sound
  - [ ] Add button click sounds
  - [ ] Implement sound toggle

- [ ] **Platform Optimizations**
  - [ ] **Windows**: Desktop layout with mouse/keyboard controls
  - [ ] **Windows**: Window resizing support
  - [ ] **Windows**: Larger table view for desktop
  - [ ] **Android**: Touch-optimized controls
  - [ ] **Android**: Portrait and landscape support
  - [ ] **Android**: Adaptive layout for phones/tablets
  - [ ] **iOS**: Touch-optimized controls
  - [ ] **iOS**: Portrait and landscape support
  - [ ] **iOS**: Adaptive layout for iPhones/iPads
  - [ ] **iOS**: iOS-specific gestures and patterns

### Phase 6: Testing and Refinement (Quality Assurance)
**Goal**: Ensure quality, performance, and correct functionality

- [ ] **Unit Tests - Game Logic**
  - [ ] Test card value calculations
  - [ ] Test hand evaluation (soft/hard/blackjack/bust)
  - [ ] Test deck shuffling and dealing
  - [ ] Test payout calculations
  - [ ] Test dealer H17 logic
  - [ ] Test split rules
  - [ ] Test double down rules
  - [ ] Test insurance calculations

- [ ] **Unit Tests - Basic Strategy**
  - [ ] Test hard totals strategy lookups
  - [ ] Test soft totals strategy lookups
  - [ ] Test pair splitting strategy lookups
  - [ ] Test strategy decision accuracy

- [ ] **UI Testing - Windows**
  - [ ] Test all pages and navigation
  - [ ] Test mouse and keyboard controls
  - [ ] Test window resizing behavior
  - [ ] Test visual layout and spacing
  - [ ] Test animations and transitions

- [ ] **UI Testing - Android**
  - [ ] Test on phone (portrait/landscape)
  - [ ] Test on tablet (portrait/landscape)
  - [ ] Test touch controls and gestures
  - [ ] Test adaptive layouts
  - [ ] Test back button behavior

- [ ] **UI Testing - iOS**
  - [ ] Test on iPhone (portrait/landscape)
  - [ ] Test on iPad (portrait/landscape)
  - [ ] Test touch controls and gestures
  - [ ] Test adaptive layouts
  - [ ] Test iOS-specific navigation

- [ ] **Performance Optimization**
  - [ ] Profile app startup time (target <3 seconds)
  - [ ] Optimize animation frame rates (target 60 FPS)
  - [ ] Implement image caching for cards
  - [ ] Optimize memory usage
  - [ ] Test app responsiveness during AI turns

- [ ] **Acceptance Criteria Verification**
  - [ ] Verify app runs on Windows, Android, iOS
  - [ ] Verify 7-seat table with selection
  - [ ] Verify 0-6 AI player configuration
  - [ ] Verify all player actions work correctly
  - [ ] Verify dealer hits soft 17
  - [ ] Verify no surrender option
  - [ ] Verify AI uses optimal Basic Strategy
  - [ ] Verify correct payouts (3:2, 1:1, 2:1)
  - [ ] Verify 6-deck shoe with shuffle point

- [ ] **Bug Fixes and Refinements**
  - [ ] Address any identified bugs
  - [ ] Refine UI based on testing feedback
  - [ ] Improve animation smoothness
  - [ ] Polish visual design
  - [ ] Optimize code structure

---

## Document Control

**Version History**:
- v1.0 (November 22, 2025) - Initial implementation plan document

**Related Documents**:
- PROJECT_SPECIFICATION.md - Complete project requirements and specifications

**Review Schedule**:
- Update progress as phases are completed
- Review before starting each new phase
