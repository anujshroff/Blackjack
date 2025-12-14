# Ashtonne's Blackjack

A classic casino blackjack experience built with .NET MAUI. Play against the dealer with AI opponents at the table, using authentic classic casino rules and mathematically optimal basic strategy.

## Availability

| Platform | Status |
|----------|--------|
| **Google Play Store** | 🔜 Coming Soon |
| **Windows** | 🔜 Coming Soon |

*Ashtonne's Blackjack will be available on app stores soon.*

## Privacy & Philosophy

This app was built with respect for the player in mind:

| Commitment | Description |
|------------|-------------|
| **No Ads** | Zero advertisements |
| **No In-App Purchases** | Completely free to play |
| **No Data Collection Code** | Zero analytics or telemetry code |
| **Open Source** | Full source code available |

> **Note:** The application source code contains no telemetry, analytics, or data collection. App distribution platforms (Google Play Store, etc.) may independently collect their own metrics outside of our control.

## Features

- **Classic Casino Rules** - 6-deck shoe with authentic casino gameplay
- **Player-Favorable 3:2 Blackjack Payout** - The good odds, not the 6:5 found at many casinos
- **Full Game Actions**:
  - Hit, Stand, Double Down, Split
  - Insurance and Even Money options
  - Double After Split (DAS) allowed
- **AI Opponents** - Play alongside AI players using mathematically optimal Basic Strategy
- **Multi-Seat Table** - Choose your seat at the table
- **Configurable Settings** - Adjustable bet limits and game options

## Game Rules

| Rule | Setting |
|------|---------|
| Number of Decks | 6 |
| Dealer Hits Soft 17 | Yes (H17) |
| Blackjack Payout | 3:2 |
| Double Down | Any two cards |
| Double After Split | Yes (except Aces) |
| Split | Up to 4 hands |
| Split Aces | One card each |
| Insurance | 2:1 payout |

## Technologies

- **.NET 10.0 MAUI** - Cross-platform app framework
- **CommunityToolkit.Mvvm** - MVVM architecture support
- **FluentIcons.Maui** - Modern iconography

### Supported Platforms

- **Android** (API 21+)
- **Windows** (Windows 10 version 1809+)

## Building from Source

### Prerequisites

- .NET 10.0 SDK
- Visual Studio 2022 or later with MAUI workload
- For Android: Android SDK

### Clone and Build

```bash
git clone https://github.com/anujshroff/Blackjack.git
cd Blackjack
```

Open `Blackjack.slnx` in Visual Studio and build for your target platform.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## AI Notice

This project was entirely generated using AI, leveraging **Cline** with **Claude Sonnet 4.5**, **Claude Opus 4.1**, and **Claude Opus 4.5** by Anthropic. From game logic and UI design to card graphics and documentation, every aspect was created through AI-assisted development. This project serves as a testament to the capabilities of modern AI in creating production-quality mobile applications.
