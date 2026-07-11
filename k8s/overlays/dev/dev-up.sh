#!/usr/bin/env bash
set -euo pipefail

NAMESPACE="college-system"

echo "=== Applying Dev environment ==="
kubectl apply -k "$(dirname "$0")"

echo "=== Waiting for API deployment to be ready ==="
kubectl rollout status deployment/api -n "$NAMESPACE" --timeout=120s

echo ""
echo "=== Port forwarding (run in separate terminals) ==="
echo "  API:      kubectl port-forward -n $NAMESPACE deployment/api 8080:8080"
echo "  Grafana:  kubectl port-forward -n $NAMESPACE deployment/grafana 3000:3000"
echo "  Prometheus: kubectl port-forward -n $NAMESPACE deployment/prometheus 9090:9090"
echo ""
echo "=== Done ==="
