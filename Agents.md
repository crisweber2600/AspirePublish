1. Project Layout
Path	Purpose	Agent Rules
/src/**	Production code—one project per micro-service or library.	Only modify code inside the target project unless told otherwise.
/tests/**	Unit & integration tests; mirror /src namespaces.	New features must add/modify tests here.
/build/	CI scripts, Dockerfiles, .editorconfig, .gitignore.	Never touch without explicit instruction.

Local solution build:

bash
Copy
Edit
dotnet tool restore
dotnet build --configuration Release
dotnet test --filter Category!=Integration
All generated code must succeed on the above. 
learn.microsoft.com

2. Coding Standards
2.1 Style
Follow Microsoft C# coding conventions—four-space indents, Allman braces, PascalCase for public identifiers. 
learn.microsoft.com

Apply .NET naming guidelines for frameworks/libraries. 
learn.microsoft.com

Keep files ≤ 400 LOC; split partial classes when size grows.

2.2 Language Features
Target C# 12 / .NET 8 (LTS) unless .global.json says otherwise.

Prefer record/record struct for immutable DTOs.

No dynamic; use generics/reflection only when unavoidable.

2.3 Async/Concurrency
Avoid .Result, .Wait()—always await. 
learn.microsoft.com
admirmujkic.medium.com

Pass CancellationToken down call-chains; cancel on shutdown.

Configure awaits with .ConfigureAwait(false) in libraries.

3. Dependency Injection
Constructor injection is mandatory; never use service locators. 
learn.microsoft.com
artemasemenov.medium.com

Register lifetimes correctly (AddSingleton, AddScoped, AddTransient).

Keep composition roots in Program.cs or StartupExtensions.cs.

4. Testing & Validation
Practice	Detail
Framework	xUnit + FluentAssertions.
Naming	Method_Scenario_ExpectedResult. 
learn.microsoft.com
AAA	Arrange-Act-Assert sections in every test.
Isolation	Mock IO & network; avoid real DB in unit tests.
Coverage Gate	PR must not drop line coverage < 80 %.

5. Security & Secrets
Store secrets in dev via dotnet user-secrets; production via Azure Key Vault or equivalent. Never commit secrets. 
learn.microsoft.com

Enable HTTPS, HSTS, and same-site cookies in web APIs.

Sanitize logs—no PII tokens.

Follow OWASP Top-10 secure-coding rules for .NET. 
mabroukmahdhi.medium.com

6. Observability
6.1 OpenTelemetry
Add OpenTelemetry().WithTracing().WithMetrics() at startup. Initialize before other services. 
opentelemetry.io

Auto-instrument ASP.NET Core & HttpClient; create custom spans for long IO.

Use semantic conventions; tag spans with net.peer.name, db.system, etc. 
betterstack.com

6.2 Logging
Serilog sinks to console in JSON; enable structured logging.

Correlate logs with trace-id.

7. Performance & Memory
Minimize allocations—favor Span<T>, Memory<T>, ArrayPool where hot. 
learn.microsoft.com
medium.com

Profile using dotnet trace or VS Perf Profiler before optimizing.

Reuse HttpClient via factories.

8. Background Workers / Actor Patterns
Implement long-running jobs via BackgroundService; respect StoppingToken for graceful exits. 
stackoverflow.com

For virtual actors, model grains with clear boundaries, idempotent commands, and avoid chatty aggregate grains. 
learn.microsoft.com
github.com
learn.microsoft.com

9. Static Analysis & Quality Gates
EnableNETAnalyzers is true; fail build on CA rules severity ≥ Warning. 
learn.microsoft.com
learn.microsoft.com

Additional analyzers via Microsoft.CodeAnalysis.NetAnalyzers NuGet keep versions in lock-file. 
nuget.org

10. CI/CD Runner Agents
10.1 Self-hosted Security
Run Azure DevOps or GitHub runners as non-admin service users; restrict outbound traffic; auto-patch weekly. 
learn.microsoft.com

10.2 Speed via Caching
Cache ~/.nuget/packages and .dotnet/tools keyed on global.json or Directory.Packages.props. Avoid caching bin/obj. 
reddit.com

11. Containerization
Build multi-stage Dockerfiles; base on mcr.microsoft.com/dotnet/aspnet:8.0-alpine or chiseled runtime. Pin digest SHAs for immutability. 
docs.docker.com
learn.microsoft.com

Copy only published output—dotnet publish -c Release -o /app.

Expose only needed ports; run as non-root UID 1000.

12. Branch, PR & Release Workflow
Feature branches: feat/<ticket-id>; hot-fix: fix/<issue>.

PR checklist auto-runs build, tests, analyzers, container scan.

Conventional Commits (feat:, fix:, etc.) for changelog automation.

Semantic versioning enforced via Git tags.

13. Agent Interaction Rules
Small surface: When fixing a bug, touch only the failing class & its tests.

Safety first: Never rewrite /build or Docker unless explicit.

Stop criteria: Build + tests + analyzers all green; memory & perf budgets unchanged.

Commit format: Single commit per task; include Fixes #<issue> in message.

14. Document Hierarchy
AGENTS.md (root) – global rules (this file).

src/<service>/AGENTS.md – service-specific overrides (e.g., special DI or infra).

The nearer file wins on conflicts (leaf overrides root).
