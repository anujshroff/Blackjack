# Blackjack Rules Review

This document reviews the implemented blackjack rules against classic casino rules.

---

## Table Configuration

| Setting | Implemented Value | Classic Casino | Status |
|---------|-------------------|----------------|--------|
| Number of Decks | 1, 2, 4, 6, or 8 (configurable) | 1-8 common | ✅ Standard |
| Shuffle Penetration | Dynamic (see below) | Varies by deck count | ✅ Standard |
| Table Minimum | $5 (from GameSettings) | $5-25 common | ✅ Standard |
| Table Maximum | $500 (from GameSettings) | Varies | ✅ Standard |
| Starting Bankroll | $1,000 (from GameSettings) | N/A (player choice) | ✅ Reasonable |

### Shuffle Penetration by Deck Count

Shuffle penetration is automatically set based on the number of decks, following standard US casino practices:

| Decks | Penetration | Cards Before Shuffle | Reasoning |
|-------|-------------|----------------------|-----------|
| 1 | ~1% | Reshuffle every round | Standard for single deck to combat card counting |
| 2 | 50% | ~52 cards dealt | Standard for double deck games |
| 4+ | 75% | Varies by total cards | Standard shoe game penetration |

All configuration values are now sourced from `GameSettings.cs` for centralized management.

---

## Dealer Rules

| Rule | Implemented | Classic Casino | Status |
|------|-------------|----------------|--------|
| Dealer hits on Soft 17 (H17) | ✅ Yes (from GameSettings) | Most casinos | ✅ Standard |
| Dealer stands on Hard 17+ | ✅ Yes | Universal | ✅ Standard |
| Dealer peeks on Ace | ✅ Yes | Universal (US) | ✅ Standard |
| Dealer peeks on 10-value | ✅ Yes | Universal (US) | ✅ Standard |

The `DealerHitsSoft17` setting is configurable in `GameSettings.cs` (default: true for H17).

---

## Payouts

| Outcome | Implemented | Classic Casino | Status |
|---------|-------------|----------------|--------|
| Blackjack | 3:2 (1.5x bet) | 3:2 or 6:5 | ✅ Standard (Good) |
| Regular Win | 1:1 (even money) | 1:1 | ✅ Standard |
| Insurance | 2:1 | 2:1 | ✅ Standard |
| Push | Bet returned | Bet returned | ✅ Standard |

**Note:** The 3:2 blackjack payout is the player-favorable standard. Many casinos have moved to 6:5, which is worse for players. This implementation uses the better 3:2 rule.

---

## Player Actions

### Hit
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Available until bust/21 | ✅ Yes | Yes | ✅ Standard |
| Available after split | ✅ Yes | Yes | ✅ Standard |

### Stand
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Always available | ✅ Yes | Yes | ✅ Standard |

### Double Down
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Available on any two cards | ✅ Yes | Most casinos | ✅ Standard |
| Restricted to 9/10/11 only | ❌ No | Some casinos | ✅ More permissive is fine |
| Receive exactly one card | ✅ Yes | Yes | ✅ Standard |
| Double after split (DAS) | ✅ Yes (from GameSettings) | Most casinos | ✅ Standard |
| Double after splitting Aces | ❌ No | Usually no | ✅ Standard |

The `DoubleAfterSplit` setting is configurable in `GameSettings.cs` (default: true, but always blocked on split Aces).

### Split
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Split pairs allowed | ✅ Yes | Yes | ✅ Standard |
| Max splits (4 hands total) | ✅ 3 resplits | 3-4 typical | ✅ Standard |
| Split 10-value cards | ✅ Yes (any 10-value) | Usually yes | ✅ Standard |
| K-Q, J-10 count as pairs | ✅ Yes | Usually yes | ✅ Standard |
| Split Aces get one card each | ✅ Yes | Universal | ✅ Standard |
| Resplit Aces | ❌ No (implied) | Usually no | ✅ Standard |

### Surrender
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Late Surrender | ❌ Not implemented | Some casinos | ℹ️ Intentional omission |
| Early Surrender | ❌ Not implemented | Rare | ℹ️ Intentional omission |

**Note:** Surrender was intentionally not implemented per the design requirements.

### Insurance
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Offered when dealer shows Ace | ✅ Yes | Yes | ✅ Standard |
| Costs half original bet | ✅ Yes | Yes | ✅ Standard |
| Pays 2:1 if dealer has BJ | ✅ Yes | Yes | ✅ Standard |

### Even Money
| Aspect | Implemented | Standard | Status |
|--------|-------------|----------|--------|
| Offered when player has BJ vs dealer Ace | ✅ Yes | Yes | ✅ Standard |
| Pays 1:1 immediately | ✅ Yes | Yes | ✅ Standard |

---

## Blackjack Handling

