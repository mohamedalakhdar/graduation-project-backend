# Kubernetes Migration Plan — College Control System
## Objective
Migrate from Docker Compose (API + PostgreSQL + Seq) to Kubernetes with:
- **Raw YAML + Kustomize** (base + overlays for dev/prod)
- **PostgreSQL** in-cluster via StatefulSet
- **Prometheus** for metrics collection & alerting
- **Loki + Promtail** for log aggregation (replaces Seq)
- **Grafana** for unified dashboards (metrics + logs)
- **Jenkins CI/CD** integration
---
## Stack Overview
```
                   ┌──────────────┐
                   │   Ingress    │  (TLS, domain routing)
                   └──────┬───────┘
                          │
                   ┌──────▼───────┐
                   │  API Service │  (ClusterIP)
                   └──────┬───────┘
                          │
              ┌───────────┼───────────┐
              │           │           │
       ┌──────▼────┐ ┌───▼────┐ ┌───▼──────┐
       │  API Pod  │ │ API    │ │ API      │
       │           │ │ Pod    │ │ Pod      │
       └───────────┘ └────────┘ └──────────┘
              │                          │
              │                   ┌──────▼────────┐
              │                   │ PostgreSQL     │
              │                   │ (StatefulSet)  │
              │                   └───────────────┘
              │
       ┌──────┴──────────────────────────────┐
       │  Promtail (DaemonSet — every node)   │
       │  → ships logs to Loki                │
       └──────────────────────────────────────┘
                          │
                   ┌──────▼───────┐
                   │    Loki      │
                   │  (logs)      │
                   └──────┬───────┘
                          │
                          │
              ┌───────────┼───────────┐
              │           │           │
       ┌──────▼────┐ ┌───▼────┐ ┌───▼──────┐
       │ Prometheus │ │ Grafana│ │ Alert-   │
       │ (metrics)  │ │ (UI)   │ │ manager  │
       └───────────┘ └────────┘ └──────────┘
```
---
## Directory Structure
```
k8s/
├── base/
│   ├── kustomization.yaml
│   ├── namespace.yaml
│   ├── configmap.yaml
│   ├── secrets.yaml                    # Template (values in prod overlay)
│   │
│   ├── postgres/
│   │   ├── kustomization.yaml
│   │   ├── statefulset.yaml
│   │   └── service.yaml
│   │
│   ├── api/
│   │   ├── kustomization.yaml
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── hpa.yaml
│   │   └── pdb.yaml
│   │
│   ├── prometheus/
│   │   ├── kustomization.yaml
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── configmap.yaml              # prometheus.yml with scrape configs
│   │   ├── pvc.yaml
│   │   └── servicemonitor.yaml         # Scrape config for API
│   │
│   ├── loki/
│   │   ├── kustomization.yaml
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── configmap.yaml              # loki.yml
│   │   └── pvc.yaml
│   │
│   ├── promtail/
│   │   ├── kustomization.yaml
│   │   ├── daemonset.yaml
│   │   └── configmap.yaml              # promtail.yml
│   │
│   └── grafana/
│       ├── kustomization.yaml
│       ├── deployment.yaml
│       ├── service.yaml
│       ├── configmap.yaml              # datasources (Prometheus + Loki), dashboards
│       └── pvc.yaml
│
├── overlays/
│   ├── dev/
│   │   ├── kustomization.yaml
│   │   ├── configmap-patch.yaml
│   │   └── secrets.yaml               # Dev credentials
│   │
│   └── prod/
│       ├── kustomization.yaml
│       ├── configmap-patch.yaml
│       ├── secrets.yaml                # Prod credentials (gitignored)
│       └── ingress.yaml
│
└── README.md                           # Apply instructions
```
---
## Task Breakdown
### Phase 1: Scaffold K8s Structure
- [x] **Task 1.1** — Create `k8s/` directory tree and `namespace.yaml`
- [x] **Task 1.2** — Create `base/kustomization.yaml` (namespace ref + component refs)
- [x] **Task 1.3** — Create `base/configmap.yaml` (shared non-sensitive settings)
- [x] **Task 1.4** — Create `base/secrets.yaml` (placeholder template)
### Phase 2: PostgreSQL (StatefulSet)
- [x] **Task 2.1** — Create `base/postgres/statefulset.yaml` with:
  - `volumeClaimTemplates` for persistent storage
  - Resource requests/limits
  - Readiness & liveness probes
  - Env vars from Secrets + ConfigMap
