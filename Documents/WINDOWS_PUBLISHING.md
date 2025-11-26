# .NET MAUI Windows Publishing Guide

This document covers the various ways to publish the Blackjack MAUI app for Windows.

## Prerequisites

### Self-Signed Certificate (for MSIX sideloading)

Create a self-signed certificate for code signing (PowerShell as Admin):

```powershell
# Create certificate
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=Anuj Shroff" -KeyUsage DigitalSignature -FriendlyName "Blackjack Development Certificate" -CertStoreLocation "Cert:\CurrentUser\My" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")

# Get the thumbprint (save this!)
$cert.Thumbprint

# Export to .cer file
Export-Certificate -Cert "Cert:\CurrentUser\My\$($cert.Thumbprint)" -FilePath "C:\path\to\BlackjackDev.cer"

# Import to Trusted People store (for sideloading)
Import-Certificate -FilePath "C:\path\to\BlackjackDev.cer" -CertStoreLocation "Cert:\LocalMachine\TrustedPeople"
```

**Important:** The Publisher in `Package.appxmanifest` must match the certificate subject (e.g., `CN=Anuj Shroff`).

### csproj Default Settings

The project uses these defaults for local development:

```xml
<Version>0.0.0</Version>
<AssemblyVersion>0.0.0.0</AssemblyVersion>
<ApplicationDisplayVersion>0.0.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
<WindowsPackageType>None</WindowsPackageType>
```

---

## Publishing Options

### 1. Standalone EXE (Self-Contained + Windows App SDK Bundled)

Creates a single portable executable that runs anywhere without requiring runtime installation.

```cmd
dotnet publish src/Blackjack/Blackjack.csproj -c Release -f net10.0-windows10.0.19041.0 -p:PublishSingleFile=true -p:SelfContained=true -p:WindowsAppSDKSelfContained=true -o ./publish
```

**Characteristics:**
- ✅ Runs anywhere, no runtime installation needed
- ✅ Single exe file
- ⚠️ Larger file size (~150MB+)
- **Output:** `./publish/Blackjack.exe`

---

### 2. MSIX Package (Signed for Sideloading)

Creates an MSIX package signed with your certificate for distribution outside the Store.

```cmd
dotnet publish src/Blackjack/Blackjack.csproj -c Release -f net10.0-windows10.0.19041.0 -p:WindowsPackageType= -p:AppxPackageSigningEnabled=true -p:PackageCertificateThumbprint=YOUR_THUMBPRINT_HERE
```

**Characteristics:**
- ✅ Proper Windows app packaging
- ✅ Smaller size than standalone
- ⚠️ Requires certificate to be trusted on target machine
- **Output:** `src\Blackjack\bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\Blackjack_X.X.X.X_Test\`

**Installing the MSIX:**
1. Ensure Developer Mode is enabled on Windows (Settings → Privacy & security → For developers)
2. Run `Install.ps1` from the output folder, or:
   ```powershell
   Add-AppPackage -Path "Blackjack_X.X.X.X_x64.msix"
   ```

---

### 3. MSIX Package (Unsigned for Microsoft Store)

Creates an unsigned MSIX for submission to the Microsoft Store (they sign it).

```cmd
dotnet publish src/Blackjack/Blackjack.csproj -c Release -f net10.0-windows10.0.19041.0 -p:WindowsPackageType= -p:AppxPackageSigningEnabled=false
```

**Characteristics:**
- ✅ For Microsoft Store submission
- ✅ Store handles signing
- **Output:** Same as signed MSIX location

---

## Versioning

### Default Versions (Local Development)

With the csproj defaults, local builds produce:
- Assembly version: `0.0.0.0`
- MSIX version: `0.0.0.1`
- Display version: `Alpha` (in app UI)

### Release Versions (CI/CD)

Override versions on the command line for releases:

```cmd
-p:Version=1.2.3 -p:AssemblyVersion=1.2.3.0 -p:ApplicationDisplayVersion=1.2.3
```

**Example full release command:**
```cmd
dotnet publish src/Blackjack/Blackjack.csproj -c Release -f net10.0-windows10.0.19041.0 -p:PublishSingleFile=true -p:SelfContained=true -p:WindowsAppSDKSelfContained=true -p:Version=1.0.0 -p:AssemblyVersion=1.0.0.0 -p:ApplicationDisplayVersion=1.0.0 -o ./publish
```

---

## Key Properties Reference

| Property | Description | Default |
|----------|-------------|---------|
| `WindowsPackageType` | `None` for unpackaged, empty for MSIX | `None` |
| `SelfContained` | Bundle .NET runtime | `false` |
| `WindowsAppSDKSelfContained` | Bundle Windows App SDK | `false` |
| `PublishSingleFile` | Create single exe | `false` |
| `AppxPackageSigningEnabled` | Sign MSIX package | `false` |
| `PackageCertificateThumbprint` | Certificate for signing | - |
| `Version` | NuGet/assembly version | `0.0.0` |
| `AssemblyVersion` | .NET assembly version | `0.0.0.0` |
| `ApplicationDisplayVersion` | MSIX display version | `0.0.0` |
| `ApplicationVersion` | MSIX revision number | `1` |

---

## Troubleshooting

### Unpackaged EXE crashes on startup

**Error:** `APPCRASH` with `coreclr.dll` or `KERNELBASE.dll`

**Cause:** Windows App SDK runtime not installed and not bundled.

**Solution:** Add `-p:WindowsAppSDKSelfContained=true` to your publish command.

### MSIX won't install - "not digitally signed"

**Cause:** Package not signed or certificate not trusted.

**Solution:**
1. Ensure you're using `-p:AppxPackageSigningEnabled=true -p:PackageCertificateThumbprint=YOUR_THUMBPRINT`
2. Import your certificate to `Cert:\LocalMachine\TrustedPeople`
3. Enable Developer Mode in Windows Settings

### MSIX version shows 1.0.0.1 instead of expected version

**Cause:** `ApplicationDisplayVersion` not overridden.

**Solution:** Add `-p:ApplicationDisplayVersion=X.Y.Z` to your publish command.
