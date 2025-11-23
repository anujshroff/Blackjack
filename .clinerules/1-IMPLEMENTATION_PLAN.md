# Blackjack MAUI Application - Implementation Plan

## Document Information
- **Project Name**: Blackjack MAUI Application
- **Version**: 1.0
- **Last Updated**: November 22, 2025
- **Related Document**: PROJECT_SPECIFICATION.md
- **Target Platforms**: Windows, Android, iOS
- **Technology Stack**: .NET 10 MAUI (Multi-platform App UI)

---

## Implementation Phases

### Phase 1: Core Game Logic (Foundation)
**Goal**: Build the fundamental game mechanics and data structures

- [ ] **Data Models**
  - [ ] Implement Card model with Suit and Rank enums
  - [ ] Implement Deck model with 6-deck shoe (312 cards)
  - [ ] Implement Hand model with value calculation
  - [ ] Implement Player model with bankroll management
  - [ ] Implement Dealer model
  - [ ] Implement GameState model with phase tracking
  - [ ] Implement GameSettings model

- [ ] **Deck Management**
  - [ ] Create 6-deck shoe initialization
  - [ ] Implement shuffle algorithm
  - [ ] Add shuffle point detection (75% penetration ~234 cards dealt)
  - [ ] Implement card dealing logic

- [ ] **Hand Evaluation**
  - [ ] Calculate hand total value
  - [ ] Detect soft hands (Ace as 11)
  - [ ] Detect hard hands (Ace as 1 or no Ace)
  - [ ] Detect blackjack (natural 21)
  - [ ] Detect bust (over 21)
  - [ ] Detect pairs for splitting

- [ ] **Basic Strategy Tables**
  - [ ] Build hard totals strategy table (H17 rules)
  - [ ] Build soft totals strategy table (H17 rules)
  - [ ] Build pair splitting strategy table
  - [ ] Create strategy lookup service

- [ ] **Game Rule Enforcement**
  - [ ] Implement H17 dealer rule (dealer hits soft 17)
  - [ ] Implement split rules (max 4 hands, Aces get 1 card)
  - [ ] Implement double down rules (available after split except Aces)
  - [ ] Implement insurance logic (2:1 payout)
  - [ ] Implement even money logic (1:1 on blackjack vs Ace)
  - [ ] Implement payout calculations (3:2 blackjack, 1:1 win, push)

### Phase 2: Basic UI (Visual Foundation)
**Goal**: Create the user interface structure and visual components

- [ ] **Asset Acquisition**
  - [ ] Source open-source card graphics (52 cards + back)
  - [ ] Create or source avatar graphics for AI players
  - [ ] Create table felt background (or use open-source)

- [ ] **MVVM Setup**
  - [ ] Set up dependency injection in MauiProgram.cs
  - [ ] Create ViewModelBase class
  - [ ] Set up navigation shell

- [ ] **Main Menu Page**
  - [ ] Create MainMenuPage.xaml layout
  - [ ] Create MainMenuViewModel
  - [ ] Add start game button
  - [ ] Add settings button
  - [ ] Add statistics button

- [ ] **Seat Selection Interface**
  - [ ] Create SeatSelectionPage.xaml
  - [ ] Create SeatSelectionViewModel
  - [ ] Display 7-seat table layout
  - [ ] Allow player to select seat (positions 1-7)
  - [ ] Add AI player count configuration (0-6)
  - [ ] Show seat positions (first base to third base)

- [ ] **Game Table Layout**
  - [ ] Create GameTablePage.xaml
  - [ ] Create GameTableViewModel
  - [ ] Design dealer area (up card, hole card, total)
  - [ ] Design 7 player position areas
  - [ ] Add player name/avatar display
  - [ ] Add bet amount display per seat
  - [ ] Add hand total display per seat
  - [ ] Add active player indicator

- [ ] **Betting Interface**
  - [ ] Create chip buttons ($1, $5, $25, $100, $500)
  - [ ] Add current bet display
  - [ ] Add clear bet button
  - [ ] Add confirm bet button
  - [ ] Display player bankroll

- [ ] **Card Display Components**
  - [ ] Create card image component
  - [ ] Add card positioning logic
  - [ ] Add face-down card rendering
  - [ ] Create card animation placeholder

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