- [x] **Task 2.2** — Create `base/postgres/service.yaml` (headless ClusterIP)
- [x] **Task 2.3** — Create `base/postgres/kustomization.yaml`
### Phase 3: API Deployment
- [x] **Task 3.1** — Create `base/api/deployment.yaml` with:
  - `replicas: 2` (overridden per overlay)
  - Readiness probe: `GET /health` (port 8080)
  - Liveness probe: `GET /health` (longer threshold)
  - Resource requests: 256Mi RAM / 250m CPU
  - Resource limits: 512Mi RAM / 500m CPU
  - Env vars from ConfigMap + Secrets
  - `terminationGracePeriodSeconds: 30`
- [x] **Task 3.2** — Create `base/api/service.yaml` (ClusterIP, port 8080)
- [x] **Task 3.3** — Create `base/api/hpa.yaml` (CPU target: 70%, min: 2, max: 6)
- [x] **Task 3.4** — Create `base/api/pdb.yaml` (minAvailable: 1)
- [x] **Task 3.5** — Create `base/api/kustomization.yaml`
### Phase 4: Prometheus Stack (Metrics)
- [x] **Task 4.1** — Create `base/prometheus/configmap.yaml` (prometheus.yml with:
  - Global scrape interval: 15s
  - Scrape configs: Kubernetes API, kubelet, cadvisor, API ServiceMonitor)
  - Rule files for alerts)
- [x] **Task 4.2** — Create `base/prometheus/pvc.yaml` (10Gi retention)
- [x] **Task 4.3** — Create `base/prometheus/deployment.yaml`
- [x] **Task 4.4** — Create `base/prometheus/service.yaml`
- [x] **Task 4.5** — Create `base/prometheus/servicemonitor.yaml` (scrape API `/health` + `/metrics` endpoints)
- [x] **Task 4.6** — Create `base/prometheus/kustomization.yaml`
- [x] **Task 4.7** — Add Prometheus ASP.NET Core metrics to the API:
  - Add `prometheus-net.AspNetCore` NuGet package
  - Add `app.UseHttpMetrics()` and `app.MapMetrics()` in `Program.cs`
### Phase 5: Loki + Promtail (Logging)
- [x] **Task 5.1** — Create `base/loki/configmap.yaml`
- [x] **Task 5.2** — Create `base/loki/pvc.yaml`
- [x] **Task 5.3** — Create `base/loki/deployment.yaml`
- [x] **Task 5.4** — Create `base/loki/service.yaml`
- [x] **Task 5.5** — Create `base/loki/kustomization.yaml`
- [x] **Task 5.6** — Create `base/promtail/configmap.yaml` (scrape container logs, add pod/namespace labels)
- [x] **Task 5.7** — Create `base/promtail/daemonset.yaml`
- [x] **Task 5.8** — Create `base/promtail/kustomization.yaml`
- [x] **Task 5.9** — Remove Seq configuration from `Secret.json` and `appsettings.json`
### Phase 6: Grafana (Unified Visualization)
- [ ] **Task 6.1** — Create `base/grafana/configmap.yaml` with:
  - Preconfigured Prometheus datasource
  - Preconfigured Loki datasource
  - Preimported dashboards (ASP.NET Core, K8s cluster, PostgreSQL)
