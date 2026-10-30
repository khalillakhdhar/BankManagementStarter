# Two-Factor Authentication Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement Google Authenticator TOTP, recovery codes, and a two-step login in the existing .NET 9 backend and Angular 21 frontend.

**Architecture:** ASP.NET Core Identity owns authenticator keys, token verification, and recovery codes. A five-minute Data Protection challenge bridges password verification to the second-factor endpoint without granting API access. Angular keeps that challenge only in memory, renders the QR code locally, and persists a JWT session only after authentication is complete.

**Tech Stack:** .NET 9, ASP.NET Core Identity, Data Protection, EF Core, xUnit, Angular 21 standalone, RxJS, Angular unit-test builder with Vitest, local QR-code package.

**Spec:** `docs/superpowers/specs/2026-10-30-two-factor-authentication-design.md`

## Global Constraints

- Work only on `feature/two-factor-auth` in both repositories.
- Preserve the existing JWT shape for users without 2FA and after completed 2FA.
- Google Authenticator requires no Google API key.
- Never persist the login challenge, authenticator key, or `otpauth` URI in browser storage.
- Never send the `otpauth` URI to an external QR-code service.
- Challenge lifetime is five minutes and challenges cannot authorize business routes.
- Recovery codes are one-time codes managed by ASP.NET Core Identity.
- Use TDD: every behavior change starts with a failing focused test.

## Review Focus

- An altered, expired, empty, or wrong-purpose challenge must produce the same generic authentication failure; covered in Task 1 challenge tests and Task 2 controller tests.
- Codes containing spaces or dashes must normalize before validation without accepting other malformed input; covered in Task 2 service tests.
- A password-only response for a 2FA account must never contain a JWT or user details; covered in Task 2 service and controller tests.
- Reloading or cancelling the Angular second step must clear the in-memory challenge and return to credentials; covered in Task 4 component tests.
- Leaving the profile setup view must remove the shared key and URI from component state; covered in Task 5 profile tests.

---

### Task 1: Signed five-minute login challenge

**Files:**
- Create: `Bank.Api/Services/Interfaces/ITwoFactorChallengeService.cs`
- Create: `Bank.Api/Services/Implementations/TwoFactorChallengeService.cs`
- Create: `Bank.Api.Tests/TwoFactorChallengeServiceTests.cs`
- Modify: `Bank.Api/Program.cs`

**Interfaces:**
- Produces: `string Create(string userId)` and `string? Validate(string challenge)`.
- Uses: `IDataProtectionProvider.CreateProtector("Bank.Api.TwoFactorLogin.v1").ToTimeLimitedDataProtector()` with a five-minute lifetime.

- [ ] Write tests proving round-trip validation, tamper rejection, empty input rejection, wrong-purpose rejection, and five-minute expiry behavior.
- [ ] Run `dotnet test Bank.Api.Tests --filter TwoFactorChallengeServiceTests` and verify the new tests fail because the service does not exist.
- [ ] Implement the interface, time-limited Data Protection service, and scoped registration in `Program.cs`.
- [ ] Re-run the focused tests and verify they pass.
- [ ] Commit with `feat: add signed two-factor login challenges`.

### Task 2: Backend 2FA login and account-management API

**Files:**
- Create: `Bank.Api/DTOs/Auth/LoginResultDto.cs`
- Create: `Bank.Api/DTOs/Auth/TwoFactorLoginDto.cs`
- Create: `Bank.Api/DTOs/Auth/TwoFactorCodeDto.cs`
- Create: `Bank.Api/DTOs/Auth/TwoFactorStatusDto.cs`
- Create: `Bank.Api/DTOs/Auth/TwoFactorSetupDto.cs`
- Create: `Bank.Api/DTOs/Auth/TwoFactorEnableResultDto.cs`
- Modify: `Bank.Api/Services/Interfaces/IAuthService.cs`
- Modify: `Bank.Api/Services/Implementations/AuthService.cs`
- Modify: `Bank.Api/Controllers/AuthController.cs`
- Create: `Bank.Api.Tests/AuthTwoFactorServiceTests.cs`
- Modify: `Bank.Api.Tests/SecurityContractTests.cs`
- Modify: `Bank.Api.Tests/Bank.Api.Tests.csproj`

**Interfaces:**
- `LoginAsync(LoginDto) -> LoginResultDto?` returns either `Session` or `RequiresTwoFactor + TwoFactorChallenge`, never both.
- `CompleteTwoFactorLoginAsync(TwoFactorLoginDto) -> AuthResponseDto?` accepts Authenticator or recovery code.
- `GetTwoFactorStatusAsync(userId) -> TwoFactorStatusDto`.
- `BeginTwoFactorSetupAsync(userId) -> TwoFactorSetupDto` returns `SharedKey` and `AuthenticatorUri`.
- `EnableTwoFactorAsync(userId, code) -> TwoFactorEnableResultDto?` returns recovery codes once.
- `GenerateRecoveryCodesAsync(userId, code) -> IReadOnlyCollection<string>?`.
- `DisableTwoFactorAsync(userId, code) -> bool`.

- [ ] Add real-Identity test fixtures with EF Core InMemory and failing tests for unchanged non-2FA login, no JWT on password-only 2FA login, valid/invalid TOTP, one-time recovery codes, setup URI encoding, activation, regeneration, and disabling.
- [ ] Run the focused service tests and verify expected failures from the missing DTOs and methods.
- [ ] Implement DTOs and minimal service methods using `UserManager<ApplicationUser>` Authenticator APIs; normalize codes by removing spaces and dashes.
- [ ] Add failing controller/security tests for `[Authorize]` management routes, anonymous login endpoints, 400 generic invalid challenge/code responses, and subject lookup from `ClaimTypes.NameIdentifier`.
- [ ] Implement `POST login/2fa`, `GET 2fa/status`, `POST 2fa/setup`, `POST 2fa/enable`, `POST 2fa/recovery-codes`, and `POST 2fa/disable`.
- [ ] Run `dotnet test BankManagement.sln` and verify the full backend suite passes.
- [ ] Commit with `feat: implement authenticator two-factor API`.

