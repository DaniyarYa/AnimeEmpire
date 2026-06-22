# Firebase setup — Anime Empire Unity

Firebase Unity SDK is NOT on UPM. Per-platform credentials required. This guide enables Firebase-backed RemoteConfig + Analytics + Crashlytics + Cloud Messaging + Auth.

## Prerequisites

- Firebase project at <https://console.firebase.google.com>
- App registered w/ bundle id `com.animeempire.app` (both Android + iOS)
- Firebase Unity SDK 12.x downloaded from <https://firebase.google.com/download/unity>

## Steps

### 1. Add platform configs

- **Android**: download `google-services.json` from Firebase Console → Project Settings → Your Apps → Android. Drop into `unity/Assets/StreamingAssets/`.
- **iOS**: download `GoogleService-Info.plist` from same panel for iOS app. Drop into `unity/Assets/StreamingAssets/`.

Both files belong in `StreamingAssets/` because the Firebase Editor plugin (`FirebaseInitializeEditor`) auto-discovers them there.

### 2. Import Firebase SDK packages

From the Firebase Unity SDK zip, drag these `.unitypackage` files into Unity (one at a time, in this order):

1. `FirebaseApp.unitypackage` (always first — core dep)
2. `FirebaseAuth.unitypackage`
3. `FirebaseAnalytics.unitypackage`
4. `FirebaseCrashlytics.unitypackage`
5. `FirebaseRemoteConfig.unitypackage`
6. `FirebaseMessaging.unitypackage`

Let Unity reimport between each. Total ~3 min.

### 3. Enable FIREBASE_ENABLED define

`Edit → Project Settings → Player → Other → Scripting Define Symbols`:

- Add `FIREBASE_ENABLED` for **Android**.
- Add `FIREBASE_ENABLED` for **iOS**.

Without the define, `Assets/AnimeEmpire/Code/Firebase/AnimeEmpire.Firebase.asmdef` (whose `defineConstraints: ["FIREBASE_ENABLED"]`) is skipped. With the define, adapters auto-register on play via `FirebaseBootstrap.[RuntimeInitializeOnLoadMethod]`.

### 4. Update link.xml

Append the following block to `unity/Assets/link.xml` to prevent IL2CPP stripping Firebase types:

```xml
<assembly fullname="Firebase.App" preserve="all"/>
<assembly fullname="Firebase.Auth" preserve="all"/>
<assembly fullname="Firebase.Analytics" preserve="all"/>
<assembly fullname="Firebase.Crashlytics" preserve="all"/>
<assembly fullname="Firebase.RemoteConfig" preserve="all"/>
<assembly fullname="Firebase.Messaging" preserve="all"/>
```

### 5. Configure RemoteConfig defaults

Firebase Console → RemoteConfig → set parameters:

- `economy.cost_growth_early` (number, 1.12)
- `economy.cost_growth_mid` (number, 1.15)
- `economy.cost_growth_late` (number, 1.18)
- `flags.enable_friends` (bool, false)
- `flags.enable_battle_pass` (bool, false)

Publish. App will fetch on next launch.

### 6. Build + run

```bash
Unity -batchmode -nographics -quit -projectPath unity \
  -executeMethod AnimeEmpire.Editor.BuildScript.BuildAndroid \
  -logFile build/android.log
```

Sideload `.aab` on device. Verify on app launch:

1. **Crashlytics**: trigger `Debug.LogException(new Exception("test"))`. Crash appears in Firebase Console within ~1 min.
2. **Analytics**: app sends `lifecycle.session.started`. Firebase Console → Analytics → Realtime → event visible within 60s.
3. **RemoteConfig**: edit `economy.cost_growth_early` to 1.2 in console + publish. Restart app. `RemoteConfig.Instance.GetFloat("economy.cost_growth_early", 1.12)` returns 1.2.
4. **Auth**: Firebase Console → Authentication → Users list shows anonymous user.
5. **Messaging**: Firebase Console → Cloud Messaging → Send test message to FCM token (read from logcat `[FirebaseMessaging] token received`). Device receives notification.

## Architecture

```
AnimeEmpire.Runtime
    │ defines interfaces (IRemoteConfigProvider, IAnalyticsSink, ICrashReporter, INotificationProvider)
    │ ships built-in default impls (HTTP RemoteConfig, DebugLogSink, LocalLogReporter, NotificationService local)
    │
    └─ swap points: RemoteConfig.RegisterProvider, AnalyticsBus.RegisterSink, CrashReporter.SetProvider
        │
        ↑ registered by
        │
AnimeEmpire.Firebase  (defineConstraints: FIREBASE_ENABLED)
    │ FirebaseBootstrap.[RuntimeInitializeOnLoadMethod]
    │ ├ FirebaseCrashlyticsReporter
    │ ├ FirebaseAnalyticsSink
    │ ├ FirebaseRemoteConfigProvider
    │ ├ FirebaseAuthService (sign in anonymously)
    │ └ FirebaseMessagingProvider (FCM register + subscribe)
```

`AnimeEmpire.Runtime` NEVER references `Firebase.*` namespaces. All Firebase calls live in `AnimeEmpire.Firebase`. Without the define, Runtime builds clean w/ default impls.

## Troubleshooting

- **"DependencyStatus" not Available**: missing google-services.json in StreamingAssets, or wrong bundle id, or Firebase Resolver hasn't run. Reimport `FirebaseApp.unitypackage`.
- **`Firebase.Auth.dll` not found at runtime**: IL2CPP stripped it. Verify link.xml entries.
- **iOS pod install fails**: run `pod install --repo-update` in Xcode project dir. Firebase often needs gRPC + Protobuf cleanup.
- **Crashlytics dSYM upload missing on iOS**: add Fastlane lane or upload manually via Firebase Console after each iOS build.
- **Auth user_id rotates on uninstall**: anonymous user is tied to device install. Phase 3 will add Apple/Google sign-in to persist.

## Phase 2 follow-ups

- Cloud Functions for IAP receipt validation.
- A/B test variant rollout via RemoteConfig `ab_variants` param (data shape ready).
- Conversion events for ad networks (Facebook SDK / AppsFlyer).
- Crashlytics dSYM auto-upload in iOS build workflow.