- [ ] **Task 6.2** — Create `base/grafana/pvc.yaml` (persistence)
- [ ] **Task 6.3** — Create `base/grafana/deployment.yaml`
- [ ] **Task 6.4** — Create `base/grafana/service.yaml`
- [ ] **Task 6.5** — Create `base/grafana/kustomization.yaml`
### Phase 7: Overlays — Dev Environment
- [ ] **Task 7.1** — Create `overlays/dev/kustomization.yaml` (patches: replicas:1, smaller resources)
- [ ] **Task 7.2** — Create `overlays/dev/configmap-patch.yaml`
- [ ] **Task 7.3** — Create `overlays/dev/secrets.yaml`
- [ ] **Task 7.4** — Create dev cleanup / port-forward helper scripts
### Phase 8: Overlays — Production Environment
- [ ] **Task 8.1** — Create `overlays/prod/kustomization.yaml` (patches: replicas:3, higher resources)
- [ ] **Task 8.2** — Create `overlays/prod/configmap-patch.yaml`
- [ ] **Task 8.3** — Create `overlays/prod/secrets.yaml` (add to `.gitignore`!)
- [ ] **Task 8.4** — Create `overlays/prod/ingress.yaml` with TLS (nginx-ingress + cert-manager)
### Phase 9: CI/CD — Jenkins Pipeline
- [ ] **Task 9.1** — Update `Jenkinsfile`:
  - Add `kubectl` credential injection
  - Add `Deploy to Dev` stage: `kubectl apply -k k8s/overlays/dev/`
  - Add `Deploy to Prod` stage: `kubectl apply -k k8s/overlays/prod/`
  - Add `Rollback` stage: `kubectl rollout undo deployment/api -n college-system`
### Phase 10: Validation & Testing
- [ ] **Task 10.1** — Local validation with `kubectl apply --dry-run=client -k k8s/base/`
- [ ] **Task 10.2** — Deploy to a test cluster (minikube / kind), verify:
  - All pods reach `Running` state
  - `/health` endpoint returns 200
  - Prometheus scrapes `/metrics`
  - Logs appear in Grafana/Loki
  - HPA scales replicas under load
- [ ] **Task 10.3** — Write a `k8s/README.md` with apply/teardown commands
---
## Prometheus Integration Details
### API Changes (Task 4.7)
```bash
dotnet add CollegeControlSystem.Presentation package prometheus-net.AspNetCore
```
In `Program.cs`:
```csharp
// Before app.Run():
app.UseHttpMetrics();
app.MapMetrics();
```
This exposes `/metrics` endpoint that Prometheus scrapes for:
- Request count, duration, in-flight requests (per endpoint, method, status code)
- .NET GC, memory, CPU metrics
- Custom metrics you define (e.g., `registrations_total`, `active_sessions`)
### Prometheus Scrape Config (in configmap.yaml)
```yaml
scrape_configs:
  - job_name: 'kubernetes-pods'
    kubernetes_sd_configs:
      - role: pod
    relabel_configs:
      - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_scrape]
        action: keep
        regex: true
      - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_path]
        action: replace
        target_label: __metrics_path__
        regex: (.+)
      - source_labels: [__address__, __meta_kubernetes_pod_annotation_prometheus_io_port]
        action: replacein
        regex: ([^:]+)(?::\d+)?;(\d+)
        replacement: $1:$2
        target_label: __address__
```
Then annotate the API pod template:
```yaml
annotations:
  prometheus.io/scrape: "true"
  prometheus.io/path: "/metrics"
  prometheus.io/port: "8080"
```
### Grafana Dashboards to Import
| Dashboard | ID | Description |
|-----------|-----|-------------|
| ASP.NET Core | 17906 | Request rate, errors, duration, GC, memory |
| .NET Runtime | 17907 | Detailed .NET runtime metrics |
| PostgreSQL | 9628 | Database performance (if pg_exporter added) |
| Kubernetes Cluster | 10000 | Cluster-wide health |
---
## 10 Rules for This Migration
1. **Never commit real secrets** — use Kustomize `secretGenerator` with `.env` files in `.gitignore`
2. **Always set resource requests & limits** — prevents noisy neighbor issues
3. **Always set probes** — without them, K8s can't manage pod lifecycle properly
4. **One concern per Kustomization** — each component is self-contained
5. **Use `kubectl apply -k`** — never hand-edit YAML in the cluster
6. **Prefer Promtail → Loki over Seq** — more K8s-native, no EULA, pairs perfectly with Grafana
7. **Expose /metrics on the API** — essential for Prometheus to gather app-level metrics
8. **Use a headless Service for StatefulSet** — ensures stable DNS names for PostgreSQL
9. **Always dry-run before apply** — `kubectl apply --dry-run=client -k overlays/dev/`
10. **Test locally first** — use `kind` or `minikube` before any production cluster
