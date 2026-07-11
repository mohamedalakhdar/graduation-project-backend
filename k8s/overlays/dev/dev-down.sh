#!/usr/bin/env bash
set -euo pipefail

NAMESPACE="college-system"

echo "=== Deleting Dev environment ==="
kubectl delete -k "$(dirname "$0")" --ignore-not-found

echo "=== Done ==="