| Scenario | Implemented | Standard | Status |
|----------|-------------|----------|--------|
| Natural BJ (first 2 cards only) | ✅ Yes | Yes | ✅ Standard |
| Split Ace + 10 = 21 (not BJ) | ✅ Yes | Yes | ✅ Standard |
| Player BJ vs Dealer BJ = Push | ✅ Yes | Yes | ✅ Standard |
| Player BJ paid before dealer plays | ✅ Yes | Yes | ✅ Standard |

---

## Card Values

| Card | Implemented Value | Standard | Status |
|------|-------------------|----------|--------|
| 2-10 | Face value | Face value | ✅ Standard |
| Jack, Queen, King | 10 | 10 | ✅ Standard |
| Ace | 1 or 11 (auto-optimized) | 1 or 11 | ✅ Standard |

---

## AI Basic Strategy

The AI players use mathematically optimal Basic Strategy for H17 rules. Key decisions reviewed:

### Hard Totals
| Hand | vs Dealer 2-6 | vs Dealer 7-A | Correct? |
|------|---------------|---------------|----------|
| 17+ | Stand | Stand | ✅ |
| 13-16 | Stand | Hit | ✅ |
| 12 | Hit on 2-3, Stand on 4-6 | Hit | ✅ |
| 11 | Double | Double | ✅ |
| 10 | Double | Hit on 10-A | ✅ |
| 9 | Hit on 2, Double 3-6 | Hit | ✅ |
| 5-8 | Hit | Hit | ✅ |

### Soft Totals
| Hand | Key Decisions | Correct? |
|------|---------------|----------|
| Soft 20 | Always Stand | ✅ |
| Soft 19 | Stand, Double on 6 | ✅ |
| Soft 18 | Double 2-6, Stand 7-8, Hit 9-A | ✅ |
| Soft 17 | Hit 2, Double 3-6, Hit 7-A | ✅ |
| Soft 13-16 | Hit low, Double mid, Hit high | ✅ |

### Pairs
| Pair | Key Decisions | Correct? |
|------|---------------|----------|
| A-A | Always Split | ✅ |
| 10-10 | Never Split | ✅ |
| 9-9 | Split 2-6,8-9; Stand 7,10,A | ✅ |
| 8-8 | Always Split | ✅ |
| 7-7 | Split 2-7, Hit 8-A | ✅ |
| 5-5 | Never Split (Double/Hit) | ✅ |

**AI Insurance Decision:** Always decline (correct per Basic Strategy - insurance is a sucker bet with negative expected value)

---

## Game Flow

| Phase | Implementation | Classic Casino | Status |
|-------|----------------|-----------------|--------|
| Betting | Player places bet, AI auto-bets | Same | ✅ |
| Dealing | 1 card each, dealer up, 1 card each, dealer hole | Correct order | ✅ |
| Insurance Offer | When dealer shows Ace | When dealer shows Ace | ✅ |
| Dealer Peek | On Ace and 10-value cards | On Ace and 10-value | ✅ |
| Player Actions | Left to right (seat order) | Same | ✅ |
| Dealer Actions | After all players, if needed | Same | ✅ |
| Settlement | Compare hands, pay winners | Same | ✅ |

---

## Summary

### ✅ Fully Standard Rules (No Issues)
- 6-deck shoe
- H17 dealer rules
- 3:2 blackjack payout
- Insurance at 2:1
- Double on any two cards
- Double after split (DAS)
- Split up to 4 hands
- Split any 10-value cards (including mixed ranks)
- Split Aces receive one card each
- Dealer peek on Ace and 10-value
- Proper card values with Ace flexibility
- Correct Basic Strategy for AI players

### ℹ️ Intentional Design Decisions
- **No Surrender** - Not implemented by design. While some casinos offer late surrender, many don't. This is a valid game configuration.

### ⚠️ Potential Enhancements (Not Bugs)
None identified. The implementation follows standard US blackjack rules correctly.

---

## Conclusion

**This implementation correctly follows classic casino blackjack rules.** All core mechanics, payouts, and player actions are implemented according to typical US classic casino standards. The AI players use mathematically correct Basic Strategy for the H17 rule set.

The only notable omission is the Surrender option, which was intentionally excluded from the design requirements. This is acceptable as surrender availability varies by casino.

---

## GameSettings Reference

All configurable game parameters are centralized in `Models/GameSettings.cs`:

| Property | Default Value | Description |
|----------|---------------|-------------|
| `TableMinimum` | $5 | Minimum bet allowed |
| `TableMaximum` | $500 | Maximum bet allowed |
| `StartingBankroll` | $1,000 | Starting chips for each player |
| `NumberOfDecks` | 6 | Number of decks in shoe |
| `DealerHitsSoft17` | true | H17 rule (dealer hits soft 17) |
| `BlackjackPayout` | 1.5 (3:2) | Blackjack payout multiplier |
| `InsurancePayout` | 2.0 (2:1) | Insurance payout multiplier |
| `MaxSplits` | 3 | Maximum re-splits allowed (4 hands total) |
| `DoubleAfterSplit` | true | Whether DAS is allowed (except on Aces) |