### Task 3: Angular test infrastructure and typed 2FA client

**Files:**
- Modify: `ibankfront/package.json`
- Modify: `ibankfront/package-lock.json`
- Modify: `ibankfront/angular.json`
- Create: `ibankfront/tsconfig.spec.json`
- Modify: `ibankfront/src/app/models/auth.model.ts`
- Modify: `ibankfront/src/app/services/auth.service.ts`
- Create: `ibankfront/src/app/services/auth.service.spec.ts`

**Interfaces:**
- Produces discriminated `LoginResult` types for completed session versus `TwoFactorRequired`.
- Produces `completeTwoFactorLogin(code, useRecoveryCode)`, `cancelTwoFactorLogin()`, `twoFactorStatus()`, `beginTwoFactorSetup()`, `enableTwoFactor(code)`, `generateRecoveryCodes(code)`, and `disableTwoFactor(code)`.
- Exposes a read-only in-memory `twoFactorChallenge` signal; only completed sessions reach `SessionStorageService.write`.

- [ ] Add Angular’s `@angular/build:unit-test` target, Vitest dependency, `npm test`, and a minimal test config; verify the empty baseline runner starts successfully.
- [ ] Write failing service tests for normal login persistence, challenge-only login without persistence, completion persistence, recovery-code flag, cancel clearing, and 2FA management endpoint mapping.
- [ ] Run `npm test -- --watch=false` and verify the focused tests fail for the missing types and methods.
- [ ] Implement the discriminated models and minimal AuthService methods.
- [ ] Re-run the service tests and verify they pass.
- [ ] Commit with `feat: add typed frontend two-factor client`.

### Task 4: Two-step Angular login screen

**Files:**
- Modify: `ibankfront/src/app/features/auth/login/login.ts`
- Modify: `ibankfront/src/app/features/auth/login/login.html`
- Modify: `ibankfront/src/app/features/auth/login/login.css`
- Create: `ibankfront/src/app/features/auth/login/login.spec.ts`

**Interfaces:**
- Consumes Task 3’s `LoginResult`, `twoFactorChallenge`, `completeTwoFactorLogin`, and `cancelTwoFactorLogin`.
- Produces credentials, Authenticator code, and recovery-code modes with one navigation only after a complete session.

- [ ] Write failing component tests for credentials-to-code transition, no early navigation, successful code navigation, recovery-code submission, invalid-code retention, cancel, and reload/no-challenge fallback.
- [ ] Run the focused login tests and verify they fail against the current one-step component.
- [ ] Implement the minimal two-state component/template and accessible form copy, preserving the existing visual language.
- [ ] Re-run the focused tests and verify they pass.
- [ ] Commit with `feat: add two-step login interface`.

### Task 5: Profile setup, QR code, recovery codes, and documentation

**Files:**
- Modify: `ibankfront/package.json`
- Modify: `ibankfront/package-lock.json`
- Modify: `ibankfront/src/app/features/profile/profile.ts`
- Modify: `ibankfront/src/app/features/profile/profile.html`
- Modify: `ibankfront/src/app/features/profile/profile.css`
- Create: `ibankfront/src/app/features/profile/profile.spec.ts`
- Modify: `BankManagementStarter/README.md`
- Modify: `ibankfront/README.md`

**Interfaces:**
- Consumes Task 3 management methods.
- Uses the local `qrcode` package to turn `AuthenticatorUri` into a data URL without network calls.
- Produces setup, activation, one-time recovery-code display, regeneration, disabling, and state cleanup.

- [ ] Write failing profile tests for status loading, QR creation from the exact URI, manual-key fallback, activation, one-time recovery-code display, regeneration, disabling, API errors, and state cleanup on destroy.
- [ ] Run the focused profile tests and verify they fail against the current read-only profile.
- [ ] Install the local QR dependency and implement the minimal profile flow; never persist or log setup secrets.
- [ ] Re-run profile tests and verify they pass.
- [ ] Update both README files from “planned” wording to exact implemented steps, endpoints, no-Google-API-key explanation, and test commands.
- [ ] Run `npm test -- --watch=false`, `npm run build`, `dotnet build BankManagement.sln --configuration Release -warnaserror`, and `dotnet test BankManagement.sln --configuration Release`.
- [ ] Commit backend docs with `docs: document implemented two-factor flow` and frontend changes with `feat: add profile two-factor management`.

### Task 6: Cross-project smoke verification and publication

**Files:**
- Modify if required: `ibankfront/scripts/smoke-api.ps1`
- Create: `ibankfront/docs/two-factor-smoke-results.md`

**Interfaces:**
- Consumes the completed backend and frontend branches.
- Produces a non-secret verification record; no authenticator key, challenge, JWT, or recovery code is written to disk.

- [ ] Add a non-destructive smoke sequence that validates legacy login, 2FA-required response shape for a disposable training fixture, invalid second-factor rejection, authorization, and existing 27-route behavior without recording secrets.
- [ ] Run full backend tests, frontend tests/build, and the smoke script against an isolated local/test database.
- [ ] Verify `git diff --check`, clean tracked worktrees, and exact remote branch SHAs in both repositories.
- [ ] Push `feature/two-factor-auth` in both repositories; do not merge to `main`/`master` and do not promote to production.

