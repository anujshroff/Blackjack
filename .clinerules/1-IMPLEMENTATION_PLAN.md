## Implementation Phases

### Phase 1: Core Game Logic (Foundation) - ✅ COMPLETE
**Goal**: Build the fundamental game mechanics and data structures
### Phase 2: Basic UI (Visual Foundation) - ✅ COMPLETE
**Goal**: Create the user interface structure and visual components
### Phase 3: Game Flow (Integration) - ✅ COMPLETE
**Goal**: Connect game logic with UI and implement complete game rounds
### Phase 4: AI Players (Intelligence) - ✅ COMPLETE
**Goal**: Implement intelligent computer opponents

### Phase 5: Polish and Features (Enhancement)
**Goal**: Add polish, additional features, and platform optimizations

- [ ] **Bankroll Persistence**
  - [ ] Create BankrollService with Save/Load methods using MAUI Preferences API
  - [ ] Save player bankroll after each round settlement completes
  - [ ] Load bankroll on game initialization (fallback to GameSettings.StartingBankroll)
  - [ ] Add "Reset Bankroll" button to MainMenuPage
  - [ ] Auto-reset bankroll on MainMenuPage load if bankroll is $0

- [ ] **Settings Menu**
  - [ ] Create SettingsPage.xaml and ViewModel
  - [ ] Add table minimum configuration (persisted)
  - [ ] Add table maximum configuration (persisted)
  - [ ] Add starting bankroll configuration (persisted)
  - [ ] Add number of decks picker: 1, 2, 4, 6, or 8 (persisted)
  - [ ] Create SettingsService with Save/Load using MAUI Preferences API
  - [ ] Load settings on app startup
  - [ ] Update GameSettings.NumberOfDecks to be settable

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
  - [ ] Polish visual design
  - [ ] Optimize code structure
