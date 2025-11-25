## Implementation Phases

### Phase 1: Core Game Logic (Foundation) - ✅ COMPLETE
**Goal**: Build the fundamental game mechanics and data structures
### Phase 2: Basic UI (Visual Foundation) - ✅ COMPLETE
**Goal**: Create the user interface structure and visual components
### Phase 3: Game Flow (Integration) - ✅ COMPLETE
**Goal**: Connect game logic with UI and implement complete game rounds
### Phase 4: AI Players (Intelligence) - ✅ COMPLETE
**Goal**: Implement intelligent computer opponents
### Phase 5: Polish and Features (Enhancement) - ✅ COMPLETE
**Goal**: Add polish, additional features, and platform optimizations

### Phase 6: Testing and Refinement (Quality Assurance)
**Goal**: Ensure quality and correct functionality through unit tests

- [ ] **Unit Tests - Hand Evaluation**
  - [ ] TotalValue: Simple hands (7+5=12), face cards (K+Q=20)
  - [ ] TotalValue: Single Ace as 11 (A+6=17), Ace as 1 (A+6+9=16)
  - [ ] TotalValue: Multiple Aces (A+A=12, A+A+9=21, A+A+A=13)
  - [ ] TotalValue: Blackjack value (A+K=21)
  - [ ] IsSoft: Soft hands (A+2=13, A+6=17, A+7=18)
  - [ ] IsSoft: NOT soft after bust protection (A+6+8=15 hard)
  - [ ] IsSoft: Multi-ace scenarios (A+A+3=15 soft)
  - [ ] IsBlackjack: True blackjack (A+K, A+10, A+Q, A+J)
  - [ ] IsBlackjack: NOT blackjack (7+7+7=21, A+5+5=21, 10+10=20)
  - [ ] IsBusted: Just busted (10+6+7=23), not busted (10+6+5=21)
  - [ ] IsBusted: Ace saves from bust (10+6+A=17)
  - [ ] IsPair: True pairs (8+8, K+K), mixed 10-values (K+Q, 10+J)
  - [ ] IsPair: NOT pair (three cards, different values)

- [ ] **Unit Tests - Payout Calculations**
  - [ ] Blackjack 3:2 ($10→$25, $25→$62.50, $100→$250)
  - [ ] Win 1:1 ($10→$20, $100→$200)
  - [ ] Insurance 2:1 ($5→$15, $50→$150)
  - [ ] Push (return original bet)
  - [ ] Even Money ($10 blackjack→$20)

- [ ] **Unit Tests - Game Rules Validation**
  - [ ] CanSplit: Valid pairs with sufficient bankroll
  - [ ] CanSplit: Mixed 10-values (K-Q) allowed
  - [ ] CanSplit: Rejected - non-pair, max splits reached, insufficient bankroll
  - [ ] CanDoubleDown: First two cards with sufficient bankroll
  - [ ] CanDoubleDown: After split (DAS enabled vs disabled)
  - [ ] CanDoubleDown: Rejected - after hit, after split Aces, insufficient bankroll
  - [ ] CanHit: Active hand, soft 17
  - [ ] CanHit: Rejected - busted, standing, blackjack
  - [ ] CanStand: Any active non-busted hand
  - [ ] CanStand: Rejected - busted or already standing

- [ ] **Unit Tests - Basic Strategy**
  - [ ] Hard totals: 8 vs 6→Hit, 11 vs any→Double, 12 vs 2→Hit, 12 vs 4→Stand
  - [ ] Hard totals: 16 vs 10→Hit, 17+ vs any→Stand
  - [ ] Soft totals: 13 vs 5→Double, 17 vs 3→Double, 18 vs 9→Hit
  - [ ] Soft totals: 18 vs 2→Double, 19 vs 6→Double, 19 vs others→Stand
  - [ ] Pairs: A-A→Split, 8-8→Split, 10-10→Stand, 5-5 vs 9→Double
  - [ ] Pairs: 9-9 vs 7→Stand, 4-4 vs 5→Split, 2-2 vs 8→Hit
